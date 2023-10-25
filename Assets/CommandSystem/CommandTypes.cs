using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandSystem.Commands
{
    public static class CommandTypes
    {
        private static readonly Dictionary<string, Type> CommandTypeFullNameDictionary = new();
        private static readonly Dictionary<string, Type> CommandTypeNameDictionary = new();

        static CommandTypes()
        {
            Reload();
        }

        public static Type GetByFullName(string fullName) =>
            CommandTypeFullNameDictionary.TryGetValue(fullName, out var value) ? value : null;

        public static Type GetByName(string name) =>
            CommandTypeNameDictionary.TryGetValue(name, out var value) ? value : null;

        public static Type GetByAlias(string alias)
        {
            var aliasDictionary = CommandJsonData.GetKeyAndValue<string[]>("", "Aliases");
            if (aliasDictionary == null) return null;
            foreach (var entry in aliasDictionary)
            {
                if (entry.Value == null) continue;
                if (entry.Value.Contains(alias))
                    return GetByName(entry.Key);
            }

            return null;
        }

        public static void Reload()
        {
            CommandTypeFullNameDictionary.Clear();
            CommandTypeNameDictionary.Clear();
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
    }
}