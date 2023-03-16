using System;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Runtime {
    public class REPLException : Exception {
        public readonly Node node;
        public readonly string message;

        public REPLException(Node node, string message) {
            this.node = node;
            this.message = message;
        }
    }
}
