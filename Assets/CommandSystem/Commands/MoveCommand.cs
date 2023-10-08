using System;
using UnityEngine;

namespace CommandSystem.Commands
{
    [Serializable]
    public class MoveCommand : Command
    {
        [SerializeField] private GameObject instance;
        [SerializeField] private Vector3 initialPosition;
        [SerializeField] private Vector3 inputPosition;
        
        public MoveCommand(string command) : base(command) { }
        
        public override void Run(params string[] args)
        {
            instance = GameObject.Find(args[1]);
            if (instance == null)
            {
                Debug.LogError($"GameObject {args[1]} not found!");
                return;
            }
            initialPosition = instance.transform.position;
            inputPosition = new Vector3(float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]));
            instance.transform.position = inputPosition;
        }
        
        public override void Undo()
        {
            instance.transform.position = initialPosition;
        }
        
        public override void Redo()
        {
            instance.transform.position = inputPosition;
        }
    }
}