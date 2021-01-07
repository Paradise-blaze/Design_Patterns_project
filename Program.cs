using System;

namespace Design_Patterns_project
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Deutschland!");
            Console.WriteLine("Mein Herz in Flamen");
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

    class RareDragon : Dragon
    {
        string specialSkill;
        double preciousness;

        public RareDragon(int h, string n, int fc, int e, string ss, double p) : base(h, n, fc, e)
        {
            this.specialSkill = ss;
            this.preciousness = p;
        }
    }
}
