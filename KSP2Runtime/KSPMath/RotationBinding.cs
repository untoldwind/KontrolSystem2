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
    public static class RotationBinding {
        public static readonly BoundType RotationType = Direct.BindType("ksp::math", "GlobalDirection",
            "Represents the rotation from an initial coordinate system when looking down the z-axis and \"up\" being the y-axis",
            typeof(RotationWrapper),
            new OperatorCollection {
                {
                    Operator.Neg,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Unit, () => RotationType,
                        typeof(Direction).GetMethod("op_UnaryNegation", new[] {typeof(Vector2d)}))
                },
            },
            new OperatorCollection {
                {
                    Operator.Add,
                    new StaticMethodOperatorEmitter(() => RotationType, () => RotationType,
                        typeof(RotationWrapper).GetMethod("op_Addition", new[] {typeof(RotationWrapper), typeof(RotationWrapper)}))
                }, {
                    Operator.AddAssign,
                    new StaticMethodOperatorEmitter(() => RotationType, () => RotationType,
                        typeof(RotationWrapper).GetMethod("op_Addition", new[] {typeof(RotationWrapper), typeof(RotationWrapper)}))
                }, {
                    Operator.Sub,
                    new StaticMethodOperatorEmitter(() => RotationType, () => RotationType,
                        typeof(RotationWrapper).GetMethod("op_Subtraction", new[] {typeof(RotationWrapper), typeof(RotationWrapper)}))
                }, {
                    Operator.SubAssign,
                    new StaticMethodOperatorEmitter(() => RotationType, () => RotationType,
                        typeof(RotationWrapper).GetMethod("op_Subtraction", new[] {typeof(RotationWrapper), typeof(RotationWrapper)}))
                }, {
                    Operator.Mul,
                    new StaticMethodOperatorEmitter(() => RotationType, () => RotationType,
                        typeof(RotationWrapper).GetMethod("op_Multiply", new[] {typeof(RotationWrapper), typeof(RotationWrapper)}))
                }, {
                    Operator.MulAssign,
                    new StaticMethodOperatorEmitter(() => RotationType, () => RotationType,
                        typeof(RotationWrapper).GetMethod("op_Multiply", new[] {typeof(RotationWrapper), typeof(RotationWrapper)}))
                }, {
                    Operator.Mul,
                    new StaticMethodOperatorEmitter(() => VectorBinding.VectorType, () => VectorBinding.VectorType,
                        typeof(RotationWrapper).GetMethod("op_Multiply", new[] {typeof(RotationWrapper), typeof(Vector)}))
                }, {
                    Operator.Eq,
                    new StaticMethodOperatorEmitter(() => RotationType, () => BuiltinType.Bool,
                        typeof(RotationWrapper).GetMethod("op_Equality", new[] {typeof(RotationWrapper), typeof(RotationWrapper)}))
                }, {
                    Operator.NotEq,
                    new StaticMethodOperatorEmitter(() => RotationType, () => BuiltinType.Bool,
                        typeof(RotationWrapper).GetMethod("op_Equality", new[] {typeof(RotationWrapper), typeof(RotationWrapper)}),
                        null, OpCodes.Ldc_I4_0, OpCodes.Ceq)
                },
            },
            new Dictionary<string, IMethodInvokeFactory> {
                {
                    "euler",
                    new BoundMethodInvokeFactory("Get euler angles in a specific coordinate system", true, () => Vector3Binding.Vector3Type,
                    () => new List<RealizedParameter>() { new RealizedParameter("frame", TransformFrameBinding.TransformFrameType) }, false, typeof(RotationWrapper),
                    typeof(RotationWrapper).GetMethod("Euler"))
                },
                {
                    "pitch",
                    new BoundMethodInvokeFactory("Get pitch angle in a specific coordinate system", true, () => BuiltinType.Float,
                        () => new List<RealizedParameter>() { new RealizedParameter("frame", TransformFrameBinding.TransformFrameType) }, false, typeof(RotationWrapper),
                        typeof(RotationWrapper).GetMethod("Pitch"))
                },
                {
                    "yaw",
                    new BoundMethodInvokeFactory("Get yaw angle in a specific coordinate system", true, () => BuiltinType.Float,
                        () => new List<RealizedParameter>() { new RealizedParameter("frame", TransformFrameBinding.TransformFrameType) }, false, typeof(RotationWrapper),
                        typeof(RotationWrapper).GetMethod("Yaw"))
                },
                {
                    "roll",
                    new BoundMethodInvokeFactory("Get roll angle in a specific coordinate system", true, () => BuiltinType.Float,
                        () => new List<RealizedParameter>() { new RealizedParameter("frame", TransformFrameBinding.TransformFrameType) }, false, typeof(RotationWrapper),
                        typeof(RotationWrapper).GetMethod("Roll"))
                },
                {
                    "to_local",
                    new BoundMethodInvokeFactory("Get local direction in a coordinate system", true,
                        () => DirectionBinding.DirectionType,
                        () => new List<RealizedParameter> {new RealizedParameter("frame", TransformFrameBinding.TransformFrameType)}, false,
                        typeof(RotationWrapper), typeof(RotationWrapper).GetMethod("ToLocal"))
                },
                {
                    "to_string",
                    new BoundMethodInvokeFactory("Convert the direction to string", true, () => BuiltinType.String,
                        () => new List<RealizedParameter>() { new RealizedParameter("frame", TransformFrameBinding.TransformFrameType) }, false, typeof(RotationWrapper),
                        typeof(RotationWrapper).GetMethod("ToString", new Type[] { typeof(ITransformFrame)}))
                }
            },
            new Dictionary<string, IFieldAccessFactory> {
                {
                    "vector",
                    new BoundPropertyLikeFieldAccessFactory(
                        "Fore vector of the rotation (i.e. looking/facing direction", () => VectorBinding.VectorType,
                        typeof(RotationWrapper), typeof(RotationWrapper).GetProperty("Vector"))
                }, {
                    "up_vector",
                    new BoundPropertyLikeFieldAccessFactory("Up vector of the rotation",
                        () => VectorBinding.VectorType, typeof(RotationWrapper),
                        typeof(RotationWrapper).GetProperty("UpVector"))
                }, {
                    "right_vector",
                    new BoundPropertyLikeFieldAccessFactory("Right vector of the rotation",
                        () => VectorBinding.VectorType, typeof(RotationWrapper),
                        typeof(RotationWrapper).GetProperty("RightVector"))
                },
            });

        public static RotationWrapper GlobalLookDirUp(Vector lookDirection, Vector upDirection) => RotationWrapper.LookRotation(lookDirection, upDirection);

        public static RotationWrapper GlobalEuler(ITransformFrame frame, double x, double y, double z) => new RotationWrapper(new Rotation(frame, QuaternionD.Euler(x, y, z)));

        public static RotationWrapper GlobalAngleAxis(double angle, Vector axis) => RotationWrapper.AngleAxis(angle, axis);

        public static RotationWrapper GlobalFromVectorToVector(Vector v1, Vector v2) => RotationWrapper.FromVectorToVector(v1, v2);
    }
}
