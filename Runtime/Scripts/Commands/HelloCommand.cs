namespace MadStark.RuntimeConsole.Commands
{
    internal static class HelloCommand
    {
        [ConsoleCommand("hello")]
        private static void Run(string[] args)
        {
            Console.Log("Hello there!");
        }
    }
}
