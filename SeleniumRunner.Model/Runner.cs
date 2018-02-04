using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using SeleniumRunner.Model.Entities;
using SeleniumRunner.Model.Extensions;
using System;
using System.Diagnostics;

namespace SeleniumRunner.Model
{
    public class Runner
    {
        private IRunnerListener Listener;

        public Runner(IRunnerListener listener)
        {
            Listener = listener;
        }

        public void Run(SideFile project)
        {

            using (RemoteWebDriver driver = CreateDriver(project.Url))
            {
                Listener.OnProjectStart(project);
                foreach (Test test in project.Tests)
                {
                    Stopwatch timer = new Stopwatch();
                    Listener.OnTestStart(test);
                    timer.Start();

                    foreach (Instruction instruction in test.Commands)
                    {
                        Listener.OnCommand(instruction);
                        ExecuteCommand(driver, instruction);
                    }

                    timer.Stop();
                    Listener.OnTestEnd(test, timer.Elapsed);
                }
            }
            Listener.OnProjectEnd(project);
        }

        #region Private Utils

        private RemoteWebDriver CreateDriver(string uri)
        {
            RemoteWebDriver driver = new ChromeDriver();
            
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            driver.Navigate().GoToUrl(uri);

            return driver;
        }

        private IWebElement GetElement(RemoteWebDriver driver, string target)
        {
            int auxIdx = target.IndexOf('=');
            string type = target.Substring(0, auxIdx);
            string selector = target.Substring(auxIdx + 1);

            if (type.EqualsIgnoreCase(Constants.Targets.Id))
                return driver.FindElementById(selector);
            else if (type.EqualsIgnoreCase(Constants.Targets.Css))
                return driver.FindElementByCssSelector(selector);
            else if (type.EqualsIgnoreCase(Constants.Targets.Name))
                return driver.FindElementByName(selector);
            else if (type.EqualsIgnoreCase(Constants.Targets.Xpath))
                return driver.FindElementByXPath(selector);

            throw new InvalidOperationException($@"Target {target} is not supported.");
        }

        private void ExecuteCommand(RemoteWebDriver driver, Instruction instruction)
        {
            if (instruction.Command.EqualsIgnoreCase(Constants.Commands.Open))
            {
                driver.Navigate().GoToUrl(instruction.Target);
                return;
            }

            IWebElement element = GetElement(driver, instruction.Target);

            if (instruction.Command.EqualsIgnoreCase(Constants.Commands.Click))
            {
                driver.ExecuteScript("arguments[0].click()", element);
            }
            else if (instruction.Command.EqualsIgnoreCase(Constants.Commands.Submit))
            {
                element.Submit();
            }
            else if (instruction.Command.EqualsIgnoreCase(Constants.Commands.Type))
            {
                element.Clear();
                element.SendKeys(instruction.Value);
            }
            else
                throw new InvalidOperationException($@"Command {instruction.Command} is not supported.");
            
        }

        #endregion
    }
}
