using SeleniumRunner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeleniumRunner.Model.Entities;

namespace SeleniumRunner.CLI
{
    internal class CLIRunnerListener : IRunnerListener
    {
        public void OnProjectStart(SideFile project)
        {
            Console.WriteLine($@"Starting test {project.Name}!");
        }

        public void OnTestStart(Test test)
        {
            Console.WriteLine($@"Starting test {test.Name}!");
        }

        public void OnCommand(Test test, Instruction instruction)
        {
            Console.WriteLine($@"{test.Name}: Applying '{instruction.Command}' command to target '{instruction.Target}'");
        }

        public void OnCommandError(Test test, Instruction instruction, Exception e)
        {
            Console.WriteLine($@"{test.Name}: Command '{instruction.Command}' being applied to target '{instruction.Target}' failed: {e}");
        }

        public void OnTestEnd(Test test, TimeSpan elapsed)
        {
            Console.WriteLine($@"Test {test.Name} finished with success, after {elapsed.Milliseconds}ms!");
        }

        public void OnProjectEnd(SideFile project)
        {
            Console.WriteLine($@"Project {project.Name} ended!");
        }
    }
}
