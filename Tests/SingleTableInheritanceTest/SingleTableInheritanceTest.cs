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
            // szymon -> "LAPTOP-BHF7G1P9", "SingleTableInheritanceTest"

            DataManager mythicalManager = new DataManager("LAPTOP-BHF7G1P9", "SingleTableInheritanceTest");

            IceDragon iceDragon = new IceDragon(1, 200, "Winter", 15, 20, 35, 50);
            GoldDragon goldDragon = new GoldDragon(2, 220, "Smaug", 10, 20, 40, 40);
            Dragon dragon = new Dragon(3, 160, "Cracow Dragon", 7, 8);
            MythicalCreature mythicalCreature1 = new MythicalCreature(4, 50, "Dwarf");
            MythicalCreature mythicalCreature2 = new MythicalCreature(5, 15, "Goblin");
            MythicalCreature mythicalCreature3 = new MythicalCreature(6, 40, "Orc");
            List<Object> creatures = new List<Object>() { iceDragon, goldDragon };

            mythicalManager.Inherit(creatures, 0);
            mythicalManager.Insert(dragon);
            mythicalManager.Insert(iceDragon);
            mythicalManager.Insert(goldDragon);
            mythicalManager.Insert(mythicalCreature1);
            mythicalManager.Insert(mythicalCreature2);
            mythicalManager.Insert(mythicalCreature3);

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

        public MythicalCreature(int id, int health, string name)
        {
            this.id = id;
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

        public Dragon(int id, int health, string name, int blastPower, int endurance) : base(id, health, name)
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

        public GoldDragon(int id, int health, string name, int fireCapacity, int endurance, int mineralHunger, double preciousness)
            : base(id, health, name, fireCapacity, endurance)
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

        public IceDragon(int id, int health, string name, int fireCapacity, int endurance, int timeFreeze, double iceFire)
            : base(id, health, name, fireCapacity, endurance)
        {
            this.timeFreeze = timeFreeze;
            this.iceFire = iceFire;
        }
    }
}
