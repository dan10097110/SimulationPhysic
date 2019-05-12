using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SimulationPhysic
{
    public class PhysicalSystem
    {
        public List<Object> objects = new List<Object>();
        public readonly double minTimeStep;
        public double time = 0;
        //readonly bool gravitation = false, electricalF = true;

        public PhysicalSystem(double minTimeStep, params Object[] objects)
        {
            this.minTimeStep = minTimeStep;
            this.objects.AddRange(objects);
        }

        public void Add(params Object[] objects) => this.objects.AddRange(objects);

        double proceededTime = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Proceed()
        {
            for (int i = 1; i < objects.Count; i++)
                for (int j = 0; j < i; j++)
                {
                    var f = Physic.Force.CoulombGravitation(objects[i], objects[j]);
                    objects[i].force = f;
                    objects[j].force = -f;
                }

            foreach(var obj in objects)//vlt linq schneller?
                obj.x += obj.v * minTimeStep;

            Collision();

            foreach (var obj in objects)//vlt linq schneller?
                obj.p += obj.force * proceededTime;

            time += proceededTime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Proceed(int repetitions)
        {
            for (int i = 0; i < repetitions; i++)
                Proceed();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Collision()
        {
            //funktioniert nicht perfekt: proton elektron collision fehlerhaft
            //materie antimateire reaktion
            //evt kein voranschreiten da nach kolision immer wieder kollision registriert wird: lösbar mit <= < oder >= >

            Object obj1 = null, obj2 = null;
            int obj1Index = -1, obj2Index = -1;
            double t = 1;
            for (int i = 1; i < objects.Count; i++)
            {
                obj1 = objects[i];
                for (int j = 0; j < i; j++)
                {
                    obj2 = objects[j];
                    var dv = obj1.v - obj2.v;
                    var vv = dv.LengthSquared();
                    var dx = obj1.x - obj2.x;
                    var xx = dx.LengthSquared();
                    double d = obj1.r + obj2.r;
                    var vx = Vector3.Dot(dv, dx);
                    double w = vx * vx - xx * vv + d * d * vv;
                    if (w > 0)
                    {
                        w = Math.Sqrt(w);
                        double t1 = (-vx - w) / vv, t2 = (-vx + w) / vv;
                        bool b1 = t1 <= 0 && t1 >= -minTimeStep, b2 = t2 <= 0 && t2 >= -minTimeStep;
                        if (b1 || b2)
                        {
                            double tt = b1 && b2 ? (t1 < t2 ? t1 : t2) : (b1 ? t1 : t2);
                            if (tt < t)
                            {
                                t = tt;
                                obj1Index = i;
                                obj2Index = j;
                            }
                        }
                    }
                }
            }
            if (t != 1)
            {
                proceededTime = minTimeStep + t;
                foreach (var obj in objects)
                    obj.x += obj.v * t;
                /*if(obj1 is Electron && obj2 is Positron)
                {
                    objects.Add(new Photon(schnittpunkt, obj1.p + obj2.p, obj1.E + obj2.E));

                    objects.RemoveAt(obj1Index);
                    objects.RemoveAt(obj2Index);
                }
                else*/
                {
                    //elastischer Stoß





                    //problem geschwindigkeit beim ersten zusammenstoß hier schon unterschiedlich




                    //guckem ob mithilfe von vektorklasse verbesserbar

                    var n_0 = Vector3.Normalize(obj1.x - obj2.x);
                    var v_s_1 = n_0 * Vector3.Dot(n_0, obj1.v);
                    var v_p_1 = obj1.v - v_s_1;
                    var v_s_2 = n_0 * Vector3.Dot(n_0, obj2.v);
                    var v_p_2 = obj2.v - v_s_2;




                    //wird direkt v verändert könte relativisitsch falsch sein
                    //provisorische lösung m_0 statt m

                    obj1.v = (v_s_2 * (2 * obj2.m_0) + v_s_1 * (obj1.m_0 - obj2.m_0)) / (obj1.m_0 + obj2.m_0) + v_p_1;
                    obj2.v = (v_s_1 * (2 * obj1.m_0) + v_s_2 * (obj2.m_0 - obj1.m_0)) / (obj1.m_0 + obj2.m_0) + v_p_2;
                }
            }
            else
                proceededTime = minTimeStep;
        }

        /*void Collision1()//elastische Stöße
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
                                if (tt > t)
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

                    obj1.v = (v_s_2 * (2 * obj2.m_0) + v_s_1 * (obj1.m_0 - obj2.m_0)) / (obj1.m_0 + obj2.m_0) + v_p_1;
                    obj2.v = (v_s_1 * (2 * obj1.m_0) + v_s_2 * (obj2.m_0 - obj1.m_0)) / (obj1.m_0 + obj2.m_0) + v_p_2;

                    obj1.x += obj1.v * -t;
                    obj2.x += obj2.v * -t;
                }
                else
                    break;
            }
        }*/
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