using System;
using Design_Patterns_project.Attributes;
using System.Collections.Generic;
using Design_Patterns_project;

namespace SingleTableInheritanceTest
{
    class SingleTableInheritanceTest
    {
        public static void Main(string[] args)
        {
            // remote -> "den1.mssql7.gear.host", "DPTest", "dptest", "Me3JyhRLOg-_"

            // local 
            // maciopelo -> "DESKTOP-HVUO0CP", "TestDB"
            // szymon -> "LAPTOP-BHF7G1P9", "Test" databases (like tests directories)

            DataManager mythicalManager = new DataManager("LAPTOP-BHF7G1P9", "SingleTableInheritanceTest");

            IceDragon iceDragon = new IceDragon(200, "Winter", 15, 20, 35, 50);
            GoldDragon goldDragon = new GoldDragon(220, "Smaug", 10, 20, 40, 40);
            List<Object> creatures = new List<Object>() { iceDragon, goldDragon };

            mythicalManager.Inherit(creatures, 0);

            Console.WriteLine("Utter success");
        }
    }

    class MythicalCreature
    {
        [PKey()]
        [Column()]
        int id { get; set; }

        [Column()]
        int health { get; set; }

        [Column("mythical_name")]
        string name { get; set; }

        public MythicalCreature(int health, string name)
        {
            this.health = health;
            this.name = name;
        }
    }

    class Dragon : MythicalCreature
    {
        [Column("blast_power")]
        int blastPower { get; set; }

        [Column()]
        int endurance { get; set; }

        public Dragon(int health, string name, int blastPower, int endurance) : base(health, name)
        {
            this.blastPower = blastPower;
            this.endurance = endurance;
        }
    }

    class GoldDragon : Dragon
    {
        [Column("mineral_hunger")]
        int mineralHunger { get; set; }

        [Column()]
        double preciousness { get; set; }

        public GoldDragon(int health, string name, int fireCapacity, int endurance, int mineralHunger, double preciousness)
            : base(health, name, fireCapacity, endurance)
        {
            this.mineralHunger = mineralHunger;
            this.preciousness = preciousness;
        }
    }

    class IceDragon : Dragon
    {
        [Column("time_freeze")]
        int timeFreeze { get; set; }

        [Column("ice_fire")]
        double iceFire { get; set; }

        public IceDragon(int health, string name, int fireCapacity, int endurance, int timeFreeze, double iceFire)
            : base(health, name, fireCapacity, endurance)
        {
            this.timeFreeze = timeFreeze;
            this.iceFire = iceFire;
        }
    }
}
