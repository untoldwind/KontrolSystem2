using System.Collections.Generic;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public class Matrix4x4Binding {
        public static readonly BoundType Matrix4x4Type = Direct.BindType("ksp::math", "Matrix4x4",
            "A 4-dimensional matrix.", typeof(Matrix4x4D),
            new OperatorCollection() {

            },
            new OperatorCollection() {
                {
                    Operator.Add,
                    new StaticMethodOperatorEmitter(() => Matrix4x4Type, () => Matrix4x4Type,
                        typeof(Matrix4x4D).GetMethod("op_Addition", new[] {typeof(Matrix4x4D), typeof(Matrix4x4D) }))
                },
                {
                    Operator.AddAssign,
                    new StaticMethodOperatorEmitter(() => Matrix4x4Type, () => Matrix4x4Type,
                        typeof(Matrix4x4D).GetMethod("op_Addition", new[] {typeof(Matrix4x4D), typeof(Matrix4x4D) }))
                },
                {
                    Operator.Sub,
                    new StaticMethodOperatorEmitter(() => Matrix4x4Type, () => Matrix4x4Type,
                        typeof(Matrix4x4D).GetMethod("op_Subtraction", new[] {typeof(Matrix4x4D), typeof(Matrix4x4D) }))
                },
                {
                    Operator.SubAssign,
                    new StaticMethodOperatorEmitter(() => Matrix4x4Type, () => Matrix4x4Type,
                        typeof(Matrix4x4D).GetMethod("op_Subtraction", new[] {typeof(Matrix4x4D), typeof(Matrix4x4D) }))
                },
            },
            new Dictionary<string, IMethodInvokeFactory> {

            },
            new Dictionary<string, IFieldAccessFactory>() {
                {
                    "m00",
                    new BoundPropertyLikeFieldAccessFactory("m00 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m00"))
                },
                {
                    "m10",
                    new BoundPropertyLikeFieldAccessFactory("m10 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m10"))
                },
                {
                    "m20",
                    new BoundPropertyLikeFieldAccessFactory("m20 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m20"))
                },
                {
                    "m30",
                    new BoundPropertyLikeFieldAccessFactory("m30 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m30"))
                },
                {
                    "m01",
                    new BoundPropertyLikeFieldAccessFactory("m01 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m01"))
                },
                {
                    "m11",
                    new BoundPropertyLikeFieldAccessFactory("m11 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m11"))
                },
                {
                    "m21",
                    new BoundPropertyLikeFieldAccessFactory("m21 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m21"))
                },
                {
                    "m31",
                    new BoundPropertyLikeFieldAccessFactory("m31 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m31"))
                },
                {
                    "m02",
                    new BoundPropertyLikeFieldAccessFactory("m02 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m02"))
                },
                {
                    "m12",
                    new BoundPropertyLikeFieldAccessFactory("m12 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m12"))
                },
                {
                    "m22",
                    new BoundPropertyLikeFieldAccessFactory("m22 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m22"))
                },
                {
                    "m32",
                    new BoundPropertyLikeFieldAccessFactory("m32 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m32"))
                },
                {
                    "m03",
                    new BoundPropertyLikeFieldAccessFactory("m03 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m03"))
                },
                {
                    "m13",
                    new BoundPropertyLikeFieldAccessFactory("m13 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m13"))
                },
                {
                    "m23",
                    new BoundPropertyLikeFieldAccessFactory("m23 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m23"))
                },
                {
                    "m33",
                    new BoundPropertyLikeFieldAccessFactory("m33 field", () => BuiltinType.Float, typeof(Matrix4x4D), typeof(Matrix4x4D).GetProperty("m33"))
                },
                {
                    "determinant",
                    new BoundPropertyLikeFieldAccessFactory("Get determinant of matrix", () => Matrix4x4Type, typeof(Matrix4x4D), typeof(Matrix4x4D).GetMethod("Det"), null)
                },
                {
                    "inverse",
                    new BoundPropertyLikeFieldAccessFactory("Invert matrix", () => Matrix4x4Type, typeof(Matrix4x4D), typeof(Matrix4x4D).GetMethod("GetInverse"), null)
                },
                {
                    "transpose",
                    new BoundPropertyLikeFieldAccessFactory("Transpose matrix", () => Matrix4x4Type, typeof(Matrix4x4D), typeof(Matrix4x4D).GetMethod("GetTranspose"), null)
                }
            }
        );

        public static Matrix4x4D Matrix4x4() => new Matrix4x4D();

    }
}
