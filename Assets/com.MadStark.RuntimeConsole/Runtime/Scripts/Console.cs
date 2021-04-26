using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Debug = UnityEngine.Debug;

namespace MadStark.RuntimeConsole
{
    public static class Console
    {
        internal const char kCommandPrefix = '/';
        internal const BindingFlags kCommandMethodBinding = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly Dictionary<string, ConsoleCommandDelegate> commands;
        public static IEnumerator<KeyValuePair<string, ConsoleCommandDelegate>> Commands => commands.GetEnumerator();

        public static event LogDelegate onLog;


        static Console()
        {
            commands = new Dictionary<string, ConsoleCommandDelegate>(50);
            RegisterCommandsInType(typeof(BuiltinCommands));
        }


        #region Command Registration

        public static void RegisterCommandsInAssembly(Assembly assembly)
        {
            IEnumerable<MethodInfo> query = assembly.GetTypes().SelectMany(t => t.GetMethods(kCommandMethodBinding));
            RegisterCommandsInMethods(query);
        }

        public static void RegisterCommandsInType(Type type)
        {
            IEnumerable<MethodInfo> query = type.GetMethods(kCommandMethodBinding);
            RegisterCommandsInMethods(query);
        }

        public static void SetCommand(string name, ConsoleCommandDelegate callback)
        {
            commands[name] = callback;
        }

        public static void UnsetCommand(string name)
        {
            if (commands.ContainsKey(name))
                commands.Remove(name);
        }

        private static void RegisterCommandsInMethods(IEnumerable<MethodInfo> methodInfos)
        {
            foreach (MethodInfo methodInfo in methodInfos)
            {
                foreach (ConsoleCommandAttribute consoleCommandAttribute in methodInfo.GetCustomAttributes<ConsoleCommandAttribute>(false))
                {
                    SetCommand(consoleCommandAttribute.name, ConsoleUtils.CreateDelegateForMethodInfo(methodInfo));
                }
            }
        }

        #endregion


        #region Commands

        private static bool IsCommand(string message) => !string.IsNullOrEmpty(message) && message[0] == kCommandPrefix;

        public static void Interpret(string message)
        {
            message = message.Trim();

            if (IsCommand(message))
            {
                string command = message.Substring(1);
                string argumentsString = null;

                for (int i = 0; i < message.Length; i++)
                {
                    if (char.IsWhiteSpace(message[i]))
                    {
                        command = message.Substring(1, i - 1);
                        argumentsString = message.Substring(i);
                        break;
                    }
                }

                string[] args = argumentsString != null ? ConsoleUtils.CommandLineToArgsArray(argumentsString) : new string[0];
                InvokeCommand(command, args);
            }
            else
                Log(message);
        }

        public static bool TryGetCommandDelegate(string name, out ConsoleCommandDelegate callback)
        {
            return commands.TryGetValue(name, out callback);
        }

        public static void InvokeCommand(string name, string[] args)
        {
            if (!TryGetCommandDelegate(name, out ConsoleCommandDelegate callback))
            {
                LogError($"Command '{name}' not found.");
                return;
            }

            callback.Invoke(args);
        }

        #endregion


        #region Logging

        public static void Log(string message, MessageSeverity severity)
        {
            onLog?.Invoke(DateTimeOffset.Now, message, severity);
        }

        public static void Log(string message) => Log(message, MessageSeverity.Information);

        public static void LogError(string message) => Log(message, MessageSeverity.Error);

        public static void LogWarning(string message) => Log(message, MessageSeverity.Warning);

        #endregion
    }
}
