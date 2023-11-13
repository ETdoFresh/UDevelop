using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandSystem
{
    public class CommandObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string[] Aliases { get; set; }
        public CommandInputDetail[] Input { get; set; }
        public CommandCallDetail[] Calls { get; set; }
        public CommandOutputDetail Output { get; set; }
        public string CommandLineOutput { get; set; }

        public override string ToString() => ToCommandletString();

        private string ToCommandletString()
        {
            var o = "";
            o += $"// Name: {Name}\n";
            o += $"// Description: {Description}\n";
            o += $"// Author: {Author}\n";
            o += $"// Aliases: {string.Join(",", Aliases)}\n";
            for (var i = 0; i < Input.Length; i++)
            {
                var input = Input[i];
                o += $"// Arg{i + 1}: {input.TypeName} {input.Name} {(input.Required ? "*" : "")}\n";
            }

            o += $"// Output: {Output.Name}\n";
            o += $"// CommandLineOutput: {CommandLineOutput}\n";

            for (var i = 0; i < Calls.Length; i++)
            {
                var call = Calls[i];
                o += "\n";
                o += string.IsNullOrEmpty(call.Command) ? call.CSharp : call.Command;
                o += string.IsNullOrEmpty(call.OutputAlias) ? "" : $" // Alias: {call.OutputAlias}";
            }

            return o;
        }

        public static CommandObject[] FromCommandletString(string commandletString)
        {
            if (string.IsNullOrWhiteSpace(commandletString)) return Array.Empty<CommandObject>();

            // Get lines and split into sections separated by lines starting with // ----- [exclude this line]
            var lines = commandletString.Split('\n');
            var sections = new List<List<string>>();
            var currentSection = new List<string>();
            foreach (var line in lines)
            {
                if (line.StartsWith("// -----"))
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
            if (callCount == 0) return null;

            var commandDetail = new CommandObject();

            if (previousCommandObject != null)
            {
                commandDetail.Name = previousCommandObject.Name;
                commandDetail.Description = previousCommandObject.Description;
                commandDetail.Author = previousCommandObject.Author;
                commandDetail.Aliases = previousCommandObject.Aliases;
            }

            foreach (var line in lines)
            {
                if (line.StartsWith("// Name: "))
                {
                    commandDetail.Name = line.Substring("// Name: ".Length);
                }
                else if (line.StartsWith("// Description: "))
                {
                    commandDetail.Description = line.Substring("// Description: ".Length);
                }
                else if (line.StartsWith("// Author: "))
                {
                    commandDetail.Author = line.Substring("// Author: ".Length);
                }
                else if (line.StartsWith("// Aliases: "))
                {
                    commandDetail.Aliases = line.Substring("// Aliases: ".Length).Split(',').Select(x => x.Trim())
                        .ToArray();
                }
                else if (line.StartsWith("// Arg"))
                {
                    var arg = new CommandInputDetail();
                    var argString = line["// Arg".Length..];
                    var colonIndex = argString.IndexOf(':');
                    argString = argString[(colonIndex + 1)..];
                    var argData = Parser.GetArgData(argString);
                    var argStrings = argData.Select(x => x.Value?.ToString()).ToArray();
                    arg.TypeName = argStrings.Length > 1 ? argStrings[0] ?? arg.TypeName : "object";
                    arg.Name = argStrings.Length > 1 ? argStrings[1] ?? arg.Name : argStrings[0] ?? arg.Name;
                    arg.Required = argString.Trim().EndsWith("*");
                    commandDetail.Input ??= Array.Empty<CommandInputDetail>();
                    commandDetail.Input = commandDetail.Input.Append(arg).ToArray();
                }
                else if (line.StartsWith("// Output: "))
                {
                    commandDetail.Output = new CommandOutputDetail { Name = line["// Output: ".Length..] };
                }
                else if (line.StartsWith("// CommandLineOutput: "))
                {
                    commandDetail.CommandLineOutput = line["// CommandLineOutput: ".Length..];
                }
                else if (!line.Trim().StartsWith("//") && !string.IsNullOrWhiteSpace(line))
                {
                    var call = new CommandCallDetail();
                    var callString = line;
                    var aliasSplit = callString.Split("// Alias: ");
                    call.OutputAlias = aliasSplit.Length > 1 ? aliasSplit[1] : null;
                    var commentSplit = callString.Split("//");
                    call.Command = commentSplit[0];
                    commandDetail.Calls ??= Array.Empty<CommandCallDetail>();
                    commandDetail.Calls = commandDetail.Calls.Append(call).ToArray();
                }
            }

            return commandDetail;
        }

        public class CommandInputDetail
        {
            public string TypeName { get; set; } = "object";
            public string Name { get; set; }
            public bool Required { get; set; }
        }

        public class CommandCallDetail
        {
            public string OutputTypeName { get; set; } = "object";
            public string OutputAlias { get; set; }
            public string CSharp { get; set; }
            public string Command { get; set; }
        }

        public class CommandOutputDetail
        {
            public string TypeName { get; set; } = "object";
            public string Name { get; set; }
        }
    }
}