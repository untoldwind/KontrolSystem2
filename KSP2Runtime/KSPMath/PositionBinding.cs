using System.Collections.Generic;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public class PositionBinding {
        public static readonly BoundType PositionType = Direct.BindType("ksp::math", "Position",
            "A position in space. This is a 3-dimensional vector in a specific coordinate system", typeof(Position),
            new OperatorCollection { },
            new OperatorCollection {
                {
                    Operator.Add,
                    new StaticMethodOperatorEmitter(() => VectorBinding.VectorType, () => PositionType,
                        typeof(Position).GetMethod("op_Addition", new[] { typeof(Position), typeof(Vector) }))
                }, {
                    Operator.AddAssign,
                    new StaticMethodOperatorEmitter(() => VectorBinding.VectorType, () => PositionType,
                        typeof(Position).GetMethod("op_Addition", new[] { typeof(Position), typeof(Vector) }))
                }, {
                    Operator.Sub,
                    new StaticMethodOperatorEmitter(() => PositionType, () => VectorBinding.VectorType,
                        typeof(Position).GetMethod("op_Subtraction", new[] { typeof(Position), typeof(Position) }))
                }, {
                    Operator.SubAssign,
                    new StaticMethodOperatorEmitter(() => PositionType, () => VectorBinding.VectorType,
                        typeof(Position).GetMethod("op_Subtraction", new[] { typeof(Position), typeof(Position) }))
                },
            },
            new Dictionary<string, IMethodInvokeFactory> {
                {
                    "distance",
                    new BoundMethodInvokeFactory("Calculate the distance of `other` position.", true,
                        () => BuiltinType.Float,
                        () => new List<RealizedParameter> { new RealizedParameter("other", PositionType) }, false,
                        typeof(Position), typeof(Position).GetMethod("Distance"))
                }, {
                    "distance_sqr",
                    new BoundMethodInvokeFactory("Calculate the squared distance of `other` position.", true,
                        () => BuiltinType.Float,
                        () => new List<RealizedParameter> { new RealizedParameter("other", PositionType) }, false,
                        typeof(Position), typeof(Position).GetMethod("DistanceSqr"))
                }, {
                    "lerp_to",
                    new BoundMethodInvokeFactory(
                        "Linear interpolate position between this and `other` position, where `t = 0.0` is this and `t = 1.0` is `other`.",
                        true,
                        () => PositionType,
                        () => new List<RealizedParameter> {
                            new RealizedParameter("other", PositionType), new RealizedParameter("t", BuiltinType.Float)
                        }, false, typeof(Position), typeof(Position).GetMethod("Lerp"))
                },
            },
            new Dictionary<string, IFieldAccessFactory> {
                {
                    "local",
                    new BoundPropertyLikeFieldAccessFactory("coordinates in coordindate system",
                        () => Vector3Binding.Vector3Type, typeof(Position),
                        typeof(Position).GetProperty("localPosition"))
                }, {
                    "coordinate_system",
                    new BoundPropertyLikeFieldAccessFactory("coordindate system",
                        () => CoordindateSystemBinding.CoordindateSystemType, typeof(Position),
                        typeof(Position).GetProperty("coordinateSystem"))
                }
            });
    }
}

