using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands
{
    [Serializable]
    public class SelectCommand : Command
    {
        private Object[] _previousSelectedObjects;
        private Object[] _selectedObjects;
        private string[] _selectedObjectNames;
        private string _scope;
        private string[] _scopeNames =
            { "scene", "s", "project", "p", "sceneall", "scene-all", "all", "sa", "projectall", "project-all", "pa", "none" };

        public override bool AddToHistory => true;
        public override string CommandOutput => _selectedObjectNames.Length > 0 ?
            $"Selected {_selectedObjectNames.Length} object(s): {string.Join(", ", _selectedObjectNames)}" :
            _scope == "none" ? "Deselected all objects" : $"No objects found using \"{CommandInput}\"!";

        public override string[] CommandNames => new[] { "select", "s" };

        public override string CommandUsage =>
            $"{CommandNames[0]} [SCOPE] [TYPE] PATH\n{CommandNames[0]} ALL-SCOPE [TYPE] [PATH]";

        public override string CommandDescription => "Selects an object in scene or project.";

        public override Dictionary<string, string> CommandArg1Descriptions { get; } = new()
        {
            { "scene, s", "Selects an object in scene" },
            { "project, p", "Selects an object in project" },
            { "sceneall, scene-all, all, sa", "Selects all objects in scene" },
            { "projectall, project-all, pa", "Selects all objects in project" },
            { "none", "Resets selection"},
            { "otherwise", "Selects first object in scene, or then project" },
        };

        public override Dictionary<string, string> CommandArg2Descriptions { get; } = new()
        {
            { "object, o", "Any Object Type" },
            { "gameobject, go", "(DEFAULT) GameObject Type" },
            { "component, c", "Any Component Type" },
            { "transform, t", "Transform Component Type" },
            { "mesh, m", "Mesh Component Type" },
            { "material, mat", "Material Component Type" },
            { "texture, tex", "Texture Component Type" },
            { "shader, sh", "Shader Component Type" },
            { "script, scr", "Script Component Type" },
            { "(custom)", "Will attempt to load type by full type name, then by type name" },
        };

        public SelectCommand(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Not enough arguments!");
            var scope = ResolveScope(args);
            var type = ResolveType(args, scope);
            var path = ResolvePath(args, scope, type);
            if (_scopeNames.Contains(scope))
            {
                switch (scope)
                {
                    case "scene":
                    case "s":
                        Select(FindFirstObjectInScene(type, path));
                        break;
                    case "project":
                    case "p":
                        Select(FindFirstObjectInProject(type, path));
                        break;
                    case "sceneall":
                    case "scene-all":
                    case "all":
                    case "sa":
                        Select(FindObjectsInScene(type, path));
                        break;
                    case "projectall":
                    case "project-all":
                    case "pa":
                        Select(FindObjectsInProject(type, path));
                        break;
                    case "none":
                        _scope = "none";
                        Select(Array.Empty<Object>());
                        break;
                }
            }
            else
            {
                Select(FindFirstObjectInScene(type, path));
                if (_selectedObjects.Length == 0)
                    Select(FindFirstObjectInProject(type, path));
            }
        }

        public override void OnUndo()
        {
            UnityEditor.Selection.objects = _previousSelectedObjects;
        }

        public override void OnRedo()
        {
            UnityEditor.Selection.objects = _selectedObjects;
        }

        private string ResolveScope(string[] args)
        {
            var possibleScope = args[1].ToLower();
            return _scopeNames.Contains(possibleScope) ? possibleScope : null;
        }

        private Type ResolveType(string[] args, string scope)
        {
            if (scope != null && args.Length < 3) return null;
            
            var possibleTypeName = scope == null ? args[1] : args[2];

            switch (possibleTypeName.ToLower())
            {
                case "object":
                case "o":
                    return typeof(Object);
                case "gameobject":
                case "go":
                    return typeof(GameObject);
                case "component":
                case "c":
                    return typeof(Component);
                case "transform":
                case "t":
                    return typeof(Transform);
                case "mesh":
                case "m":
                    return typeof(Mesh);
                case "material":
                case "mat":
                    return typeof(Material);
                case "texture":
                case "tex":
                    return typeof(Texture);
                case "shader":
                case "sh":
                    return typeof(Shader);
                case "script":
                case "scr":
                    return typeof(UnityEditor.MonoScript);
            }

            var possibleType = Type.GetType(possibleTypeName);
            if (possibleType != null) return possibleType;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.FullName.StartsWith("Unity.") && !x.FullName.StartsWith("UnityEngine.") && !x.FullName.StartsWith("UnityEditor.") && !x.FullName.StartsWith("System.") && !x.FullName.StartsWith("mscorlib."));
            var types = assemblies.SelectMany(assembly => assembly.GetTypes());
            foreach (var type in types)
            {
                if (!string.Equals(type.Name, possibleTypeName, StringComparison.CurrentCultureIgnoreCase)) continue;
                return type;
            }

            return null;
        }

        private string ResolvePath(string[] args, string scope, Type type)
        {
            if (scope == null && type == null)
                return string.Join("_", args[1..]);
            if (scope != null && type == null)
                return args.Length < 3 ? null : string.Join("_", args[2..]);
            if (scope == null && type != null)
                return string.Join("_", args[2..]);

            return args.Length < 4 ? null : string.Join("_", args[3..]);
        }

        private IEnumerable<Object> FindObjectsInScene(Type type, string path)
        {
            var objectName = path;
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                if (objectName != null && gameObject.name != objectName) continue;
                if (type == null || type == typeof(GameObject) || type == typeof(Object))
                    yield return gameObject;
                else if (type.IsSubclassOf(typeof(Component)))
                    foreach (var component in gameObject.GetComponents(type))
                        yield return component;
                else if (type == typeof(Mesh))
                    foreach (var meshFilter in gameObject.GetComponents<MeshFilter>())
                        yield return meshFilter.mesh;
                else if (type == typeof(Material))
                    foreach (var meshRenderer in gameObject.GetComponents<MeshRenderer>())
                    foreach (var material in meshRenderer.materials)
                        yield return material;
            }
        }

        private IEnumerable<Object> FindFirstObjectInScene(Type type, string path)
        {
            foreach (var obj in FindObjectsInScene(type, path))
            {
                yield return obj;
                break; // Only return first object
            }
        }

        private IEnumerable<Object> FindObjectsInProject(Type type, string path)
        {
            var objectName = path;

            var typeFilter = "";
            if (type == null || type == typeof(Object)) typeFilter = "";
            else if (type == typeof(GameObject)) typeFilter = "prefab";
            else if (type.IsSubclassOf(typeof(MonoBehaviour)) && path == null) objectName = type.Name;
            else typeFilter = type.Name.ToLower();
            typeFilter = typeFilter == "" ? "" : $"t:{typeFilter} ";

            var guids = UnityEditor.AssetDatabase.FindAssets($"{typeFilter}{objectName}");
            foreach (var guid in guids)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                yield return UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            }
        }

        private IEnumerable<Object> FindFirstObjectInProject(Type type, string path)
        {
            foreach (var obj in FindObjectsInProject(type, path))
            {
                yield return obj;
                break; // Only return first object
            }
        }

        private void Select(IEnumerable<Object> objects)
        {
            _previousSelectedObjects = UnityEditor.Selection.objects;
            _selectedObjects = objects.Where(x => x).ToArray();
            _selectedObjectNames = new string[_selectedObjects.Length];
            for (var i = 0; i < _selectedObjects.Length; i++)
                _selectedObjectNames[i] = _selectedObjects[i].name;

            UnityEditor.Selection.objects = _selectedObjects;
        }
    }
}