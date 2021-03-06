﻿using System;
using System.Runtime.CompilerServices;
using static SimulationPhysic.Physic.Constant;

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
        Vector3 force;

        public override Vector3 f { get; set; }
        public override double m => h * f.Length() / cc;
        public override double E => h * f.Length();
        public override Vector3 p
        {
            get => h * f / c;
            set => f = p * c / h;
        }
        public override Vector3 Force
        {
            get => force;
            set => force = new Vector3();
        }

        public Photon(Vector3 x, Vector3 direction, double E) : base(0, 0, 1E-40, x, direction.Normalize() * c)
        {
            f = direction.Normalize() * E / h;
            if (direction.IsZero())
                this.v = Vector3.Random().Normalize() * c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Move(double t) => x += v.Normalize() * (c * t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Accelerate(double t) { }
    }

    public class Object
    {
        public int matter;//1: matter, 0: none, -1: antimatter
        public bool stable = true;
        public Vector3 x;
        public Vector3 v;
        public readonly double m_0, r, q, E_Extra, t_half;

        public virtual Vector3 Force { get; set; }
        public virtual double m => m_0 / Math.Sqrt(1 - v.LengthSquared() / cc);
        public Vector3 p_0 => m_0 * v;
        public virtual Vector3 p
        {
            get => m * v;
            set => v = value / Math.Sqrt(m_0 * m_0 + value.LengthSquared() / cc);
        }
        public virtual double E => m * cc + E_Extra;
        public double E_0 => m_0 * cc;
        public double E_kin => (1 / Math.Sqrt(1 - v.LengthSquared() / cc) - 1) * m_0 * cc;
        public virtual Vector3 f
        {
            get => c * p / h;
            set => p = h * value / c;
        }

        public Object(Object o) : this(o.m_0, o.q, o.r, o.x, o.v, o.matter, o.E_Extra, o.stable, o.t_half, o.Force) { }
        public Object(double m_0, double q, double r, Vector3 x) : this(m_0, q, r, x, new Vector3(), 1) { }
        public Object(double m_0, double q, double r, Vector3 x, Vector3 v) : this(m_0, q, r, x, v, 1) { }
        public Object(double m_0, double q, double r, Vector3 x, Vector3 v, int matter) : this(m_0, q, r, x, v, matter, 0, true, 0, new Vector3()) { }
        public Object(double m_0, double q, double r, Vector3 x, Vector3 v, int matter, double E_Extra, bool stable, double t_half, Vector3 force)
        {
            this.m_0 = m_0;
            this.q = q;
            this.x = x;
            this.v = v;
            this.r = r;
            this.matter = matter;
            this.E_Extra = E_Extra;
            this.stable = stable;
            this.t_half = t_half;
            this.Force = force;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Move(double t) => x += v * t;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Accelerate(double t)
        {
            p += Force * t;
            Force = new Vector3();
        }

        public Object Clone() => new Object(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Annihilating(Object o1, Object o2) => o1.m_0 == o2.m_0 && o1.q == -o2.q && o1.r == o2.r && o1.matter == -o2.matter && o1.matter != 0;

        public Vector3 FieldStrength(Vector3 x)
        {
            var r = this.x - x;
            double sumR = r.Length();
            return r * ((gravitationConstant * m + q / (4 * Math.PI * electricConstant)) / (sumR * sumR * sumR));
        }
    }
}