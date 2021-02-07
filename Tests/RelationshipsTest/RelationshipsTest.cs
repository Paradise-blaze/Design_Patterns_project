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

            Dog testDog = new Dog(1, "Burek", 6);
            
            testDog.AddFlea(flea1);
            testDog.AddFlea(flea2);
            testDog.AddFlea(flea3);

            testDog.AddBowl(bowl);

            Label label1 = new Label(1, 234);
            Label label2 = new Label(2, 235);
            Label label3 = new Label(3, 236);

            Sheep sheep1 = new Sheep(1, "Marcysia", 2.35);
            sheep1.AddLabel(label1);

            Sheep sheep2 = new Sheep(2, "Pola", 1.84);
            sheep2.AddLabel(label2);

            Sheep sheep3 = new Sheep(3, "Hania", 1.27);
            sheep3.AddLabel(label3);

            Alp alp1 = new Alp(1, "Rozlegla dolina", 5.61);
            Alp alp2 = new Alp(2, "Gorska tajemnica", 7.42);

            Shepherd testShepherd = new Shepherd(1, "Franek");
            testShepherd.AddDog(testDog);

            testShepherd.AddSheep(sheep1);
            testShepherd.AddSheep(sheep2);
            testShepherd.AddSheep(sheep3);

            testShepherd.AddAlp(alp1);
            testShepherd.AddAlp(alp2);

            //create
            mountainManager.CreateTable(testShepherd);
            mountainManager.Insert(testShepherd);

            // //select
            // List<SqlCondition> selectConditions1 = new List<SqlCondition> { SqlCondition.LowerThan("id", 11) };
            // string select1 = mountainManager.SelectAsString(typeof(Sheep), selectConditions1);
            // Console.WriteLine('\n' + select1 + '\n');

            // List<SqlCondition> selectConditions2 = new List<SqlCondition> { SqlCondition.LowerThan("id", 11) };
            // string select2 = mountainManager.SelectAsString(typeof(Flea), selectConditions2);
            // Console.WriteLine('\n' + select2 + '\n');

            // //delete
            // mountainManager.Delete(flea3);
            // List<SqlCondition> deleteConditions = new List<SqlCondition> { SqlCondition.LowerThan("nr_owcy", 236) };
            // mountainManager.Delete("znacznik", deleteConditions);

            // //select
            // select1 = mountainManager.SelectAsString(typeof(Sheep), selectConditions1);
            // Console.WriteLine('\n' + select1 + '\n');

            // select2 = mountainManager.SelectAsString(typeof(Flea), selectConditions2);
            // Console.WriteLine('\n' + select2 + '\n');

            // //update
            // List<Tuple<string, object>> valuesToSet = new List<Tuple<string, object>> {new Tuple<string,object>("jakosc_welny",1.5)};
            // List<SqlCondition> updateConditions = new List<SqlCondition> {SqlCondition.GreaterThan("jakosc_welny",1.25)};
            // mountainManager.Update(typeof(Sheep), valuesToSet, updateConditions);

            // //select
            // select1 = mountainManager.SelectAsString(typeof(Sheep), selectConditions1);
            // Console.WriteLine('\n' + select1 + '\n');

            // select2 = mountainManager.SelectAsString(typeof(Flea), selectConditions2);
            // Console.WriteLine('\n' + select2 + '\n');

            // //Test for relation-object mapping
            // List<SqlCondition> selectShepherdConditions = new List<SqlCondition> { SqlCondition.Equals("identyfikator", 1) };
            // List<Object> objects = mountainManager.Select(typeof(Shepherd), selectShepherdConditions);
            // Shepherd newShepherd = (Shepherd)objects[0];

            // Console.WriteLine("New shepherd");
            // Console.WriteLine("   name: {0}", newShepherd.GetName());
            // Console.WriteLine("   id: {0}", newShepherd.GetId());
            // Console.WriteLine("   New dog");
            // Console.WriteLine("      id: {0}", newShepherd.GetDog().GetId());
            // Console.WriteLine("      name: {0}", newShepherd.GetDog().GetName());
            // Console.WriteLine("      age: {0}", newShepherd.GetDog().GetAge());
            // Console.WriteLine("      New bowl");
            // Console.WriteLine("         id: {0}", newShepherd.GetDog().GetBowl().GetId());
            // Console.WriteLine("         mark: {0}", newShepherd.GetDog().GetBowl().GetMark());
            // Console.WriteLine("         size: {0}", newShepherd.GetDog().GetBowl().GetSize());

            // Console.WriteLine("      New fleas");

            // foreach (Flea flea in newShepherd.GetDog().GetFleas())
            // {
            //     Console.WriteLine("      flea");
            //     Console.WriteLine("         id: {0}", flea.GetId());
            //     Console.WriteLine("         nick: {0}", flea.GetNick());
            //     Console.WriteLine("         jump level: {0}", flea.GetJumpLevel());
            // }

            // Console.WriteLine("   New sheep");

            // foreach(Sheep sheep in newShepherd.GetSheep())
            // {
            //     Console.WriteLine("   sheep");
            //     Console.WriteLine("      id: {0}", sheep.GetId());
            //     Console.WriteLine("      name: {0}", sheep.GetName());
            //     Console.WriteLine("      wool quality: {0}", sheep.GetWoolQuality());

            //     if (sheep.GetLabel() != null)
            //     {
            //         Console.WriteLine("      New label");

            //         Console.WriteLine("         id: {0}", sheep.GetLabel().GetId());
            //         Console.WriteLine("         nr: {0}", sheep.GetLabel().GetNr());
            //     }
            // }

            // Console.WriteLine("   New alps");

            // foreach (Alp alp in newShepherd.GetAlps())
            // {
            //     Console.WriteLine("   alp");
            //     Console.WriteLine("      id: {0}", alp.GetId());
            //     Console.WriteLine("      name: {0}", alp.GetName());
            //     Console.WriteLine("      area: {0}", alp.GetArea());
            // }

            // Console.WriteLine();
            Console.WriteLine("Utter success");
        }
    }

    [Table("shepherds")]
    class Shepherd
    {
        [PKey()]
        [Column("id")]
        int id { get; set; }

        [Column("name")]
        string name { get; set; }

        [OneToOne]
        Dog dog { get; set; }

        [OneToMany]
        List<Sheep> sheep { get; set; } = new List<Sheep>();

        [ManyToMany]
        List<Alp> alps { get; set; } = new List<Alp>();

        public int GetId()
        {
            return this.id;
        }

        public string GetName()
        {
            return this.name;
        }

        public Dog GetDog()
        {
            return this.dog;
        }

        public List<Sheep> GetSheep()
        {
            return this.sheep;
        }

        public List<Alp> GetAlps()
        {
            return this.alps;
        }

        public Shepherd(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public void AddDog(Dog dog)
        {
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

    [Table("dogs")]
    class Dog
    {
        [PKey()]
        [Column("id")]
        int id { get; set; }

        [Column("name")]
        string name { get; set; }

        [Column("age")]
        double age { get; set; }

        [OneToOne]
        Bowl bowl { get; set; }

        [OneToMany]
        List<Flea> fleas { get; set; } = new List<Flea>();

        public void AddFlea(Flea flea)
        {
            this.fleas.Add(flea);
        }

        public void AddBowl(Bowl bowl)
        {
            this.bowl = bowl;
        }

        public Dog(int id, string name, double age)
        {
            this.id = id;
            this.name = name;
            this.age = age;
        }

        public int GetId()
        {
            return this.id;
        }
        public string GetName()
        {
            return this.name;
        }

        public double GetAge()
        {
            return this.age;
        }

        public Bowl GetBowl()
        {
            return this.bowl;
        }

        public List<Flea> GetFleas()
        {
            return this.fleas;
        }
    }

    [Table("fleas")]
    class Flea
    {
        [PKey()]
        [Column()]
        int id { get; set; }

        [Column("nickname")]
        string nick { get; set; }

        [Column("jump_level")]
        double jumpLevel { get; set; }

        public Flea(int id, string nick, double jumpLevel)
        {
            this.id = id;
            this.nick = nick;
            this.jumpLevel = jumpLevel;
        }

        public int GetId()
        {
            return this.id;
        }

        public string GetNick()
        {
            return this.nick;
        }

        public double GetJumpLevel()
        {
            return this.jumpLevel;
        }
    }


    [Table("bowls")]
    class Bowl
    {
        [PKey()]
        [Column("id")]
        int id { get; set; }

        [Column("mark")]
        string mark { get; set; }

        [Column("size")]
        int size { get; set; }

        public Bowl(int id, string mark, int size)
        {
            this.id = id;
            this.mark = mark;
            this.size = size;
        }

        public int GetId()
        {
            return this.id;
        }

        public string GetMark()
        {
            return this.mark;
        }

        public int GetSize()
        {
            return this.size;
        }
    }


    [Table("sheep")]
    class Sheep
    {
        [PKey()]
        [Column()]
        int id { get; set; }

        [Column("name")]
        string name { get; set; }

        [Column("wool_quality")]
        double woolQuality { get; set; }

        [OneToOne]
        Label label { get; set; }

        public void AddLabel(Label label)
        {
            this.label = label;
        }

        public Sheep(int id, string name, double woolQuality)
        {
            this.id = id;
            this.name = name;
            this.woolQuality = woolQuality;
        }

        public int GetId()
        {
            return this.id;
        }

        public string GetName()
        {
            return this.name;
        }

        public double GetWoolQuality()
        {
            return this.woolQuality;
        }

        public Label GetLabel()
        {
            return this.label;
        }
    }

    [Table("labels")]
    class Label
    {

        [PKey()]
        [Column("id")]
        int id { get; set; }

        [Column("num")]
        int nr { get; set; }

        public Label(int id, int nr)
        {
            this.id = id;
            this.nr = nr;
        }

        public int GetId()
        {
            return this.id;
        }
        public int GetNr()
        {
            return this.nr;
        }
    }

    [Table("alps")]
    class Alp
    {
        [PKey()]
        [Column("id")]
        int id { get; set; }

        [Column("name")]
        string name { get; set; }

        [Column("area")]
        double area { get; set; }

        public Alp(int id, string name, double area)
        {
            this.id = id;
            this.name = name;
            this.area = area;
        }

        public int GetId()
        {
            return this.id;
        }
        public string GetName()
        {
            return this.name;
        }

        public double GetArea()
        {
            return this.area;
        }
    }
}
