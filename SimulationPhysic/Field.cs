using System.Collections.Generic;
using System.Linq;

namespace SimulationPhysic
{
    public class EField : Field
    {
        List<Charge> charges = new List<Charge>();

        public EField(params Charge[] charge)
        {
            charges.AddRange(charge);
        }

        public void Add(params Charge[] charge)
        {
            charges.AddRange(charge);
        }

        public override void ApplyForce()
        {
            foreach (var c in charges)
                c.acc += Vector3.Mul(Strength(c.pos), c.charge / c.mass);
        }

        public override Vector3 Strength(Vector3 pos)
        {
            var s = new Vector3();
            foreach (Charge c in charges)
                if (pos != c.pos)
                    s += c.ElectricalField(pos);
            return s;
        }
    }
    public class EMField : Field
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
                    s += c.ElectricalField(pos);
            //var t = (s - last.Strength(pos) / minTimeStep);
            return s;
        }

        public EMField Clone() => new EMField(charges.Select(c => c.Clone()).ToArray());
    }

    public class GField : Field
    {
        List<Body> bodies = new List<Body>();

        public GField(params Body[] bodies)
        {
            this.bodies = bodies.ToList();
        }

        public override void ApplyForce()
        {
            foreach (var b in bodies)
                b.acc += Strength(b.pos);
        }

        public override Vector3 Strength(Vector3 pos)
        {
            var s = new Vector3();
            foreach (var b in bodies)
                if (pos != b.pos)
                    s += b.GField(pos);
            return s;
        }
    }

    public abstract class Field
    {
        public abstract Vector3 Strength(Vector3 pos);
        public abstract void ApplyForce();
    }
}