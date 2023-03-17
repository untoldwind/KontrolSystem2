using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public struct VelocityAtPosition {
        public readonly Position position;
        public readonly Velocity velocity;

        public VelocityAtPosition(Velocity velocity, Position position) {
            this.position = position;
            this.velocity = velocity;
        }
    }
}
