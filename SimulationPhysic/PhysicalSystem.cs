using System.Collections.Generic;

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
            foreach(Body b in bodies)
            {
                foreach(Body c in bodies)
                {
                    if(b != c)
                    {
                        double d = b.radius + c.radius;
                        var v = b.vel - c.vel;
                        var s = b.pos - c.pos;
                        var vs = v * s;
                        var vv = v * v;
                        double w = vs*vs-s.Square()*vv-d*d*vv;
                        if(iw >= 0)
                        {
                            w = Math.Sqrt(w);
                            double t, t1 = (-vs - w) / vv, t2 = (-vs + w) / vv;
                            bool b1 = t1 <= 0 && -t1 <= minTimeStep, b2 = t2 <= 0 && -t2 <= minTimeStep;
                            double t = b1 && b2 ? t1 < t2 ? t1 : t2 : b1 ? t1 : t2;
                            b.pos += b.vel * t;
                            c.pos += c.vel * t;    
                            var x = b.vel.Clone();
                            var y = c.vel.Clone();
                            b.vel = y * (c.mass / b.mass);
                            c.vel = x * (b.mass / c.mass);
                            b.pos += b.vel * -t;
                            c.pos += c.vel * -t;
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