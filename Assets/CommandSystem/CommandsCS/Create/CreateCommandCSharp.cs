using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreateCommandCSharp : CommandCSharp
    {
        [FormerlySerializedAs("_createCommand")] [SerializeField] private CommandCSharp createCommandCSharp;
        
        public CreateCommandCSharp(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            // if (args.Length < 2) throw new ArgumentException("Please specify type of object to create!");
            // var arg1 = args[1];
            // var arg1Aliases = CommandArgs.GetArgAliases(GetType(), 1);
            // var arg1Name = arg1Aliases
            //     .Where(x => x.Value.Contains(arg1))
            //     .Select(x => x.Key)
            //     .FirstOrDefault();
            // var arg1SubcommandTypeName = CommandJsonData.Get<string>($"{TypeName}.Arg1.PossibleValues.{arg1Name}.Subcommand");
            // var arg1SubcommandType = CommandTypes.GetByName(arg1SubcommandTypeName);
            // if (arg1SubcommandType == null) throw new ArgumentException($"Invalid type of object to create: {arg1}");
            // var subArgs = string.Join(' ', args.Skip(1));
            // createCommandCSharp = (CommandCSharp)Activator.CreateInstance(arg1SubcommandType, subArgs);
            // createCommandCSharp.Run();
        }
        
        public override void OnUndo()
        {
            createCommandCSharp.OnUndo();
        }
        
        public override void OnRedo()
        {
            createCommandCSharp.OnRedo();
        }
    }
}