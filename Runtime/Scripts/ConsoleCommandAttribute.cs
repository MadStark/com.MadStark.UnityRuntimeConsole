using System;

namespace MadStark.RuntimeConsole
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleCommandAttribute : Attribute
    {
        public readonly string name;

        public ConsoleCommandAttribute(string name)
        {
            this.name = name;
        }
    }
}
