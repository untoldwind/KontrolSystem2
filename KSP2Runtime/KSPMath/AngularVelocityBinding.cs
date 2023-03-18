﻿using System;
using System.Collections.Generic;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public class AngularVelocityBinding {
        public static readonly BoundType AngularVelocityType = Direct.BindType("ksp::math", "AngularVelocity",
            "An angular velocity in space, that can be projected to a 3-dimensional vector in a specific frame of reference", typeof(VelocityAtPosition),
            new OperatorCollection {
            },
            new OperatorCollection {
            },
            new Dictionary<string, IMethodInvokeFactory> {
                {
                    "to_local",
                    new BoundMethodInvokeFactory("Get local angular velocity in a frame of reference", true,
                        () => Vector3Binding.Vector3Type,
                        () => new List<RealizedParameter> {new RealizedParameter("frame", TransformFrameBinding.TransformFrameType)}, false,
                        typeof(AngularVelocityBinding), typeof(AngularVelocityBinding).GetMethod("ToLocal"))
                }, {
                    "to_string",
                    new BoundMethodInvokeFactory("Convert angular velocity to string in a given coordinate system.", true, () => BuiltinType.String,
                        () => new List<RealizedParameter>() { new RealizedParameter("frame", TransformFrameBinding.TransformFrameType) }, false, typeof(VelocityBinding),
                        typeof(AngularVelocityBinding).GetMethod("ToString", new Type[] { typeof(AngularVelocity), typeof(ITransformFrame) }))
                }, {
                    "to_fixed",
                    new BoundMethodInvokeFactory("Convert angular velocity to string with fixed number of `decimals` in a given coordinate system.",
                        true,
                        () => BuiltinType.String,
                        () => new List<RealizedParameter>() {new RealizedParameter("frame", TransformFrameBinding.TransformFrameType), new RealizedParameter("decimals", BuiltinType.Int)},
                        false, typeof(AngularVelocityBinding), typeof(AngularVelocityBinding).GetMethod("ToFixed"))
                }, {
                    "relative_to",
                    new BoundMethodInvokeFactory("Get relative angular velocity to a frame of reference", true,
                        () => VectorBinding.VectorType,
                        () => new List<RealizedParameter> {new RealizedParameter("frame", TransformFrameBinding.TransformFrameType)}, false,
                        typeof(AngularVelocityBinding), typeof(AngularVelocityBinding).GetMethod("RelativeTo"))
                }, 
            },
            new Dictionary<string, IFieldAccessFactory> { });

        public static Vector3d ToLocal(AngularVelocity velocity, ITransformFrame frame) => frame.motionFrame.ToLocalAngularVelocity(velocity);

        public static string ToString(AngularVelocity velocity, ITransformFrame frame) => Vector3Binding.ToString(frame.motionFrame.ToLocalAngularVelocity(velocity));

        public static string ToFixed(AngularVelocity velocity, ITransformFrame frame, long decimals) =>
            Vector3Binding.ToFixed(frame.motionFrame.ToLocalAngularVelocity(velocity), decimals);

        public static Vector RelativeTo(AngularVelocity velocity, ITransformFrame frame) => frame.motionFrame.ToRelativeAngularVelocity(velocity);

    }
}
