using UnityEngine;

namespace MadStark.RuntimeConsole
{
    internal static class RegisterCommandsInAssembly
    {
        [RuntimeInitializeOnLoadMethod]
        private static void RegisterCommands()
        {
            Console.RegisterCommandsInAssembly(typeof(RegisterCommandsInAssembly).Assembly);
        }
    }
}
