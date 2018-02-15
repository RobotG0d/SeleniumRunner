using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SeleniumRunner.Model.Entities;
using SeleniumRunner.Model.Exceptions;
using SeleniumRunner.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SeleniumRunner.Model
{
    public class Runner
    {
        #region Static Dictionaries 

        private static readonly IDictionary<string, Func<RemoteWebDriver, string, IWebElement>> ElementGetters = new Dictionary<string, Func<RemoteWebDriver, string, IWebElement>>(StringComparer.InvariantCultureIgnoreCase)
        {
            {Constants.Targets.Id, (driver, id) => driver.FindElementById(id) },
            {Constants.Targets.Css, (driver, css) => driver.FindElementByCssSelector(css) },
            {Constants.Targets.Name, (driver, name) => driver.FindElementByName(name) },
            {Constants.Targets.Xpath, (driver, xpath) => driver.FindElementByXPath(xpath) }
        };

        private static readonly IDictionary<string, Action<RemoteWebDriver, IWebElement, string>> CommandExecutors = new Dictionary<string, Action<RemoteWebDriver, IWebElement, string>>(StringComparer.InvariantCultureIgnoreCase)
        {
            {Constants.Commands.Submit, (driver, element, value) => element.Submit() },
            {Constants.Commands.Click, (driver, element, value) => driver.ExecuteScript("arguments[0].click()", element) },
            {Constants.Commands.MouseOut, (driver, element, value) => driver.ExecuteScript("if(arguments[0].mouseout) arguments[0].mouseout();", element) },
            {Constants.Commands.MouseOver, (driver, element, value) => {
                    Actions action = new Actions(driver);
                    action.MoveToElement(element).Build().Perform();
                }
            },
            {Constants.Commands.Type, (driver, element, value) => {
                    element.Clear();
                    element.SendKeys(value);
                }
            },
            {Constants.Commands.Select, (driver, element, value) => {
                    SelectElement select = new SelectElement(element);
                    select.SelectByText(value.Remove(0, "label=".Length));
                }
            },
            {Constants.Commands.AssertText, (driver, element, value) => {
                    if (!element.Text.EqualsIgnoreCase(value))
                        throw new AssertException($"Expected {value} but found {element.Text} instead.");
                }
            },
        };

        #endregion

        private readonly IRunnerListener Listener;

        public Runner(IRunnerListener listener)
        {
            Listener = listener;
        }

        public Runner() : this(new DummyRunnerListener())
        {

        }

        public ProjectReport Run(SideFile project)
        {
            using (RemoteWebDriver driver = CreateDriver(project.Url))
            {
                LinkedList<TestReport> testReports = new LinkedList<TestReport>();
                Listener.OnProjectStart(project);

                foreach (Test test in project.Tests)
                {
                    TestReport report = RunTest(driver, test);
                    testReports.AddLast(report);
                }

                Listener.OnProjectEnd(project);
                return new ProjectReport(project, testReports);
            }
        }

        #region Private Utils

        private RemoteWebDriver CreateDriver(string uri)
        {
            RemoteWebDriver driver = new ChromeDriver();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(uri);

            return driver;
        }

        private TestReport RunTest(RemoteWebDriver driver, Test test)
        {
            Stopwatch timer = new Stopwatch();
            Listener.OnTestStart(test);
            timer.Start();
            foreach (Instruction instruction in test.Commands)
            {
                try
                {
                    Listener.OnCommand(test, instruction);
                    ExecuteCommand(driver, instruction);
                }
                catch (Exception e)
                {
                    timer.Stop();
                    Listener.OnCommandError(test, instruction, e);
                    return new TestReport(test, timer.Elapsed, e);
                }
            }

            timer.Stop();
            Listener.OnTestEnd(test, timer.Elapsed);

            return new TestReport(test, timer.Elapsed);
        }

        private IWebElement GetElement(RemoteWebDriver driver, string target)
        {
            string type = Constants.Targets.Xpath;
            string selector = target;

            if (!target.StartsWith("//"))
            {
                int auxIdx = target.IndexOf('=');
                type = target.Substring(0, auxIdx);
                selector = target.Substring(auxIdx + 1);
            }

            bool exists = ElementGetters.TryGetValue(type, out var getter);
            if (!exists)
                throw new UnsupportedException($@"Target {target} is not supported.");

            return getter(driver, selector);
        }

        private void ExecuteCommand(RemoteWebDriver driver, Instruction instruction)
        {
            if (instruction.Command.EqualsIgnoreCase(Constants.Commands.Open))
            {
                driver.Navigate().GoToUrl(instruction.Target);
                return;
            }

            bool exists = CommandExecutors.TryGetValue(instruction.Command, out var executor);
            if (!exists)
                throw new UnsupportedException($@"Command {instruction.Command} is not supported.");

            executor(driver, GetElement(driver, instruction.Target), instruction.Value);
        }

        #endregion
    }
}
