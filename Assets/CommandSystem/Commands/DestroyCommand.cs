using System;
using UnityEngine;

namespace CommandSystem.Commands
{
    [Serializable]
    public class DestroyCommand : Command
    {
        [SerializeField] private GameObject instance;
        
        public DestroyCommand(string commandInput) : base(commandInput) { }
        
        public override void Run(params string[] args)
        {
            instance = GameObject.Find(args[1]);
            if (instance == null)
            {
                Debug.LogError($"GameObject {args[1]} not found!");
                return;
            }
            instance.SetActive(false);
        }
        
        public override void Undo()
        {
            instance.SetActive(true);
        }
        
        public override void Redo()
        {
            instance.SetActive(false);
        }
    }
}