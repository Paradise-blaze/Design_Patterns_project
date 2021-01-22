using System;
using Design_Patterns_project.Attributes;
using System.Collections.Generic;
using Design_Patterns_project;

namespace RelationshipsTest
{
    class RelationshipsTest
    {
        public static void Main(string[] args)
        {
            // remote -> "den1.mssql7.gear.host", "DPTest", "dptest", "Me3JyhRLOg-_"

            // local 
            // maciopelo -> "DESKTOP-HVUO0CP", "TestDB"
            // szymon -> "LAPTOP-BHF7G1P9", "Test" databases (like tests directories)

            DataManager mountainManager = new DataManager("LAPTOP-BHF7G1P9", "RelationshipsTest");

            Dog testDog = new Dog(1, "Burek", 6);
            Sheep sheep1 = new Sheep(1, "Marcysia", 2.35);
            Sheep sheep2 = new Sheep(2, "Pola", 1.84);
            Sheep sheep3 = new Sheep(3, "Hania", 1.27);
            Alp alp1 = new Alp(1, "Rozlegla dolina", 5.61);
            Alp alp2 = new Alp(2, "Gorska tajemnica", 7.42);
            Shepherd testShepherd = new Shepherd(1, "Franek", testDog);

            testShepherd.AddSheep(sheep1);
            testShepherd.AddSheep(sheep2);
            testShepherd.AddSheep(sheep3);

            testShepherd.AddAlp(alp1);
            testShepherd.AddAlp(alp2);

            mountainManager.CreateTable(testShepherd);

            Console.WriteLine("Utter success");
        }
    }

    [Table("pasterz")]
    class Shepherd
    {
        [PKey()]
        [Column("identyfikator")]
        int id { get; set; }

        [Column("imie")]
        string name { get; set; }

        [OneToOne]
        Dog dog { get; set; }

        [OneToMany]
        List<Sheep> sheep { get; set; } = new List<Sheep>();

        [ManyToMany]
        List<Alp> alps { get; set; } = new List<Alp>();

        public Shepherd(int id, string name, Dog dog)
        {
            this.id = id;
            this.name = name;
            this.dog = dog;
        }

        public void AddSheep(Sheep newSheep)
        {
            this.sheep.Add(newSheep);
        }

        public void AddAlp(Alp newAlp)
        {
            this.alps.Add(newAlp);
        }
    }

    [Table("pies")]
    class Dog
    {
        [PKey()]
        [Column("identyfikator")]
        int id { get; set; }

        [Column("imie")]
        string name { get; set; }

        [Column("wiek")]
        double age { get; set; }

        public Dog(int id, string name, double age)
        {
            this.id = id;
            this.name = name;
            this.age = age;
        }
    }

    [Table("owca")]
    class Sheep
    {
        [PKey()]
        [Column("identyfikator")]
        int id { get; set; }

        [Column("imie")]
        string name { get; set; }

        [Column("jakosc_welny")]
        double woolQuality { get; set; }

        public Sheep(int id, string name, double woolQuality)
        {
            this.id = id;
            this.name = name;
            this.woolQuality = woolQuality;
        }
    }

    [Table("hala_gorska")]
    class Alp
    {
        [PKey()]
        [Column("identyfikator")]
        int id { get; set; }

        [Column("nazwa")]
        string name { get; set; }

        [Column("powierzchnia")]
        double area { get; set; }

        public Alp(int id, string name, double area)
        {
            this.id = id;
            this.name = name;
            this.area = area;
        }
    }
}
