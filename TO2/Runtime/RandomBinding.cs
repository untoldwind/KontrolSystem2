using System;
using System.Collections.Generic;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.TO2.Runtime {
    public static class RandomBinding {
        public static readonly BoundType RandomType = Direct.BindType(DirectBindingMath.ModuleName, "Random",
            "Random number generator", typeof(Random),
            BuiltinType.NoOperators,
            BuiltinType.NoOperators,
            new Dictionary<string, IMethodInvokeFactory> {
                {
                    "next_int",
                    new BoundMethodInvokeFactory("Get next random number between `min` and `max`", true,
                        () => BuiltinType.Int,
                        () => new List<RealizedParameter> {
                            new RealizedParameter("min", BuiltinType.Int), new RealizedParameter("max", BuiltinType.Int)
                        }, false, typeof(RandomBinding), typeof(RandomBinding).GetMethod("NextInt"))
                }, {
                    "next_float",
                    new BoundMethodInvokeFactory("Get next random number between 0.0 and 1.0", true,
                        () => BuiltinType.Float,
                        () => new List<RealizedParameter>(), false, typeof(Random),
                        typeof(Random).GetMethod("NextDouble"))
                }, {
                    "next_gaussian",
                    new BoundMethodInvokeFactory("Get next gaussian distributed random number", true,
                        () => BuiltinType.Float,
                        () => new List<RealizedParameter> {
                            new RealizedParameter("mu", BuiltinType.Float, new FloatDefaultValue(0.0)),
                            new RealizedParameter("sigma", BuiltinType.Float, new FloatDefaultValue(1.0))
                        }, false, typeof(RandomBinding), typeof(RandomBinding).GetMethod("NextGaussian"))
                },
            },
            BuiltinType.NoFields);

        public static long NextInt(Random random, long min, long max) {
            long rand = random.Next();
            rand = (rand << 32) + random.Next();

            return rand % (max - min) + min;
        }

        public static double NextGaussian(Random random, double mu, double sigma) {
            double u1 = random.NextDouble();
            double u2 = random.NextDouble();

            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double randNormal = mu + sigma * randStdNormal;

            return randNormal;
        }
    }
}
