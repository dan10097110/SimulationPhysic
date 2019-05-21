using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static SimulationPhysic.Physic.Constant;

namespace SimulationPhysic
{
    public class PhysicalSystem
    {
        public List<Object> objects = new List<Object>();
        public readonly double minTimeStep;
        public double time = 0;
        double proceededTime = 0;

        public PhysicalSystem(double minTimeStep, params Object[] objects)
        {
            this.minTimeStep = minTimeStep;
            this.objects.AddRange(objects);
        }

        public void Add(params Object[] objects) => this.objects.AddRange(objects);

        Random random = new Random();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Proceed()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (!objects[i].stable)
                {
                    if (random.Next(0, int.MaxValue) < (1 - Math.Pow(0.5, proceededTime / objects[i].t_half)) * int.MaxValue)
                    {
                        //auf nicht photonen zerfall anpassen;
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
            }

            for (int i = 1; i < objects.Count; i++)
                for (int j = 0; j < i; j++)
                {
                    var f = Physic.Force.CoulombGravitation(objects[i], objects[j]);
                    objects[i].Force += f;
                    objects[j].Force -= f;
                }

            objects.ForEach(obj => obj.Move(minTimeStep));

            int index1 = -1, index2 = -1;
            double collisionTime = 0;
            bool collision = false;
            for (int i = 1; i < objects.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    var dv = objects[i].v - objects[j].v;
                    var vv = dv.LengthSquared();
                    var dx = objects[i].x - objects[j].x;
                    var xx = dx.LengthSquared();
                    double d = objects[i].r + objects[j].r;
                    var vx = Vector3.Dot(dv, dx);
                    double w = vx * vx - xx * vv + d * d * vv;
                    if (w > 0)
                    {
                        double t = (-vx - Math.Sqrt(w)) / vv;
                        if (t <= 0 && t >= -proceededTime && t <= collisionTime)
                        {
                            collision = true;
                            collisionTime = t;
                            index1 = i;
                            index2 = j;
                        }
                    }
                }
            }
            proceededTime = minTimeStep + collisionTime;
            if (collision)
            {
                objects.ForEach(obj =>
                {
                    obj.Move(collisionTime);
                    obj.Accelerate(proceededTime);
                });
                if (Object.MatterAntiMatter(objects[index1], objects[index2]))//materie antimaterie reaktion
                {
                    double E_ges = objects[index1].E + objects[index2].E;

                    double temp = 1 / Math.Sqrt(1 / objects[index1].v.LengthSquared() - 1 / cc) + 1 / Math.Sqrt(1 / objects[index2].v.LengthSquared() - 1 / cc);
                    double v = 1 / Math.Sqrt(4 / (temp * temp) + 1 / cc);
                    var direction = objects[index1].v + objects[index2].v;
                    double length = direction.Length();
                    direction = length == 0 ? new Vector3() : direction / length;
                    var fusion = new Object(2 * objects[index1].m, 0, 1.06E-10, (objects[index1].x + objects[index2].x) / 2, v * direction, 0);
                    fusion.E_Extra = fusion.E - objects[index1].E - objects[index2].E;
                    fusion.t_half = 1.25E-10;//anpassen an nicht ELEKTRON POSITRON
                    fusion.stable = false;
                    objects.Add(fusion);
                    objects.RemoveAt(index1);
                    objects.RemoveAt(index2);
                }
                else//elastischer Stoß
                {
                    if(!(objects[index1].m == 0 && objects[index1].m == 0))
                    {
                        var n_0 = (objects[index1].x - objects[index2].x).Normalize();
                        var v_s_1 = n_0 * Vector3.Dot(n_0, objects[index1].v);
                        var v_s_2 = n_0 * Vector3.Dot(n_0, objects[index2].v);


                        //wird direkt v verändert könte relativisitsch falsch sein


                        objects[index1].v += (v_s_2 * (2 * objects[index2].m) + v_s_1 * (objects[index1].m - objects[index2].m)) / (objects[index1].m + objects[index2].m) - v_s_1;
                        objects[index2].v += (v_s_1 * (2 * objects[index1].m) + v_s_2 * (objects[index2].m - objects[index1].m)) / (objects[index1].m + objects[index2].m) - v_s_2;

                        /* var cc = Physic.Constant.ligthSpeed * Physic.Constant.ligthSpeed;
                        var c = Physic.Constant.ligthSpeed;

                        var p_1 = v_s_1 * objects[obj1Index].m;
                        var p_2 = v_s_2 * objects[obj2Index].m;
                        var p = p_1 + p_2;

                        var E_1 = objects[obj1Index].m * cc;
                        var E_2 = objects[obj2Index].m * cc;
                        var E = E_1 + E_2;

                        var E_01 = objects[obj1Index].m_0 * cc;
                        var E_02 = objects[obj2Index].m_0 * cc;

                        var v = E * E + E_1 * E_1 - E_2 * E_2 - cc * Vector3.Dot(p, p);
                        var v1 = (p * c * v + Math.Sqrt(v * v * (Vector3.Dot(p, p) * cc - cc * Vector3.Dot(p, p) + E * E) + 4 * E * E * E_01 * E_01 * (cc * Vector3.Dot(p, p) - E * E)));

                        objects[obj1Index].p += v1 / (2 * c * (cc * Vector3.Dot(p, p) - E * E)) - v_s_1;
                        objects[obj2Index].p +=  - v_s_2;*/
                    }
                }
            }
            else
                objects.ForEach(obj => obj.Accelerate(proceededTime));

            time += proceededTime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Proceed(int repetitions)
        {
            for (int i = 0; i < repetitions; i++)
                Proceed();
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