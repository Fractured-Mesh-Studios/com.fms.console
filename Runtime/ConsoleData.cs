using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConsoleEngine
{
    [System.Serializable]
    public struct ConsoleData 
    {
        public string name;
        public string stackTrace;
        public LogType type;
        public int amount;
        public bool expanded;
        public DateTime time;

        public ConsoleData(ConsoleData data)
        {
            name = data.name;
            stackTrace = data.stackTrace;
            type = data.type;
            amount = data.amount;
            expanded = data.expanded;
            time = DateTime.Now;
        }

        public ConsoleData(string name, string stackTrace, LogType type)
        {
            this.name = name;
            this.stackTrace = stackTrace;
            this.type = type;
            this.amount = 1;
            this.expanded = false;
            time = DateTime.Now;
        }

        public ConsoleData(string name, string stackTrace, LogType type, int amount, bool expanded)
        {
            this.name = name;
            this.stackTrace = stackTrace;
            this.type = type;
            this.amount = amount;
            this.expanded = expanded;
            time = DateTime.Now;
        }

        public override bool Equals(object obj)
        {
            return obj is ConsoleData data &&
                   name == data.name &&
                   stackTrace == data.stackTrace &&
                   type == data.type &&
                   amount == data.amount;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, stackTrace, type, amount);
        }

        public override string ToString()
        {
            return $"{name}:{type}";
        }

        public static bool operator==(ConsoleData a, ConsoleData b)
        {
            bool validName = string.CompareOrdinal(a.name, b.name) == 0;
            bool validType = a.type == b.type;
            return validName && validType;
        }

        public static bool operator!=(ConsoleData a, ConsoleData b)
        {
            bool validName = string.CompareOrdinal(a.name, b.name) != 0;
            bool validType = a.type != b.type;
            return validName && validType;
        }
    }
}
