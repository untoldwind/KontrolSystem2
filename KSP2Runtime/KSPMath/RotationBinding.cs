using System.Collections.Generic;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Api;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public class RotationBinding {
        public static readonly BoundType RotationType = Direct.BindType("ksp::math", "Rotation",
            "A rotation in a specific coordinate system", typeof(Rotation),
            new OperatorCollection { },
            new OperatorCollection { },
            new Dictionary<string, IMethodInvokeFactory> {
                {
                    "to_local",
                    new BoundMethodInvokeFactory("Get local direction in a coordinate system", true,
                        () => DirectionBinding.DirectionType,
                        () => new List<RealizedParameter> {new RealizedParameter("frame", TransformFrameBinding.TransformFrameType)}, false,
                        typeof(RotationBinding), typeof(RotationBinding).GetMethod("ToLocal"))
                },
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
            new Dictionary<string, IFieldAccessFactory> { });

        public static Direction ToLocal(Rotation rotation, ICoordinateSystem coordinateSystem) => new Direction(coordinateSystem.ToLocalRotation(rotation));
    }
}
