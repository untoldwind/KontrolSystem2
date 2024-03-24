using System;

namespace KontrolSystem.Benchmarks;

public class ExtraMath {
    public const double DegToRad = Math.PI / 180.0;
    public const double RadToDeg = 180.0 / Math.PI;

    //For some reason, Math doesn't have the inverse hyperbolic trigonometric functions:
    //asinh(x) = log(x + sqrt(x^2 + 1))
    public static double Asinh(double x) => Math.Log(x + Math.Sqrt(x * x + 1));

    //acosh(x) = log(x + sqrt(x^2 - 1))
    public static double Acosh(double x) => Math.Log(x + Math.Sqrt(x * x - 1));

    //atanh(x) = (log(1+x) - log(1-x))/2
    public static double Atanh(double x) => 0.5 * (Math.Log(1 + x) - Math.Log(1 - x));

    //since there doesn't seem to be a Math.Clamp?
    public static double Clamp(double x, double min, double max) {
        if (x < min) return min;
        if (x > max) return max;
        return x;
    }

    //keeps angles in the range 0 to 360
    public static double ClampDegrees360(double angle) {
        angle %= 360.0;
        if (angle < 0) return angle + 360.0;
        else return angle;
    }

    //keeps angles in the range -180 to 180
    public static double ClampDegrees180(double angle) {
        angle = ClampDegrees360(angle);
        if (angle > 180) angle -= 360;
        return angle;
    }

    public static double ClampRadiansTwoPi(double angle) {
        angle %= (2 * Math.PI);
        if (angle < 0) return angle + 2 * Math.PI;
        else return angle;
    }

    public static double ClampRadiansPi(double angle) {
        angle = ClampRadiansTwoPi(angle);
        if (angle > Math.PI) angle -= 2 * Math.PI;
        return angle;
    }
}
