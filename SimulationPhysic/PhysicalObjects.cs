namespace SimulationPhysic
{
    public class Electron : Charge
    {
        public Electron(Vector3 pos) : base(pos, Physic.Constant.electronCharge, Physic.Constant.electronMass, Physic.Constant.electronRadius)
        {
        }
    }

    public class Proton : Charge
    {
        public Proton(Vector3 pos) : base(pos, Physic.Constant.protonCharge, Physic.Constant.protonMass, Physic.Constant.protonRadius)
        {
        }
    }

    public class Positron : Charge
    {
        public Positron(Vector3 pos) : base(pos, -Physic.Constant.electronCharge, Physic.Constant.electronMass, Physic.Constant.electronRadius)
        {
        }
    }

    public abstract class Body
    {
        public Vector3 pos, vel, acc;
        public double mass, radius;

        public Body(double mass, double radius, Vector3 pos) : this(mass, radius, pos, new Vector3(), new Vector3()) { }
        public Body(double mass, double radius, Vector3 pos, Vector3 vel) : this(mass, radius, pos, vel, new Vector3()) { }
        public Body(double mass, double radius, Vector3 pos, Vector3 vel, Vector3 acc)
        {
            this.mass = mass;
            this.pos = pos;
            this.vel = vel;
            this.acc = acc;
            this.radius = radius;
        }

        public void Move(double time)
        {
            pos += vel * time;
            vel += acc * time;
            acc = new Vector3();
        }

        public Vector3 GField(Vector3 pos)
        {

        }
    }

    public abstract class Charge : Body
    {
        public double charge;

        public Charge(Vector3 pos, double charge, double mass, double radius) : base(mass, radius, pos)
        {
            this.charge = charge;
        }

        public Vector3 ElectricalField(Vector3 pos)
        {
            return Physic.Force.Coulomb(1, charge, Vector3.Sub(pos, this.pos));
        }

        public override Charge Clone() => new Charge(pos.Clone(), charge, mass, radius);
    }
}
