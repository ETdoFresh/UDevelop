using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

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
                var selectType = typeof(SelectGameObjectByNameCommand);
                var subArgs = string.Join(' ', args);
                _selectCommand = (Command)Activator.CreateInstance(selectType, subArgs);
                _selectCommand.Run();
            }
            else
            {
                var selectTypeName =
                    CommandJsonData.Get<string>($"{TypeName}.Arg1.PossibleValues.{selectArg1Name}.Subcommand");
                var selectType = CommandTypes.GetByName(selectTypeName);
                if (selectType == null)
                    throw new ArgumentException($"Invalid type of object to select: {selectTypeAlias}");
                var subArgs = string.Join(' ', args.Skip(1));
                _selectCommand = (Command)Activator.CreateInstance(selectType, subArgs);
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