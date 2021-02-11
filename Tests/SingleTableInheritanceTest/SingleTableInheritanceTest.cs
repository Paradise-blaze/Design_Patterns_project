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
            //DataManager mythicalManager = new DataManager("DESKTOP-HVUO0CP", "SingleTableInheritanceTest");

            IceDragon iceDragon1 = new IceDragon(220, "Winter", 15, 20, 35, 50);
            IceDragon iceDragon2 = new IceDragon(210, "Whiter", 19, 22, 37, 51);
            IceDragon iceDragon3 = new IceDragon(200, "Freezer", 14, 23, 31, 46);
            GoldDragon goldDragon1 = new GoldDragon(220, "Smaug", 10, 20, 40, 40);
            GoldDragon goldDragon2 = new GoldDragon(250, "Borch", 19, 25, 45, 55);
            Dragon dragon1 = new Dragon(160, "Cracow Dragon", 12, 8);
            Dragon dragon2 = new Dragon(150, "English Dragon", 5, 10);
            MythicalCreature mythicalCreature1 = new MythicalCreature(50, "Dwarf");
            MythicalCreature mythicalCreature2 = new MythicalCreature(15, "Goblin");
            MythicalCreature mythicalCreature3 = new MythicalCreature(40, "Orc");
            List<Object> creatures = new List<Object>() { iceDragon1, goldDragon1 };

            //cases of incorrect usage of methods (before creating tables)
            mythicalManager.Delete(dragon2);
            mythicalManager.Update(goldDragon2);
            mythicalManager.Insert(dragon1);
            mythicalManager.Insert(dragon2);
            mythicalManager.Inherit(creatures, -1);

            //create and inherit
            mythicalManager.Inherit(creatures, 0);

            //insert
            mythicalManager.Insert(iceDragon1);
            mythicalManager.Insert(iceDragon2);
            mythicalManager.Insert(iceDragon3);
            mythicalManager.Insert(goldDragon1);
            mythicalManager.Insert(goldDragon2);
            mythicalManager.Insert(mythicalCreature1);
            mythicalManager.Insert(mythicalCreature2);
            mythicalManager.Insert(mythicalCreature3);

            //update
            iceDragon1.iceFire = 49;
            goldDragon2.health = 220;
            dragon2.name = "British Dragon";
            mythicalCreature3.health = 45;
            mythicalManager.Update(iceDragon1);
            mythicalManager.Update(goldDragon2);
            mythicalManager.Update(dragon2);
            mythicalManager.Update(mythicalCreature3);

            //delete
            mythicalManager.Delete(mythicalCreature1);
            mythicalManager.Delete(mythicalCreature1);
            mythicalManager.Delete(dragon1);

            //Test for relation-object mapping
            GoldDragon newDragon = (GoldDragon)mythicalManager.Select(typeof(GoldDragon), 7);

            Console.WriteLine("\nNew gold dragon");
            Console.WriteLine("    name: {0}", newDragon.name);
            Console.WriteLine("    health: {0}", newDragon.health);
            Console.WriteLine("    blast power: {0}", newDragon.blastPower);
            Console.WriteLine("    endurance: {0}", newDragon.endurance);
            Console.WriteLine("    mineral hunger: {0}", newDragon.mineralHunger);
            Console.WriteLine("    preciousness: {0}", newDragon.preciousness);

            MythicalCreature newCreature = (MythicalCreature)mythicalManager.Select(typeof(MythicalCreature), 100);

            if (newCreature == null)
            {
                Console.WriteLine("\nThere is no object of that id in table\n");
            }

            Console.WriteLine("Utter success");
        }
    }

    [Table()]
    class MythicalCreature
    {
        [Column()]
        public int health { get; set; }

        [Column("mythical_name")]
        public string name { get; set; }

        public MythicalCreature(int health, string name)
        {
            this.health = health;
            this.name = name;
        }
    }

    [Table()]
    class Dragon : MythicalCreature
    {
        [Column("blast_power")]
        public int blastPower { get; set; }

        [Column()]
        public int endurance { get; set; }

        public Dragon(int health, string name, int blastPower, int endurance) : base(health, name)
        {
            this.blastPower = blastPower;
            this.endurance = endurance;
        }
    }

    [Table()]
    class GoldDragon : Dragon
    {
        [Column("mineral_hunger")]
        public int mineralHunger { get; set; }

        [Column()]
        public double preciousness { get; set; }

        public GoldDragon(int health, string name, int blastPower, int endurance, int mineralHunger, double preciousness)
            : base(health, name, blastPower, endurance)
        {
            this.mineralHunger = mineralHunger;
            this.preciousness = preciousness;
        }
    }

    [Table()]
    class IceDragon : Dragon
    {
        [Column("time_freeze")]
        public int timeFreeze { get; set; }

        [Column("ice_fire")]
        public double iceFire { get; set; }

        public IceDragon(int health, string name, int fireCapacity, int endurance, int timeFreeze, double iceFire)
            : base(health, name, fireCapacity, endurance)
        {
            this.timeFreeze = timeFreeze;
            this.iceFire = iceFire;
        }
    }
}
