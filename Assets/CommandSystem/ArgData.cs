using System;
using System.Linq;

namespace CommandSystem
{
    public class ArgData
    {
        public string Name { get; }
        public Type Type { get; }
        public object Value { get; }
        public bool Required { get; }

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
            
            if (Name == "{null}")
                return !(inputType == typeof(int) ||
                         inputType == typeof(float) ||
                         inputType == typeof(double) ||
                         inputType == typeof(bool) ||
                         inputType.IsEnum);

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
            if (inputType.IsArray && (Type.IsArray || Value?.GetType().IsArray == true))
            {
                var inputElementType = inputType.GetElementType();
                if (inputElementType == null) return false;
                return ((Array)Value).Cast<object>().All(item => inputElementType.IsInstanceOfType(item));
            }

            return false;
        }

        public ArgData ConvertType(Type toType)
        {
            if (toType == null) return this;
            var obj = Value;
            if (obj == null) return this;
            var fromType = obj.GetType();
            if (fromType == toType) return this;
            if (toType.IsAssignableFrom(fromType)) return this;
            if (toType.IsEnum && fromType == typeof(int))
                return new ArgData(Name, toType, Enum.Parse(toType, obj.ToString()));
            if (toType.IsEnum && fromType == typeof(string))
                return new ArgData(Name, toType, Enum.Parse(toType, obj.ToString()));
            if (toType == typeof(int) && fromType == typeof(string))
                return new ArgData(Name, toType, int.Parse(obj.ToString()));
            if (toType == typeof(float) && fromType == typeof(string))
                return new ArgData(Name, toType, float.Parse(obj.ToString()));
            if (toType == typeof(double) && fromType == typeof(string))
                return new ArgData(Name, toType, double.Parse(obj.ToString()));
            if (toType == typeof(bool) && fromType == typeof(string))
                return new ArgData(Name, toType, bool.Parse(obj.ToString()));

            if (toType.IsArray && fromType.IsArray)
            {
                // Convert array types
                var fromElementType = fromType.GetElementType();
                var toElementType = toType.GetElementType();
                if (fromElementType == toElementType) return this;
                var fromArray = (Array)obj;
                var toArray = Array.CreateInstance(toElementType, fromArray.Length);
                for (var i = 0; i < fromArray.Length; i++)
                {
                    var fromElement = fromArray.GetValue(i);
                    var fromElementOutput = new ArgData(Name, fromElementType, fromElement);
                    var toElement = fromElementOutput.ConvertType(toElementType);
                    toArray.SetValue(toElement.Value, i);
                }

                return new ArgData(Name, toType, toArray);
            }

            return obj is IConvertible ? new ArgData(Name, toType, Convert.ChangeType(obj, toType)) : this;
        }

        public ArgData Rename(string newName)
        {
            return new ArgData(newName, Type, Value, Required);
        }
    }
}