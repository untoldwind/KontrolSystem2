using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Emit;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPMath;

public static class Vector3Binding {
    public static readonly RecordStructType Vector3Type = new("ksp::math", "Vec3",
        "A 3-dimensional vector.", typeof(Vector3d),
        [
            new RecordStructField("x", "x-coordinate", BuiltinType.Float, typeof(Vector3d).GetField("x")),
            new RecordStructField("y", "y-coordinate", BuiltinType.Float, typeof(Vector3d).GetField("y")),
            new RecordStructField("z", "z-coordinate", BuiltinType.Float, typeof(Vector3d).GetField("z"))
        ],
        new OperatorCollection {
            {
                Operator.Neg,
                new StaticMethodOperatorEmitter(() => BuiltinType.Unit, LazyVector3Type,
                    typeof(Vector3d).GetMethod("op_UnaryNegation", [typeof(Vector3d)]))
            }, {
                Operator.Mul,
                new StaticMethodOperatorEmitter(() => BuiltinType.Float, LazyVector3Type,
                    typeof(Vector3d).GetMethod("op_Multiply", [typeof(double), typeof(Vector3d)]))
            }
        },
        new OperatorCollection {
            {
                Operator.Add,
                new StaticMethodOperatorEmitter(LazyVector3Type, LazyVector3Type,
                    typeof(Vector3d).GetMethod("op_Addition", [typeof(Vector3d), typeof(Vector3d)]))
            }, {
                Operator.AddAssign,
                new StaticMethodOperatorEmitter(LazyVector3Type, LazyVector3Type,
                    typeof(Vector3d).GetMethod("op_Addition", [typeof(Vector3d), typeof(Vector3d)]))
            }, {
                Operator.Sub,
                new StaticMethodOperatorEmitter(LazyVector3Type, LazyVector3Type,
                    typeof(Vector3d).GetMethod("op_Subtraction", [typeof(Vector3d), typeof(Vector3d)]))
            }, {
                Operator.SubAssign,
                new StaticMethodOperatorEmitter(LazyVector3Type, LazyVector3Type,
                    typeof(Vector3d).GetMethod("op_Subtraction", [typeof(Vector3d), typeof(Vector3d)]))
            }, {
                Operator.Mul,
                new StaticMethodOperatorEmitter(() => BuiltinType.Float, LazyVector3Type,
                    typeof(Vector3d).GetMethod("op_Multiply", [typeof(Vector3d), typeof(double)]))
            }, {
                Operator.Mul,
                new StaticMethodOperatorEmitter(LazyVector3Type, () => BuiltinType.Float,
                    typeof(Vector3d).GetMethod("Dot"))
            }, {
                Operator.MulAssign,
                new StaticMethodOperatorEmitter(() => BuiltinType.Float, LazyVector3Type,
                    typeof(Vector3d).GetMethod("op_Multiply", [typeof(Vector3d), typeof(double)]))
            }, {
                Operator.Div,
                new StaticMethodOperatorEmitter(() => BuiltinType.Float, LazyVector3Type,
                    typeof(Vector3d).GetMethod("op_Division", [typeof(Vector3d), typeof(double)]))
            }, {
                Operator.DivAssign,
                new StaticMethodOperatorEmitter(() => BuiltinType.Float, LazyVector3Type,
                    typeof(Vector3d).GetMethod("op_Division", [typeof(Vector3d), typeof(double)]))
            }, {
                Operator.Eq,
                new StaticMethodOperatorEmitter(LazyVector3Type, () => BuiltinType.Bool,
                    typeof(Vector3d).GetMethod("op_Equality", [typeof(Vector3d), typeof(Vector3d)]))
            }, {
                Operator.NotEq,
                new StaticMethodOperatorEmitter(LazyVector3Type, () => BuiltinType.Bool,
                    typeof(Vector3d).GetMethod("op_Equality", [typeof(Vector3d), typeof(Vector3d)]),
                    null, OpCodes.Ldc_I4_0, OpCodes.Ceq)
            }
        },
        new Dictionary<string, IMethodInvokeFactory> {
            {
                "cross",
                new BoundMethodInvokeFactory("Calculate the cross/other product with `other` vector.", true,
                    LazyVector3Type,
                    () => [new("other", Vector3Type!, "Other vector")], false,
                    typeof(Vector3d), typeof(Vector3d).GetMethod("Cross"))
            }, {
                "dot",
                new BoundMethodInvokeFactory("Calculate the dot/inner product with `other` vector.", true,
                    () => BuiltinType.Float,
                    () => [new("other", Vector3Type!, "Other vector")], false,
                    typeof(Vector3d), typeof(Vector3d).GetMethod("Dot"))
            }, {
                "angle_to",
                new BoundMethodInvokeFactory("Calculate the angle in degree to `other` vector.", true,
                    () => BuiltinType.Float,
                    () => [new("other", Vector3Type!, "Other vector")], false,
                    typeof(Vector3d), typeof(Vector3d).GetMethod("Angle"))
            }, {
                "lerp_to",
                new BoundMethodInvokeFactory(
                    "Linear interpolate position between this and `other` vector, where `t = 0.0` is this and `t = 1.0` is `other`.",
                    true,
                    LazyVector3Type,
                    () => [
                        new("other", Vector3Type!, "Other vector"),
                        new("t", BuiltinType.Float, "Relative position of mid-point (0.0 - 1.0)")
                    ], false, typeof(Vector3d), typeof(Vector3d).GetMethod("Lerp"))
            }, {
                "project_to",
                new BoundMethodInvokeFactory("Project this on `other` vector.", true, () => Vector3Type!,
                    () => [new("other", Vector3Type!, "Other vector")], false,
                    typeof(Vector3d), typeof(Vector3d).GetMethod("Project"))
            }, {
                "distance_to",
                new BoundMethodInvokeFactory("Calculate the distance between this and `other` vector.", true,
                    () => BuiltinType.Float,
                    () => [new("other", Vector3Type!, "Other vector")], false,
                    typeof(Vector3d), typeof(Vector3d).GetMethod("Distance"))
            }, {
                "exclude_from",
                new BoundMethodInvokeFactory("Exclude this from `other` vector.", true, () => Vector3Type!,
                    () => [new("other", Vector3Type!, "Other vector")], false,
                    typeof(Vector3d), typeof(Vector3d).GetMethod("Exclude"))
            }, {
                "to_string",
                new BoundMethodInvokeFactory("Convert vector to string.", true, () => BuiltinType.String,
                    () => [], false, typeof(Vector3Binding),
                    typeof(Vector3Binding).GetMethod("ToString", [typeof(Vector3d)]))
            }, {
                "to_fixed",
                new BoundMethodInvokeFactory("Convert the vector to string with fixed number of `decimals`.",
                    true,
                    () => BuiltinType.String,
                    () => [new("decimals", BuiltinType.Int, "Number of decimals")],
                    false, typeof(Vector3Binding), typeof(Vector3Binding).GetMethod("ToFixed"))
            }, {
                "to_position",
                new BoundMethodInvokeFactory("Consider this vector as position in a coordinate system",
                    true,
                    () => PositionBinding.PositionType,
                    () => [new("frame", TransformFrameBinding.TransformFrameType, "Frame of reference")],
                    false, typeof(Vector3Binding), typeof(Vector3Binding).GetMethod("ToPosition"))
            }, {
                "to_global",
                new BoundMethodInvokeFactory("Associate this vector with a coordinate system",
                    true,
                    () => VectorBinding.VectorType,
                    () => [new("frame", TransformFrameBinding.TransformFrameType, "Frame of reference")],
                    false, typeof(Vector3Binding), typeof(Vector3Binding).GetMethod("ToGlobal"))
            }, {
                "to_direction",
                new BoundMethodInvokeFactory("Point in direction of this vector.",
                    true,
                    () => DirectionBinding.DirectionType,
                    () => [],
                    false, typeof(Vector3Binding), typeof(Vector3Binding).GetMethod("ToDirection"))
            }
        },
        new Dictionary<string, IFieldAccessFactory> {
            {
                "magnitude",
                new BoundPropertyLikeFieldAccessFactory("Magnitude/length of the vector", () => BuiltinType.Float,
                    typeof(Vector3d), typeof(Vector3d).GetProperty("magnitude"))
            }, {
                "sqr_magnitude",
                new BoundPropertyLikeFieldAccessFactory("Squared magnitude of the vector", () => BuiltinType.Float,
                    typeof(Vector3d), typeof(Vector3d).GetProperty("sqrMagnitude"))
            }, {
                "normalized",
                new BoundPropertyLikeFieldAccessFactory("Normalized vector (i.e. scaled to length 1)",
                    LazyVector3Type, typeof(Vector3d), typeof(Vector3d).GetProperty("normalized"))
            }, {
                "xzy",
                new BoundPropertyLikeFieldAccessFactory("Swapped y- and z-coordinate", () => Vector3Type!,
                    typeof(Vector3d), typeof(Vector3d).GetProperty("SwapYAndZ"))
            }
        });

    private static RecordStructType LazyVector3Type() => Vector3Type;

    public static Vector3d Vec3(double x, double y, double z) {
        return new Vector3d(x, y, z);
    }

    public static string ToString(Vector3d v) {
        return "[" + v.x.ToString(CultureInfo.InvariantCulture) + ", " +
               v.y.ToString(CultureInfo.InvariantCulture) + ", " + v.z.ToString(CultureInfo.InvariantCulture) + "]";
    }

    public static string ToFixed(Vector3d v, long decimals) {
        var format = decimals <= 0 ? "F0" : "F" + decimals;
        return "[" + v.x.ToString(format, CultureInfo.InvariantCulture) + ", " +
               v.y.ToString(format, CultureInfo.InvariantCulture) + ", " +
               v.z.ToString(format, CultureInfo.InvariantCulture) + "]";
    }

    public static Position ToPosition(Vector3d local, ITransformFrame frame) {
        return new Position(frame, local);
    }

    public static Vector ToGlobal(Vector3d local, ITransformFrame frame) {
        return new Vector(frame, local);
    }

    public static Direction ToDirection(Vector3d v) {
        return new Direction(v, false);
    }
}
