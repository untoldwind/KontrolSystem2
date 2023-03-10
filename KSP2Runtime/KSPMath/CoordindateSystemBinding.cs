using System;
using System.Collections.Generic;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Api;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public class CoordindateSystemBinding {
        public static readonly BoundType CoordindateSystemType = Direct.BindType("ksp::math", "CoordinateSystem",
            "Representation of a coordinate system", typeof(ICoordinateSystem),
            new OperatorCollection { },
            new OperatorCollection { },
            new Dictionary<string, IMethodInvokeFactory> { },
            new Dictionary<string, IFieldAccessFactory> {
                { "forward", new BoundPropertyLikeFieldAccessFactory("forward vector of the coordinate system", () => VectorBinding.VectorType, typeof(ICoordinateSystem), typeof(ICoordinateSystem).GetProperty("forward") )},
                { "back", new BoundPropertyLikeFieldAccessFactory("backward vector of the coordinate system", () => VectorBinding.VectorType, typeof(ICoordinateSystem), typeof(ICoordinateSystem).GetProperty("back") )},
                { "up", new BoundPropertyLikeFieldAccessFactory("up vector of the coordinate system", () => VectorBinding.VectorType, typeof(ICoordinateSystem), typeof(ICoordinateSystem).GetProperty("up") )},
                { "down", new BoundPropertyLikeFieldAccessFactory("down vector of the coordinate system", () => VectorBinding.VectorType, typeof(ICoordinateSystem), typeof(ICoordinateSystem).GetProperty("down") )},
                { "right", new BoundPropertyLikeFieldAccessFactory("right vector of the coordinate system", () => VectorBinding.VectorType, typeof(ICoordinateSystem), typeof(ICoordinateSystem).GetProperty("right") )},
                { "left", new BoundPropertyLikeFieldAccessFactory("left vector of the coordinate system", () => VectorBinding.VectorType, typeof(ICoordinateSystem), typeof(ICoordinateSystem).GetProperty("left") )},
            }
        );
    }

    public class CoordinateSystemProvider : ICoordinateSystem {
        private readonly Func<ICoordinateSystem> provider;

        public CoordinateSystemProvider(Func<ICoordinateSystem> provider) {
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
    }
}
