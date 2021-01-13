using System;
using Design_Patterns_project.Attributes;
using System.Collections.Generic;

namespace Design_Patterns_project
{
    class Program
    {
        public static void Main(string[] args)
        {


            //remote -> "den1.mssql7.gear.host", "DPTest", "dptest", "Me3JyhRLOg-_"
            //local -> "DESKTOP-HVUO0CP", "TestDB"

            /*MsSqlConnectionConfig remoteConfig = new MsSqlConnectionConfig("den1.mssql7.gear.host", "DPTest", "dptest", "Me3JyhRLOg-_");
            MsSqlConnectionConfig localConfig = new MsSqlConnectionConfig("DESKTOP-HVUO0CP", "TestDB");
            MsSqlConnection connection = new MsSqlConnection(localConfig);
            connection.ConnectAndOpen();
            string query = "SELECT * FROM dbo.Players;";
            string output = connection.ExecuteSelectCommand(query);
            Console.WriteLine(output);
            connection.Dispose();*/


            IceDragon iceDragon = new IceDragon(200, "Winterrrer", 10, 20, 20, 50);
            GoldDragon goldDragon = new GoldDragon(220, "Sauman", 10, 20, 40, 40);
            DataManager mythicalManager = new DataManager();
            List<Object> creatures = new List<Object> () { iceDragon, goldDragon };
            List<Object> wizards = new List<Object>() { new Wizard("Romas", 10, 2.41) };
            mythicalManager.Inherit(creatures, 2);
            mythicalManager.Inherit(wizards, 1);
            Console.WriteLine("Utter success");
        }
    }

    class Character
    {
        string name;

        public Character(string name)
        {
            this.name = name;
        }
    }

    class Wizard : Character
    {
        [PKey()]
        int id;
        int health;
        double magicPower;

        [OneToMany()]
        List<Dragon> dragons1 = new List<Dragon>();

        [OneToMany()]
        List<Dragon> dragons2 = new List<Dragon>();

        public Wizard(string name, int health, double magicPower) : base(name)
        {
            this.health = health;
            this.magicPower = magicPower;
        }
    }

    class MythicalCreature
    {
        [Column()]
        int health { get; set; }
        string name;

        public MythicalCreature(int health, string name)
        {
            this.health = health;
            this.name = name;
        }
    }

    class Dragon : MythicalCreature
    {
        int blastPower;
        int endurance { get; set; }

        public Dragon(int health, string name, int blastPower, int endurance) : base(health, name)
        {
            this.blastPower = blastPower;
            this.endurance = endurance;
        }
    }

    class GoldDragon : Dragon
    {
        int mineralHunger;
        double preciousness;

        public GoldDragon(int health, string name, int fireCapacity, int endurance, int mineralHunger, double preciousness)
            : base(health, name, fireCapacity, endurance)
        {
            this.mineralHunger = mineralHunger;
            this.preciousness = preciousness;
        }
    }

    class IceDragon : Dragon
    {
        int iceCapacity;
        double timeFreeze;

        public IceDragon(int health, string name, int fireCapacity, int endurance, int iceCapacity, double timeFreeze)
            : base(health, name, fireCapacity, endurance)
        {
            this.iceCapacity = iceCapacity;
            this.timeFreeze = timeFreeze;

        }
    }
}
