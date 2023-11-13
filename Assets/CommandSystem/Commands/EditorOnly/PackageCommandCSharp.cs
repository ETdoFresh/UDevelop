using System;
using UnityEditor.PackageManager.Requests;

namespace CommandSystem.Commands.EditorOnly
{
    [Serializable]
    public class PackageCommandCSharp : CommandCSharp
    {
        private static ListRequest _packageListRequest;

        private string _commandOutput;

        public override string CommandOutput => _commandOutput;

        public PackageCommandCSharp(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            var action = args[1];

            switch (action)
            {
                case "add":
                    var packageName = args[2];
                    UnityEditor.PackageManager.Client.Add(packageName);
                    _commandOutput = $"Added package {packageName}";
                    break;
                case "remove":
                    var packageName2 = args[2];
                    UnityEditor.PackageManager.Client.Remove(packageName2);
                    _commandOutput = $"Removed package {packageName2}";
                    break;
                case "list":
                    if (_packageListRequest == null)
                    {
                        _packageListRequest = UnityEditor.PackageManager.Client.List();
                        _commandOutput = "Loading... Try again in a few seconds.";
                    }
                    else if (!_packageListRequest.IsCompleted)
                    {
                        _commandOutput = "Still loading... Try again in a few seconds.";
                    }
                    else
                    {
                        _commandOutput = "";
                        foreach (var package in _packageListRequest.Result)
                            _commandOutput += $"{package.name} - {package.version}\n";
                    }
                    break;
                default:
                    throw new Exception($"Package action {action} not found!");
            }
        }
    }
}