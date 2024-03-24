using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public abstract class StringInterpolationPart {
    public class StringPart(string value) : StringInterpolationPart {
        public readonly string value = value;
    }

    public class ExpressionPart(Expression expression, string alignOrFormat) : StringInterpolationPart {
        public readonly Expression expression = expression;
        public readonly string alignOrFormat = alignOrFormat;
    }
}

public class StringInterpolation : Expression {
    private readonly string formatString;
    private readonly List<Expression> placeholders;

    public StringInterpolation(List<StringInterpolationPart> parts, Position start, Position end) : base(start, end) {
        var format = new StringBuilder();

        placeholders = [];

        foreach (var part in parts) {
            switch (part) {
            case StringInterpolationPart.StringPart stringPart:
                format.Append(stringPart.value);
                break;
            case StringInterpolationPart.ExpressionPart expressionPart:
                format.Append("{" + placeholders.Count + expressionPart.alignOrFormat + "}");
                placeholders.Add(expressionPart.expression);
                break;
            }
        }

        formatString = format.ToString();
    }

    public override IVariableContainer? VariableContainer {
        set {
            foreach (var placeholder in placeholders) placeholder.VariableContainer = value;
        }
    }

    public override TO2Type ResultType(IBlockContext context) => BuiltinType.String;

    public override void Prepare(IBlockContext context) {
        foreach (var placeholder in placeholders) placeholder.Prepare(context);
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (dropResult) return;

        if (placeholders.Count == 0) {
            context.IL.Emit(OpCodes.Ldstr, formatString);
            return;
        }

        context.IL.EmitCall(OpCodes.Call, typeof(CultureInfo).GetProperty("InvariantCulture")!.GetMethod, 0);

        context.IL.Emit(OpCodes.Ldstr, formatString);

        context.IL.Emit(OpCodes.Ldc_I4, placeholders.Count);
        context.IL.Emit(OpCodes.Newarr, typeof(object));

        for (var i = 0; i < placeholders.Count; i++) {
            var valueType = placeholders[i].ResultType(context).GeneratedType(context.ModuleContext);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldc_I4, i);
            placeholders[i].EmitCode(context, false);
            if (valueType.IsValueType) context.IL.Emit(OpCodes.Box, valueType);
            context.IL.Emit(OpCodes.Stelem, typeof(object));
        }

        context.IL.EmitCall(OpCodes.Call, typeof(string).GetMethod("Format", [typeof(CultureInfo), typeof(string), typeof(object[])])!, 3);
    }

    public override REPLValueFuture Eval(REPLContext context) {
        var expressionFutures = placeholders.Select(p => p.Eval(context)).ToArray();
        return REPLValueFuture.ChainN(new ArrayType(BuiltinType.Unit), expressionFutures, values => {
            var args = new object?[values.Length];
            for (var i = 0; i < values.Length; i++)
                args[i] = values[i].Value;
            return REPLValueFuture.Success(new REPLString(string.Format(CultureInfo.InvariantCulture, formatString, args)));
        });
    }
}
