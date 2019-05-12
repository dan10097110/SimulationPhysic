using System;
using System.Runtime.CompilerServices;

namespace SimulationPhysic
{
    public class Electron : Object
    {
        public Electron(Vector3 x) : base(Physic.Constant.electronMass, Physic.Constant.electronCharge, Physic.Constant.electronRadius, x) { }
        public Electron(Vector3 x, Vector3 v) : base(Physic.Constant.electronMass, Physic.Constant.electronCharge, Physic.Constant.electronRadius, x, v) { }
    }

    public class Proton : Object
    {
        public Proton(Vector3 x) : base(Physic.Constant.protonMass, Physic.Constant.protonCharge, Physic.Constant.protonRadius, x) { }
        public Proton(Vector3 x, Vector3 v) : base(Physic.Constant.protonMass, Physic.Constant.protonCharge, Physic.Constant.protonRadius, x, v) { }
    }

    public class Positron : Object
    {
        public Positron(Vector3 x) : base(Physic.Constant.electronMass, -Physic.Constant.electronCharge, Physic.Constant.electronRadius, x) { }
        public Positron(Vector3 x, Vector3 v) : base(Physic.Constant.electronMass, -Physic.Constant.electronCharge, Physic.Constant.electronRadius, x, v) { }
    }

    public class Photon : Object
    {
        public Photon(Vector3 x, Vector3 v, double E) : base(Physic.Constant.planckConstant / (Physic.Constant.ligthSpeed * Physic.Constant.ligthSpeed) * E / Physic.Constant.planckConstant, 0, 0, x, Vector3.Normalize(v) * Physic.Constant.ligthSpeed)
        {
            freezeA = true;
        }
    }


    //properties auf vars direkt zurückführen
    //inlining für properties
    //relativistisch korrekt machen, wahrscheinlich über impuls umgesetztz
    public class Object
    {
        public bool freezeX, freezeA;

        public Vector3 x, v, force;
        public readonly double m_0, r, q;
        public double m => m_0 / Math.Sqrt(1 - v.LengthSquared() / (Physic.Constant.ligthSpeed * Physic.Constant.ligthSpeed));

        public Vector3 p_0 => m_0 * v;
        public Vector3 p
        {
            get => m * v;
            set
            {
                v = value / Math.Sqrt(m_0 * m_0 + Vector3.Dot(value, value) / (Physic.Constant.ligthSpeed * Physic.Constant.ligthSpeed));
            }
        }
        public double E => m * Physic.Constant.ligthSpeed * Physic.Constant.ligthSpeed;
        public double E_0 => m_0 * Physic.Constant.ligthSpeed * Physic.Constant.ligthSpeed;
        public double E_kin => (m - m_0) * Physic.Constant.ligthSpeed * Physic.Constant.ligthSpeed;
        public double waveLength
        {
            get => Physic.Constant.planckConstant / p.Length();
            set { p = Vector3.Normalize(v) * (Physic.Constant.planckConstant / value); }
        }

        public Object(Object o) : this(o.m_0, o.q, o.r, o.x, o.v) { }
        public Object(double m_0, double q, double r, Vector3 x) : this(m_0, q, r, x, new Vector3()) { }
        public Object(double m_0, double q, double r, Vector3 x, Vector3 v)
        {
            this.m_0 = m_0;
            this.q = q;
            this.x = x;
            this.v = v;
            this.r = r;
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

        public Object Clone() => new Object(m_0, q, r, x, v);
    }
}