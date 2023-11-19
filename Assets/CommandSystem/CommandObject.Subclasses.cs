namespace CommandSystem
{
    public partial class CommandObject
    {
        public class CommandInputDetail
        {
            public string Type { get; set; } = "object";
            public string Name { get; set; }
            public bool Required { get; set; }
        }

        public class CommandCallDetail
        {
            public string Type { get; set; } = "object";
            public string Name { get; set; }
            public string CSharp { get; set; }
            public string Command { get; set; }
        }

        public class CommandOutputDetail
        {
            public string Type { get; set; } = "object";
            public string Name { get; set; }
        }
    }
}