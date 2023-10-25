using System;
using System.Linq;
using UnityEngine;

namespace CommandSystem.Commands.Create
{
    [Serializable]
    public class CreateCommand : Command
    {
        [SerializeField] private Command _createCommand;
        
        public CreateCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Please specify type of object to create!");
            var arg1 = args[1];
            var arg1Aliases = CommandArgs.GetArgAliases(GetType(), 1);
            var arg1Name = arg1Aliases
                .Where(x => x.Value.Contains(arg1))
                .Select(x => x.Key)
                .FirstOrDefault();
            var arg1SubCommandTypeName = CommandJsonData.Get<string>($"{TypeName}.Arg1.PossibleValues.{arg1Name}.SubCommand");
            var arg1SubCommandType = CommandTypes.GetByName(arg1SubCommandTypeName);
            if (arg1SubCommandType == null) throw new ArgumentException($"Invalid type of object to create: {arg1}");
            var subArgs = args.Skip(1).ToArray();
            _createCommand = (Command)Activator.CreateInstance(arg1SubCommandType, subArgs);
            _createCommand.Run();
        }
        
        public override void OnUndo()
        {
            _createCommand.OnUndo();
        }
        
        public override void OnRedo()
        {
            _createCommand.OnRedo();
        }
    }
}