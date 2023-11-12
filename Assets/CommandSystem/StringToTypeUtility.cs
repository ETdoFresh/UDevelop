using System;
using System.Collections.Generic;

namespace CommandSystem
{
    public static class StringToTypeUtility
    {
        private static readonly Dictionary<string, Type> Types = new();
        
        public static Type Get(string typeString)
        {
            if (Types.Count == 0) PrePopulatePrimitiveTypes();
            if (Types.TryGetValue(typeString, out var type)) return type;
            var typeFound = Type.GetType(typeString);
            if (typeFound != null)
            {
                Types.Add(typeString, typeFound);
                return typeFound;
            }
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                typeFound = assembly.GetType(typeString);
                if (typeFound == null) continue;
                Types.Add(typeString, typeFound);
                return typeFound;
            }
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var typeInAssembly in assembly.GetTypes())
                {
                    if (!string.Equals(typeInAssembly.Name, typeString, StringComparison.CurrentCultureIgnoreCase)) continue;
                    Types.Add(typeString, typeInAssembly);
                    return typeInAssembly;
                }
            }
            return null;
        }

        private static void PrePopulatePrimitiveTypes()
        {
            Types.Add("bool", typeof(bool));
            Types.Add("byte", typeof(byte));
            Types.Add("sbyte", typeof(sbyte));
            Types.Add("char", typeof(char));
            Types.Add("decimal", typeof(decimal));
            Types.Add("double", typeof(double));
            Types.Add("float", typeof(float));
            Types.Add("int", typeof(int));
            Types.Add("uint", typeof(uint));
            Types.Add("long", typeof(long));
            Types.Add("ulong", typeof(ulong));
            Types.Add("object", typeof(object));
            Types.Add("short", typeof(short));
            Types.Add("ushort", typeof(ushort));
            Types.Add("string", typeof(string));
            
            Types.Add("bool[]", typeof(bool[]));
            Types.Add("byte[]", typeof(byte[]));
            Types.Add("sbyte[]", typeof(sbyte[]));
            Types.Add("char[]", typeof(char[]));
            Types.Add("decimal[]", typeof(decimal[]));
            Types.Add("double[]", typeof(double[]));
            Types.Add("float[]", typeof(float[]));
            Types.Add("int[]", typeof(int[]));
            Types.Add("uint[]", typeof(uint[]));
            Types.Add("long[]", typeof(long[]));
            Types.Add("ulong[]", typeof(ulong[]));
            Types.Add("object[]", typeof(object[]));
            Types.Add("short[]", typeof(short[]));
            Types.Add("ushort[]", typeof(ushort[]));
            Types.Add("string[]", typeof(string[]));
            
        }
    }
}