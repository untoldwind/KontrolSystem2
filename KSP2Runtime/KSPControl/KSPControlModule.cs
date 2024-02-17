using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPControl;

[KSModule("ksp::control")]
public partial class KSPControlModule {
    [KSFunction(
        "pid_loop",
        Description = "Create a new PIDLoop with given parameters."
    )]
    public static PidLoop CreatePidLoop(double kp, double ki, double kd, double minOutput, double maxOutput,
        bool extraUnwind = false) {
        return new PidLoop(kp, ki, kd, minOutput, maxOutput, extraUnwind);
    }

    [KSFunction(
        "moving_average",
        Description = "Create a new MovingAverage with given sample limit."
    )]
    public static MovingAverage CreateMovingAverage(long sampleLimit) {
        return new MovingAverage { SampleLimit = sampleLimit };
    }

    [KSFunction(
        "torque_pi",
        Description = "Create a new TorquePI with given parameters.")]
    public static TorquePI CreateTorquePI(double ts) {
        var torquePi = new TorquePI();
        torquePi.Ts = ts;
        return torquePi;
    }
}
