using System;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public class Direction {
        private Vector3d euler;
        private Quaternion rotation;
        private Vector3d vector;

        public Direction() {
        }

        public Direction(Quaternion q) {
            rotation = q;
            euler = q.eulerAngles;
            vector = rotation * Vector3d.forward;
        }

        public Direction(Vector3d v3D, bool isEuler) {
            if (isEuler) {
                Euler = v3D;
            } else {
                Vector = v3D;
            }
        }

        public Vector3d Vector {
            get => vector;
            set {
                vector = value.normalized;
                rotation = Quaternion.LookRotation(value);
                euler = rotation.eulerAngles;
            }
        }

        public Vector3d Euler {
            get => euler;
            set {
                euler = value;
                rotation = Quaternion.Euler(value);
                vector = rotation * Vector3d.forward;
            }
        }

        public Quaternion Rotation {
            get => rotation;
            set {
                rotation = value;
                euler = value.eulerAngles;
                vector = rotation * Vector3d.forward;
            }
        }

        public double Pitch => Euler.x;

        public double Yaw => Euler.y;

        public double Roll => Euler.z;

        public Vector3d UpVector => rotation * Vector3d.up;

        public Vector3d RightVector => rotation * Vector3d.right;

        /// <summary>
        /// Produces a direction that if it was applied to vector v1, would
        /// cause it to rotate to be the same direction as vector v2.
        /// Note that there are technically an infinite number of such
        /// directions that it could legally return, because this method
        /// does not control the roll, the roll being lost information
        /// when dealing with just a single vector.
        /// </summary>
        /// <param name="v1">start from this vector</param>
        /// <param name="v2">go to this vector </param>
        /// <returns></returns>
        public static Direction FromVectorToVector(Vector3d v1, Vector3d v2) =>
            new Direction(Quaternion.FromToRotation(v1, v2));

        /// <summary>
        /// Produces a direction in which you are looking in lookdirection, and
        /// rolled such that up is upDirection.
        /// Note, lookDirection and Updirection do not have to be perpendicular, but
        /// the farther from perpendicular they are, the worse the accuracy gets.
        /// </summary>
        /// <param name="lookDirection">direction to point</param>
        /// <param name="upDirection">direction for the 'TOP' of the roll axis</param>
        /// <returns>new direction.</returns>
        public static Direction LookRotation(Vector3d lookDirection, Vector3d upDirection) =>
            new Direction(Quaternion.LookRotation(lookDirection, upDirection));

        // This next one doesn't have a common signature, but it's kept as a static
        // instead of a constructor because it's coherent with the other ones that way:

        /// <summary>
        /// Make a rotation of a given angle around a given axis vector.
        /// <param name="degrees">The angle around the axis to rotate, in degrees.</param>
        /// <param name="axis">The axis to rotate around.  Rotations use a left-hand rule because it's a left-handed coord system.</param>
        /// </summary>
        public static Direction AngleAxis(double degrees, Vector3d axis) =>
            new Direction(Quaternion.AngleAxis((float)degrees, axis));

        public static Direction operator *(Direction a, Direction b) => new Direction(a.Rotation * b.Rotation);

        public static Vector3d operator *(Direction a, Vector3d b) => a.Rotation * b;

        public static Vector3d operator *(Vector3d b, Direction a) => a.Rotation * b;

        public static Vector3d operator +(Direction a, Vector3d b) => a.Rotation * b;

        public static Vector3d operator +(Vector3d b, Direction a) => a.Rotation * b;

        public static Direction operator +(Direction a, Direction b) => new Direction(a.Euler + b.Euler, true);

        public static Direction operator -(Direction a, Direction b) => new Direction(a.Euler - b.Euler, true);

        public static Direction operator -(Direction a) => new Direction(Quaternion.Inverse(a.rotation));

        public override bool Equals(object obj) {
            Type compareType = typeof(Direction);
            if (compareType.IsInstanceOfType(obj)) {
                Direction d = obj as Direction;
                return d is { } && rotation.Equals(d.rotation);
            }

            return false;
        }

        // Needs to be overwritten because Equals() is overridden.
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => rotation.GetHashCode();

        public static bool operator ==(Direction a, Direction b) {
            Type compareType = typeof(Direction);
            if (compareType.IsInstanceOfType(a)) {
                return a.Equals(b); // a is not null, we can use the built in equals function
            }

            return !compareType.IsInstanceOfType(b); // a is null, return true if b is null and false if not null
        }

        public static bool operator !=(Direction a, Direction b) => !(a == b);

        /// <summary>
        /// Returns this rotation relative to a starting rotation - ie.. how you would
        /// have to rotate from that start rotation to get to this one.
        /// </summary>
        /// <param name="fromDir">start rotation.</param>
        /// <returns>new Direction representing such a rotation.</returns>
        public Direction RelativeFrom(Direction fromDir) =>
            new Direction(Quaternion.RotateTowards(fromDir.rotation, rotation, 99999.0f));

        public override string ToString() =>
            $"R({Math.Round(euler.x, 3)},{Math.Round(euler.y, 3)},{Math.Round(euler.z, 3)})";
    }
}
