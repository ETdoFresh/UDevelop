using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommandSystem.Commands
{
    [Serializable]
    public class CreatePrimitiveCommand : Command
    {
        [SerializeField] private GameObject instance;
        [SerializeField] private Vector3 inputPosition;
        [SerializeField] private Quaternion inputRotation;
        [SerializeField] private Vector3 inputScale;
        private static readonly Dictionary<string, PrimitiveType> primitiveTypeDictionary = new();
        
        public CreatePrimitiveCommand(string command) : base(command) { }
        
        public override void FirstRun()
        {
            foreach(var primitiveType in Enum.GetValues(typeof(PrimitiveType)))
            {
                primitiveTypeDictionary.Add(primitiveType.ToString().ToLower(), (PrimitiveType) primitiveType);
            }
        }
        
        public override void Run(params string[] args)
        {
            var primitiveType = PrimitiveType.Cube;
            inputPosition = Vector3.zero;
            inputRotation = Quaternion.identity;
            inputScale = Vector3.one;
        
            if (args.Length > 1)
            {
                primitiveType = primitiveTypeDictionary[args[1].ToLower()];
            }
            if (args.Length > 4)
            {
                inputPosition = new Vector3(float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]));
            }
            if (args.Length > 8)
            {
                inputRotation = new Quaternion(float.Parse(args[5]), float.Parse(args[6]), float.Parse(args[7]), float.Parse(args[8]));
            }
            if (args.Length > 11)
            {
                inputScale = new Vector3(float.Parse(args[9]), float.Parse(args[10]), float.Parse(args[11]));
            }
        
            instance = GameObject.CreatePrimitive(primitiveType);
            instance.transform.position = inputPosition;
            instance.transform.rotation = inputRotation;
            instance.transform.localScale = inputScale;
        }
        
        public override void Undo()
        {
            instance.SetActive(false);
        }
        
        public override void Redo()
        {
            instance.SetActive(true);
        }
    }
}
