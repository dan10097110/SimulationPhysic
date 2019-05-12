using System;
using System.Runtime.CompilerServices;

namespace SimulationPhysic
{
    public static class Physic
    {
        public static class Force
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector3 Coulomb(double q1, double q2, Vector3 r)
            {
                double sumR = r.Length();
                return r * ((q1 * q2) / (4 * Math.PI * Constant.electricConstant * sumR * sumR * sumR));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector3 Gravitation(double m1, double m2, Vector3 r)
            {
                double sumR = r.Length();
                return r * ((Constant.gravitationConstant * m1 * m2) / (sumR * sumR * sumR));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector3 Coulomb(Object o1, Object o2)
            {
                var r = o1.x - o2.x;
                double sumR = r.Length();
                return r * ((o1.q * o2.q) / (4 * Math.PI * Constant.electricConstant * sumR * sumR * sumR));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector3 Gravitation(Object o1, Object o2)
            {
                var r = o1.x - o2.x;
                double sumR = r.Length();
                return r * ((Constant.gravitationConstant * o1.m * o2.m) / (sumR * sumR * sumR));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector3 CoulombGravitation(Object o1, Object o2)
            {
                var r = o1.x - o2.x;
                double sumR = r.Length();
                return r * ((Constant.gravitationConstant * o1.m * o2.m + o1.q * o2.q / (4 * Math.PI * Constant.electricConstant)) / (sumR * sumR * sumR));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Vector3 Lorentz(double q, Vector3 v, Vector3 B) => q * Vector3.Cross(v, B);
        }

        public static class Constant
        {
            public const double electronCharge = -protonCharge;
            public const double protonCharge = 1.6021766208E-19;
            public const double electronMass = 9.10938356E-31;
            public const double protonMass = 1.672621898E-27;
            public const double electricConstant = 8.854187817E-12;
            public const double electronRadius = 1E-2;//1E-18
            public const double protonRadius = 8.8751E-1;//8.8751E-16
            public const double gravitationConstant = 6.67408E-11;
            public const double planckConstant = 6.62607015E-34;
            public const double ligthSpeed = 299792458;
        }
    }
}
