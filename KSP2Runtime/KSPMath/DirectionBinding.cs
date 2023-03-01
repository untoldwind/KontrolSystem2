using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public static class DirectionBinding {
        public static readonly BoundType DirectionType = Direct.BindType("ksp::math", "Direction",
            "Represents the rotation from an initial coordinate system when looking down the z-axis and \"up\" being the y-axis",
            typeof(Direction),
            new OperatorCollection {
                {
                    Operator.Neg,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Unit, () => DirectionType,
                        typeof(Direction).GetMethod("op_UnaryNegation", new[] {typeof(Vector2d)}))
                }, {
                    Operator.Mul,
                    new StaticMethodOperatorEmitter(() => Vector3Binding.Vector3Type, () => Vector3Binding.Vector3Type,
                        typeof(Direction).GetMethod("op_Multiply", new[] {typeof(Vector3d), typeof(Direction)}))
                },
            },
            new OperatorCollection {
                {
                    Operator.Add,
                    new StaticMethodOperatorEmitter(() => DirectionType, () => DirectionType,
                        typeof(Direction).GetMethod("op_Addition", new[] {typeof(Direction), typeof(Direction)}))
                }, {
                    Operator.AddAssign,
                    new StaticMethodOperatorEmitter(() => DirectionType, () => DirectionType,
                        typeof(Direction).GetMethod("op_Addition", new[] {typeof(Direction), typeof(Direction)}))
                }, {
                    Operator.Sub,
                    new StaticMethodOperatorEmitter(() => DirectionType, () => DirectionType,
                        typeof(Direction).GetMethod("op_Subtraction", new[] {typeof(Direction), typeof(Direction)}))
                }, {
                    Operator.SubAssign,
                    new StaticMethodOperatorEmitter(() => DirectionType, () => DirectionType,
                        typeof(Direction).GetMethod("op_Subtraction", new[] {typeof(Direction), typeof(Direction)}))
                }, {
                    Operator.Mul,
                    new StaticMethodOperatorEmitter(() => DirectionType, () => DirectionType,
                        typeof(Direction).GetMethod("op_Multiply", new[] {typeof(Direction), typeof(Direction)}))
                }, {
                    Operator.MulAssign,
                    new StaticMethodOperatorEmitter(() => DirectionType, () => DirectionType,
                        typeof(Direction).GetMethod("op_Multiply", new[] {typeof(Direction), typeof(Direction)}))
                }, {
                    Operator.Mul,
                    new StaticMethodOperatorEmitter(() => Vector3Binding.Vector3Type, () => Vector3Binding.Vector3Type,
                        typeof(Direction).GetMethod("op_Multiply", new[] {typeof(Direction), typeof(Vector3d)}))
                }, {
                    Operator.Eq,
                    new StaticMethodOperatorEmitter(() => DirectionType, () => BuiltinType.Bool,
                        typeof(Direction).GetMethod("op_Equality", new[] {typeof(Direction), typeof(Direction)}))
                }, {
                    Operator.NotEq,
                    new StaticMethodOperatorEmitter(() => DirectionType, () => BuiltinType.Bool,
                        typeof(Direction).GetMethod("op_Equality", new[] {typeof(Direction), typeof(Direction)}),
                        OpCodes.Ldc_I4_0, OpCodes.Ceq)
                },
            },
            new Dictionary<string, IMethodInvokeFactory> {
                {
                    "to_string",
                    new BoundMethodInvokeFactory("Convert the direction to string", true, () => BuiltinType.String,
                        () => new List<RealizedParameter>(), false, typeof(Direction),
                        typeof(Direction).GetMethod("ToString"))
                }
            },
            new Dictionary<string, IFieldAccessFactory> {
                {
                    "euler",
                    new BoundPropertyLikeFieldAccessFactory("Euler angles in degree of the rotation",
                        () => Vector3Binding.Vector3Type, typeof(Direction),
                        typeof(Direction).GetProperty("Euler"))
                }, {
                    "vector",
                    new BoundPropertyLikeFieldAccessFactory(
                        "Fore vector of the rotation (i.e. looking/facing direction", () => Vector3Binding.Vector3Type,
                        typeof(Direction), typeof(Direction).GetProperty("Vector"))
                }, {
                    "up_vector",
                    new BoundPropertyLikeFieldAccessFactory("Up vector of the rotation",
                        () => Vector3Binding.Vector3Type, typeof(Direction),
                        typeof(Direction).GetProperty("UpVector"))
                }, {
                    "right_vector",
                    new BoundPropertyLikeFieldAccessFactory("Right vector of the rotation",
                        () => Vector3Binding.Vector3Type, typeof(Direction),
                        typeof(Direction).GetProperty("RightVector"))
                }, {
                    "pitch",
                    new BoundPropertyLikeFieldAccessFactory("Pitch in degree", () => BuiltinType.Float,
                        typeof(Direction), typeof(Direction).GetProperty("Pitch"))
                }, {
                    "yaw",
                    new BoundPropertyLikeFieldAccessFactory("Yaw in degree", () => BuiltinType.Float, typeof(Direction),
                        typeof(Direction).GetProperty("Yaw"))
                }, {
                    "roll",
                    new BoundPropertyLikeFieldAccessFactory("Roll in degree", () => BuiltinType.Float,
                        typeof(Direction), typeof(Direction).GetProperty("Roll"))
                },
            });

        public static Direction LookDirUp(Vector3d lookDirection, Vector3d upDirection) =>
            Direction.LookRotation(lookDirection, upDirection);

        public static Direction Euler(double x, double y, double z) => new Direction(new Vector3d(x, y, z), true);

        public static Direction AngleAxis(double angle, Vector3d axis) => Direction.AngleAxis(angle, axis);

        public static Direction FromVectorToVector(Vector3d v1, Vector3d v2) => Direction.FromVectorToVector(v1, v2);
    }
}
