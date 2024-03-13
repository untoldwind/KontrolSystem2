using System;

namespace KontrolSystem.Benchmarks;

public static class LambertIzzoSolver {
    /// <summary>
    /// Solve the Lambert problem using Izzo's method.
    /// </summary>
    /// <param name="r1">first cartesian position.</param>
    /// <param name="r2">Sirst cartesian position.</param>
    /// <param name="tof">time of flight.</param>
    /// <param name="mu">gravity parameter.</param>
    /// <param name="clockwise">Clockwise / Counter-Clockwise solution (prograde/retrograde)</param>
    /// <param name="v1">Velocity at r1.</param>
    /// <param name="v2">Velocity at r2.</param>
    public static Vector3d Solve(Vector3d r1, Vector3d r2, double tof, double mu, bool clockwise) {
        if (tof <= 0) throw new Exception("Time of flight is negative");
        if (mu <= 0) throw new Exception("Gravity parameter is negative");

        double c = (r2 - r1).magnitude;
        double R1 = r1.magnitude;
        double R2 = r2.magnitude;
        double s = (c + R1 + R2) / 2.0;
        Vector3d ir1 = r1.normalized;
        Vector3d ir2 = r2.normalized;
        Vector3d ih = Vector3d.Cross(ir1, ir2).normalized;
        double lambda2 = 1 - c / s;
        double lambda = Math.Sqrt(lambda2);
        Vector3d it1, it2;

        if (ih.z < 0.0) {
            lambda = -lambda;
            it1 = Vector3d.Cross(ir1, ih).normalized;
            it2 = Vector3d.Cross(ir2, ih).normalized;
        } else {
            it1 = Vector3d.Cross(ih, ir1).normalized;
            it2 = Vector3d.Cross(ih, ir2).normalized;
        }

        if (!clockwise) {
            lambda = -lambda;
            it1 = -it1;
            it2 = -it2;
        }

        double lambda3 = lambda * lambda2;
        double T = Math.Sqrt(2.0 * mu / s / s / s) * tof;

        // 2 - We now have lambda, T and we will find all x
        // 2.1 - Let us first detect the maximum number of revolutions for which there exists a solution
        double T0 = Math.Acos(lambda) + lambda * Math.Sqrt(1.0 - lambda2);
        double T1 = 2.0 / 3.0 * (1.0 - lambda3);

        // 3 - We may now find all solutions in x,y
        // 3.1 0 rev solution
        // 3.1.1 initial guess
        double tmp;
        if (T >= T0) {
            tmp = -(T - T0) / (T - T0 + 4);
        } else if (T <= T1) {
            tmp = T1 * (T1 - T) / (2.0 / 5.0 * (1 - lambda2 * lambda3) * T) + 1;
        } else {
            tmp = Math.Pow((T / T0), 0.69314718055994529 / Math.Log(T1 / T0)) - 1.0;
        }

        // 3.1.2 Householder iterations
        (double x, int iters) = householder(lambda, T, tmp, 0, 1e-5, 15);

        // 4 - For each found x value we reconstruct the terminal velocities
        double gamma = Math.Sqrt(mu * s / 2.0);
        double rho = (R1 - R2) / c;
        double sigma = Math.Sqrt(1 - rho * rho);
        double y = Math.Sqrt(1.0 - lambda2 + lambda2 * x * x);
        double vr1 = gamma * ((lambda * y - x) - rho * (lambda * y + x)) / R1;
        double vr2 = -gamma * ((lambda * y - x) + rho * (lambda * y + x)) / R2;
        double vt = gamma * sigma * (y + lambda * x);
        double vt1 = vt / R1;
        double vt2 = vt / R2;

        return
            vr1 * ir1 + vt1 * it1 + vr2 * ir2 +
            vt2 * it2; // Actually a tuple but converting this to a delegate is a mess
    }

    static (double, int) householder(double lambda, double T, double x0, int N, double eps, int iter_max) {
        int it = 0;
        double err = 1.0;
        double xnew = 0.0;

        double x = x0;
        while ((err > eps) && (it < iter_max)) {
            double tof = x2tof(lambda, x, N);
            (double DT, double DDT, double DDDT) = dTdx(lambda, x, tof);
            double delta = tof - T;
            double DT2 = DT * DT;
            xnew = x - delta * (DT2 - delta * DDT / 2.0) / (DT * (DT2 - delta * DDT) + DDDT * delta * delta / 6.0);
            err = Math.Abs(x - xnew);
            x = xnew;
            it++;
        }

        return (x, it);
    }

    static (double DT, double DDT, double DDDT) dTdx(double lambda, double x, double T) {
        double l2 = lambda * lambda;
        double l3 = l2 * lambda;
        double umx2 = 1.0 - x * x;
        double y = Math.Sqrt(1.0 - l2 * umx2);
        double y2 = y * y;
        double y3 = y2 * y;
        double DT = 1.0 / umx2 * (3.0 * T * x - 2.0 + 2.0 * l3 * x / y);
        double DDT = 1.0 / umx2 * (3.0 * T + 5.0 * x * DT + 2.0 * (1.0 - l2) * l3 / y3);
        double DDDT = 1.0 / umx2 * (7.0 * x * DDT + 8.0 * DT - 6.0 * (1.0 - l2) * l2 * l3 * x / y3 / y2);

        return (DT, DDT, DDDT);
    }

    static double x2tof(double lambda, double x, int N) {
        double battin = 0.01;
        double lagrange = 0.2;
        double dist = Math.Abs(x - 1);
        if (dist < lagrange && dist > battin) {
            // We use Lagrange tof expression
            return x2tof2(lambda, x, N);
        }

        double K = lambda * lambda;
        double E = x * x - 1.0;
        double rho = Math.Abs(E);
        double z = Math.Sqrt(1 + K * E);
        if (dist < battin) {
            // We use Battin series tof expression
            double eta = z - lambda * x;
            double S1 = 0.5 * (1.0 - lambda - x * eta);
            double Q = hypergeometricF(S1, 1e-11);
            Q = 4.0 / 3.0 * Q;
            return (eta * eta * eta * Q + 4.0 * lambda * eta) / 2.0 + N * Math.PI / Math.Pow(rho, 1.5);
        } else {
            // We use Lancaster tof expresion
            double y = Math.Sqrt(rho);
            double g = x * z - lambda * E;
            double d = 0.0;
            if (E < 0) {
                double l = Math.Acos(g);
                d = N * Math.PI + l;
            } else {
                double f = y * (z - lambda * x);
                d = Math.Log(f + g);
            }

            return (x - lambda * z - d / y) / E;
        }
    }

    static double x2tof2(double lambda, double x, int N) {
        double a = 1.0 / (1.0 - x * x);
        if (a > 0) {
            // ellipse
            double alfa = 2.0 * Math.Acos(x);
            double beta = 2.0 * Math.Asin(Math.Sqrt(lambda * lambda / a));
            if (lambda < 0.0) beta = -beta;
            return ((a * Math.Sqrt(a) * ((alfa - Math.Sin(alfa)) - (beta - Math.Sin(beta)) + 2.0 * Math.PI * N)) /
                    2.0);
        } else {
            double alfa = 2.0 * ExtraMath.Acosh(x);
            double beta = 2.0 * ExtraMath.Asinh(Math.Sqrt(-lambda * lambda / a));
            if (lambda < 0.0) beta = -beta;
            return (-a * Math.Sqrt(-a) * ((beta - Math.Sinh(beta)) - (alfa - Math.Sinh(alfa))) / 2.0);
        }
    }

    static double hypergeometricF(double z, double tol) {
        double Sj = 1.0;
        double Cj = 1.0;
        double err = 1.0;
        double Cj1 = 0.0;
        double Sj1 = 0.0;
        int j = 0;
        while (err > tol) {
            Cj1 = Cj * (3.0 + j) * (1.0 + j) / (2.5 + j) * z / (j + 1);
            Sj1 = Sj + Cj1;
            err = Math.Abs(Cj1);
            Sj = Sj1;
            Cj = Cj1;
            j = j + 1;
        }

        return Sj;
    }
}


