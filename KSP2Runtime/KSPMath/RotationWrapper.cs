using System;
using KSP.Sim;
using SimRotation = KSP.Sim.Rotation;

namespace KontrolSystem.KSP.Runtime.KSPMath;

public class RotationWrapper {
    private SimRotation rotation;
    private Vector vector;

    public RotationWrapper() {
    }

    public RotationWrapper(SimRotation rotation) {
        this.rotation = rotation;
        vector = new Vector(this.rotation.coordinateSystem, rotation.localRotation * Vector3d.forward);
    }

    public RotationWrapper(Vector vector) {
        this.vector = Vector.normalize(vector);
        rotation = RotationFromVector(this.vector);
    }

    public Vector Vector {
        get => new(rotation.coordinateSystem, rotation.localRotation * Vector3d.forward);
        set {
            vector = Vector.normalize(value);
            rotation = RotationFromVector(vector);
        }
    }

    public Rotation Rotation {
        get => rotation;
        set {
            rotation = value;
            vector = new Vector(rotation.coordinateSystem, rotation.localRotation * Vector3d.forward);
        }
    }

    public Vector UpVector => new(rotation.coordinateSystem, rotation.localRotation * Vector3d.up);

    public Vector RightVector => new(rotation.coordinateSystem, rotation.localRotation * Vector3d.right);

    public Vector3d Euler(ITransformFrame frame) {
        return frame.ToLocalRotation(rotation).eulerAngles;
    }

    public double Pitch(ITransformFrame frame) {
        return frame.ToLocalRotation(rotation).eulerAngles.x;
    }

    public double Yaw(ITransformFrame frame) {
        return frame.ToLocalRotation(rotation).eulerAngles.y;
    }

    public double Roll(ITransformFrame frame) {
        return frame.ToLocalRotation(rotation).eulerAngles.z;
    }

    /// <summary>
    ///     Produces a direction that if it was applied to vector v1, would
    ///     cause it to rotate to be the same direction as vector v2.
    ///     Note that there are technically an infinite number of such
    ///     directions that it could legally return, because this method
    ///     does not control the roll, the roll being lost information
    ///     when dealing with just a single vector.
    /// </summary>
    /// <param name="v1">start from this vector</param>
    /// <param name="v2">go to this vector </param>
    /// <returns></returns>
    public static RotationWrapper FromVectorToVector(Vector v1, Vector v2) {
        return new RotationWrapper(SimRotation.FromTo(v1, v2));
    }

    /// <summary>
    ///     Produces a direction in which you are looking in lookdirection, and
    ///     rolled such that up is upDirection.
    ///     Note, lookDirection and Updirection do not have to be perpendicular, but
    ///     the farther from perpendicular they are, the worse the accuracy gets.
    /// </summary>
    /// <param name="lookDirection">direction to point</param>
    /// <param name="upDirection">direction for the 'TOP' of the roll axis</param>
    /// <returns>new direction.</returns>
    public static RotationWrapper LookRotation(Vector lookDirection, Vector upDirection) {
        return new RotationWrapper(SimRotation.LookRotation(lookDirection, upDirection));
    }

    // This next one doesn't have a common signature, but it's kept as a static
    // instead of a constructor because it's coherent with the other ones that way:

    /// <summary>
    ///     Make a rotation of a given angle around a given axis vector.
    ///     <param name="degrees">The angle around the axis to rotate, in degrees.</param>
    ///     <param name="axis">
    ///         The axis to rotate around.  Rotations use a left-hand rule because it's a left-handed coord
    ///         system.
    ///     </param>
    /// </summary>
    public static RotationWrapper AngleAxis(double degrees, Vector axis) {
        return new RotationWrapper(new Rotation(axis.coordinateSystem, QuaternionD.AngleAxis(degrees, axis.vector)));
    }

    public static RotationWrapper operator *(RotationWrapper a, RotationWrapper b) {
        return new RotationWrapper(new Rotation(a.rotation.coordinateSystem,
            a.rotation.localRotation * a.rotation.coordinateSystem.ToLocalRotation(b.rotation)));
    }

    public static Vector operator *(RotationWrapper a, Vector b) {
        return new Vector(a.rotation.coordinateSystem,
            a.rotation.localRotation * a.rotation.coordinateSystem.ToLocalVector(b));
    }

    public static RotationWrapper operator +(RotationWrapper a, RotationWrapper b) {
        return new RotationWrapper(new Rotation(a.rotation.coordinateSystem,
            QuaternionD.Euler(a.rotation.localRotation.eulerAngles +
                              a.rotation.coordinateSystem.ToLocalRotation(b.rotation).eulerAngles)));
    }

    public static RotationWrapper operator -(RotationWrapper a, RotationWrapper b) {
        return new RotationWrapper(new Rotation(a.rotation.coordinateSystem,
            QuaternionD.Euler(a.rotation.localRotation.eulerAngles -
                              a.rotation.coordinateSystem.ToLocalRotation(b.rotation).eulerAngles)));
    }

    public static RotationWrapper operator -(RotationWrapper a) {
        return new RotationWrapper(new Rotation(a.rotation.coordinateSystem,
            QuaternionD.Inverse(a.rotation.localRotation)));
    }

    public Direction ToLocal(ITransformFrame frame) {
        return new Direction(frame.ToLocalRotation(rotation));
    }

    public override bool Equals(object obj) {
        var compareType = typeof(Direction);
        if (compareType.IsInstanceOfType(obj)) {
            var d = obj as RotationWrapper;
            return d is not null &&
                   rotation.localRotation.Equals(rotation.coordinateSystem.ToLocalRotation(d.rotation));
        }

        return false;
    }

    // Needs to be overwritten because Equals() is overridden.
    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() {
        return rotation.GetHashCode();
    }

    public static bool operator ==(RotationWrapper a, RotationWrapper b) {
        var compareType = typeof(RotationWrapper);
        if (compareType.IsInstanceOfType(a))
            return a.Equals(b); // a is not null, we can use the built in equals function

        return !compareType.IsInstanceOfType(b); // a is null, return true if b is null and false if not null
    }

    public static bool operator !=(RotationWrapper a, RotationWrapper b) {
        return !(a == b);
    }


    public string ToString(ITransformFrame frame) {
        var euler = frame.ToLocalRotation(rotation).eulerAngles;
        return $"R({Math.Round(euler.x, 3)},{Math.Round(euler.y, 3)},{Math.Round(euler.z, 3)})";
    }

    private static SimRotation RotationFromVector(Vector vector) {
        var up = new Vector(vector.coordinateSystem, Vector3d.up);
        if (Math.Abs(Vector.dot(vector, up)) > 0.99)
            return SimRotation.LookRotation(vector, new Vector(vector.coordinateSystem, Vector3d.right));
        return SimRotation.LookRotation(vector, up);
    }
}
