namespace Experiments {
    public class CommandShell {
        private readonly KSPConsoleBuffer consoleBuffer;
        public CommandShell(KSPConsoleBuffer consoleBuffer) {
            this.consoleBuffer = consoleBuffer;
        }
        
        public void RunCommand(string command) {
            if (command == "test") {
                for (int i = 0; i < 20; i++) {
                    consoleBuffer.PrintLine($"Test line {i}\n");
                }
            } else if (command == "clear") {
                consoleBuffer.Clear();
            }
        }
    }
}
