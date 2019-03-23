using System.Collections.Generic;

namespace SimulationPhysic
{
    public class ElectricalField : Field
    {
        List<Charge> charges = new List<Charge>();

        public ElectricalField(params Charge[] charge)
        {
            charges.AddRange(charge);
        }

        public void Add(params Charge[] charge)
        {
            charges.AddRange(charge);
        }

        public override void Calculate()
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

    public abstract class Field
    {
        public abstract Vector3 Strength(Vector3 pos);
        public abstract void Calculate();
    }
}
