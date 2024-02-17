using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPMath;

public struct VelocityAtPosition {
    public Position position;
    public Velocity velocity;

    public VelocityAtPosition(Velocity velocity, Position position) {
        this.position = position;
        this.velocity = velocity;
    }

    public Vector Vector {
        get => velocity.relativeVelocity;
        set => velocity.relativeVelocity = value;
    }
}
