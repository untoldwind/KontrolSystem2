using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    [KSModule("ksp::control")]
    public partial class KSPControlModule {
        [KSFunction(
            "pid_loop",
            Description = "Create a new PIDLoop with given parameters."
        )]
        public static PidLoop CreatePidLoop(double kp, double ki, double kd, double minOutput, double maxOutput) =>
            new PidLoop(kp, ki, kd, minOutput, maxOutput);

        [KSFunction(
            "moving_average",
            Description = "Create a new MovingAverage with given sample limit."
        )]
        public static MovingAverage CreateMovingAverage(long sampleLimit) =>
            new MovingAverage() { SampleLimit = sampleLimit };
    }
}
