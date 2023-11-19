using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using static System.Reflection.BindingFlags;

namespace CommandSystem
{
    public class CommandObject
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

        public override string ToString() => ToCommandletString();

        private string ToCommandletString()
        {
            var o = "";
            o += $"Name: {Name}\n";
            o += $"Description: {Description}\n";
            o += $"Author: {Author}\n";
            o += $"Aliases: {string.Join(",", Aliases)}\n";
            for (var i = 0; i < Input.Length; i++)
            {
                var input = Input[i];
                o += $"Arg{i + 1}: {input.Type} {input.Name} {(input.Required ? "*" : "")}\n";
            }

            o += $"Output: {Output.Name}\n";
            o += $"CommandLineOutput: {CommandLineOutput}\n";

            for (var i = 0; i < Calls.Length; i++)
            {
                var call = Calls[i];
                o += "\n";
                o += string.IsNullOrEmpty(call.Command) ? call.CSharp : call.Command;
                o += string.IsNullOrEmpty(call.Name) ? "" : $" >> {call.Name}";
            }

            return o;
        }

        public static CommandObject[] FromCommandletString(string commandletString)
        {
            if (string.IsNullOrWhiteSpace(commandletString)) return Array.Empty<CommandObject>();

            // Get lines and split into sections separated by lines starting with ---- [exclude this line]
            var lines = commandletString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var sections = new List<List<string>>();
            var currentSection = new List<string>();
            foreach (var line in lines)
            {
                if (line.StartsWith("----"))
                {
                    sections.Add(currentSection);
                    currentSection = new List<string>();
                }
                else
                {
                    currentSection.Add(line);
                }
            }

            sections.Add(currentSection);

            var commandDetails = new List<CommandObject>();
            var previousCommandDetail = (CommandObject)null;
            for (var i = 0; i < sections.Count; i++)
            {
                var section = sections[i];
                if (section.Count == 0) continue;

                var commandDetail = GetCommandDetailFromCommandletSection(previousCommandDetail, section);
                if (commandDetail == null) continue;
                commandDetails.Add(commandDetail);
                previousCommandDetail = commandDetail;
            }

            return commandDetails.ToArray();
        }

        public static CommandObject GetCommandDetailFromCommandletSection(CommandObject previousCommandObject,
            List<string> lines)
        {
            var callCount = lines.Count(line => !line.Trim().StartsWith("//") && !string.IsNullOrWhiteSpace(line));
            callCount += lines.Count(line => line.StartsWith("Name: NoOp"));
            if (callCount == 0) return null;

            var commandDetail = new CommandObject();

            if (previousCommandObject != null)
            {
                commandDetail.Name = previousCommandObject.Name;
                commandDetail.Version = previousCommandObject.Version;
                commandDetail.Description = previousCommandObject.Description;
                commandDetail.Author = previousCommandObject.Author;
                commandDetail.Aliases = previousCommandObject.Aliases;
            }

            foreach (var line in lines)
            {
                if (line.StartsWith("Filename: "))
                {
                    var filename = line.Substring("Filename: ".Length);
                    var indexOfLastPeriod = filename.LastIndexOf('.');
                    var name = indexOfLastPeriod > -1 ? filename[..indexOfLastPeriod] : filename;
                    var versionString = indexOfLastPeriod > -1 ? filename[(indexOfLastPeriod + 1)..] : null;
                    versionString = versionString?.Replace("v", "").Replace("V", "");
                    var version = int.TryParse(versionString, out var versionInt) ? versionInt : 0;
                    commandDetail.Name = name;
                    commandDetail.Version = version;
                    commandDetail.Description = $"{name} v{version}";
                    commandDetail.Author = "Unknown";
                    commandDetail.Aliases = new[] { name.ToLower() };
                }
                else if (line.StartsWith("Name: "))
                {
                    commandDetail.Name = line["Name: ".Length..];
                }
                else if (line.StartsWith("Version: "))
                {
                    var versionString = line["Version: ".Length..];
                    commandDetail.Version = int.TryParse(versionString, out var version) ? version : 0;
                }
                else if (line.StartsWith("Description: "))
                {
                    commandDetail.Description = line["Description: ".Length..];
                }
                else if (line.StartsWith("Author: "))
                {
                    commandDetail.Author = line["Author: ".Length..];
                }
                else if (line.StartsWith("Aliases: "))
                {
                    commandDetail.Aliases = line["Aliases: ".Length..].Split(',').Select(x => x.Trim())
                        .ToArray();
                }
                else if (line.StartsWith("Arg"))
                {
                    var arg = new CommandInputDetail();
                    var argString = line["Arg".Length..];
                    var colonIndex = argString.IndexOf(':');
                    argString = argString[(colonIndex + 1)..];
                    var argData = Parser.GetArgData(argString);
                    var argStrings = argData.Select(x => x.Value?.ToString()).ToArray();
                    arg.Type = argStrings.Length > 1 ? argStrings[0] ?? arg.Type : "object";
                    arg.Name = argStrings.Length > 1 ? argStrings[1] ?? arg.Name : argStrings[0] ?? arg.Name;
                    arg.Required = argString.Trim().EndsWith("*");
                    commandDetail.Input ??= Array.Empty<CommandInputDetail>();
                    commandDetail.Input = commandDetail.Input.Append(arg).ToArray();
                }
                else if (line.StartsWith("Output1: "))
                {
                    var outputString = line["Output1: ".Length..];
                    var args = Parser.GetArgData(outputString);
                    commandDetail.Output = new CommandOutputDetail();
                    commandDetail.Output.Type = args.Length > 1 ? args[0].Value?.ToString() ?? "object" : "object";
                    commandDetail.Output.Name = args.Length > 1 ? args[1].Value?.ToString() ?? "void" : args[0].Value?.ToString() ?? "void";
                }
                else if (line.StartsWith("Output0: "))
                {
                    commandDetail.CommandLineOutput = line["Output0: ".Length..];
                }
                else if (!line.Trim().StartsWith("//") && !string.IsNullOrWhiteSpace(line))
                {
                    var call = new CommandCallDetail();
                    var callString = line;
                    var aliasSplit = callString.Split(">> ");
                    call.Name = aliasSplit.Length > 1 ? aliasSplit[1] : null;
                    var commentSplit = callString.Split(new [] {"//", ">>"}, StringSplitOptions.RemoveEmptyEntries);
                    call.Command = commentSplit[0];
                    commandDetail.Calls ??= Array.Empty<CommandCallDetail>();
                    commandDetail.Calls = commandDetail.Calls.Append(call).ToArray();
                }
            }

            return commandDetail;
        }

        private string ToJsonString()
        {
            var jObject = new JObject();
            jObject["Name"] = Name;
            jObject["Version"] = Version;
            jObject["Description"] = Description;
            jObject["Author"] = Author;
            jObject["Aliases"] = new JArray(Aliases);
            jObject["Input"] = new JArray(Input.Select(x =>
            {
                var jObject2 = new JObject();
                jObject2["Type"] = x.Type;
                jObject2["Name"] = x.Name;
                jObject2["Required"] = x.Required;
                return jObject2;
            }));
            jObject["Calls"] = new JArray(Calls.Select(x =>
            {
                var jObject2 = new JObject();
                jObject2["Type"] = x.Type;
                jObject2["Name"] = x.Name;
                jObject2["CSharp"] = x.CSharp;
                jObject2["Command"] = x.Command;
                return jObject2;
            }));
            jObject["Output1"] = new JObject
            {
                ["Type"] = Output.Type,
                ["Name"] = Output.Name
            };
            jObject["Output0"] = CommandLineOutput;
            return jObject.ToString();
        }
        
        public static CommandObject[] FromJsonString(string jsonString)
        {
            var jObject = JObject.Parse(jsonString);
            var overloads = jObject["Overloads"] as JArray;
            var commandDetails = new CommandObject[overloads?.Count ?? 0];
            var previousCommandDetail = (CommandObject)null;
            for (var i = 0; i < (overloads?.Count ?? 0); i++)
            {
                var overload = overloads[i];
                var commandDetail = new CommandObject();
                if (previousCommandDetail != null)
                {
                    commandDetail.Name = previousCommandDetail.Name;
                    commandDetail.Version = previousCommandDetail.Version;
                    commandDetail.Description = previousCommandDetail.Description;
                    commandDetail.Author = previousCommandDetail.Author;
                    commandDetail.Aliases = previousCommandDetail.Aliases;
                }
                commandDetail.Name = jObject["Name"]?.ToString() ?? commandDetail.Name;
                commandDetail.Version = jObject["Version"]?.ToObject<int>() ?? commandDetail.Version;
                commandDetail.Description = jObject["Description"]?.ToString() ?? commandDetail.Description;
                commandDetail.Author = jObject["Author"]?.ToString() ?? commandDetail.Author;
                commandDetail.Aliases = jObject["Aliases"]?.ToObject<string[]>() ?? commandDetail.Aliases;
                commandDetail.Input = overload["Input"]?.ToObject<CommandInputDetail[]>();
                commandDetail.Calls = overload["Calls"]?.ToObject<CommandCallDetail[]>();
                commandDetail.Output = overload["Output1"]?.ToObject<CommandOutputDetail>();
                commandDetail.CommandLineOutput = overload["Output0"]?.ToString();
                commandDetails[i] = commandDetail;
                previousCommandDetail = commandDetail;
            }
            return commandDetails;
        }

        private OutputData Run(ArgData[] args, Dictionary<string, ArgData> argMemory)
        {
            argMemory = new Dictionary<string, ArgData>(argMemory);
            for (var i = 0; i < (Input?.Length ?? 0); i++)
            {
                var argValue = i < args.Length ? args[i].Value : null;
                var argName = Input[i].Name;
                var argRequired = Input[i].Required;
                var argTypeString = Input[i].Type;
                var argType = StringToTypeUtility.Get(argTypeString);
                argMemory[argName] = new ArgData(argName, argType, argValue, argRequired);
                argMemory[$"{{Arg{i + 1}}}"] = new ArgData($"{{Arg{i + 1}}}", argType, argValue, argRequired);
            }

            for (var i = 0; i < (Calls?.Length ?? 0); i++)
            {
                try
                {
                    var call = Calls?[i];
                    if (call?.Type != "void")
                    {
                        var callType = StringToTypeUtility.Get(call?.Type);
                        if (TryRunCSharp(call?.CSharp, callType, argMemory, out var callOutput))
                        {
                            if (call != null && call.Name != null)
                                argMemory[call.Name] = new ArgData(call.Name, callType, callOutput?.Value);
                            argMemory[$"{{Step{i + 1}}}"] = new ArgData($"{{Step{i + 1}}}", callType, callOutput?.Value);

                            if (showIntermediateCommandLineOutput)
                                CommandHandlerScriptableObject.Display.Add(GetCommandLineOutput(call.Name, callOutput.CommandLineOutput, argMemory));
                        }
                        else if (TryRun(call?.Command, callType, argMemory, out callOutput))
                        {
                            if (call != null && call.Name != null)
                                argMemory[call.Name] = new ArgData(call.Name, callType, callOutput?.Value);
                            argMemory[$"{{Step{i + 1}}}"] = new ArgData($"{{Step{i + 1}}}", callType, callOutput?.Value);
                            
                            if (showIntermediateCommandLineOutput)
                                CommandHandlerScriptableObject.Display.Add(GetCommandLineOutput(call.Name, callOutput.CommandLineOutput, argMemory));
                        }
                        else
                        {
                            ThrowException("No CSharp or Command found!", Name, argMemory, args);
                        }
                    }
                    else
                    {
                        if (TryRunCSharp(call?.CSharp, null, argMemory, out var callOutput))
                        {
                            if (showIntermediateCommandLineOutput)
                                CommandHandlerScriptableObject.Display.Add(GetCommandLineOutput(call.Name, callOutput.CommandLineOutput, argMemory));
                        }
                        else if (TryRun(call?.Command, null, argMemory, out callOutput))
                        {
                            if (showIntermediateCommandLineOutput)
                                CommandHandlerScriptableObject.Display.Add(GetCommandLineOutput(call.Name, callOutput.CommandLineOutput, argMemory));
                        }
                        else ThrowException("No CSharp or Command found!", Name, argMemory, args);
                    }
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    var call = Calls?[i];
                    var callString = call?.CSharp ?? call?.Command;
                    message += $"\nCall[{i}]: {callString}";
                    throw new Exception(message, ex);
                }
            }

            var outputName = Output?.Name;
            var outputValue = outputName != null && outputName != "void" ? argMemory[outputName].Value : null;
            var commandLineOutput = GetCommandLineOutput(outputName, CommandLineOutput, argMemory);
            return new OutputData { Value = outputValue, CommandLineOutput = commandLineOutput };
        }

        private string GetCommandLineOutput(string outputName, string lineOutput, Dictionary<string, ArgData> argMemory)
        {
            var commandLineOutput = lineOutput ?? outputName ?? "";
            commandLineOutput = commandLineOutput?.Replace("\\n", "\n");
            var outputRegEx = new System.Text.RegularExpressions.Regex(@"\{.*?\}");
            commandLineOutput = outputRegEx.Replace(commandLineOutput, m => argMemory[m.Value].Value?.ToString() ?? "null");
            return commandLineOutput;
        }

        public static bool TryRun(string commandString, out OutputData outputData)
        {
            return TryRun(commandString, null, new Dictionary<string, ArgData>(), out outputData);
        }

        public static bool TryRun(string commandString, Type outputType, Dictionary<string, ArgData> argMemory,
            out OutputData outputData)
        {
            if (!argMemory.ContainsKey("{void}")) 
                argMemory["{void}"] = new ArgData("{void}", typeof(void), null);
            
            if (string.IsNullOrWhiteSpace(commandString))
            {
                outputData = null;
                return false;
            }
            
            if (commandString.Equals("CommandSystem.CSharp.Execute({CommandInput})"))
            {
                var commandInput = argMemory["{CommandInput}"].Value?.ToString();
                var spaceIndex = commandInput.IndexOf(' ');
                var commandInputWithoutCommand = spaceIndex > 0 ? commandInput[(spaceIndex + 1)..] : "";
                var outputValue = CSharp.Execute(commandInputWithoutCommand, argMemory);
                outputData = new OutputData { Value = outputValue, CommandLineOutput = commandString };
                return true;
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
                foreach (var commandObject in commandObjects)
                {
                    if (!IsValidOverload(commandObject, commandArgs)) continue;
                    argMemory = new Dictionary<string, ArgData>(argMemory);
                    argMemory["{CommandInput}"] = new ArgData("{CommandInput}", typeof(string), commandString);
                    outputData = commandObject.Run(commandArgs, argMemory);
                    outputData = ConvertType(outputData, outputType);
                    return true;
                }
                
                ThrowException("No valid overload found!", commandString, argMemory, commandArgs);
            }
            ThrowException($"Command {commandAlias} not found!", commandString, argMemory, commandArgs);
            outputData = null;
            return false;
        }

        private static bool TryRunCSharp(string cSharpCode, Type outputType, Dictionary<string, ArgData> argMemory,
            out OutputData outputData)
        {
            if (string.IsNullOrWhiteSpace(cSharpCode))
            {
                outputData = null;
                return false;
            }
            
            // New Example
            // new UnityEngine.GameObject({GameObject Name}, {Component Types})
            // Looks up {GameObject Name} in localRunValues and cast as localRunType lookup
            // Looks up {Component Types} in localRunValues and cast as localRunType lookup
            if (cSharpCode.StartsWith("new"))
            {
                var withoutNew = cSharpCode[3..];
                var split = withoutNew.Split('(');
                var typeString = split[0];
                var argsString = split[1][..^1];
                var argStrings = argsString.Split(',').Select(x => x.Trim()).ToArray();
                var type = StringToTypeUtility.Get(typeString);
                var argObjects = new List<object>();
                for (var i = 0; i < argStrings.Length; i++)
                {
                    var argString = argStrings[i];
                    if (!argMemory.TryGetValue(argString, out var arg)) continue;
                    if (arg.Required || arg.Value != null)
                        argObjects.Add(arg.Value);
                }

                var outputValue = Activator.CreateInstance(type, argObjects.ToArray());
                outputData = new OutputData { Value = outputValue, CommandLineOutput = cSharpCode };
                return true;
            }
            else
            {
                // Function Call Example
                // UnityEngine.GameObject.Find({GameObject Name})
                // Type: UnityEngine.GameObject
                // Function: Find
                // Args: {GameObject Name}
                var split = cSharpCode.Split('(');

                if (split.Length == 1)
                {
                    if (outputType.IsEnum)
                    {
                        var flagSplit = cSharpCode.Split('|');
                        var enumValue = 0;
                        foreach (var flag in flagSplit)
                        {
                            var flagString = flag.Trim();
                            var flagValue = (int)Enum.Parse(outputType, flagString);
                            enumValue |= flagValue;
                        }

                        var outputValue = enumValue;
                        outputData = new OutputData { Value = outputValue, CommandLineOutput = cSharpCode };
                        return true;
                    }

                    // Else return c# code as string
                    {
                        var outputValue = cSharpCode;
                        outputData = new OutputData { Value = outputValue, CommandLineOutput = cSharpCode };
                        return true;
                    }
                }

                var methodSplit = split[0].Split('.');
                var methodName = methodSplit[^1];
                var fullTypeName = methodSplit[..^1];
                var fullTypeString = string.Join('.', fullTypeName);
                var type = StringToTypeUtility.Get(fullTypeString);

                // Else run method
                var argsString = split[1][..^1];
                var argStrings = string.IsNullOrEmpty(argsString)
                    ? Array.Empty<string>()
                    : argsString.Split(',').Select(x => x.Trim()).ToArray();

                var self = (ArgData)null;
                if (argStrings.Length > 0 && argStrings[0].StartsWith("this"))
                {
                    var selfString = argStrings[0][4..].Trim();
                    self = argMemory[selfString];
                    argStrings = argStrings.Skip(1).ToArray();
                }

                var args = new ArgData[argStrings.Length];
                for (var i = 0; i < argStrings.Length; i++)
                {
                    var argString = argStrings[i];
                    if (argMemory.TryGetValue(argString, out var arg))
                        args[i] = arg;
                }

                var argTypes = args.Select(x => x?.Type ?? typeof(object)).ToArray();
                
                var argObjects = new object[argTypes.Length];
                for (var i = 0; i < argTypes.Length; i++)
                {
                    var argType = argTypes[i];
                    var argOutputData = new OutputData{ Value = args[i].Value };
                    if (args[i].IsConvertible(argType))
                        ConvertType(argOutputData, argType);
                    argObjects[i] = argOutputData.Value;
                }
                
                var bindingFlags = Public | NonPublic | Instance | Static;
                // The following works pretty well, but does not work very well for generics
                // var method = type.GetMethod(methodName, bindingFlags, null, argTypes, null);

                // To make generics work, we first try above method, then try MakeGenericMethod
                var method = type.GetMethod(methodName, bindingFlags, null, argTypes, null);
                if (method == null)
                {
                    var isLinq = type.FullName.StartsWith("System.Linq");
                    if (isLinq)
                    {
                        var genericMethod = type.GetMethods(bindingFlags).FirstOrDefault(x =>
                            x.Name == methodName && x.GetParameters().Length == argTypes.Length);
                        var typeFromArray = (Type)null;
                        if (self != null)
                            typeFromArray = self.GetType().IsArray ? self.GetType().GetElementType() : self.GetType();
                        else if (argObjects.Length > 0)
                            typeFromArray = argTypes[0].IsArray ? argTypes[0].GetElementType() : argTypes[0];
                        method = genericMethod?.MakeGenericMethod(typeFromArray);
                    }
                    else
                    {
                        method = type.GetMethods(bindingFlags).FirstOrDefault(x =>
                            x.Name == methodName && x.GetParameters().Length == argTypes.Length);
                    }
                }

                if (method == null) ThrowException("Method not found!", cSharpCode, argMemory);

                try
                {
                    
                    var outputValue2 = method.Invoke(self?.Value, argObjects);
                    outputData = new OutputData { Value = outputValue2, CommandLineOutput = cSharpCode };
                    return true;
                }
                catch (TargetInvocationException ex)
                {
                    ThrowException(ex.InnerException, cSharpCode, argMemory);
                }
                catch (Exception ex)
                {
                    ThrowException(ex, cSharpCode, argMemory);
                }
                outputData = null;
                return false;
            }
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
        
        private static OutputData ConvertType(OutputData outputData, Type toType)
        {
            if (toType == null) return outputData;
            var obj = outputData.Value;
            if (obj == null) return outputData;
            var fromType = obj.GetType();
            if (fromType == toType) return outputData;
            if (toType.IsAssignableFrom(fromType)) return outputData;
            if (toType.IsEnum && fromType == typeof(int)) return outputData;
            if (toType.IsEnum && fromType == typeof(string))
                return outputData.Replace(Enum.Parse(toType, obj.ToString()));
            if (toType == typeof(int) && fromType == typeof(string))
                return outputData.Replace(int.Parse(obj.ToString()));
            if (toType == typeof(float) && fromType == typeof(string))
                return outputData.Replace(float.Parse(obj.ToString()));
            if (toType == typeof(double) && fromType == typeof(string))
                return outputData.Replace(double.Parse(obj.ToString()));
            if (toType == typeof(bool) && fromType == typeof(string))
                return outputData.Replace(bool.Parse(obj.ToString()));

            if (toType.IsArray && fromType.IsArray)
            {
                // Convert array types
                var fromElementType = fromType.GetElementType();
                var toElementType = toType.GetElementType();
                if (fromElementType == toElementType) return outputData;
                var fromArray = (Array)obj;
                var toArray = Array.CreateInstance(toElementType, fromArray.Length);
                for (var i = 0; i < fromArray.Length; i++)
                {
                    var fromElement = fromArray.GetValue(i);
                    var fromElementOutput = new OutputData { Value = fromElement };
                    var toElement = ConvertType(fromElementOutput, toElementType);
                    toArray.SetValue(toElement.Value, i);
                }

                return outputData.Replace(toArray);
            }

            return obj is IConvertible ? outputData.Replace(Convert.ChangeType(obj, toType)) : outputData;
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