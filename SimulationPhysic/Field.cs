using System.Collections.Generic;
using System.Linq;

namespace SimulationPhysic
{
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

    public class Field
    {
        public List<Object> objects = new List<Object>();

        public Field(params Object[] objects)
        {
            this.objects.AddRange(objects);
        }

        public void Add(params Object[] objects)
        {
            this.objects.AddRange(objects);
        }

        public virtual void ApplyForce()
        {
            for(int i = 1; i < objects.Count; i++)
                for(int j = 0; j < i; j++)
                {
                    var f = Physic.Force.Gravitation(objects[i], objects[j]) + Physic.Force.Coulomb(objects[i], objects[j]);
                    objects[i].a -= f / objects[i].m;
                    objects[j].a += f / objects[j].m;
                }
        }
    }
}