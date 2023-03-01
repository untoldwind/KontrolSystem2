using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public static class Vector2Binding {
        public static readonly RecordStructType Vector2Type = new RecordStructType("ksp::math", "Vec2",
            "A 2-dimensional vector.", typeof(Vector2d),
            new[] {
                new RecordStructField("x", "x-coordinate", BuiltinType.Float, typeof(Vector2d).GetField("x")),
                new RecordStructField("y", "y-coordinate", BuiltinType.Float, typeof(Vector2d).GetField("y")),
            },
            new OperatorCollection {
                {
                    Operator.Neg,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Unit, () => Vector2Type,
                        typeof(Vector2d).GetMethod("op_UnaryNegation", new[] {typeof(Vector2d)}))
                }, {
                    Operator.Mul,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Float, () => Vector2Type,
                        typeof(Vector2d).GetMethod("op_Multiply", new[] {typeof(double), typeof(Vector2d)}))
                },
            },
            new OperatorCollection {
                {
                    Operator.Add,
                    new StaticMethodOperatorEmitter(() => Vector2Type, () => Vector2Type,
                        typeof(Vector2d).GetMethod("op_Addition", new[] {typeof(Vector2d), typeof(Vector2d)}))
                }, {
                    Operator.AddAssign,
                    new StaticMethodOperatorEmitter(() => Vector2Type, () => Vector2Type,
                        typeof(Vector2d).GetMethod("op_Addition", new[] {typeof(Vector2d), typeof(Vector2d)}))
                }, {
                    Operator.Sub,
                    new StaticMethodOperatorEmitter(() => Vector2Type, () => Vector2Type,
                        typeof(Vector2d).GetMethod("op_Subtraction", new[] {typeof(Vector2d), typeof(Vector2d)}))
                }, {
                    Operator.SubAssign,
                    new StaticMethodOperatorEmitter(() => Vector2Type, () => Vector2Type,
                        typeof(Vector2d).GetMethod("op_Subtraction", new[] {typeof(Vector2d), typeof(Vector2d)}))
                }, {
                    Operator.Mul,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Float, () => Vector2Type,
                        typeof(Vector2d).GetMethod("op_Multiply", new[] {typeof(Vector2d), typeof(double)}))
                }, {
                    Operator.MulAssign,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Float, () => Vector2Type,
                        typeof(Vector2d).GetMethod("op_Multiply", new[] {typeof(Vector2d), typeof(double)}))
                }, {
                    Operator.Div,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Float, () => Vector2Type,
                        typeof(Vector2d).GetMethod("op_Division", new[] {typeof(Vector2d), typeof(double)}))
                }, {
                    Operator.DivAssign,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Float, () => Vector2Type,
                        typeof(Vector2d).GetMethod("op_Division", new[] {typeof(Vector2d), typeof(double)}))
                }, {
                    Operator.Eq,
                    new StaticMethodOperatorEmitter(() => Vector2Type, () => BuiltinType.Bool,
                        typeof(Vector2d).GetMethod("op_Equality", new[] {typeof(Vector2d), typeof(Vector2d)}))
                }, {
                    Operator.NotEq,
                    new StaticMethodOperatorEmitter(() => Vector2Type, () => BuiltinType.Bool,
                        typeof(Vector2d).GetMethod("op_Equality", new[] {typeof(Vector2d), typeof(Vector2d)}),
                        OpCodes.Ldc_I4_0, OpCodes.Ceq)
                },
            },
            new Dictionary<string, IMethodInvokeFactory> {
                {
                    "angle_to",
                    new BoundMethodInvokeFactory("Calculate the angle in degree to `other` vector.", true,
                        () => BuiltinType.Float,
                        () => new List<RealizedParameter> {new RealizedParameter("other", Vector2Type)}, false,
                        typeof(Vector2d), typeof(Vector2d).GetMethod("Angle"))
                }, {
                    "to_string",
                    new BoundMethodInvokeFactory("Convert the vector to string", true, () => BuiltinType.String,
                        () => new List<RealizedParameter>(), false, typeof(Vector2d),
                        typeof(Vector2d).GetMethod("ToString", new Type[0]))
                }, {
                    "to_fixed",
                    new BoundMethodInvokeFactory("Convert the vector to string with fixed number of `decimals`.",
                        true,
                        () => BuiltinType.String,
                        () => new List<RealizedParameter>() {new RealizedParameter("decimals", BuiltinType.Int)},
                        false, typeof(Vector2Binding), typeof(Vector2Binding).GetMethod("ToFixed"))
                }
            },
            new Dictionary<string, IFieldAccessFactory> {
                {
                    "magnitude",
                    new BoundPropertyLikeFieldAccessFactory("Magnitude/length of the vector", () => BuiltinType.Float,
                        typeof(Vector2d), typeof(Vector2d).GetProperty("magnitude"))
                }, {
                    "sqr_magnitude",
                    new BoundPropertyLikeFieldAccessFactory("Squared magnitude of the vector", () => BuiltinType.Float,
                        typeof(Vector2d), typeof(Vector2d).GetProperty("sqrMagnitude"))
                }, {
                    "normalized",
                    new BoundPropertyLikeFieldAccessFactory("Normalized vector (i.e. scaled to length 1)",
                        () => Vector2Type, typeof(Vector2d), typeof(Vector2d).GetProperty("normalized"))
                }
            });

        public static Vector2d Vec2(double x, double y) => new Vector2d(x, y);

        public static string ToFixed(Vector2d v, long decimals) => v.ToString(decimals <= 0 ? "F0" : "F" + decimals);
    }
}
