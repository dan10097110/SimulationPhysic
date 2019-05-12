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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Proceed()
        {
            objects.SelectMany(l => objects, (l, r) => (l, r));

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
                /*if(obj1 is Electron && obj2 is Positron)//materie antimaterie reaktion
                {
                    objects.Add(new Photon(schnittpunkt, obj1.p + obj2.p, obj1.E + obj2.E));

                    objects.RemoveAt(obj1Index);
                    objects.RemoveAt(obj2Index);
                }
                else*/
                {
                    //elastischer Stoß

                    var n_0 = Vector3.Normalize(objects[obj1Index].x - objects[obj2Index].x);
                    var v_s_1 = n_0 * Vector3.Dot(n_0, objects[obj1Index].v);
                    var v_s_2 = n_0 * Vector3.Dot(n_0, objects[obj2Index].v);



                    //wird direkt v verändert könte relativisitsch falsch sein




                    objects[obj1Index].v += (v_s_2 * (2 * objects[obj2Index].m) + v_s_1 * (objects[obj1Index].m - objects[obj2Index].m)) / (objects[obj1Index].m + objects[obj2Index].m) - v_s_1;
                    objects[obj2Index].v += (v_s_1 * (2 * objects[obj1Index].m) + v_s_2 * (objects[obj2Index].m - objects[obj1Index].m)) / (objects[obj1Index].m + objects[obj2Index].m) - v_s_2;
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