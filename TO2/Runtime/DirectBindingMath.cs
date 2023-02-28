using System;
using System.Collections.Generic;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.TO2.Runtime {
    public static class DirectBindingMath {
        public const string ModuleName = "core::math";

        public static readonly double Pi = Math.PI;
        public static readonly double E = Math.E;

        public static readonly double DegToRad = Math.PI / 180.0;
        public static readonly double RadToDeg = 180.0 / Math.PI;

        public static readonly long MaxInt = Int64.MaxValue;
        public static readonly long MinInt = Int64.MinValue;
        public static readonly double MaxFloat = Double.MaxValue;
        public static readonly double MinFloat = Double.MinValue;
        public static readonly double Epsilon = Double.Epsilon;

        //For some reason, Math doesn't have the inverse hyperbolic trigonometric functions:
        //asinh(x) = log(x + sqrt(x^2 + 1))
        public static double Asinh(double x) => Math.Log(x + Math.Sqrt(x * x + 1));

        //acosh(x) = log(x + sqrt(x^2 - 1))
        public static double Acosh(double x) => Math.Log(x + Math.Sqrt(x * x - 1));

        //atanh(x) = (log(1+x) - log(1-x))/2
        public static double Atanh(double x) => 0.5 * (Math.Log(1 + x) - Math.Log(1 - x));

        public static double SinDeg(double x) => Math.Sin(x * DegToRad);

        public static double CosDeg(double x) => Math.Cos(x * DegToRad);

        public static double TanDeg(double x) => Math.Tan(x * DegToRad);

        public static double AsinDeg(double x) => Math.Asin(x) * RadToDeg;

        public static double AcosDeg(double x) => Math.Acos(x) * RadToDeg;

        public static double AtanDeg(double x) => Math.Atan(x) * RadToDeg;

        public static double Atan2Deg(double y, double x) => Math.Atan2(y, x) * RadToDeg;

        //since there doesn't seem to be a Math.Clamp?
        public static double Clamp(double x, double min, double max) {
            if (x < min) return min;
            if (x > max) return max;
            return x;
        }

        //keeps angles in the range 0 to 360
        public static double ClampDegrees360(double angle) {
            angle = angle % 360.0;
            if (angle < 0) return angle + 360.0;
            return angle;
        }

        //keeps angles in the range -180 to 180
        public static double ClampDegrees180(double angle) {
            angle = ClampDegrees360(angle);
            if (angle > 180) angle -= 360;
            return angle;
        }

        public static double ClampRadians2Pi(double angle) {
            angle = angle % (2 * Math.PI);
            if (angle < 0) return angle + 2 * Math.PI;
            return angle;
        }

        public static double ClampRadiansPi(double angle) {
            angle = ClampRadians2Pi(angle);
            if (angle > Math.PI) angle -= 2 * Math.PI;
            return angle;
        }

        public static Random Random() => new Random();

        public static Random RandomFromSeed(long seed) => new Random((int)seed);

        public static IKontrolModule Module {
            get {
                BindingGenerator.RegisterTypeMapping(typeof(Random), RandomBinding.RandomType);

                List<CompiledKontrolFunction> functions = new List<CompiledKontrolFunction> {
                    Direct.BindFunction(typeof(Math), "Abs", "Returns the absolute value of a number.", typeof(double)),
                    Direct.BindFunction(typeof(Math), "Acos",
                        "Returns the angle in radian whose cosine is the specified number.", typeof(double)),
                    Direct.BindFunction(typeof(DirectBindingMath), "AcosDeg",
                        "Returns the angle in degree whose cosine is the specified number.", typeof(double)),
                    Direct.BindFunction(typeof(Math), "Asin",
                        "Returns the angle in radian whose sine is the specified number.", typeof(double)),
                    Direct.BindFunction(typeof(DirectBindingMath), "AsinDeg",
                        "Returns the angle in degree whose sine is the specified number.", typeof(double)),
                    Direct.BindFunction(typeof(Math), "Atan",
                        "Returns the angle in radian whose tanget is the specified number.", typeof(double)),
                    Direct.BindFunction(typeof(DirectBindingMath), "AtanDeg",
                        "Returns the angle in degree whose tangent is the specified number.", typeof(double)),
                    Direct.BindFunction(typeof(Math), "Atan2",
                        "Returns the angle in redian whose tangent is the quotient of two specified numbers.",
                        typeof(double), typeof(double)),
                    Direct.BindFunction(typeof(DirectBindingMath), "Atan2Deg",
                        "Returns the angle in degree whose tangent is the quotient of two specified numbers.",
                        typeof(double), typeof(double)),
                    Direct.BindFunction(typeof(Math), "Ceiling",
                        "Returns the smallest integral value that is greater than or equal to the specified number.",
                        typeof(double)),
                    Direct.BindFunction(typeof(Math), "Cos", "Returns the cosine of the specified angle in redian.",
                        typeof(double)),
                    Direct.BindFunction(typeof(DirectBindingMath), "CosDeg",
                        "Returns the cosine of the specified angle in degree.", typeof(double)),
                    Direct.BindFunction(typeof(Math), "Cosh", "Returns the hyperbolic cosine of the specified angle.",
                        typeof(double)),
                    Direct.BindFunction(typeof(Math), "Exp", "Returns e raised to the specified power.",
                        typeof(double)),
                    Direct.BindFunction(typeof(Math), "Floor",
                        "Returns the largest integral value less than or equal to the specified number.",
                        typeof(double)),
                    Direct.BindFunction(typeof(Math), "Log",
                        "Returns the natural (base e) logarithm of a specified number.", typeof(double)),
                    Direct.BindFunction(typeof(Math), "Log10", "Returns the base 10 logarithm of a specified number.",
                        typeof(double)),
                    Direct.BindFunction(typeof(Math), "Max", "Returns the larger of two decimal numbers.",
                        typeof(double), typeof(double)),
                    Direct.BindFunction(typeof(Math), "Min", "Returns the smaller of two decimal numbers.",
                        typeof(double), typeof(double)),
                    Direct.BindFunction(typeof(Math), "Pow",
                        "Returns a specified number raised to the specified power.", typeof(double), typeof(double)),
                    Direct.BindFunction(typeof(Math), "Round",
                        "Rounds a decimal value to the nearest integral value, and rounds midpoint values to the nearest even number.",
                        typeof(double)),
                    Direct.BindFunction(typeof(Math), "Sin", "Returns the sine of the specified angle in redian.",
                        typeof(double)),
                    Direct.BindFunction(typeof(DirectBindingMath), "SinDeg",
                        "Returns the sine of the specified angle in degree.", typeof(double)),
                    Direct.BindFunction(typeof(Math), "Sinh", "Returns the hyperbolic sine of the specified angle.",
                        typeof(double)),
                    Direct.BindFunction(typeof(Math), "Sqrt", "Returns the square root of a specified number.",
                        typeof(double)),
                    Direct.BindFunction(typeof(Math), "Tan", "Returns the sine of the specified angle in redian.",
                        typeof(double)),
                    Direct.BindFunction(typeof(DirectBindingMath), "TanDeg",
                        "Returns the sine of the specified angle in degree.", typeof(double)),
                    Direct.BindFunction(typeof(Math), "Tanh", "Returns the hyperbolic tangent of the specified angle.",
                        typeof(double)),
                    Direct.BindFunction(typeof(Math), "Truncate", "Calculates the integral part of a specified number.",
                        typeof(double)),

                    Direct.BindFunction(typeof(DirectBindingMath), "Acosh",
                        "Returns the angle whose hyperbolic cosine is the specified number.", typeof(double)),
                    Direct.BindFunction(typeof(DirectBindingMath), "Asinh",
                        "Returns the angle whose hyperbolic sine is the specified number.", typeof(double)),
                    Direct.BindFunction(typeof(DirectBindingMath), "Atanh",
                        "Returns the angle whose hyperbolic tanget is the specified number.", typeof(double)),
                    Direct.BindFunction(typeof(DirectBindingMath), "Clamp",
                        "Clamp a number between a given minimum and maximum", typeof(double), typeof(double),
                        typeof(double)),
                    Direct.BindFunction(typeof(DirectBindingMath), "ClampDegrees360",
                        "Clamp an angle between 0 and 360 degree", typeof(double)),
                    Direct.BindFunction(typeof(DirectBindingMath), "ClampDegrees180",
                        "Clamp an angle between -180 and 180 degree", typeof(double)),
                    Direct.BindFunction(typeof(DirectBindingMath), "ClampRadians2Pi",
                        "Clamp an angle between 0 and 2π", typeof(double)),
                    Direct.BindFunction(typeof(DirectBindingMath), "ClampRadiansPi",
                        "Clamp an angle between -π and π", typeof(double)),
                    Direct.BindFunction(typeof(DirectBindingMath), "Random", "New random number generator"),
                    Direct.BindFunction(typeof(DirectBindingMath), "RandomFromSeed",
                        "New random number generator from given seed", typeof(long))
                };

                List<CompiledKontrolConstant> constants = new List<CompiledKontrolConstant> {
                    Direct.BindConstant(typeof(DirectBindingMath), "Pi",
                        "Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π."),
                    Direct.BindConstant(typeof(DirectBindingMath), "E",
                        "Represents the natural logarithmic base, specified by the e constant,"),
                    Direct.BindConstant(typeof(DirectBindingMath), "DegToRad",
                        "Multiplicator to convert an angle of degree to radian."),
                    Direct.BindConstant(typeof(DirectBindingMath), "RadToDeg",
                        "Multiplicator to convert an angle of radian to degree."),
                    Direct.BindConstant(typeof(DirectBindingMath), "MinInt", "Minimum possible integer number."),
                    Direct.BindConstant(typeof(DirectBindingMath), "MaxInt", "Maximum possible integer number."),
                    Direct.BindConstant(typeof(DirectBindingMath), "MinFloat",
                        "Minimum possible floating point number."),
                    Direct.BindConstant(typeof(DirectBindingMath), "MaxFloat",
                        "Maximum possible floating point number."),
                    Direct.BindConstant(typeof(DirectBindingMath), "Epsilon",
                        "Machine epsilon, i.e lowest possible resolution of a floating point number."),
                };


                return Direct.BindModule(ModuleName, "Collection of basic mathematical functions.",
                    new List<RealizedType> {
                        RandomBinding.RandomType
                    },
                    constants,
                    functions);
            }
        }
    }
}
