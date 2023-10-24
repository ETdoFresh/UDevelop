using System;
using System.Collections.Generic;
using CommandSystem.Commands.Create;

namespace CommandSystem.Commands
{
    [Serializable]
    public class CreateCommand : Command
    {
        private string _commandOutput = "Created Object!";
        private Command _createCommand;
        
        public CreateCommand(string commandInput) : base(commandInput) { }

        public override bool AddToHistory => true;
        public override string CommandOutput => _commandOutput;
        
        public override string[] CommandAliases => new[] { "create", "c" };
        public override string CommandUsage => $"{CommandAliases[0]} [OBJECT_TYPE] [OBJECT_NAME]";
        public override string CommandDescription => "Creates a new object in project or scene.";
        public override Dictionary<string, string> CommandArg1Descriptions => new()
        {
            { "prefab, p", "Creates an empty .prefab object in project" },
            { "scene, s", "Creates an empty .unity object in project" },
            { "scriptableobject, so [ASSET_TYPE]", "Creates an empty .asset object of ASSET_TYPE in project" },
            { "material, m", "Creates an empty .mat object in project" },
            { "folder, f", "Creates an empty folder in project" },
            { "instance, i [PREFAB_NAME/PATH]", "Creates an instance of an object in scene" },
            { "gameobject, go", "Creates an empty GameObject in scene" },
            { "empty, e", "Creates an empty GameObject in scene" },
            { "cube", "Creates a cube GameObject in scene" },
            { "sphere", "Creates a sphere GameObject in scene" },
            { "capsule", "Creates a capsule GameObject in scene" },
            { "cylinder", "Creates a cylinder GameObject in scene" },
            { "plane", "Creates a plane GameObject in scene" },
            { "quad", "Creates a quad GameObject in scene" },
            { "light, l", "Creates a light GameObject in scene" },
            { "camera, cam", "Creates a camera GameObject in scene" },
            { "audio, a", "Creates an audio source GameObject in scene" },
            { "particle, p", "Creates a particle emitter GameObject in scene" },
            { "canvas, cv", "Creates a canvas GameObject in scene" },
            { "text, t", "Creates a text GameObject in scene" },
            { "image, im", "Creates an image GameObject in scene" },
            { "rawimage, ri", "Creates a raw image GameObject in scene" },
            { "button, b", "Creates a button GameObject in scene" },
            { "slider, sl", "Creates a slider GameObject in scene" },
            { "scrollbar, sb", "Creates a scrollbar GameObject in scene" },
            { "toggle, to", "Creates a toggle GameObject in scene" },
            { "inputfield, if", "Creates an input field GameObject in scene" },
            { "dropdown, dd", "Creates a dropdown GameObject in scene" },
            { "scrollview, sv", "Creates a scroll view GameObject in scene" },
            { "eventsystem, es", "Creates an event system GameObject in scene" },
            { "tmp_text, tmp_t", "Creates a TMP text GameObject in scene" },
            { "tmp_dropdown, tmp_dd", "Creates a TMP dropdown GameObject in scene" },
            { "tmp_inputfield, tmp_if", "Creates a TMP input field GameObject in scene" },
            { "tmp_button, tmp_b", "Creates a TMP button GameObject in scene" },
            { "tmp_toggle, tmp_to", "Creates a TMP toggle GameObject in scene" },
        };
        
        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Not enough arguments!");
            var objectType = args[1].ToLower();
            _createCommand = objectType switch
            {
                "prefab" => new CreatePrefabCommand(CommandInput),
                "p" => new CreatePrefabCommand(CommandInput),
                "scene" => new CreateSceneCommand(CommandInput),
                "s" => new CreateSceneCommand(CommandInput),
                // case "scriptableobject": _createCommand = new CreateScriptableObjectCommand(CommandInput); break;
                // case "so": _createCommand = new CreateScriptableObjectCommand(CommandInput); break;
                "material" => new CreateMaterialCommand(CommandInput),
                "m" => new CreateMaterialCommand(CommandInput),
                // case "folder": _createCommand = new CreateFolderCommand(CommandInput); break;
                // case "f": _createCommand = new CreateFolderCommand(CommandInput); break;
                // case "instance": _createCommand = new CreateInstanceCommand(CommandInput); break;
                // case "i": _createCommand = new CreateInstanceCommand(CommandInput); break;
                "gameobject" => new CreateGameObjectCommand(CommandInput),
                "go" => new CreateGameObjectCommand(CommandInput),
                "empty" => new CreateGameObjectCommand(CommandInput),
                "e" => new CreateGameObjectCommand(CommandInput),
                "cube" => new CreateCubeCommand(CommandInput),
                "sphere" => new CreateSphereCommand(CommandInput),
                "capsule" => new CreateCapsuleCommand(CommandInput),
                "cylinder" => new CreateCylinderCommand(CommandInput),
                "plane" => new CreatePlaneCommand(CommandInput),
                "quad" => new CreateQuadCommand(CommandInput),
                "light" => new CreateLightCommand(CommandInput),
                "l" => new CreateLightCommand(CommandInput),
                "camera" => new CreateCameraCommand(CommandInput),
                "cam" => new CreateCameraCommand(CommandInput),
                // case "audio": _createCommand = new CreateAudioCommand(CommandInput); break;
                // case "a": _createCommand = new CreateAudioCommand(CommandInput); break;
                // case "particle": _createCommand = new CreateParticleCommand(CommandInput); break;
                // case "p": _createCommand = new CreateParticleCommand(CommandInput); break;
                // case "canvas": _createCommand = new CreateCanvasCommand(CommandInput); break;
                // case "cv": _createCommand = new CreateCanvasCommand(CommandInput); break;
                // case "text": _createCommand = new CreateTextCommand(CommandInput); break;
                // case "t": _createCommand = new CreateTextCommand(CommandInput); break;
                // case "image": _createCommand = new CreateImageCommand(CommandInput); break;
                // case "im": _createCommand = new CreateImageCommand(CommandInput); break;
                // case "rawimage": _createCommand = new CreateRawImageCommand(CommandInput); break;
                // case "ri": _createCommand = new CreateRawImageCommand(CommandInput); break;
                // case "button": _createCommand = new CreateButtonCommand(CommandInput); break;
                // case "b": _createCommand = new CreateButtonCommand(CommandInput); break;
                // case "slider": _createCommand = new CreateSliderCommand(CommandInput); break;
                // case "sl": _createCommand = new CreateSliderCommand(CommandInput); break;
                // case "scrollbar": _createCommand = new CreateScrollbarCommand(CommandInput); break;
                // case "sb": _createCommand = new CreateScrollbarCommand(CommandInput); break;
                // case "toggle": _createCommand = new CreateToggleCommand(CommandInput); break;
                // case "to": _createCommand = new CreateToggleCommand(CommandInput); break;
                // case "inputfield": _createCommand = new CreateInputFieldCommand(CommandInput); break;
                // case "if": _createCommand = new CreateInputFieldCommand(CommandInput); break;
                // case "dropdown": _createCommand = new CreateDropdownCommand(CommandInput); break;
                // case "dd": _createCommand = new CreateDropdownCommand(CommandInput); break;
                // case "scrollview": _createCommand = new CreateScrollViewCommand(CommandInput); break;
                // case "sv": _createCommand = new CreateScrollViewCommand(CommandInput); break;
                // case "eventsystem": _createCommand = new CreateEventSystemCommand(CommandInput); break;
                // case "es": _createCommand = new CreateEventSystemCommand(CommandInput); break;
                // case "tmp_text": _createCommand = new CreateTMPTextCommand(CommandInput); break;
                // case "tmp_t": _createCommand = new CreateTMPTextCommand(CommandInput); break;
                // case "tmp_dropdown": _createCommand = new CreateTMPDropdownCommand(CommandInput); break;
                // case "tmp_dd": _createCommand = new CreateTMPDropdownCommand(CommandInput); break;
                // case "tmp_inputfield": _createCommand = new CreateTMPInputFieldCommand(CommandInput); break;
                // case "tmp_if": _createCommand = new CreateTMPInputFieldCommand(CommandInput); break;
                // case "tmp_button": _createCommand = new CreateTMPButtonCommand(CommandInput); break;
                // case "tmp_b": _createCommand = new CreateTMPButtonCommand(CommandInput); break;
                // case "tmp_toggle": _createCommand = new CreateTMPToggleCommand(CommandInput); break;
                // "tmp_to" => new CreateTMPToggleCommand(CommandInput),
                _ => throw new ArgumentException($"Unknown object type \"{objectType}\"!")
            };
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