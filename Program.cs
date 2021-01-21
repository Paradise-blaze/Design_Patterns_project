using System;
using Design_Patterns_project.Attributes;
using Design_Patterns_project.Connection;
using System.Collections.Generic;



namespace Design_Patterns_project
{
    class Program
    {
        public static void Main(string[] args)
        {


            //remote -> "den1.mssql7.gear.host", "DPTest", "dptest", "Me3JyhRLOg-_"
            //local -> "DESKTOP-HVUO0CP", "TestDB"

            // MsSqlConnectionConfig remoteConfig = new MsSqlConnectionConfig("den1.mssql7.gear.host", "DPTest", "dptest", "Me3JyhRLOg-_");
            //MsSqlConnectionConfig localConfig = new MsSqlConnectionConfig("DESKTOP-HVUO0CP", "TestDB");

            //MsSqlConnection connection = new MsSqlConnection(localConfig);
            //connection.ConnectAndOpen();
            

            // string testSelectQuery = "SELECT * FROM dbo.Players;";
            // string testInsertQuery = "INSERT INTO dbo.Players (PlayerID,Name,Surname,Age,Nick) VALUES (4,'Gerard','Pique',33,'Lion');";
            // string testDeleteQuery = "DELETE FROM dbo.Players WHERE PlayerID = 4;";

            // string output = "";
            // connection.ExecuteQuery(testInsertQuery);
            // output = connection.ExecuteSelectQuery(testSelectQuery);
            // Console.WriteLine(output);
            

            // connection.ExecuteQuery(testDeleteQuery);
            // output = connection.ExecuteSelectQuery(testSelectQuery);
            // Console.WriteLine(output);

            //connection.Dispose();


            IceDragon iceDragon = new IceDragon(200, "Winterrrer", 10, 20, 20, 50);
            GoldDragon goldDragon = new GoldDragon(220, "Sauman", 10, 20, 40, 40);
            DataManager mythicalManager = new DataManager("DESKTOP-HVUO0CP", "TestDB");
            List<Object> creatures = new List<Object> () { iceDragon, goldDragon };
            List<Object> wizards = new List<Object>() { new Wizard("Romas", 1, 10, 2.41) };
            mythicalManager.Inherit(creatures, 2);
            mythicalManager.Inherit(wizards, 1);
            Console.WriteLine("Utter success");
            Wizard gandalf = new Wizard("Gandalf", 2, 20, 2.31);
            //gandalf.dragons1.Add(iceDragon);
            //gandalf.dragons2.Add(iceDragon);
            gandalf.dragon1 = iceDragon;
            mythicalManager.CreateTable(gandalf);
            
            
            
            
        }
    }

    class Character
    {
        [Column()]
        string name { get; set; }

        public Character(string name)
        {
            this.name = name;
        }
    }

    class Wizard
    {
        [PKey()]
        [Column()]
        int id { get; set; }

        [Column()]
        int health { get; set; }

        [Column("magic_power")]
        double magicPower { get; set; }

        [OneToOne]
        public Dragon dragon1 {get; set;}

        // test for all these relationships, prepare new classes
        
        // [OneToMany()]
        // public List<Dragon> dragons1 { get; set; } = new List<Dragon>();

        // [ManyToMany()]
        // public List<Dragon> dragons2 { get; set; } = new List<Dragon>();

        public Wizard(string name, int id, int health, double magicPower)
        {
            this.health = health;
            this.id = id;
            this.magicPower = magicPower;
        }
    }

    class MythicalCreature
    {
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
        [PKey()]
        [Column()]
        int id { get; set; }

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
        [PKey()]
        [Column()]
        int id { get; set; }

        [Column("ice_capacity")]
        public int iceCapacity { get; set; }

        [Column("time_freeze")]
        double timeFreeze { get; set; }

        public IceDragon(int health, string name, int fireCapacity, int endurance, int iceCapacity, double timeFreeze)
            : base(health, name, fireCapacity, endurance)
        {
            this.iceCapacity = iceCapacity;
            this.timeFreeze = timeFreeze;
        }
    }
}
