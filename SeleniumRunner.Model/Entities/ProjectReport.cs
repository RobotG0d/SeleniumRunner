using System.Collections.Generic;

namespace SeleniumRunner.Model.Entities
{
    public class ProjectReport
    {
        public SideFile Project { get; set; }
        public IEnumerable<TestReport> TestReports { get; set; }

        public ProjectReport(SideFile project, IEnumerable<TestReport> reports)
        {
            Project = project;
            TestReports = reports;
        }
    }
}
