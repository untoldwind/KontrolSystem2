using System;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Runtime {
    public class REPLException : Exception {
        public readonly Position start;
        public readonly Position end;
        public readonly string message;

        public REPLException(Node node, string message) {
            start = node.Start;
            end = node.End;
            this.message = message;
        }

        public REPLException(Position start, Position end, string message) {
            this.start = start;
            this.end = end;
            this.message = message;
        }

    }
}
