using System;
using System.Linq;
using UnityEngine;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectCommand : Command
    {
        [SerializeField] private Command _selectCommand;

        public SelectCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Not enough arguments!");

            var selectTypeAlias = args[1].ToLower();
            var selectAliases = CommandArgs.GetArgAliases(GetType(), 1);
            var selectArg1Name = selectAliases
                .Where(x => x.Value.Contains(selectTypeAlias))
                .Select(x => x.Key)
                .FirstOrDefault();

            if (selectArg1Name == null && args.Length == 2)
            {
                // var selectType = typeof(SelectGameObjectByNameCommand);
                // var subArgs = string.Join(' ', args);
                // _selectCommand = (Command)Activator.CreateInstance(selectType, subArgs);
                // _selectCommand.Run();
            }
            else
            {
                var subcommand = CommandJsonData.Get<string>($"{TypeName}.Arg1.PossibleValues.{selectArg1Name}.Subcommand");
                var extraArgs = new[] { subcommand };
                if (subcommand.Contains(" "))
                {
                    extraArgs = subcommand.Split(' ').ToArray();
                    subcommand = subcommand.Split(' ')[0];
                }
                var selectType = CommandTypes.GetByName(subcommand);
                if (selectType == null)
                    throw new ArgumentException($"Invalid type of object to select: {selectTypeAlias}");
                var subArgs = extraArgs != null ?  extraArgs.Concat(args[2..]).ToArray() : args[2..];
                var subArgsString = string.Join(' ', subArgs);
                _selectCommand = (Command)Activator.CreateInstance(selectType, subArgsString);
                _selectCommand.Run();
            }
        }

        public override void OnUndo()
        {
            _selectCommand.OnUndo();
        }

        public override void OnRedo()
        {
            _selectCommand.OnRedo();
        }
    }
}