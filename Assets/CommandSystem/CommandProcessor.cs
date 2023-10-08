using System;
using System.Collections.Generic;
using CommandSystem.Commands;
using ETdoFresh.UnityPackages.EventBusSystem;
using UnityEngine;

namespace CommandSystem
{
    public class CommandProcessor : MonoBehaviour
    {
        [SerializeField] private List<CommandEntry> possibleCommands = new();
        [SerializeField] private List<Command> commandHistory = new();
        [SerializeField] private int commandHistoryIndex = 0;

        private void Awake()
        {
            possibleCommands.Add(new CommandEntry{ commandName = "create", commandType = typeof(CreatePrimitiveCommand) });
            possibleCommands.Add(new CommandEntry{ commandName = "destroy", commandType = typeof(DestroyCommand) });
            possibleCommands.Add(new CommandEntry{ commandName = "move", commandType = typeof(MoveCommand) });
        }

        private void OnEnable()
        {
            EventBus.AddListener<CommandEvent>(OnCommandEvent);
        }
        
        private void OnDisable()
        {
            EventBus.RemoveListener<CommandEvent>(OnCommandEvent);
        }
        
        private void OnCommandEvent(CommandEvent e)
        {
            var commands = e.Command.Split('\n');
            foreach (var command in commands)
            {
                var trimmedCommand = command.Trim();
                var commandName = trimmedCommand.Split(' ')[0];

                if (string.Equals(commandName, "undo", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (commandHistoryIndex <= 0) continue;
                    commandHistoryIndex--;
                    commandHistory[commandHistoryIndex].Undo();
                    continue;
                }
                if (string.Equals(commandName, "redo", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (commandHistoryIndex >= commandHistory.Count) continue;
                    commandHistory[commandHistoryIndex].Redo();
                    commandHistoryIndex++;
                    continue;
                }
            
                var commandEntry = possibleCommands.Find(entry => string.Equals(entry.commandName, commandName, StringComparison.CurrentCultureIgnoreCase));
                if (commandEntry == null)
                {
                    Debug.LogError($"Command {commandName} not found!");
                    continue;
                }
                var commandType = commandEntry.commandType;
                var commandInstance = Activator.CreateInstance(commandType, trimmedCommand);
                commandHistory.Insert(commandHistoryIndex, (Command) commandInstance);
                commandHistoryIndex++;
                commandHistory.RemoveRange(commandHistoryIndex, commandHistory.Count - commandHistoryIndex);
            }
        }
        
        [ContextMenu("Debug Log Command History")]
        private void DebugLogCommandHistory()
        {
            var output = "";
            foreach (var command in commandHistory) 
                output += $"{command.CommandInput}\n";
            Debug.Log(output);
        }
    }

    [Serializable]
    public class CommandEntry
    {
        public string commandName;
        public Type commandType;
    }
}