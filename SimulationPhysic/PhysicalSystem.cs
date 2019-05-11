using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Proceed()
        {
            time += minTimeStep;
            ApplyForce();
            objects.ForEach(b => b.Move(minTimeStep));
            Collision();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Proceed(int repetitions)
        {
            for (int i = 0; i < repetitions; i++)
                Proceed();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Collision1()//elastische Stöße
        {
            Object obj1 = null, obj2 = null;
            for (; ; )
            { 
                double t = -1;
                for (int i = 1; i < objects.Count; i++)
                {
                    obj1 = objects[i];
                    for (int j = 0; j < i; j++)
                    {
                        obj2 = objects[j];
                        var dv = obj1.v - obj2.v;
                        var dx = obj1.x - obj2.x;
                        double d = obj1.r + obj2.r;
                        var vs = Vector3.Dot(dv, dx);
                        var vv = dv.LengthSquared();
                        double w = (vs * vs) - (dx.LengthSquared() * vv) + (d * d * vv);
                        if (w > 0)
                        {
                            w = Math.Sqrt(w);
                            double t1 = (-vs - w) / vv, t2 = (-vs + w) / vv;
                            bool b1 = t1 <= 0 && -t1 <= minTimeStep, b2 = t2 <= 0 && -t2 <= minTimeStep;
                            if (b1 || b2)
                            {
                                double tt = b1 && b2 ? (t1 < t2 ? t1 : t2) : (b1 ? t1 : t2);
                                if(tt > t)
                                    t = tt;
                            }
                        }
                    }
                }
                if (t > -1)
                {
                    obj1.x += obj1.v * t;
                    obj2.x += obj2.v * t;

                    var n_0 = Vector3.Normalize(obj1.x - obj2.x);
                    var v_s_1 = n_0 * Vector3.Dot(n_0, obj1.v);
                    var v_p_1 = obj1.v - v_s_1;
                    var v_s_2 = n_0 * Vector3.Dot(n_0, obj2.v);
                    var v_p_2 = obj2.v - v_s_2;

                    obj1.v = (v_s_2 * (2 * obj2.m) + v_s_1 * (obj1.m - obj2.m)) / (obj1.m + obj2.m) + v_p_1;
                    obj2.v = (v_s_1 * (2 * obj1.m) + v_s_2 * (obj2.m - obj1.m)) / (obj1.m + obj2.m) + v_p_2;

                    obj1.x += obj1.v * -t;
                    obj2.x += obj2.v * -t;
                }
                else
                    break;
            }
        }
        void Collision()//elastische Stöße
        {
            //funktioniert nicht perfekt
            Object obj1 = null, obj2 = null;
            double t = 1;
            for (int i = 1; i < objects.Count; i++)
            {
                obj1 = objects[i];
                for (int j = 0; j < i; j++)
                {
                    obj2 = objects[j];
                    var dv = obj1.v - obj2.v;
                    var dx = obj1.x - obj2.x;
                    double d = obj1.r + obj2.r;
                    var vs = Vector3.Dot(dv, dx);
                    var vv = dv.LengthSquared();
                    double w = (vs * vs) - (dx.LengthSquared() * vv) + (d * d * vv);
                    if (w > 0)
                    {
                        w = Math.Sqrt(w);
                        double t1 = (-vs - w) / vv, t2 = (-vs + w) / vv;
                        bool b1 = t1 <= 0 && -t1 <= minTimeStep, b2 = t2 <= 0 && -t2 <= minTimeStep;
                        if (b1 || b2)
                        {
                            double tt = b1 && b2 ? (t1 < t2 ? t1 : t2) : (b1 ? t1 : t2);
                            if (tt < t)
                                t = tt;
                        }
                    }
                }
            }
            if (t != 1)
            {
                obj1.x += obj1.v * t;
                obj2.x += obj2.v * t;

                var n_0 = Vector3.Normalize(obj1.x - obj2.x);
                var v_s_1 = n_0 * Vector3.Dot(n_0, obj1.v);
                var v_p_1 = obj1.v - v_s_1;
                var v_s_2 = n_0 * Vector3.Dot(n_0, obj2.v);
                var v_p_2 = obj2.v - v_s_2;

                obj1.v = (v_s_2 * (2 * obj2.m) + v_s_1 * (obj1.m - obj2.m)) / (obj1.m + obj2.m) + v_p_1;
                obj2.v = (v_s_1 * (2 * obj1.m) + v_s_2 * (obj2.m - obj1.m)) / (obj1.m + obj2.m) + v_p_2;

                obj1.x += obj1.v * -t;
                obj2.x += obj2.v * -t;
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