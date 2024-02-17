using System.Collections.Generic;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Sim;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPMath;

public class KSPMathModule {
    private const string ModuleName = "ksp::math";

    private static KSPMathModule? _instance;

    public IKontrolModule module;

    private KSPMathModule() {
        var types = new List<RealizedType> {
            Vector2Binding.Vector2Type,
            Vector3Binding.Vector3Type,
            DirectionBinding.DirectionType,
            Matrix2x2Binding.Matrix2x2Type,
            AngularVelocityBinding.AngularVelocityType,
            PositionBinding.PositionType,
            TransformFrameBinding.TransformFrameType,
            VectorBinding.VectorType,
            VelocityBinding.VelocityType,
            RotationBinding.RotationType
        };

        BindingGenerator.RegisterTypeMapping(typeof(Vector2d), Vector2Binding.Vector2Type);
        BindingGenerator.RegisterTypeMapping(typeof(Vector3d), Vector3Binding.Vector3Type);
        BindingGenerator.RegisterTypeMapping(typeof(Direction), DirectionBinding.DirectionType);
        BindingGenerator.RegisterTypeMapping(typeof(Matrix2x2), Matrix2x2Binding.Matrix2x2Type);
        BindingGenerator.RegisterTypeMapping(typeof(Matrix4x4D), Matrix4x4Binding.Matrix4x4Type);
        BindingGenerator.RegisterTypeMapping(typeof(ITransformFrame), TransformFrameBinding.TransformFrameType);
        BindingGenerator.RegisterTypeMapping(typeof(Position), PositionBinding.PositionType);
        BindingGenerator.RegisterTypeMapping(typeof(Vector), VectorBinding.VectorType);
        BindingGenerator.RegisterTypeMapping(typeof(VelocityAtPosition), VelocityBinding.VelocityType);
        BindingGenerator.RegisterTypeMapping(typeof(AngularVelocity), AngularVelocityBinding.AngularVelocityType);
        BindingGenerator.RegisterTypeMapping(typeof(RotationWrapper), RotationBinding.RotationType);

        var constants = new List<CompiledKontrolConstant>();

        var functions = new List<CompiledKontrolFunction> {
            Direct.BindFunction(typeof(Vector2Binding), "Vec2", "Create a new 2-dimensional vector", typeof(double),
                typeof(double)),
            Direct.BindFunction(typeof(Vector3Binding), "Vec3", "Create a new 3-dimensional vector", typeof(double),
                typeof(double), typeof(double)),
            Direct.BindFunction(typeof(DirectionBinding), "Euler", "Create a Direction from euler angles in degree",
                typeof(double), typeof(double), typeof(double)),
            Direct.BindFunction(typeof(DirectionBinding), "AngleAxis",
                "Create a Direction from a given axis with rotation angle in degree", typeof(double),
                typeof(Vector3d)),
            Direct.BindFunction(typeof(DirectionBinding), "FromVectorToVector",
                "Create a Direction to rotate from one vector to another", typeof(Vector3d), typeof(Vector3d)),
            Direct.BindFunction(typeof(DirectionBinding), "LookDirUp",
                "Create a Direction from a fore-vector and an up-vector", typeof(Vector3d), typeof(Vector3d)),
            Direct.BindFunction(typeof(RotationBinding), "GlobalEuler",
                "Create a Direction from euler angles in degree",
                typeof(ITransformFrame), typeof(double), typeof(double), typeof(double)),
            Direct.BindFunction(typeof(RotationBinding), "GlobalAngleAxis",
                "Create a Direction from a given axis with rotation angle in degree", typeof(double),
                typeof(Vector)),
            Direct.BindFunction(typeof(RotationBinding), "GlobalFromVectorToVector",
                "Create a Direction to rotate from one vector to another", typeof(Vector), typeof(Vector)),
            Direct.BindFunction(typeof(RotationBinding), "GlobalLookDirUp",
                "Create a Direction from a fore-vector and an up-vector", typeof(Vector), typeof(Vector)),

            Direct.BindFunction(typeof(ExtraMath), "AngleDelta",
                "Calculate the difference between to angles in degree (-180 .. 180)", typeof(double),
                typeof(double)),
            Direct.BindFunction(typeof(Matrix2x2Binding), "Matrix2x2", "Create a new 2-dimensional matrix",
                typeof(double), typeof(double), typeof(double), typeof(double)),
            Direct.BindFunction(typeof(Matrix4x4Binding), "Matrix4x4", "Create a new 4-dimensional matrix")
        };

        module = Direct.BindModule(ModuleName, "Collection of KSP/Unity related mathematical functions.", types,
            constants, functions);
    }

    public static KSPMathModule Instance {
        get {
            if (_instance == null) _instance = new KSPMathModule();
            return _instance;
        }
    }
}
