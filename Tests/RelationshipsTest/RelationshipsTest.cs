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
            // szymon -> "LAPTOP-BHF7G1P9", "RelationshipsTest" databases (like tests directories)
            // Blacki7 - > "DESKTOP-BO1NL9H", "test1"  (work in progress)

            //DataManager mountainManager = new DataManager("LAPTOP-BHF7G1P9", "RelationshipsTest");
            DataManager mountainManager = new DataManager("DESKTOP-HVUO0CP", "RelationshipsTest");

            Flea flea1 = new Flea(1, "Skoczuszka", 42.9);
            Flea flea2 = new Flea(2, "Sokolica", 12.19);
            Flea flea3 = new Flea(3, "Perelka", 42.93);

            Bowl bowl = new Bowl(1, "DogFood", 10);

            Dog testDog = new Dog(1, "Burek", 6, bowl);

            testDog.AddFlea(flea1);
            testDog.AddFlea(flea2);
            testDog.AddFlea(flea3);

            Label label1 = new Label(1, 234);
            Label label2 = new Label(2, 235);
            Label label3 = new Label(3, 236);

            Sheep sheep1 = new Sheep(1, "Marcysia", 2.35, label1);
            Sheep sheep2 = new Sheep(2, "Pola", 1.84, label2);
            Sheep sheep3 = new Sheep(3, "Hania", 1.27, label3);

            Alp alp1 = new Alp(1, "Rozlegla dolina", 5.61);
            Alp alp2 = new Alp(2, "Gorska tajemnica", 7.42);

            Shepherd testShepherd = new Shepherd(1, "Franek", testDog);

            testShepherd.AddSheep(sheep1);
            testShepherd.AddSheep(sheep2);
            testShepherd.AddSheep(sheep3);

            testShepherd.AddAlp(alp1);
            testShepherd.AddAlp(alp2);

            // mountainManager.CreateTable(testShepherd);
            // mountainManager.Insert(testShepherd);

            // mountainManager.Delete(flea3);
            // List<SqlCondition> conditions = new List<SqlCondition> { SqlCondition.LowerThan("nr_owcy", 236) };
            // mountainManager.Delete("znacznik", conditions);

            List<Tuple<string, object>> valuesToSet = new List<Tuple<string, object>> {new Tuple<string,object>("jakosc_welny",1.5)};
            List<SqlCondition> conditions = new List<SqlCondition> {SqlCondition.GreaterThan("jakosc_welny",1.25)};
            mountainManager.Update(typeof(Sheep),valuesToSet,conditions);

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

        [OneToOne]
        Bowl bowl { get; set; }

        [OneToMany]
        List<Flea> fleas { get; set; } = new List<Flea>();

        public void AddFlea(Flea flea)
        {
            this.fleas.Add(flea);
        }

        public Dog(int id, string name, double age, Bowl bowl)
        {
            this.id = id;
            this.name = name;
            this.age = age;
            this.bowl = bowl;
        }
    }

    [Table("pchla")]
    class Flea
    {
        [PKey()]
        [Column("identyfikator")]
        int id { get; set; }

        [Column("pseudonim")]
        string nick { get; set; }

        [Column("skocznosc")]
        double jumpLevel { get; set; }

        public Flea(int id, string name, double age)
        {
            this.id = id;
            this.nick = name;
            this.jumpLevel = age;
        }

    }


    [Table("miska")]
    class Bowl
    {
        [PKey()]
        [Column("identyfikator")]
        int id { get; set; }

        [Column("marka")]
        string mark { get; set; }

        [Column("wielkosc")]
        int size { get; set; }

        public Bowl(int id, string mark, int size)
        {
            this.id = id;
            this.mark = mark;
            this.size = size;
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

        [OneToOne]
        Label label { get; set; }


        public Sheep(int id, string name, double woolQuality, Label label)
        {
            this.id = id;
            this.name = name;
            this.woolQuality = woolQuality;
            this.label = label;
        }
    }

    [Table("znacznik")]
    class Label
    {

        [PKey()]
        [Column("identyfikator")]
        int id { get; set; }

        [Column("nr_owcy")]
        int nr { get; set; }
        public Label(int id, int nr)
        {
            this.id = id;
            this.nr = nr;
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
