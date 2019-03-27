namespace SimulationPhysic
{
    public class Electron : Body
    {
        public Electron(Vector3 pos) : base(Physic.Constant.electronMass, Physic.Constant.electronCharge, Physic.Constant.electronRadius, pos) { }
    }

    public class Proton : Body
    {
        public Proton(Vector3 pos) : base(Physic.Constant.protonMass, Physic.Constant.protonCharge, Physic.Constant.protonRadius, pos) { }
    }

    public class Positron : Body
    {
        public Positron(Vector3 pos) : base(Physic.Constant.electronMass, -Physic.Constant.electronCharge, Physic.Constant.electronRadius, pos) { }
    }


///OBJECT OBJECT
    public class Body
    {
        //umbennenene
        public Vector3 pos, vel, acc;
        public double mass, radius, charge;

        public Vector3 Momentum => mass * vel;

        public Body(Body body) : this(body.mass, body.charge, body.radius, body.pos, body.vel, body.acc) { }
        public Body(double mass, double charge, double radius, Vector3 pos) : this(mass, charge, radius, pos, new Vector3(), new Vector3()) { }
        public Body(double mass, double charge, double radius, Vector3 pos, Vector3 vel) : this(mass, charge, radius, pos, vel, new Vector3()) { }
        public Body(double mass, double charge, double radius, Vector3 pos, Vector3 vel, Vector3 acc)
        {
            this.mass = mass;
            this.charge = charge;
            this.pos = pos;
            this.vel = vel;
            this.acc = acc;
            this.radius = radius;
        }

        public void Move(double time, bool resetAcc)
        {
            pos += vel * time;
            vel += acc * time;
            if(resetAcc)
                acc = new Vector3();
        }

        public void Move(double time) => Move(time, true);

        public Vector3 GField(Vector3 pos) => Physic.Force.Gravitation(1, mass, Vector3.Sub(pos, this.pos));
        
        public Vector3 EField(Vector3 pos) => Physic.Force.Coulomb(1, charge, Vector3.Sub(pos, this.pos));

        public Body Clone() => new Body(mass, charge, radius, pos, vel, acc);
    }
}
