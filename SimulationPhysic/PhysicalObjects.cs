﻿namespace SimulationPhysic
{
    public class Electron : Object
    {
        public Electron(Vector3 x) : base(Physic.Constant.electronMass, Physic.Constant.electronCharge, Physic.Constant.electronRadius, x) { }
    }

    public class Proton : Object
    {
        public Proton(Vector3 x) : base(Physic.Constant.protonMass, Physic.Constant.protonCharge, Physic.Constant.protonRadius, x) { }
    }

    public class Positron : Object
    {
        public Positron(Vector3 x) : base(Physic.Constant.electronMass, -Physic.Constant.electronCharge, Physic.Constant.electronRadius, x) { }
    }

    public class Object
    {
        public Vector3 x, v, a;
        public double m, r, q;

        public Vector3 p => m * v;

        public Object(Object o) : this(o.m, o.q, o.r, o.x, o.v, o.a) { }
        public Object(double m, double q, double r, Vector3 x) : this(m, q, r, x, new Vector3(), new Vector3()) { }
        public Object(double m, double q, double r, Vector3 x, Vector3 v) : this(m, q, r, x, v, new Vector3()) { }
        public Object(double m, double q, double r, Vector3 x, Vector3 v, Vector3 a)
        {
            this.m = m;
            this.q = q;
            this.x = x;
            this.v = v;
            this.a = a;
            this.r = r;
        }

        public void Move(double t, bool resetAcc)
        {
            x += v * t;
            v += a * t;
            if(resetAcc)
                a = new Vector3();
        }

        public void Move(double t) => Move(t, true);

        public Vector3 GField(Vector3 x) => Physic.Force.Gravitation(1, m, Vector3.Sub(x, this.x));
        
        public Vector3 EField(Vector3 x) => Physic.Force.Coulomb(1, q, Vector3.Sub(x, this.x));

        public Object Clone() => new Object(m, q, r, x, v, a);
    }
}
