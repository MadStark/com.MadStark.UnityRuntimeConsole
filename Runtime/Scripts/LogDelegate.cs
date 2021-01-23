using System;

namespace MadStark.RuntimeConsole
{
    public delegate void LogDelegate(DateTimeOffset timestamp, string message, MessageSeverity severity);
}
