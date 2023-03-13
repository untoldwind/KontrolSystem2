using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Api;
using KSP.Sim;
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
                    new StaticMethodOperatorEmitter(() => VectorBinding.VectorType, () => VectorBinding.VectorType,
                        typeof(Direction).GetMethod("op_Multiply", new[] {typeof(Direction), typeof(Vector)}))
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
                    "euler",
                    new BoundMethodInvokeFactory("Get euler angles in a specific coordinate system", true, () => Vector3Binding.Vector3Type,
                    () => new List<RealizedParameter>() { new RealizedParameter("frame", TransformFrameBinding.TransformFrameType) }, false, typeof(Direction),
                    typeof(Direction).GetMethod("Euler"))
                },
                {
                    "pitch",
                    new BoundMethodInvokeFactory("Get pitch angle in a specific coordinate system", true, () => BuiltinType.Float,
                        () => new List<RealizedParameter>() { new RealizedParameter("frame", TransformFrameBinding.TransformFrameType) }, false, typeof(Direction),
                        typeof(Direction).GetMethod("Pitch"))
                },
                {
                    "yaw",
                    new BoundMethodInvokeFactory("Get yaw angle in a specific coordinate system", true, () => BuiltinType.Float,
                        () => new List<RealizedParameter>() { new RealizedParameter("frame", TransformFrameBinding.TransformFrameType) }, false, typeof(Direction),
                        typeof(Direction).GetMethod("Yaw"))
                },
                {
                    "roll",
                    new BoundMethodInvokeFactory("Get roll angle in a specific coordinate system", true, () => BuiltinType.Float,
                        () => new List<RealizedParameter>() { new RealizedParameter("frame", TransformFrameBinding.TransformFrameType) }, false, typeof(Direction),
                        typeof(Direction).GetMethod("Roll"))
                },
                {
                    "to_string",
                    new BoundMethodInvokeFactory("Convert the direction to string", true, () => BuiltinType.String,
                        () => new List<RealizedParameter>() { new RealizedParameter("frame", TransformFrameBinding.TransformFrameType) }, false, typeof(Direction),
                        typeof(Direction).GetMethod("ToString", new Type[] { typeof(ITransformFrame)}))
                }
            },
            new Dictionary<string, IFieldAccessFactory> {
                {
                    "vector",
                    new BoundPropertyLikeFieldAccessFactory(
                        "Fore vector of the rotation (i.e. looking/facing direction", () => VectorBinding.VectorType,
                        typeof(Direction), typeof(Direction).GetProperty("Vector"))
                }, {
                    "up_vector",
                    new BoundPropertyLikeFieldAccessFactory("Up vector of the rotation",
                        () => VectorBinding.VectorType, typeof(Direction),
                        typeof(Direction).GetProperty("UpVector"))
                }, {
                    "right_vector",
                    new BoundPropertyLikeFieldAccessFactory("Right vector of the rotation",
                        () => VectorBinding.VectorType, typeof(Direction),
                        typeof(Direction).GetProperty("RightVector"))
                },
            });

        public static Direction LookDirUp(Vector lookDirection, Vector upDirection) => Direction.LookRotation(lookDirection, upDirection);

        public static Direction Euler(ITransformFrame frame, double x, double y, double z) => new Direction(new Rotation(frame, QuaternionD.Euler(x, y, z)));

        public static Direction AngleAxis(double angle, Vector axis) => Direction.AngleAxis(angle, axis);

        public static Direction FromVectorToVector(Vector v1, Vector v2) => Direction.FromVectorToVector(v1, v2);
    }
}
