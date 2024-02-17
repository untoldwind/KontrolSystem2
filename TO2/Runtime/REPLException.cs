using System;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Runtime;

public class REPLException : Exception {
    public readonly Position end;
    public readonly Position start;

    public REPLException(string message) : base(message) {
        start = new Position();
        end = new Position();
    }

    public REPLException(Node node, string message) : base($"{node?.Start}: {message}") {
        start = node!.Start;
        end = node.End;
    }

    public REPLException(Position start, Position end, string message) : base($"{start}: {message}") {
        this.start = start;
        this.end = end;
    }
}
