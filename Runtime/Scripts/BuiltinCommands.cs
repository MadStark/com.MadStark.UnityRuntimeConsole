namespace MadStark.RuntimeConsole
{
    internal static class BuiltinCommands
    {
        [ConsoleCommand("hello")]
        private static void Run()
        {
            Console.Log("Hello World!");
        }
    }
}
