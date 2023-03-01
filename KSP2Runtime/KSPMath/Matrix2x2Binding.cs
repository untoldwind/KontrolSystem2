using System.Collections.Generic;
using KontrolSystem.TO2.AST;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public static class Matrix2x2Binding {
        public static readonly RecordStructType Matrix2x2Type = new RecordStructType("ksp::math", "Matrix2x2",
            "A 2-dimensional matrix.", typeof(Matrix2x2),
            new[] {
                new RecordStructField("a", "a", BuiltinType.Float, typeof(Matrix2x2).GetField("a")),
                new RecordStructField("b", "b", BuiltinType.Float, typeof(Matrix2x2).GetField("b")),
                new RecordStructField("c", "c", BuiltinType.Float, typeof(Matrix2x2).GetField("c")),
                new RecordStructField("d", "d", BuiltinType.Float, typeof(Matrix2x2).GetField("d")),
            },
            new OperatorCollection {
                {
                    Operator.Neg,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Unit, () => Matrix2x2Type,
                        typeof(Matrix2x2).GetMethod("op_UnaryNegation", new[] {typeof(Matrix2x2)}))
                },
            },
            new OperatorCollection {
                {
                    Operator.Mul,
                    new StaticMethodOperatorEmitter(() => Vector2Binding.Vector2Type, () => Vector2Binding.Vector2Type,
                        typeof(Matrix2x2).GetMethod("op_Multiply", new[] {typeof(Matrix2x2), typeof(Vector2d) }))
                },
            },
            new Dictionary<string, IMethodInvokeFactory> {
            },
            new Dictionary<string, IFieldAccessFactory> {
                {
                    "determinant",
                    new BoundPropertyLikeFieldAccessFactory("Get determinant of matrix", () => BuiltinType.Float,
                        typeof(Matrix2x2), typeof(Matrix2x2).GetProperty("Determinant"))
                },
                {
                    "inverse",
                    new BoundPropertyLikeFieldAccessFactory("Invert matrix", () => Matrix2x2Type,
                        typeof(Matrix2x2), typeof(Matrix2x2).GetProperty("Inverse"))
                },
            });

        public static Matrix2x2 Matrix2x2(double a, double b, double c, double d) => new Matrix2x2(a, b, c, d);
    }
}
