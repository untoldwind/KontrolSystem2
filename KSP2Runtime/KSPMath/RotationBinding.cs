using System.Collections.Generic;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public class RotationBinding {
        public static readonly BoundType RotationType = Direct.BindType("ksp::math", "Rotation",
            "A rotation in a specific coordinate system", typeof(Rotation),
            new OperatorCollection { },
            new OperatorCollection { },
            new Dictionary<string, IMethodInvokeFactory> {
                {
                    "lerp_to",
                    new BoundMethodInvokeFactory(
                        "Linear interpolate position between this and `other` rotation, where `t = 0.0` is this and `t = 1.0` is `other`.",
                        true,
                        () => RotationType,
                        () => new List<RealizedParameter> {
                            new RealizedParameter("other", RotationType), new RealizedParameter("t", BuiltinType.Float)
                        }, false, typeof(Rotation), typeof(Rotation).GetMethod("Lerp"))
                },
            },
            new Dictionary<string, IFieldAccessFactory> {
                {
                    "local",
                    new BoundPropertyLikeFieldAccessFactory("coordinates in coordindate system",
                        () => DirectionBinding.DirectionType, typeof(RotationBinding),
                        typeof(RotationBinding).GetMethod("ToDirection"), null)
                }, {
                    "coordinate_system",
                    new BoundPropertyLikeFieldAccessFactory("coordindate system",
                        () => CoordindateSystemBinding.CoordindateSystemType, typeof(Rotation),
                        typeof(Rotation).GetProperty("coordinateSystem"))
                }
            });
        
        public static Direction ToDirection(Rotation rotation) => new Direction(rotation.localRotation);
    }
}
