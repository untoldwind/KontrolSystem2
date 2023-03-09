using System;
using System.Collections.Generic;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Api;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public class TransformFrameBinding {
        public static readonly BoundType TransformFrameType = Direct.BindType("ksp::math", "TransformFrame",
            "Representation of a coordinate frame of reference", typeof(ITransformFrame),
            new OperatorCollection { },
            new OperatorCollection { },
            new Dictionary<string, IMethodInvokeFactory> {
                {
                    "to_local_position",
                    new BoundMethodInvokeFactory("Get local coordinates of a position", true, () => Vector3Binding.Vector3Type,
                        () => new List<RealizedParameter>() { new RealizedParameter("position", PositionBinding.PositionType) }, false, typeof(ITransformFrame),
                        typeof(ICoordinateSystem).GetMethod("ToLocalPosition", new Type[] { typeof(Position) }))
                },
                {
                    "to_local_vector",
                    new BoundMethodInvokeFactory("Get local coordinates of a vector", true, () => Vector3Binding.Vector3Type,
                        () => new List<RealizedParameter>() { new RealizedParameter("position", VectorBinding.VectorType) }, false, typeof(ITransformFrame),
                        typeof(ICoordinateSystem).GetMethod("ToLocalVector", new Type[] { typeof(Vector) }))
                },
            },
            new Dictionary<string, IFieldAccessFactory> {
                { "forward", new BoundPropertyLikeFieldAccessFactory("forward vector of the coordinate system", () => VectorBinding.VectorType, typeof(ITransformFrame), typeof(ICoordinateSystem).GetProperty("forward") )},
                { "back", new BoundPropertyLikeFieldAccessFactory("backward vector of the coordinate system", () => VectorBinding.VectorType, typeof(ITransformFrame), typeof(ICoordinateSystem).GetProperty("back") )},
                { "up", new BoundPropertyLikeFieldAccessFactory("up vector of the coordinate system", () => VectorBinding.VectorType, typeof(ITransformFrame), typeof(ICoordinateSystem).GetProperty("up") )},
                { "down", new BoundPropertyLikeFieldAccessFactory("down vector of the coordinate system", () => VectorBinding.VectorType, typeof(ITransformFrame), typeof(ICoordinateSystem).GetProperty("down") )},
                { "right", new BoundPropertyLikeFieldAccessFactory("right vector of the coordinate system", () => VectorBinding.VectorType, typeof(ITransformFrame), typeof(ICoordinateSystem).GetProperty("right") )},
                { "left", new BoundPropertyLikeFieldAccessFactory("left vector of the coordinate system", () => VectorBinding.VectorType, typeof(ITransformFrame), typeof(ICoordinateSystem).GetProperty("left") )},
            }
        );
    }

    public class TransformFrameProvider : ITransformFrame {
        private readonly Func<ITransformFrame> provider;

        public TransformFrameProvider(Func<ITransformFrame> provider) {
            this.provider = provider;
        }

        public Vector3d ToLocalPosition(Position position) => provider().ToLocalPosition(position);

        public Vector3d ToLocalPosition(ICoordinateSystem coordinateSystem, Vector3d coordinateSystemPosition) => provider().ToLocalPosition(coordinateSystem, coordinateSystemPosition);

        public Vector3d ToLocalVector(Vector vector) => provider().ToLocalVector(vector);

        public Vector3d ToLocalVector(ICoordinateSystem coordinateSystem, Vector3d coordinateSystemVector) => provider().ToLocalVector(coordinateSystem, coordinateSystemVector);

        public QuaternionD ToLocalRotation(Rotation rotation) => provider().ToLocalRotation(rotation);

        public QuaternionD ToLocalRotation(ICoordinateSystem coordinateSystem, QuaternionD coordinateSystemRotation) =>
            provider().ToLocalRotation(coordinateSystem, coordinateSystemRotation);

        public Matrix4x4D ToLocalTransformationMatrix(ICoordinateSystem coordinateSystem) => provider().ToLocalTransformationMatrix(coordinateSystem);

        public Vector forward => provider().forward;
        public Vector back => provider().back;
        public Vector up => provider().up;
        public Vector down => provider().down;
        public Vector right => provider().right;
        public Vector left => provider().left;

        public TransformFrameType type => provider().type;

        public ITransformModel transform => provider().transform;

        public IMotionFrame motionFrame => provider().motionFrame;
    }
}
