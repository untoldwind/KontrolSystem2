using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class LiteralBool(bool value, Position start = new(), Position end = new()) : Expression(start,
    end) {
    public readonly bool value = value;

    public override IVariableContainer? VariableContainer {
        set { }
    }

    public override void Prepare(IBlockContext context) {
    }

    public override TO2Type ResultType(IBlockContext context) => BuiltinType.Bool;

    public override REPLValueFuture Eval(REPLContext context) => REPLValueFuture.Success(new REPLBool(value));

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (!dropResult) context.IL.Emit(value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
    }

    public override string ToString() => $"{value}";
}

public class LiteralString : Expression {
    public readonly string value;

    public LiteralString(string value, Position start = new(), Position end = new()) : base(start,
        end) {
        this.value = value;
    }

    public LiteralString(char[] chars, Position start = new(), Position end = new()) : base(start,
        end) {
        value = new string(chars);
    }

    public override IVariableContainer? VariableContainer {
        set { }
    }

    public override TO2Type ResultType(IBlockContext context) => BuiltinType.String;

    public override void Prepare(IBlockContext context) {
    }

    public override REPLValueFuture Eval(REPLContext context) => REPLValueFuture.Success(new REPLString(value));

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (!dropResult) context.IL.Emit(OpCodes.Ldstr, value);
    }

    public override string ToString() => $"\"{value.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\"", "\\\"")}\"";
}

public class LiteralInt(long value, Position start = new(), Position end = new()) : Expression(start, end) {
    public readonly long value = value;

    public override IVariableContainer? VariableContainer {
        set { }
    }

    public override TO2Type ResultType(IBlockContext context) => BuiltinType.Int;

    public override void Prepare(IBlockContext context) {
    }

    public override REPLValueFuture Eval(REPLContext context) => REPLValueFuture.Success(new REPLInt(value));

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (!dropResult) context.IL.Emit(OpCodes.Ldc_I8, value);
    }

    public override string ToString() => $"{value}";
}

public class LiteralFloat(double value, Position start = new(), Position end = new()) : Expression(start,
    end) {
    public readonly double value = value;

    public override IVariableContainer? VariableContainer {
        set { }
    }

    public override void Prepare(IBlockContext context) {
    }

    public override TO2Type ResultType(IBlockContext context) => BuiltinType.Float;

    public override REPLValueFuture Eval(REPLContext context) => REPLValueFuture.Success(new REPLFloat(value));

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (!dropResult) context.IL.Emit(OpCodes.Ldc_R8, value);
    }

    public override string ToString() => $"{value}";
}
