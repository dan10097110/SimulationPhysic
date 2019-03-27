using System.Collections.Generic;
using System;

namespace SimulationPhysic
{
    public class PhysicalSystem
    {
        public List<Object> objects = new List<Object>();
        public Field field = new Field();
        public readonly double minTimeStep;
        public double time = 0;

        public PhysicalSystem(double minTimeStep)
        {
            this.minTimeStep = minTimeStep;
        }

        public void AddCharge(params Object[] objects)
        {
            field.Add(objects);
            this.objects.AddRange(objects);
        }

        public void Proceed()
        {
            time += minTimeStep;
            field.ApplyForce();
            objects.ForEach(b => b.Move(minTimeStep));
            //elastische Stöße
            for(int i = 1; i < objects.Count; i++)
            {
                for(int j = 0; j < i; j++)
                {
                    double d = objects[i].r + objects[j].r;
                    var v = objects[i].v - objects[j].v;
                    var s = objects[i].x - objects[j].x;
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
                            objects[i].x += objects[i].v * t;
                            objects[j].x += objects[j].v * t;
                            var x = objects[i].v.Clone();
                            var y = objects[j].v.Clone();


                            var n_0 = (objects[i].x - objects[j].x).Unit();
                            var v_s_1 = n_0 * (n_0 * objects[i].v);
                            var v_p_1 = objects[i].v - v_s_1;
                            var v_s_2 = n_0 * (n_0 * objects[j].v);
                            var v_p_2 = objects[j].v - v_s_2;

                            objects[i].v = (2 * v_s_2 * objects[j].m + v_s_1 * objects[i].m - v_s_1 * objects[j].m) / (objects[i].m + objects[j].m) + v_p_1;
                            objects[j].v = (2 * v_s_1 * objects[i].m + v_s_2 * objects[j].m - v_s_2 * objects[i].m) / (objects[i].m + objects[j].m) + v_p_2;

                            
                            objects[i].x += objects[i].v * -t;
                            objects[j].x += objects[j].v * -t;
                        }
                    }
                }
            }
        }
    }
}