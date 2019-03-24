using System.Collections.Generic;
using System;

namespace SimulationPhysic
{
    public class PhysicalSystem
    {
        public List<Body> bodies = new List<Body>();
        public EField eField = new EField();
        public readonly double minTimeStep;
        public double time = 0;

        public PhysicalSystem(double minTimeStep)
        {
            this.minTimeStep = minTimeStep;
        }

        public void AddCharge(params Charge[] charges)
        {
            eField.Add(charges);
            bodies.AddRange(charges);
        }

        public void Proceed()
        {
            time += minTimeStep;
            eField.ApplyForce();
            bodies.ForEach(b => b.Move(minTimeStep));
            //elastische Stöße
            for(int i = 1; i < bodies.Count; i++)
            {
                for(int j = 0; j < i; j++)
                {
                    double d = bodies[i].radius + bodies[j].radius;
                    var v = bodies[i].vel - bodies[j].vel;
                    var s = bodies[i].pos - bodies[j].pos;
                    var vs = v * s;
                    var vv = v * v;
                    double w = (vs * vs) - (s.Square() * vv) + (d * d * vv);
                    if (w > 0)
                    {
                        w = Math.Sqrt(w);
                        double t1 = (-vs - w) / vv, t2 = (-vs + w) / vv;
                        bool b1 = t1 <= 0 && -t1 <= minTimeStep, b2 = t2 <= 0 && -t2 <= minTimeStep;
                        if (b1 || b2)
                        {
                            double t = b1 && b2 ? (t1 < t2 ? t1 : t2) : (b1 ? t1 : t2);
                            bodies[i].pos += bodies[i].vel * t;
                            bodies[j].pos += bodies[j].vel * t;
                            var x = bodies[i].vel.Clone();
                            var y = bodies[j].vel.Clone();
                            bodies[i].vel = y * (bodies[j].mass / bodies[i].mass);
                            bodies[j].vel = x * (bodies[i].mass / bodies[j].mass);
                            bodies[i].pos += bodies[i].vel * -t;
                            bodies[j].pos += bodies[j].vel * -t;
                        }
                    }
                }
            }
        }
        public void Proceed1()
        {
            time += minTimeStep;
            eField.ApplyForce();
            bodies.ForEach(b => b.Move(minTimeStep));
            foreach(Body b in bodies)
            {
                foreach(Body c in bodies)
                {
                    if(b != c)
                        if ((b.pos - c.pos).Sum() <= b.radius + c.radius)
                        {
                            var v = b.vel.Clone();
                            var w = c.vel.Clone();
                            b.vel = w * (c.mass / b.mass);
                            c.vel = v * (b.mass / c.mass);
                            for(; (b.pos - c.pos).Sum() <= b.radius + c.radius;)
                            {
                                b.Move(minTimeStep);
                                c.Move(minTimeStep);
                            }
                        }
                }
            }
        }
    }
}