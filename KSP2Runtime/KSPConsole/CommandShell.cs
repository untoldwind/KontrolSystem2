using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.KSP.Runtime.KSPConsole;

namespace KontrolSystem.KSP.Runtime.Core;

public class CommandShell(KSPConsoleBuffer consoleBuffer) {
    interface ICommand {
        string Name { get; }

        string ShortHelp { get; }

        void Run(KSPConsoleBuffer consoleBuffer, string[] arguments);
    }

    class HelpCommand : ICommand {
        public string Name => "help";

        public string ShortHelp => "This help message";

        public void Run(KSPConsoleBuffer consoleBuffer, string[] arguments) {
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

        public void Run(KSPConsoleBuffer consoleBuffer, string[] arguments) {
            consoleBuffer.Clear();
        }
    }

    class RebootCommand : ICommand {
        public string Name => "reboot";

        public string ShortHelp => "Reboot the KontrolSystem";

        public void Run(KSPConsoleBuffer consoleBuffer, string[] arguments) {
            Mainframe.Instance!.Reboot(true);
        }
    }

    class TestCommand : ICommand {
        public string Name => "test";

        public string ShortHelp => "Run unit tests.";

        public void Run(KSPConsoleBuffer consoleBuffer, string[] arguments) {
            Mainframe.Instance!.RunUnitTests(arguments.Length > 0 ? arguments[0] : null);
        }
    }

    class ListCommand : ICommand {
        public string Name => "list";
        public string ShortHelp => "List available processes";

        public void Run(KSPConsoleBuffer consoleBuffer, string[] arguments) {
            foreach (var process in Mainframe.Instance!.AvailableProcesses) {
                consoleBuffer.PrintLine(process.Name);
            }
        }
    }

    class StartCommand : ICommand {
        public string Name => "start";

        public string ShortHelp => "Start process by name";

        public void Run(KSPConsoleBuffer consoleBuffer, string[] arguments) {
            if (arguments.Length == 0) {
                consoleBuffer.PrintLine("Usage: start <name>");
                return;
            }

            var name = arguments[0];
            foreach (var process in Mainframe.Instance!.AvailableProcesses) {
                if (process.Name == name) {
                    Mainframe.Instance!.StartProcess(process);
                    return;
                }
            }
            consoleBuffer.PrintLine($"No such process: {name}");
        }
    }

    private static readonly SortedDictionary<string, ICommand> MAIN_COMMANDS = new SortedDictionary<string, ICommand>(
        new List<ICommand> {
            new HelpCommand(),
            new ClearCommand(),
            new RebootCommand(),
            new TestCommand(),
            new ListCommand(),
            new StartCommand(),
        }.ToDictionary(command => command.Name));

    public void RunCommand(string command) {
        if (string.IsNullOrWhiteSpace(command)) return;

        consoleBuffer.PrintLine($"$> " + command);

        var parts = command.Trim().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0) return;

        if (MAIN_COMMANDS.TryGetValue(parts[0], out var cmd)) {
            cmd.Run(consoleBuffer, parts[1..]);
        } else {
            Mainframe.Instance!.RunREPL(command);
        }
    }
}
