using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace MadStark.RuntimeConsole
{
    public static class ConsoleUtils
    {
        public static ConsoleCommandDelegate CreateDelegateForMethodInfo(MethodInfo method) => delegate(string[] args) { method.Invoke(null, BindingFlags.Static, null, new object[] {args}, CultureInfo.CurrentCulture); };

        internal static string[] CommandLineToArgsArray(string command)
        {
            bool inQuotes = false;
            char quote = (char)0;

            string currentArg = string.Empty;
            List<string> args = new List<string>(10);

            void MoveNext()
            {
                if (!string.IsNullOrEmpty(currentArg))
                {
                    args.Add(currentArg);
                    currentArg = string.Empty;
                }
            }

            for (int i = 0; i < command.Length; i++)
            {
                char character = command[i];

                if (!inQuotes && (character == '"' || character == '\''))
                {
                    inQuotes = true;
                    quote = character;
                }
                else if (inQuotes && character == quote)
                {
                    inQuotes = false;
                    quote = (char)0;
                }
                else if (!inQuotes && char.IsWhiteSpace(character))
                {
                    MoveNext();
                }
                else
                {
                    currentArg += character;
                }
            }

            MoveNext();
            return args.ToArray();
        }
    }
}
