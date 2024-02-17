using KontrolSystem.Parsing;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public abstract class Node {
    protected Node(Position start, Position end) {
        Start = start;
        End = end;
    }

    public Position Start { get; }
    public Position End { get; }

    public abstract REPLValueFuture Eval(REPLContext context);
}
