using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static SimulationPhysic.Physic.Constant;

namespace SimulationPhysic
{
    public class PhysicalSystem
    {
        public List<Object> objects;
        public readonly double minTimeStep;
        public double time;
        double proceededTime;

        static Random random = new Random();

        public PhysicalSystem(double minTimeStep, params Object[] objects)
        {
            this.minTimeStep = minTimeStep;
            this.objects = new List<Object>(objects);
        }

        public void Add(params Object[] objects) => this.objects.AddRange(objects);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Proceed(int repetitions)
        {
            for (int i = 0; i < repetitions; i++)
                Proceed();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Proceed()
        {
            Decay();

            CalcForces();

            int index1 = -1, index2 = -1;
            proceededTime = minTimeStep;
            bool collisionHappend = false;
            GetFirstCollision(ref collisionHappend, ref index1, ref index2, ref proceededTime);
            objects.ForEach(obj =>
            {
                obj.Move(proceededTime);
                obj.Accelerate(proceededTime);
            });
            if (collisionHappend)
            {
                if (Object.Annihilating(objects[index1], objects[index2]))
                    Annihilate(index1, index2);
                else
                    ElasticCollision(objects[index1], objects[index2]);
            }

            time += proceededTime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Decay()//auf nicht photonen zerfall anpassen
        {
            for (int i = 0; i < objects.Count; i++)
                if (!objects[i].stable && random.Next(0, int.MaxValue) < (1 - Math.Pow(0.5, proceededTime / objects[i].t_half)) * int.MaxValue)
                {
                    var p_p = objects[i].p / 2;
                    var dir = new Vector3(p_p.X, p_p.Y, p_p.Z);
                    if (dir == Vector3.Zero)
                        dir = Vector3.Random();
                    var p_s = Vector3.Cross(Vector3.Random(), dir).Normalize() * Math.Sqrt(cc - p_p.LengthSquared());

                    objects.Add(new Photon(objects[i].x, p_p + p_s, objects[i].E / 2));
                    objects.Add(new Photon(objects[i].x, p_p - p_s, objects[i].E / 2));
                    objects.RemoveAt(i);
                }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CalcForces()
        {
            for (int i = 1; i < objects.Count; i++)
                for (int j = 0; j < i; j++)
                {
                    var f = Physic.Force.CoulombGravitation(objects[i], objects[j]);
                    objects[i].Force += f;
                    objects[j].Force -= f;
                }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetFirstCollision(ref bool collision, ref int index1, ref int index2, ref double collisionTime)
        {
            for (int i = 1; i < objects.Count; i++)
                for (int j = 0; j < i; j++)
                    if(!(objects[i].m == 0 && objects[j].m == 0))
                    {
                        var dv = objects[i].v - objects[j].v;
                        var dx = objects[i].x - objects[j].x;
                        double dvdx = Vector3.Dot(dv, dx);
                        double dvdv = dv.LengthSquared();
                        double d = objects[i].r + objects[j].r;
                        double w = dvdx * dvdx + (d * d - dx.LengthSquared()) * dvdv;
                        if (w >= 0)
                        {
                            double t = (-dvdx - Math.Sqrt(w)) / dvdv;
                            if (t > 0 && t <= collisionTime)
                            {
                                collision = true;
                                collisionTime = t;
                                index1 = i;
                                index2 = j;
                            }
                        }
                    }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Annihilate(int i1, int i2)//anpassen an nicht ELEKTRON POSITRON
        {
            double temp = 1 / Math.Sqrt(1 / objects[i1].v.LengthSquared() - 1 / cc) + 1 / Math.Sqrt(1 / objects[i2].v.LengthSquared() - 1 / cc);
            double vv = 1 / (4 / (temp * temp) + 1 / cc);
            var direction = objects[i1].p + objects[i2].p;
            double lengthSqaured = direction.LengthSquared();
            direction = lengthSqaured == 0 ? new Vector3() : direction / Math.Sqrt(lengthSqaured);
            objects.Add(new Object(2 * objects[i1].m, 0, 1.06E-10, (objects[i1].x + objects[i2].x) / 2, Math.Sqrt(vv) * direction, 0, 2 * objects[i1].m * cc / Math.Sqrt(1 - vv / cc) - (objects[i1].E + objects[i2].E), false, 1.25E-10, new Vector3()));
            objects.RemoveAt(i1);
            objects.RemoveAt(i2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ElasticCollision(Object o1, Object o2)
        {
            var n_0 = (o1.x - o2.x).Normalize();
            var v_s_1 = n_0 * Vector3.Dot(n_0, o1.v);
            var v_s_2 = n_0 * Vector3.Dot(n_0, o2.v);

            //o1.v += (v_s_2 * (2 * o2.m) + v_s_1 * (o1.m - o2.m)) / (o1.m + o2.m) - v_s_1;
            //o2.v += (v_s_1 * (2 * o1.m) + v_s_2 * (o2.m - o1.m)) / (o1.m + o2.m) - v_s_2;

            //https://en.wikipedia.org/wiki/Elastic_collision
            var Z = Math.Sqrt((1 - v_s_1.LengthSquared() / cc) * (1 - v_s_2.LengthSquared() / cc));
            //o1.v += (2 * o1.m * o2.m * cc * v_s_2 * Z + 2 * o2.m * o2.m * cc * v_s_2 - (o1.m * o1.m + o2.m * o2.m) * v_s_1 * v_s_2.LengthSquared() + (o1.m * o1.m - o2.m * o2.m) * cc * v_s_1) / (2 * o1.m * o2.m * cc * Z - 2 * o2.m * o2.m * Vector3.Dot(v_s_1, v_s_2) - (o1.m * o1.m - o2.m * o2.m) * v_s_2.LengthSquared() + (o1.m * o1.m + o2.m * o2.m) * cc) - v_s_1;
            //o2.v += (2 * o2.m * o1.m * cc * v_s_1 * Z + 2 * o1.m * o1.m * cc * v_s_1 - (o2.m * o2.m + o1.m * o1.m) * v_s_2 * v_s_1.LengthSquared() + (o2.m * o2.m - o1.m * o1.m) * cc * v_s_2) / (2 * o2.m * o1.m * cc * Z - 2 * o1.m * o1.m * Vector3.Dot(v_s_2, v_s_1) - (o2.m * o2.m - o1.m * o1.m) * v_s_1.LengthSquared() + (o2.m * o2.m + o1.m * o1.m) * cc) - v_s_2;

            o1.v += (2 * o1.m_0 * o2.m_0 * cc * v_s_2 * Z + 2 * o2.m_0 * o2.m_0 * cc * v_s_2 - (o1.m_0 * o1.m_0 + o2.m_0 * o2.m_0) * v_s_1 * v_s_2.LengthSquared() + (o1.m_0 * o1.m_0 - o2.m_0 * o2.m_0) * cc * v_s_1) / (2 * o1.m_0 * o2.m_0 * cc * Z - 2 * o2.m_0 * o2.m_0 * Vector3.Dot(v_s_1, v_s_2) - (o1.m_0 * o1.m_0 - o2.m_0 * o2.m_0) * v_s_2.LengthSquared() + (o1.m_0 * o1.m_0 + o2.m_0 * o2.m_0) * cc) - v_s_1;
            o2.v += (2 * o2.m_0 * o1.m_0 * cc * v_s_1 * Z + 2 * o1.m_0 * o1.m_0 * cc * v_s_1 - (o2.m_0 * o2.m_0 + o1.m_0 * o1.m_0) * v_s_2 * v_s_1.LengthSquared() + (o2.m_0 * o2.m_0 - o1.m_0 * o1.m_0) * cc * v_s_2) / (2 * o2.m_0 * o1.m_0 * cc * Z - 2 * o1.m_0 * o1.m_0 * Vector3.Dot(v_s_2, v_s_1) - (o2.m_0 * o2.m_0 - o1.m_0 * o1.m_0) * v_s_1.LengthSquared() + (o2.m_0 * o2.m_0 + o1.m_0 * o1.m_0) * cc) - v_s_2;
        }

        public Vector3 FieldStrength(Vector3 pos)
        {
            Vector3 str = Vector3.Zero;
            foreach (Object obj in objects)
                str += obj.FieldStrength(pos);
            return str;
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