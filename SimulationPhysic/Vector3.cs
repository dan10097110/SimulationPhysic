using System;

namespace SimulationPhysic
{
    public class Vector3
    {
        public double x, y, z;

        public Vector3() : this(0, 0, 0) { }

        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 operator +(Vector3 a, Vector3 b) => Add(a, b);
        public static bool operator ==(Vector3 a, Vector3 b) => a.x == b.x && a.y == b.y && a.z == b.z;
        public static bool operator !=(Vector3 a, Vector3 b) => a.x != b.x || a.y != b.y || a.z != b.z;
        public static Vector3 operator -(Vector3 a, Vector3 b) => Sub(a, b);
        public static double operator *(Vector3 a, Vector3 b) => Mul(a, b);
        public static Vector3 operator *(Vector3 a, double d) => Mul(a, d);
        public static Vector3 operator *(double d, Vector3 a) => Mul(a, d);
        public static Vector3 operator /(Vector3 a, double d) => Div(a, d);

        public double Sum() => Math.Sqrt(x * x + y * y + z * z);

        public static Vector3 Add(Vector3 v1, Vector3 v2) => new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);

        public static Vector3 Sub(Vector3 v1, Vector3 v2) => new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);

        public static double Mul(Vector3 v1, Vector3 v2) => v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;

        public static Vector3 Mul(Vector3 v, double d) => new Vector3(v.x * d, v.y * d, v.z * d);

        public static Vector3 Div(Vector3 v, double d) => new Vector3(v.x / d, v.y / d, v.z / d);

        public static Vector3 CrossP(Vector3 a, Vector3 b) => new Vector3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);

        public Vector3 Round() => new Vector3(Math.Round(x), Math.Round(y), Math.Round(z));

        public Vector3 Inverse() => new Vector3(-x, -y, -z);

        public double Square() => this * this;

        public override string ToString() => "(" + x + ";" + y + ";" + z + ")";

        public Vector3 Clone() => new Vector3(x, y, z);
    }
}
