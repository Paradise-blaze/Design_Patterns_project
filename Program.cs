using System;
using System.Collections.Generic;

namespace Design_Patterns_project
{
    class Program
    {
        public static void Main(string[] args)
        {
            IceDragon iceDragon = new IceDragon(20, "Gecko", 10, 20, 20, 50);
            GoldDragon goldDragon = new GoldDragon(20, "Gecko", 10, 20, "magnetic", 40);
            TableInheritance mythicalInheritance = new TableInheritance();
            List<Object> creatures = new List<Object> () { iceDragon, goldDragon };
            mythicalInheritance.InheritSingle(creatures);
            mythicalInheritance.InheritClass(creatures);
            mythicalInheritance.InheritConcrete(creatures);
        }
    }

    class MythicalCreature
    {
        int health;
        string name;

        public MythicalCreature(int h, string n)
        {
            this.health = h;
            this.name = n;
        }
    }

    class Dragon : MythicalCreature
    {
        int fireCapacity;
        int endurance;

        public Dragon(int h, string n, int fc, int e) : base(h, n)
        {
            this.fireCapacity = fc;
            this.endurance = e;
        }
    }

    class GoldDragon : Dragon
    {
        string specialSkill;
        double preciousness;

        public GoldDragon(int h, string n, int fc, int e, string ss, double p) : base(h, n, fc, e)
        {
            this.specialSkill = ss;
            this.preciousness = p;
        }
    }

    class IceDragon : Dragon
    {
        int iceCapacity;
        double timeFreeze;

        public IceDragon(int h, string n, int fc, int e, int ic, double tf) : base(h, n, fc, e)
        {
            this.iceCapacity = ic;
            this.timeFreeze = tf;
        }
    }
}
