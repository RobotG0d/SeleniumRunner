namespace SeleniumRunner.Model.Entities
{
    public class SideFile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Test[] Tests { get; set; }
        public object[] Suites { get; set; }
        public string[] Urls { get; set; }
    }
}