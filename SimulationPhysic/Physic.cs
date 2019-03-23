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

            public static Vector3 Lorentz(double q, Vector3 vel, Vector3 B) => q * Vector3.CrossP(vel, B);
        }

        public static class Constant
        {
            public const double electronCharge = -0.0000000000000000001602;
            public const double protonCharge = 0.0000000000000000001602;
            public const double electronMass = 0.0000000000000000000000000000009101;
            public const double protonMass = 0.00000000000000000000000000167262158;
            public const double electricConstant = 0.000000000008854187817;
            public const double electronRadius = 0.02817940322;/*00000000000000*/
            public const double protonRadius = 0.00875161;
        }
    }
}
