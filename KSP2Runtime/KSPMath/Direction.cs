﻿using System;
using KSP.Sim;
using UnityEngine;
using SimRotation = KSP.Sim.Rotation;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public class Direction {
        private SimRotation rotation;
        private Vector vector;

        public Direction() {
        }

        public Direction(SimRotation rotation) {
            this.rotation = rotation;
            vector = new Vector(this.rotation.coordinateSystem, rotation.localRotation * Vector3d.forward);
        }

        public Vector Vector {
            get => vector;
            set {
                vector = Vector.normalize(value);
                rotation = SimRotation.LookRotation(vector, new Vector(vector.coordinateSystem, Vector3d.up));
            }
        }

        public Vector3d Euler(ITransformFrame frame) => frame.ToLocalRotation(rotation).eulerAngles;

        public Rotation Rotation {
            get => rotation;
            set {
                rotation = value;
                vector = new Vector(this.rotation.coordinateSystem, rotation.localRotation * Vector3d.forward);
            }
        }

        public double Pitch(ITransformFrame frame) => frame.ToLocalRotation(rotation).eulerAngles.x;

        public double Yaw(ITransformFrame frame) => frame.ToLocalRotation(rotation).eulerAngles.y;

        public double Roll(ITransformFrame frame) => frame.ToLocalRotation(rotation).eulerAngles.z;

        public Vector UpVector => new Vector(this.rotation.coordinateSystem, rotation.localRotation * Vector3d.up);

        public Vector RightVector => new Vector(this.rotation.coordinateSystem, rotation.localRotation * Vector3d.right);

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
        public static Direction FromVectorToVector(Vector v1, Vector v2) => new Direction(SimRotation.FromTo(v1, v2));

        /// <summary>
        /// Produces a direction in which you are looking in lookdirection, and
        /// rolled such that up is upDirection.
        /// Note, lookDirection and Updirection do not have to be perpendicular, but
        /// the farther from perpendicular they are, the worse the accuracy gets.
        /// </summary>
        /// <param name="lookDirection">direction to point</param>
        /// <param name="upDirection">direction for the 'TOP' of the roll axis</param>
        /// <returns>new direction.</returns>
        public static Direction LookRotation(Vector lookDirection, Vector upDirection) => new Direction(SimRotation.LookRotation(lookDirection, upDirection));

        // This next one doesn't have a common signature, but it's kept as a static
        // instead of a constructor because it's coherent with the other ones that way:

        /// <summary>
        /// Make a rotation of a given angle around a given axis vector.
        /// <param name="degrees">The angle around the axis to rotate, in degrees.</param>
        /// <param name="axis">The axis to rotate around.  Rotations use a left-hand rule because it's a left-handed coord system.</param>
        /// </summary>
        public static Direction AngleAxis(double degrees, Vector axis) =>
            new Direction(new Rotation(axis.coordinateSystem, QuaternionD.AngleAxis(degrees, axis.vector)));

        public static Direction operator *(Direction a, Direction b) => new Direction(new Rotation(a.rotation.coordinateSystem, a.rotation.localRotation * a.rotation.coordinateSystem.ToLocalRotation(b.rotation)));

        public static Vector operator *(Direction a, Vector b) => new Vector(a.rotation.coordinateSystem, a.rotation.localRotation * a.rotation.coordinateSystem.ToLocalVector(b));

        public static Direction operator +(Direction a, Direction b) => new Direction(new Rotation(a.rotation.coordinateSystem, QuaternionD.Euler(a.rotation.localRotation.eulerAngles + a.rotation.coordinateSystem.ToLocalRotation(b.rotation).eulerAngles)));

        public static Direction operator -(Direction a, Direction b) => new Direction(new Rotation(a.rotation.coordinateSystem, QuaternionD.Euler(a.rotation.localRotation.eulerAngles - a.rotation.coordinateSystem.ToLocalRotation(b.rotation).eulerAngles)));

        public static Direction operator -(Direction a) => new Direction(new Rotation(a.rotation.coordinateSystem, QuaternionD.Inverse(a.rotation.localRotation)));

        public override bool Equals(object obj) {
            Type compareType = typeof(Direction);
            if (compareType.IsInstanceOfType(obj)) {
                Direction d = obj as Direction;
                return d is { } && rotation.localRotation.Equals(rotation.coordinateSystem.ToLocalRotation(d.rotation));
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

        public string ToString(ITransformFrame frame) {
            var euler = frame.ToLocalRotation(rotation).eulerAngles;
            return $"R({Math.Round(euler.x, 3)},{Math.Round(euler.y, 3)},{Math.Round(euler.z, 3)})";
        }
    }
}
