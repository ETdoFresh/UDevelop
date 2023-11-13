namespace CommandSystem
{
    public class OutputData
    {
        public object Value;
        public string CommandLineOutput;
        
        public OutputData Replace(object newValue)
        {
            Value = newValue;
            return this;
        }
    }
}