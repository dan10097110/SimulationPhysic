using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace SimulationPhysic
{
    public class PhysicalSystem
    {
        public List<Object> objects = new List<Object>();
        public readonly double minTimeStep;
        public double time = 0;
        double proceededTime = 0;
        //readonly bool gravitation = false, electricalF = true;

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
            for(int i = 0; i < objects.Count; i++)
            {
                if(!objects[i].stable)
                {
                    int i = random.Next(0, int.MaxValue);
                    if(i < decayProb * int.MaxValue)
                    {
                        //auf nicht photonen zerfall anpassen;
                        double p_p = objects[i].p / 2;
                        double p_s = Vector3.Cross(Vector3.Random(), objects[i].p).Normalize() * Math.Sqrt(Physic.Constant.ligthSpeed * Physic.Constant.ligthSpeed - p_s*p_s);
                        
                        double f = objects[i].E / (2 * Physic.Constant.planckconstant);  

                        objects.AddRange(new Photon(objects[i].x, p_p + p_s), new Photon(objects[i].x, p_p - p_s));
                        objects.RemoveAt(i);
                    }
                }
            }

            for (int i = 1; i < objects.Count; i++)
                for (int j = 0; j < i; j++)
                {
                    var f = Physic.Force.CoulombGravitation(objects[i], objects[j]);
                    objects[i].force += f;
                    objects[j].force += -f;
                }

            objects.ForEach(obj => obj.x += obj.v * minTimeStep);

            int obj1Index = -1, obj2Index = -1;
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
                            obj1Index = i;
                            obj2Index = j;
                        }
                    }
                }
            }
            proceededTime = minTimeStep + collisionTime;
            if (collision)
            {
                objects.ForEach(obj => {
                    obj.x += obj.v * collisionTime;
                    obj.p += obj.force * proceededTime;
                    obj.force = new Vector3();
                    });
                if(Object.MatterAntiMatter(objects[obj1Index], objects[obj2Index]))//materie antimaterie reaktion
                {
                    double E_ges = objects[obj1Index].E + objects[obj2Index].E;

                    double temp = 1 / Math.Sqrt(1 / (objects[obj1Index].v * objects[obj1Index].v) - 1 / (Physic.Constant.ligthSpeed * Physica.Constant.ligthSpeed))+1 / Math.Sqrt(1 / (objects[obj2Index].v * objects[obj2Index].v) - 1 / (Physic.Constant.ligthSpeed * Physica.Constant.ligthSpeed));
                    double v = 1 / Math.Sqrt(4 / (temp * temp) + 1 / (Physic.Constant.ligthSpeed * Physica.Constant.ligthSpeed));
                    var fusion = new Object(objects[obj1Index] * 2, 0, 0/*ändern */, schnittpunkt, v, 0));
                    fusion.E_Extra = fusion.E - objects[obj1Index].E - objects[obj2Index].E;
                    fusion.t_half = 1.25E-10;//anpassen an nicht ELEKTRON POSITRON
                    objects.Add(fusion);
                    objects.RemoveAt(obj1Index);
                    objects.RemoveAt(obj2Index);
                }
                else//elastischer Stoß
                {


                    var n_0 = Vector3.Normalize(objects[obj1Index].x - objects[obj2Index].x);
                    var v_s_1 = n_0 * Vector3.Dot(n_0, objects[obj1Index].v);
                    var v_s_2 = n_0 * Vector3.Dot(n_0, objects[obj2Index].v);



                    //wird direkt v verändert könte relativisitsch falsch sein




                    objects[obj1Index].v += (v_s_2 * (2 * objects[obj2Index].m) + v_s_1 * (objects[obj1Index].m - objects[obj2Index].m)) / (objects[obj1Index].m + objects[obj2Index].m) - v_s_1;
                    objects[obj2Index].v += (v_s_1 * (2 * objects[obj1Index].m) + v_s_2 * (objects[obj2Index].m - objects[obj1Index].m)) / (objects[obj1Index].m + objects[obj2Index].m) - v_s_2;

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
            else
                objects.ForEach(obj => {
                    obj.p += obj.force * proceededTime;
                    obj.force = new Vector3();
                });

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