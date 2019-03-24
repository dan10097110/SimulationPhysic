using System.Collections.Generic;

namespace SimulationPhysic
{
    public class PhysicalSystem
    {
        public List<Body> bodies = new List<Body>();
        public ElectricalField eField = new ElectricalField();
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
            eField.Calculate();
            bodies.ForEach(b => b.Move(minTimeStep));
            foreach(Body b in bodies)
            {
                foreach(Body c in bodies)
                {
                    if(b != c)
                    {
                        double d = b.radius + c.radius;
                        var v = b.vel - c.vel;
                        var s = b.pos - c.pos;
                        double t1 = (-(v*s) - d*v.Sum()) / v.Square(), t2 = (-(v*s) + d*v.Sum()) / v.Square();
                    }
                }
            }
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
        public void Proceed1()
        {
            time += minTimeStep;
            eField.Calculate();
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