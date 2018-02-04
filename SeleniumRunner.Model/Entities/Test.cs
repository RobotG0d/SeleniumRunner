namespace SeleniumRunner.Model.Entities
{
    public class Test
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Instruction[] Commands { get; set; }
    }
}
