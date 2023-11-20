using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace CommandSystem
{
    public partial class CommandObject
    {
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

            var commandObjects = new List<CommandObject>();
            var previousCommandObject = (CommandObject)null;
            for (var i = 0; i < sections.Count; i++)
            {
                var section = sections[i];
                if (section.Count == 0) continue;

                var commandObject = GetCommandObjectFromSection(previousCommandObject, section);
                if (commandObject == null) continue;
                commandObjects.Add(commandObject);
                previousCommandObject = commandObject;
            }

            return commandObjects.ToArray();
        }

        public static CommandObject GetCommandObjectFromSection(CommandObject previousCommandObject, List<string> lines)
        {
            var callCount = lines.Count(line => !line.Trim().StartsWith("//") && !string.IsNullOrWhiteSpace(line));
            callCount += lines.Count(line => line.StartsWith("Name: NoOp"));
            if (callCount == 0) return null;

            var commandObject = new CommandObject();

            if (previousCommandObject != null)
            {
                commandObject.Name = previousCommandObject.Name;
                commandObject.Version = previousCommandObject.Version;
                commandObject.Description = previousCommandObject.Description;
                commandObject.Author = previousCommandObject.Author;
                commandObject.Aliases = previousCommandObject.Aliases;
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
                    commandObject.Name = name;
                    commandObject.Version = version;
                    commandObject.Description = $"{name} v{version}";
                    commandObject.Author = "Unknown";
                    commandObject.Aliases = new[] { name.ToLower() };
                }
                else if (line.StartsWith("Name: "))
                {
                    commandObject.Name = line["Name: ".Length..];
                }
                else if (line.StartsWith("Version: "))
                {
                    var versionString = line["Version: ".Length..];
                    commandObject.Version = int.TryParse(versionString, out var version) ? version : 0;
                }
                else if (line.StartsWith("Description: "))
                {
                    commandObject.Description = line["Description: ".Length..];
                }
                else if (line.StartsWith("Author: "))
                {
                    commandObject.Author = line["Author: ".Length..];
                }
                else if (line.StartsWith("Aliases: "))
                {
                    commandObject.Aliases = line["Aliases: ".Length..].Split(',').Select(x => x.Trim())
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
                    commandObject.Input ??= Array.Empty<CommandInputDetail>();
                    commandObject.Input = commandObject.Input.Append(arg).ToArray();
                }
                else if (line.StartsWith("Output1: "))
                {
                    var outputString = line["Output1: ".Length..];
                    var args = Parser.GetArgData(outputString);
                    commandObject.Output = new CommandOutputDetail();
                    commandObject.Output.Type = args.Length > 1 ? args[0].Value?.ToString() ?? "object" : "object";
                    commandObject.Output.Name = args.Length > 1 ? args[1].Value?.ToString() ?? "void" : args[0].Value?.ToString() ?? "void";
                }
                else if (line.StartsWith("Output0: "))
                {
                    commandObject.CommandLineOutput = line["Output0: ".Length..];
                }
                else if (!line.Trim().StartsWith("//") && !string.IsNullOrWhiteSpace(line))
                {
                    var call = new CommandCallDetail();
                    var commandAliasSplit = Parser.CommandAliasSplit(line);
                    call.Command = commandAliasSplit[0];
                    call.Name = commandAliasSplit.Length > 1 ? commandAliasSplit[1] : null;
                    commandObject.Calls ??= Array.Empty<CommandCallDetail>();
                    commandObject.Calls = commandObject.Calls.Append(call).ToArray();
                }
            }

            return commandObject;
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
            var commandObjects = new CommandObject[overloads?.Count ?? 0];
            var previousCommandObject = (CommandObject)null;
            for (var i = 0; i < (overloads?.Count ?? 0); i++)
            {
                var overload = overloads[i];
                var commandObject = new CommandObject();
                if (previousCommandObject != null)
                {
                    commandObject.Name = previousCommandObject.Name;
                    commandObject.Version = previousCommandObject.Version;
                    commandObject.Description = previousCommandObject.Description;
                    commandObject.Author = previousCommandObject.Author;
                    commandObject.Aliases = previousCommandObject.Aliases;
                }
                commandObject.Name = jObject["Name"]?.ToString() ?? commandObject.Name;
                commandObject.Version = jObject["Version"]?.ToObject<int>() ?? commandObject.Version;
                commandObject.Description = jObject["Description"]?.ToString() ?? commandObject.Description;
                commandObject.Author = jObject["Author"]?.ToString() ?? commandObject.Author;
                commandObject.Aliases = jObject["Aliases"]?.ToObject<string[]>() ?? commandObject.Aliases;
                commandObject.Input = overload["Input"]?.ToObject<CommandInputDetail[]>();
                commandObject.Calls = overload["Calls"]?.ToObject<CommandCallDetail[]>();
                commandObject.Output = overload["Output1"]?.ToObject<CommandOutputDetail>();
                commandObject.CommandLineOutput = overload["Output0"]?.ToString();
                commandObjects[i] = commandObject;
                previousCommandObject = commandObject;
            }
            return commandObjects;
        }
    }
}