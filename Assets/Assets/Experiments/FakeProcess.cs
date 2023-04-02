namespace Experiments {
    public class FakeProcess {
        public string name;
        public bool isRunning;
        public int parameters;

        public FakeProcess(string name, bool isRunning, int parameters) {
            this.name = name;
            this.isRunning = isRunning;
            this.parameters = parameters;
        }
    }
}
