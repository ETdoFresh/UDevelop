using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommandSystem.Commands.Select
{
    [Serializable]
    public class SelectGameObjectByPathCommandCSharp : CommandCSharp
    {
        [SerializeField] private Object[] _previousSelectedObjects;
        [SerializeField] private Object[] _selectedObjects;

        public SelectGameObjectByPathCommandCSharp(string commandInput) : base(commandInput) { }

        public override void OnRun(params string[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Not enough arguments!");
            var objectPath = string.Join(" ", args[1..]);
            var objectNameArray = objectPath.Split('/');

            List<Transform> currentParents = null;
            var foundPath = "";
            var remainingPath = objectPath;
            foreach (var objectName in objectNameArray)
            {
                var objectNameWithoutIndex = SelectionUtil.RemoveIndexFromName(objectName);
                var nextParents = new List<Transform>();

                if (currentParents == null)
                {
                    var objectsByName = Object
                        .FindObjectsOfType<GameObject>(true)
                        .Where(x => !x.transform.parent)
                        .Where(x => string.Equals(x.name, objectNameWithoutIndex,
                            StringComparison.CurrentCultureIgnoreCase))
                        .OrderBy(SelectionUtil.GetGameObjectOrder)
                        .Cast<Object>();
                    if (!objectsByName.Any()) throw new ArgumentException($"No GameObjects found! {remainingPath}");
                    nextParents.AddRange(SelectionUtil.ParseAndSelectIndex(objectsByName, objectName)
                        .Select(x => ((GameObject)x).transform));
                }
                else
                {
                    foreach (var currentParent in currentParents)
                    {
                        var objectsByName = currentParent
                            .Cast<Transform>()
                            .Select(x => x.gameObject)
                            .Where(x => string.Equals(x.name, objectNameWithoutIndex,
                                StringComparison.CurrentCultureIgnoreCase))
                            .OrderBy(SelectionUtil.GetGameObjectOrder)
                            .Cast<Object>();
                        if (!objectsByName.Any()) throw new ArgumentException($"No GameObjects found! {foundPath} -- {remainingPath}");
                        nextParents.AddRange(SelectionUtil.ParseAndSelectIndex(objectsByName, objectName)
                            .Select(x => ((GameObject)x).transform).ToArray());
                    }
                }

                currentParents = nextParents;
                foundPath += $"{objectName}";
                remainingPath = objectPath[foundPath.Length..];
            }
            
            if (currentParents == null) throw new ArgumentException($"No GameObjects found! {foundPath}");
            _previousSelectedObjects = Selection.objects;
            _selectedObjects = currentParents.Select(x => x.gameObject).Cast<Object>().ToArray();
            Selection.objects = _selectedObjects;
        }
    }
}

public class Player : MonoBehaviour
{
    private void Update()
    {
        var rigidbody = GetComponent<Rigidbody>();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody.AddForce(Vector3.up * 10, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.W))
        {
            rigidbody.AddForce(transform.forward * 10, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rigidbody.AddForce(-transform.forward * 10, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rigidbody.AddTorque(Vector3.up * 10, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rigidbody.AddTorque(-transform.forward * 10, ForceMode.Impulse);
        }
    }
}