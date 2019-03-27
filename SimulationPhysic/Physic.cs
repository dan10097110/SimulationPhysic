using System;

namespace SimulationPhysic
{
    public static class Physic
    {
        public static class Force
        {
            public static Vector3 Coulomb(double q1, double q2, Vector3 r)
            {
                double sumR = r.Sum();
                return Vector3.Mul(r, (q1 * q2) / (4 * Math.PI * Constant.electricConstant * sumR * sumR * sumR));
            }

            public static Vector3 Gravitation(double m1, double m2, Vector3 r)
            {
                double sumR = r.Sum();
                return Vector3.Mul(r, (Constant.gravitationConstant * m1 * m2) / (sumR * sumR * sumR));
            }

            public static Vector3 Coulomb(Object o1, Object o2)
            {
                var r = o1.x - o2.x;
                double sumR = r.Sum();
                return Vector3.Mul(r, (o1.q * o2.q) / (4 * Math.PI * Constant.electricConstant * sumR * sumR * sumR));
            }

            public static Vector3 Gravitation(Object o1, Object o2)
            {
                var r = o1.x - o2.x;
                double sumR = r.Sum();
                return Vector3.Mul(r, (Constant.gravitationConstant * o1.m * o2.m) / (sumR * sumR * sumR));
            }

            public static Vector3 Lorentz(double q, Vector3 v, Vector3 B) => q * Vector3.CrossP(v, B);
        }

        public static class Constant
        {
            public const double electronCharge = -protonCharge;
            public const double protonCharge = 1.6021766208E-19;
            public const double electronMass = 9.10938356E-31;
            public const double protonMass = 1.672621898E-27;
            public const double electricConstant = 8.854187817E-12;
            public const double electronRadius = 1E-4;//1E-18
            public const double protonRadius = 8.8751E-2;//8.8751E-16
            public const double gravitationConstant = 6.67408E-11;
        }
    }
}
