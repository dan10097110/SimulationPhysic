using System;
using System.Collections.Generic;

namespace SimulationPhysic
{
    public class PhysicalSystem
    {
        public List<Object> objects = new List<Object>();
        public readonly double minTimeStep;
        public double time = 0;
        readonly bool gravitation = false, electricalF = true;

        public PhysicalSystem(double minTimeStep, params Object[] objects)
        {
            this.minTimeStep = minTimeStep;
            this.objects.AddRange(objects);
        }

        public void Add(params Object[] objects) => this.objects.AddRange(objects);

        public void Proceed()
        {
            time += minTimeStep;
            ApplyForce();
            objects.ForEach(b => b.Move(minTimeStep));
            Collision();
        }

        void ApplyForce()
        {
            for (int i = 1; i < objects.Count; i++)
                for (int j = 0; j < i; j++)
                {
                    var f = new Vector3();
                    if (gravitation)
                        f += Physic.Force.Gravitation(objects[i], objects[j]);
                    if (electricalF)
                        f += Physic.Force.Coulomb(objects[i], objects[j]);
                    objects[i].f += f;
                    objects[j].f -= f;
                }
        }

        void Collision()//elastische Stöße
        {
            //die reihenfolge von stößen die sich gegenseitig beeinflussen ist relevant
            //reihenfolge wird nicht abgdeckt
            for (int i = 1; i < objects.Count; i++)
            {
                var o1 = objects[i];
                for (int j = 0; j < i; j++)
                {
                    var o2 = objects[j];
                    var dv = o1.v - o2.v;
                    var dx = o1.x - o2.x;
                    double d = o1.r + o2.r;
                    var vs = dv * dx;
                    var vv = dv * dv;
                    double w = (vs * vs) - (dx.Square() * vv) + (d * d * vv);
                    if (w > 0)
                    {
                        w = Math.Sqrt(w);
                        double t1 = (-vs - w) / vv, t2 = (-vs + w) / vv;
                        bool b1 = t1 <= 0 && -t1 <= minTimeStep, b2 = t2 <= 0 && -t2 <= minTimeStep;
                        if (b1 || b2)
                        {
                            double t = b1 && b2 ? (t1 < t2 ? t1 : t2) : (b1 ? t1 : t2);
                            o1.x += o1.v * t;
                            o2.x += o2.v * t;


                            var n_0 = (o1.x - o2.x).Unit();
                            var v_s_1 = n_0 * (n_0 * o1.v);
                            var v_p_1 = o1.v - v_s_1;
                            var v_s_2 = n_0 * (n_0 * o2.v);
                            var v_p_2 = o2.v - v_s_2;

                            o1.v = (2 * v_s_2 * o2.m + v_s_1 * o1.m - v_s_1 * o2.m) / (o1.m + o2.m) + v_p_1;
                            o2.v = (2 * v_s_1 * o1.m + v_s_2 * o2.m - v_s_2 * o1.m) / (o1.m + o2.m) + v_p_2;


                            o1.x += o1.v * -t;
                            o2.x += o2.v * -t;
                        }
                    }
                }
            }
        }
    }

    /*public class EMField : Field
    {
        List<Charge> charges = new List<Charge>();
        EField last;

        public EMField(params Charge[] charge)
        {
            charges.AddRange(charge);
            last = new EField(charges.Select(c => c.Clone()).ToArray());
        }

        public override void ApplyForce()
        {
            last = new EField(charges.Select(c => c.Clone()).ToArray());;
            foreach (var c in charges)
                c.acc += Vector3.Mul(Strength(c.pos), c.charge / c.mass);
        }

        //Wie bestimme ich das magenetfeld?
        public override Vector3 Strength(Vector3 pos)
        {
            var s = new Vector3();
            foreach (var c in charges)
                if (pos != c.pos)
                    s += c.EField(pos);
            //var t = (s - last.Strength(pos) / minTimeStep);
            return s;
        }

        public EMField Clone() => new EMField(charges.Select(c => c.Clone()).ToArray());
    }*/
}