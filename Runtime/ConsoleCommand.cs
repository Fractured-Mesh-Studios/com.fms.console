using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ConsoleEngine
{
    [System.Serializable]
    public class ConsoleCommand
    {
        public string name;
        public string description;

        public virtual void Process(object[] args)
        {
            
        }
    }
}
