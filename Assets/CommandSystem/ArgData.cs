using System;

namespace CommandSystem
{
    public class ArgData
    {
        public string Name;
        public Type Type;
        public object Value;
        public bool Required;

        public ArgData(string name, Type type, object value, bool required)
        {
            Name = name;
            Type = type;
            Value = value;
            Required = required;
        }
    }
}