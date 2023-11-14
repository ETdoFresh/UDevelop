using System;

namespace CommandSystem
{
    public class ArgData
    {
        public string Name;
        public Type Type;
        public object Value;
        public bool Required;

        public ArgData(string name, Type type, object value, bool required = false)
        {
            Name = name;
            Type = type;
            Value = value;
            Required = required;
        }

        public override string ToString()
        {
            var required = Required ? "*" : "";
            return $"{Name}{required} ({Type}) = {Value}";
        }
        
        public bool IsConvertible(Type inputType)
        {
            if (Type == inputType) return true;
            if (inputType.IsAssignableFrom(Type)) return true;
            var argTypeAttempt2 = Value?.GetType();
            if (argTypeAttempt2 != null && inputType.IsAssignableFrom(argTypeAttempt2)) return true;
            if (inputType.IsEnum && Type == typeof(int)) return true;
            if (Type == typeof(string) && inputType.IsEnum)
                return Enum.TryParse(inputType, Value?.ToString(), out _);
            if (Type == typeof(string) && inputType == typeof(int))
                return int.TryParse(Value?.ToString(), out _);
            if (Type == typeof(string) && inputType == typeof(float))
                return float.TryParse(Value?.ToString(), out _);
            if (Type == typeof(string) && inputType == typeof(double))
                return double.TryParse(Value?.ToString(), out _);
            if (Type == typeof(string) && inputType == typeof(bool))
                return bool.TryParse(Value?.ToString(), out _);
            if (inputType.IsArray && Type.IsArray)
            {
                var inputElementType = inputType.GetElementType();
                var argElementType = Type.GetElementType();
                var array = (Array)Value ?? Array.CreateInstance(argElementType ?? typeof(object), 0);
                foreach (var argElement in array)
                    if (!new ArgData(Name, argElementType, argElement, Required).IsConvertible(inputElementType))
                        return false;
                return true;
            }

            return false;
        }
    }
}