using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPMath;

public struct VelocityAtPosition(Velocity velocity, Position position) {
    public Position position = position;
    public Velocity velocity = velocity;

    public Vector Vector {
        get => velocity.relativeVelocity;
        set => velocity.relativeVelocity = value;
    }
}
