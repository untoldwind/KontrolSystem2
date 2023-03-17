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
    public class VectorBinding {
        public static readonly BoundType VectorType = Direct.BindType("ksp::math", "Vector",
            "Abstract vector in space that can be projected to a concrete 3-dimensional vector in a specific coordinate system", typeof(Vector),
            new OperatorCollection {
                {
                    Operator.Neg,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Unit, () => VectorType,
                        typeof(Vector).GetMethod("negate", new[] {typeof(Vector)}))
                }, {
                    Operator.Mul,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Float, () => VectorType,
                        typeof(VectorBinding).GetMethod("Multiply", new[] {typeof(double), typeof(Vector)}))
                },
            },
            new OperatorCollection {
                {
                    Operator.Add,
                    new StaticMethodOperatorEmitter(() => VectorType, () => VectorType,
                        typeof(Vector).GetMethod("op_Addition", new[] {typeof(Vector), typeof(Vector)}))
                }, {
                    Operator.AddAssign,
                    new StaticMethodOperatorEmitter(() => VectorType, () => VectorType,
                        typeof(Vector).GetMethod("op_Addition", new[] {typeof(Vector), typeof(Vector)}))
                }, {
                    Operator.Sub,
                    new StaticMethodOperatorEmitter(() => VectorType, () => VectorType,
                        typeof(Vector).GetMethod("op_Subtraction", new[] {typeof(Vector), typeof(Vector)}))
                }, {
                    Operator.SubAssign,
                    new StaticMethodOperatorEmitter(() => VectorType, () => VectorType,
                        typeof(Vector).GetMethod("op_Subtraction", new[] {typeof(Vector), typeof(Vector)}))
                }, {
                    Operator.Mul,
                    new StaticMethodOperatorEmitter(() => VectorBinding.VectorType, () => BuiltinType.Float,
                        typeof(Vector).GetMethod("dot"))
                }, {
                    Operator.Mul,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Float, () => VectorType,
                        typeof(Vector).GetMethod("op_Multiply", new[] {typeof(Vector), typeof(double)}))
                }, {
                    Operator.MulAssign,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Float, () => VectorType,
                        typeof(Vector).GetMethod("op_Multiply", new[] {typeof(Vector), typeof(double)}))
                },
            },
            new Dictionary<string, IMethodInvokeFactory> {
                {
                    "to_local",
                    new BoundMethodInvokeFactory("Get local vector in a coordinate system", true,
                        () => Vector3Binding.Vector3Type,
                        () => new List<RealizedParameter> {new RealizedParameter("frame", TransformFrameBinding.TransformFrameType)}, false,
                        typeof(VectorBinding), typeof(VectorBinding).GetMethod("ToLocal"))
                }, {
                    "to_string",
                    new BoundMethodInvokeFactory("Convert vector to string in a given coordinate system.", true, () => BuiltinType.String,
                        () => new List<RealizedParameter>() { new RealizedParameter("frame", TransformFrameBinding.TransformFrameType) }, false, typeof(VectorBinding),
                        typeof(VectorBinding).GetMethod("ToString", new Type[] { typeof(Vector), typeof(ITransformFrame) }))
                }, {
                    "to_fixed",
                    new BoundMethodInvokeFactory("Convert the vector to string with fixed number of `decimals` in a given coordinate system.",
                        true,
                        () => BuiltinType.String,
                        () => new List<RealizedParameter>() {new RealizedParameter("frame", TransformFrameBinding.TransformFrameType), new RealizedParameter("decimals", BuiltinType.Int)},
                        false, typeof(VectorBinding), typeof(VectorBinding).GetMethod("ToFixed"))
                }, {
                    "to_direction",
                    new BoundMethodInvokeFactory("Convert the vector to a direction in space.",
                        true,
                        () => DirectionBinding.DirectionType,
                        () => new List<RealizedParameter>() { },
                        false, typeof(VectorBinding), typeof(VectorBinding).GetMethod("ToDirection"))
                }, {
                    "cross",
                    new BoundMethodInvokeFactory("Calculate the cross/other product with `other` vector.", true,
                        () => VectorType,
                        () => new List<RealizedParameter> {new RealizedParameter("other", VectorType)}, false,
                        typeof(Vector), typeof(Vector).GetMethod("cross"))
                }, {
                    "dot",
                    new BoundMethodInvokeFactory("Calculate the dot/inner product with `other` vector.", true,
                        () => BuiltinType.Float,
                        () => new List<RealizedParameter> {new RealizedParameter("other", VectorType)}, false,
                        typeof(Vector), typeof(Vector).GetMethod("dot"))
                }, {
                    "lerp_to",
                    new BoundMethodInvokeFactory(
                        "Linear interpolate position between this and `other` vector, where `t = 0.0` is this and `t = 1.0` is `other`.",
                        true,
                        () => VectorType,
                        () => new List<RealizedParameter> {
                            new RealizedParameter("other", VectorType), new RealizedParameter("t", BuiltinType.Float)
                        }, false, typeof(Vector), typeof(Vector).GetMethod("Lerp"))
                }, {
                    "exclude_from",
                    new BoundMethodInvokeFactory("Exclude this from `other` vector.", true, () => VectorType,
                        () => new List<RealizedParameter> {new RealizedParameter("other", VectorType)}, false,
                        typeof(VectorBinding), typeof(VectorBinding).GetMethod("ExcludeFrom"))
                }, 
            },
            new Dictionary<string, IFieldAccessFactory> {
                {
                    "magnitude",
                    new BoundPropertyLikeFieldAccessFactory("Magnitude/length of the vector", () => BuiltinType.Float,
                        typeof(Vector), typeof(Vector).GetProperty("magnitude"))
                }, {
                    "sqr_magnitude",
                    new BoundPropertyLikeFieldAccessFactory("Squared magnitude of the vector", () => BuiltinType.Float,
                        typeof(Vector), typeof(Vector).GetProperty("sqrMagnitude"))
                }, {
                    "normalized",
                    new BoundPropertyLikeFieldAccessFactory("Normalized vector (i.e. scaled to length 1)",
                        () => VectorType, typeof(Vector), typeof(Vector).GetMethod("normalize"), null)
                },
            });

        public static Vector3d ToLocal(Vector vector, ITransformFrame frame) => frame.ToLocalVector(vector);

        public static string ToString(Vector v, ITransformFrame frame) => Vector3Binding.ToString(frame.ToLocalVector(v));

        public static string ToFixed(Vector v, ITransformFrame frame, long decimals) =>
            Vector3Binding.ToFixed(frame.ToLocalVector(v), decimals);

        public static Direction ToDirection(Vector v) => new Direction(v);

        public static Vector ExcludeFrom(Vector v, Vector other) {
            var otherLocal = v.coordinateSystem.ToLocalVector(other);
            return new Vector(v.coordinateSystem, Vector3d.Exclude(v.vector, otherLocal));
        }

        public static Vector Multiply(double scale, Vector v) => v * scale;
    }
}
