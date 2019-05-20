using System;
using static SimulationPhysic.Physic.Constant;
using System.Runtime.CompilerServices;

namespace SimulationPhysic
{
    public class Electron : Object
    {
        public Electron(Vector3 x) : this(x, new Vector3(), 1) { }
        public Electron(Vector3 x, Vector3 v) : this(x, v, 1) { }
        public Electron(Vector3 x, int matter) : this(x, new Vector3(), matter) { }
        public Electron(Vector3 x, Vector3 v, int matter) : base(electronMass, matter * electronCharge, electronRadius, x, v, matter) { }
    }

    public class Proton : Object
    {
        public Proton(Vector3 x) : this(x, new Vector3()) { }
        public Proton(Vector3 x, Vector3 v) : base(protonMass, protonCharge, protonRadius, x, v) { }
    }

    public class Photon : Object
    {
        public override Vector3 f { get; set; }
        public override double m => h * f.Length() / cc;
        public override double E => h * f.Length();

        public Photon(Vector3 x, Vector3 direction, double E) : base(0, 0, 0, x, direction.Normalize() * c)
        {
            f = direction.Normalize() * E / h;
            if (direction.IsZero())
                this.v = Vector3.Random().Normalize() * c;
            freezeA = true;
        }
    }


    //properties auf vars direkt zurückführen
    //inlining für properties
    //relativistisch korrekt machen, wahrscheinlich über impuls umgesetztz
    public class Object
    {
        public int matter;//1: matter, 0: none, -1: antimatter
        public bool freezeX, freezeA, stable = true;

        public Vector3 x, v, force;
        public double E_Extra, t_half;
        public readonly double m_0, r, q;
        public virtual double m => m_0 / Math.Sqrt(1 - v.LengthSquared() / cc);

        public Vector3 p_0 => m_0 * v;
        public Vector3 p
        {
            get => m * v;
            set => v = value / Math.Sqrt(m_0 * m_0 + Vector3.Dot(value, value) / cc);
        }
        public virtual double E => m * cc + E_Extra;
        public double E_0 => m_0 * cc;
        public double E_kin => (m - m_0) * cc;
        public virtual Vector3 f
        {
            get => c * p / h;
            set => p = h * value / c;
        }

        public Object(Object o) : this(o.m_0, o.q, o.r, o.x, o.v, o.matter) { }
        public Object(double m_0, double q, double r, Vector3 x) : this(m_0, q, r, x, new Vector3(), 1) { }
        public Object(double m_0, double q, double r, Vector3 x, Vector3 v) : this(m_0, q, r, x, v, 1) { }
        public Object(double m_0, double q, double r, Vector3 x, Vector3 v, int matter)
        {
            this.m_0 = m_0;
            this.q = q;
            this.x = x;
            this.v = v;
            this.r = r;
            this.matter = matter;
            force = new Vector3();
        }

        //bei photon move überschreibnen da dort aufjedenfall die geschwindigkeit nicht verändert werden kann

        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Move(double t)
        {
            //extrem unperformant if abfrage
            //if(!freezeX)
            x += v * t;
            //if (!freezeV)
            p += dp;
        }*/

        public Object Clone() => new Object(m_0, q, r, x, v, matter);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool MatterAntiMatter(Object o1, Object o2) => o1.m_0 == o2.m_0 && o1.q == -o2.q && o1.matter == -o2.matter && o1.matter != 0;
    }
}