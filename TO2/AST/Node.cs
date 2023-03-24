using System;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public abstract class Node {
        public Position Start { get; }
        public Position End { get; }

        protected Node(Position start, Position end) {
            Start = start;
            End = end;
        }

        public abstract REPLValueFuture Eval(REPLContext context);
    }
}
