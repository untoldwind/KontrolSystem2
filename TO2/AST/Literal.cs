using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class LiteralBool : Expression {
    public readonly bool value;

    public LiteralBool(bool value, Position start = new(), Position end = new()) : base(start,
        end) {
        this.value = value;
    }

    public override IVariableContainer? VariableContainer {
        set { }
    }

    public override void Prepare(IBlockContext context) {
    }

    public override TO2Type ResultType(IBlockContext context) {
        return BuiltinType.Bool;
    }

    public override REPLValueFuture Eval(REPLContext context) {
        return REPLValueFuture.Success(new REPLBool(value));
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (!dropResult) context.IL.Emit(value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
    }
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

    public override TO2Type ResultType(IBlockContext context) {
        return BuiltinType.String;
    }

    public override void Prepare(IBlockContext context) {
    }

    public override REPLValueFuture Eval(REPLContext context) {
        return REPLValueFuture.Success(new REPLString(value));
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (!dropResult) context.IL.Emit(OpCodes.Ldstr, value);
    }
}

public class LiteralInt : Expression {
    public readonly long value;

    public LiteralInt(long value, Position start = new(), Position end = new()) :
        base(start, end) {
        this.value = value;
    }

    public override IVariableContainer? VariableContainer {
        set { }
    }

    public override TO2Type ResultType(IBlockContext context) {
        return BuiltinType.Int;
    }

    public override void Prepare(IBlockContext context) {
    }

    public override REPLValueFuture Eval(REPLContext context) {
        return REPLValueFuture.Success(new REPLInt(value));
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (!dropResult) context.IL.Emit(OpCodes.Ldc_I8, value);
    }
}

public class LiteralFloat : Expression {
    public readonly double value;

    public LiteralFloat(double value, Position start = new(), Position end = new()) : base(start,
        end) {
        this.value = value;
    }

    public override IVariableContainer? VariableContainer {
        set { }
    }

    public override void Prepare(IBlockContext context) {
    }

    public override TO2Type ResultType(IBlockContext context) {
        return BuiltinType.Float;
    }

    public override REPLValueFuture Eval(REPLContext context) {
        return REPLValueFuture.Success(new REPLFloat(value));
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (!dropResult) context.IL.Emit(OpCodes.Ldc_R8, value);
    }
}
