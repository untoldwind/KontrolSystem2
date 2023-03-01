namespace KontrolSystem.KSP.Runtime.KSPMath {
    public class ExtraMath {
        public static double AngleDelta(double a, double b) {
            var delta = b - a;

            return DegreeFix(delta, -180.0);
        }

        public static double DegreeFix(double inAngle, double rangeStart) {
            double rangeEnd = rangeStart + 360.0;
            double outAngle = inAngle;
            while (outAngle > rangeEnd)
                outAngle -= 360.0;
            while (outAngle < rangeStart)
                outAngle += 360.0;
            return outAngle;
        }
    }
}
