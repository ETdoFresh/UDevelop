using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommandSystem
{
    public partial class CommandObject
    {
        private static bool showIntermediateCommandLineOutput = false;

        public string Name { get; set; }
        public int Version { get; set; } = -1;
        public string Description { get; set; }
        public string Author { get; set; } = "Unknown";
        public string[] Aliases { get; set; }
        public CommandInputDetail[] Input { get; set; }
        public CommandCallDetail[] Calls { get; set; }
        public CommandOutputDetail Output { get; set; }
        public string CommandLineOutput { get; set; }

        private Dictionary<string, ArgData> Run(Dictionary<string, ArgData> argMemory, string commandString)
        {
            argMemory ??= new Dictionary<string, ArgData>();
            argMemory = new Dictionary<string, ArgData>(argMemory);
            argMemory.TryAdd("{void}", new ArgData("{void}", typeof(void), null));
            argMemory.TryAdd("{null}", new ArgData("{null}", typeof(object), null));
            argMemory.TryAdd("Depth", new ArgData("Depth", typeof(int), 0));
            currentArgMemory = argMemory;

            // Copying Input Args to Named Input Args in argMemory
            var argKeys = argMemory.Keys.Where(x => x.StartsWith("{Arg")).OrderBy(x => x).ToArray();
            for (var i = 0; i < (Input?.Length ?? 0); i++)
            {
                var argName = Input?[i]?.Name;
                if (argName == null) continue;
                var argRequired = Input?[i]?.Required ?? false;
                if (argKeys.Length <= i && !argRequired) continue;
                if (argKeys.Length <= i && argRequired) throw new Exception($"Required arg {argName} not found!");
                var argKey = argKeys[i];
                argMemory[argName] = argMemory[argKey];
            }

            // Converting Input Arg Types to Command Input Types
            for (var i = 0; i < (Input?.Length ?? 0); i++)
            {
                var argName = Input[i].Name;
                var argRequired = Input[i].Required;
                var argTypeString = Input[i].Type;
                var argType = StringToTypeUtility.Get(argTypeString);
                if (!argMemory.ContainsKey(argName))
                {
                    if (argRequired)
                        throw new Exception($"Required arg {argName} not found!");
                    argMemory[argName] = new ArgData(argName, argType, null, argRequired);
                }
            }

            // Run each call and put output into argMemory (if >>)
            argMemory["Depth"] = new ArgData("Depth", typeof(int), (int) argMemory["Depth"].Value + 1);
            for (var i = 0; i < (Calls?.Length ?? 0); i++)
            {
                var call = Calls?[i];
                if (call?.Command?.ToLower().StartsWith("csharp") ?? false)
                {
                    var newArgMemory = CSharp.Execute(call?.Command, argMemory);
                    if (call?.Name != null) argMemory[call.Name] = newArgMemory["{Output1}"];
                }
                else
                {
                    var newArgMemory = RunCommandString(call?.Command, argMemory);
                    if (call?.Name != null) argMemory[call.Name] = newArgMemory["{Output1}"];
                }
            }
            argMemory["Depth"] = new ArgData("Depth", typeof(int), (int) argMemory["Depth"].Value - 1);

            // Setup Output Keys and Return
            argMemory["{Output0}"] = GetCommandLineOutput(CommandLineOutput, argMemory);
            if (Output?.Name != null) argMemory["{Output1}"] = argMemory[Output.Name];

            if (!showIntermediateCommandLineOutput) return argMemory;
            
            var depth = (int) argMemory["Depth"].Value + 1;
            
            var chevrons = new string('>', depth) + " ";
            CommandHandlerScriptableObject.Display.Add(chevrons + commandString);
            
            if (depth <= 1) return argMemory;
            CommandHandlerScriptableObject.Display.Add(argMemory["{Output0}"].Value?.ToString());
            return argMemory;
        }

        public static Dictionary<string, ArgData> RunCommandString(string commandString,
            Dictionary<string, ArgData> argMemory)
        {
            argMemory ??= new Dictionary<string, ArgData>();
            argMemory = new Dictionary<string, ArgData>(argMemory);
            argMemory.TryAdd("{void}", new ArgData("{void}", typeof(void), null));
            currentArgMemory = argMemory;

            if (string.IsNullOrWhiteSpace(commandString))
                return argMemory;

            var commands = commandString.Split('\n');
            if (commands.Length > 1)
            {
                foreach (var command in commands)
                    argMemory = RunCommandString(command, argMemory);
                return argMemory;
            }

            var firstSpaceIndex = commandString.IndexOf(' ');
            var commandAlias = firstSpaceIndex > 0 ? commandString[..firstSpaceIndex] : commandString;
            var commandArgString = firstSpaceIndex > 0 ? commandString[(firstSpaceIndex + 1)..] : "";
            var commandArgs = Parser.GetArgData(commandArgString);
            for (var i = 0; i < commandArgs.Length; i++)
            {
                if (commandArgs[i].Type != typeof(string)) continue;
                var argString = commandArgs[i].Value?.ToString();
                if (argString == null) continue;
                if (!argMemory.TryGetValue(argString, out var arg)) continue;
                commandArgs[i] = arg;
            }

            if (CommandRunner.CommandMap.TryGetValue(commandAlias.ToLower(), out var commandObjects))
            {
                try
                {
                    foreach (var commandObject in commandObjects.OrderBy(x => Mathf.Abs(x.Input?.Length ?? 0 - commandArgs.Length)))
                    {
                        if (!IsValidOverload(commandObject, commandArgs)) continue;
                        argMemory["{CommandInput}"] = new ArgData("{CommandInput}", typeof(string), commandString);
                        for (var i = 0; i < commandArgs.Length; i++) argMemory[$"{{Arg{i}}}"] = commandArgs[i];
                        var newArgMemory = commandObject.Run(argMemory, commandString);
                        argMemory["{Output0}"] = newArgMemory["{Output0}"];
                        argMemory["{Output1}"] = newArgMemory["{Output1}"];
                        return argMemory;
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"{ex.Message}\n{commandString}", ex);
                }

                ThrowException("No valid overload found!", commandString, argMemory, commandArgs);
            }

            ThrowException($"Command {commandAlias} not found!", commandString, argMemory, commandArgs);
            return argMemory;
        }

        public ArgData GetCommandLineOutput(string commandLineOutput, Dictionary<string, ArgData> argMemory)
        {
            commandLineOutput ??= "";
            commandLineOutput = commandLineOutput.Replace("\\n", "\n");
            var outputRegEx = new System.Text.RegularExpressions.Regex(@"\{.*?\}");
            commandLineOutput =
                outputRegEx.Replace(commandLineOutput, m => argMemory[m.Value].Value?.ToString() ?? "null");
            return new ArgData("{Output0}", typeof(string), commandLineOutput);
        }

        private static bool IsValidOverload(CommandObject commandObject, ArgData[] commandArgs)
        {
            var inputMinLength = commandObject?.Input?.Count(x => x.Required) ?? 0;
            var inputMaxLength = commandObject?.Input?.Length ?? 0;
            if (commandArgs.Length < inputMinLength || commandArgs.Length > inputMaxLength) return false;

            for (var i = 0; i < (commandObject?.Input?.Length ?? 0); i++)
            {
                var input = commandObject.Input[i];
                var isInputRequired = input.Required;
                if (isInputRequired && i >= commandArgs.Length) return false;
                if (!isInputRequired && i >= commandArgs.Length) continue;
                var commandArg = commandArgs[i];
                var inputType = StringToTypeUtility.Get(input.Type);
                if (!commandArg.IsConvertible(inputType)) return false;
            }

            return true;
        }

        private static void ThrowException(string message, string commandString = null,
            Dictionary<string, ArgData> localArgs = null, ArgData[] args = null)
        {
            ThrowException(new ArgumentException(message), commandString, localArgs, args);
        }

        private static void ThrowException(Exception ex, string commandString = null,
            Dictionary<string, ArgData> localArgs = null, ArgData[] args = null)
        {
            var message = ex.Message; //ex.ToString();
            if (commandString != null) message += $"\nCommand: {commandString}";

            if (localArgs != null)
            {
                message += "\n\nArg Memory:";
                foreach (var localArg in localArgs)
                    message += $"\n{localArg.Value}";
            }

            if (args != null)
            {
                message += "\n\nArgs:";
                foreach (var arg in args)
                    message += $"\n{arg}";
            }

            message += "\n\nCommand Call Trace:";

            throw new ArgumentException(message, ex);
        }
    }
}