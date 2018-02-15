using System;
using SeleniumRunner.Model.Entities;

namespace SeleniumRunner.Model
{
    internal class DummyRunnerListener : IRunnerListener
    {
        public void OnProjectStart(SideFile project)
        {

        }

        public void OnTestStart(Test test)
        {

        }

        public void OnCommand(Test test, Instruction instruction)
        {

        }

        public void OnCommandError(Test test, Instruction instruction, Exception e)
        {

        }


        public void OnTestEnd(Test test, TimeSpan elapsed)
        {

        }

        public void OnProjectEnd(SideFile project)
        {

        }
    }
}
