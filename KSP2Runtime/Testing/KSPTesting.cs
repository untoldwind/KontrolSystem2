using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim;
using KSP.Sim.impl;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.Testing;

[KSModule("ksp::testing")]
public class KSPTesting : CoreTesting {
    [KSConstant("IDENTITY_COORDINATE_SYSTEM")]
    public static readonly ITransformFrame IDENTITY_COORDINATE_SYSTEM = new TransformModel("Ident").celestialFrame;

    [KSFunction]
    public static void AssertVec2(Vector2d expected, Vector2d actual, double delta = 1e-10) {
        TestContext?.IncrAssertions();
        if ((expected - actual).magnitude > delta)
            throw new AssertException($"assert_vec2: {expected} != {actual}");
    }

    [KSFunction]
    public static void AssertVec3(Vector3d expected, Vector3d actual, double delta = 1e-10) {
        TestContext?.IncrAssertions();
        if ((expected - actual).magnitude > delta)
            throw new AssertException($"assert_vec3: {expected} != {actual}");
    }
}
