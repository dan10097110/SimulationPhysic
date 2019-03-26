﻿namespace SimulationPhysic
{
    public class Electron : Charge
    {
        public Electron(Vector3 pos) : base(Physic.Constant.electronCharge, new Body(Physic.Constant.electronMass, Physic.Constant.electronRadius, pos))
        {
        }
    }

    public class Proton : Charge
    {
        public Proton(Vector3 pos) : base(Physic.Constant.protonCharge, new Body(Physic.Constant.protonMass, Physic.Constant.protonRadius, pos))
        {
        }
    }

    public class Positron : Charge
    {
        public Positron(Vector3 pos) : base(-Physic.Constant.electronCharge, new Body(Physic.Constant.electronMass, Physic.Constant.electronRadius, pos))
        {
        }
    }

    public class Body
    {
        public Vector3 pos, vel, acc;
        public double mass, radius;

        public Vector3 Momentum => mass * vel;

        public Body(Body body) : this(body.mass, body.radius, body.pos, body.vel, body.acc) { }
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
            return Physic.Force.Gravitation(1, mass, Vector3.Sub(pos, this.pos));
        }
    }

    public class Charge : Body
    {
        public double charge;

        public Charge(double charge, Body body) : base(body)
        {
            this.charge = charge;
        }

        public Vector3 ElectricalField(Vector3 pos)
        {
            return Physic.Force.Coulomb(1, charge, Vector3.Sub(pos, this.pos));
        }

        public Charge Clone() => new Charge(charge, new Body(mass, radius, pos.Clone()));
    }
}
