using SeleniumRunner.Model.Entities;
using System;

namespace SeleniumRunner.Model
{
    public interface IRunnerListener
    {
        void OnProjectStart(SideFile project);
        void OnTestStart(Test test);
        void OnCommand(Instruction instruction);
        void OnTestEnd(Test test, TimeSpan elapsed);
        void OnProjectEnd(SideFile project);
    }
}
