using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2;

namespace KontrolSystem.KSP.Runtime.Core;

public class CommandShell {
    interface ICommand {
        string Name { get; }

        string ShortHelp { get; }

        void Run(string[] arguments);
    }

    class HelpCommand : ICommand {
        public string Name => "help";

        public string ShortHelp => "This help message";

        public void Run(string[] arguments) {
            var consoleBuffer = Mainframe.Instance!.ConsoleBuffer;

            consoleBuffer.PrintLine("The following commands are supported:\n");
            foreach (var command in MAIN_COMMANDS.Values) {
                consoleBuffer.PrintLine($"{command.Name}: {command.ShortHelp}");
            }
            consoleBuffer.PrintLine("\nOtherwise the input will be interpreted as TO2 expression, e.g.");
            consoleBuffer.PrintLine("  1 + 2 + 3");
            consoleBuffer.PrintLine("  ksp::vessel::active_vessel().value.position");
        }
    }

    class ClearCommand : ICommand {
        public string Name => "clear";

        public string ShortHelp => "Clear the console";

        public void Run(string[] arguments) {
            Mainframe.Instance!.ConsoleBuffer.Clear();
        }
    }

    class RebootCommand : ICommand {
        public string Name => "reboot";

        public string ShortHelp => "Reboot the KontrolSystem";

        public void Run(string[] arguments) {
            Mainframe.Instance!.Reboot(true);
        }
    }

    private static readonly SortedDictionary<string, ICommand> MAIN_COMMANDS = new SortedDictionary<string, ICommand>(
        new List<ICommand> {
            new HelpCommand(),
            new ClearCommand(),
            new RebootCommand(),
        }.ToDictionary(command => command.Name));

    public static void RunCommand(string command) {
        if (string.IsNullOrWhiteSpace(command)) return;

        Mainframe.Instance!.ConsoleBuffer.PrintLine($"$> " + command);

        var parts = command.Trim().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0) return;

        if (MAIN_COMMANDS.TryGetValue(parts[0], out var cmd)) {
            cmd.Run(parts[1..]);
        } else {
            Mainframe.Instance!.RunREPL(command);
        }
    }
}
