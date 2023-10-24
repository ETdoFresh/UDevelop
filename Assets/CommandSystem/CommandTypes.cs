using System;
using System.Collections.Generic;

namespace CommandSystem.Commands
{
    public static class CommandTypes
    {
        private static readonly Dictionary<string, Type> CommandTypeFullNameDictionary = new();
        private static readonly Dictionary<string, Type> CommandTypeNameDictionary = new();

        static CommandTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (!typeof(Command).IsAssignableFrom(type)) continue;
                    if (type == typeof(Command)) continue;
                    if (type.FullName == null) continue;
                    CommandTypeFullNameDictionary.Add(type.FullName, type);
                    CommandTypeNameDictionary.Add(type.Name, type);
                }
            }
        }
        
        public static Type GetByFullName(string fullName) => 
            CommandTypeFullNameDictionary.TryGetValue(fullName, out var value) ? value : null;
        
        public static Type GetByName(string name) =>
            CommandTypeNameDictionary.TryGetValue(name, out var value) ? value : null;
    }
}