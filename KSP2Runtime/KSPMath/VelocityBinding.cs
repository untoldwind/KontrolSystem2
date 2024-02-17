using System.Collections.Generic;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPMath;

public class VelocityBinding {
    public static readonly BoundType VelocityType = Direct.BindType("ksp::math", "GlobalVelocity",
        "A velocity in space, that can be projected to a 3-dimensional vector in a specific frame of reference",
        typeof(VelocityAtPosition),
        new OperatorCollection(),
        new OperatorCollection(),
        new Dictionary<string, IMethodInvokeFactory> {
            {
                "to_relative",
                new BoundMethodInvokeFactory("Get relative velocity to frame of reference", true,
                    () => VectorBinding.VectorType,
                    () => new List<RealizedParameter> {
                        new("frame", TransformFrameBinding.TransformFrameType, "Frame of reference")
                    }, false,
                    typeof(VelocityBinding), typeof(VelocityBinding).GetMethod("ToRelative"))
            }, {
                "to_local",
                new BoundMethodInvokeFactory("Get local velocity in a frame of reference", true,
                    () => Vector3Binding.Vector3Type,
                    () => new List<RealizedParameter> {
                        new("frame", TransformFrameBinding.TransformFrameType, "Frame of reference")
                    }, false,
                    typeof(VelocityBinding), typeof(VelocityBinding).GetMethod("ToLocal"))
            }, {
                "to_string",
                new BoundMethodInvokeFactory("Convert vector to string in a given coordinate system.", true,
                    () => BuiltinType.String,
                    () => new List<RealizedParameter> {
                        new("frame", TransformFrameBinding.TransformFrameType, "Frame of reference")
                    }, false, typeof(VelocityBinding),
                    typeof(VelocityBinding).GetMethod("ToString",
                        new[] { typeof(VelocityAtPosition), typeof(ITransformFrame) }))
            }, {
                "to_fixed",
                new BoundMethodInvokeFactory(
                    "Convert the vector to string with fixed number of `decimals` in a given coordinate system.",
                    true,
                    () => BuiltinType.String,
                    () => new List<RealizedParameter> {
                        new("frame", TransformFrameBinding.TransformFrameType, "Frame of reference"),
                        new("decimals", BuiltinType.Int, "Number of decimals")
                    },
                    false, typeof(VelocityBinding), typeof(VelocityBinding).GetMethod("ToFixed"))
            }
        },
        new Dictionary<string, IFieldAccessFactory> {
            {
                "position",
                new BoundFieldAccessFactory("Position the velocity was measured at", () => PositionBinding.PositionType,
                    typeof(VelocityAtPosition), typeof(VelocityAtPosition).GetField("position"))
            }, {
                "vector",
                new BoundPropertyLikeFieldAccessFactory("Relative velocity vector", () => VectorBinding.VectorType,
                    typeof(VelocityAtPosition), typeof(VelocityAtPosition).GetProperty("Vector"))
            }
        });

    public static Vector ToRelative(VelocityAtPosition velocity, ITransformFrame frame) {
        return frame.motionFrame.ToRelativeVelocity(velocity.velocity, velocity.position);
    }

    public static Vector3d ToLocal(VelocityAtPosition velocity, ITransformFrame frame) {
        return frame.ToLocalVector(frame.motionFrame.ToRelativeVelocity(velocity.velocity, velocity.position));
    }

    public static string ToString(VelocityAtPosition velocity, ITransformFrame frame) {
        return Vector3Binding.ToString(frame.motionFrame.ToLocalVelocity(velocity.velocity, velocity.position));
    }

    public static string ToFixed(VelocityAtPosition velocity, ITransformFrame frame, long decimals) {
        return Vector3Binding.ToFixed(frame.motionFrame.ToLocalVelocity(velocity.velocity, velocity.position),
            decimals);
    }
}
