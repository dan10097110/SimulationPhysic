using System.Collections.Generic;
using System.Linq;

namespace SimulationPhysic
{
    public class EField : Field
    {
        public EField(params Charge[] charges)
        {
            objects.AddRange(charges.Select(c => (IFieldObject)c));
        }

        public void Add(params Charge[] charges)
        {
            objects.AddRange(charges.Select(c => (IFieldObject)c));
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

    public class GField : Field
    {
        public GField(params Body[] bodies)
        {
            objects.AddRange(bodies.Select(b => (IFieldObject)b));
        }
    }

    public abstract class Field
    {
        public List<IFieldObject> objects = new List<IFieldObject>();

        public Vector3 Strength(Vector3 pos)
        {
            var s = new Vector3();
            foreach (var o in objects)
                if (pos != o.Pos())
                    s += o.Field(pos);
            return s;
        }

        public virtual void ApplyForce()
        {
            foreach (var o in objects)
                o.AddForceByStrength(Strength(o.Pos()));
        }
    }
}