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
        public Photon(Vector3 x, Vector3 v, double frequency) : base(Physic.Constant.planckConstant / (Physic.Constant.ligthSpeed * Physic.Constant.ligthSpeed) * frequency, 0, 0, x, Vector3.Normalize(v) * Physic.Constant.ligthSpeed)
        {
            freezeA = true;
        }
    }

    public class Object
    {
        public bool freezeX, freezeV, freezeA;

        public Vector3 x, v, a, f;
        public double m, r, q;

        public Vector3 p => m * v;
        public double waveLenght => Physic.Constant.planckConstant / p.Length();

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
            f = new Vector3();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Move(double t)
        {
            //extrem unperformant
            //(!freezeA)
                a += f / m;
            //if(!freezeX)
                x += v * t;
            //if (!freezeV)
                v += a * t;
            f = new Vector3();
            a = new Vector3();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 GField(Vector3 x) => Physic.Force.Gravitation(1, m, x - this.x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 EField(Vector3 x) => Physic.Force.Coulomb(1, q, x - this.x);

        public Object Clone() => new Object(m, q, r, x, v, a);
    }
}
