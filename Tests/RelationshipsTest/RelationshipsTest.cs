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

            DataManager mountainManager = new DataManager("LAPTOP-BHF7G1P9", "RelationshipsTest");
            //DataManager mountainManager = new DataManager("DESKTOP-HVUO0CP", "RelationshipsTest");

            Flea flea1 = new Flea("Jumper", 42.9);
            Flea flea2 = new Flea("Pavel", 12.19);
            Flea flea3 = new Flea("Pearly", 42.93);

            Bowl bowl = new Bowl("DogFood", 10);

            Dog testDog = new Dog("Barry", 6);
            Dog dog = new Dog("Burek", 10);

            testDog.AddFlea(flea1);
            testDog.AddFlea(flea2);
            testDog.AddFlea(flea3);

            testDog.SetBowl(bowl);

            Label label1 = new Label(234);
            //Label label2 = new Label(235);
            Label label3 = new Label(236);

            Sheep sheep1 = new Sheep("Mary", 2.35);
            sheep1.SetLabel(label1);

            Sheep sheep2 = new Sheep("Pole", 1.84);
            //sheep2.SetLabel(label2);

            Sheep sheep3 = new Sheep("Ann", 1.27);
            sheep3.SetLabel(label3);

            Alp alp1 = new Alp("Not so Silicon Valley", 5.61);
            //Alp alp2 = new Alp("Mountain Secret", 7.42);

            Shepherd testShepherd = new Shepherd("Frank");

            testShepherd.SetDog(testDog);

            testShepherd.AddSheep(sheep1);
            testShepherd.AddSheep(sheep2);
            testShepherd.AddSheep(sheep3);

            testShepherd.AddAlp(alp1);
            //testShepherd.AddAlp(alp2);

            //create
            mountainManager.CreateTable(testShepherd);
            mountainManager.Insert(testShepherd);

            testShepherd.SetDog(dog);
            mountainManager.Insert(dog);

            mountainManager.Update(dog);

            //update
            testShepherd.name = "Johnny";
            sheep2.name = "Madeleine";
            flea3.jumpLevel = 35.15;
            mountainManager.Update(testShepherd);
            mountainManager.Update(sheep2);
            mountainManager.Update(flea3);

            //delete
            mountainManager.Delete(flea3);
            mountainManager.Delete(dog);

            //Test for relation-object mapping
            Shepherd newShepherd = (Shepherd)mountainManager.Select(typeof(Shepherd), 1);

            IntroduceShepherd(newShepherd);
            Console.WriteLine("Utter success");
        }

        public static void IntroduceShepherd(Shepherd shepherd)
        {
            if (shepherd == null)
            {
                return;
            }

            Console.WriteLine("\nNew shepherd");
            Console.WriteLine("   name: {0}", shepherd.name);
            Console.WriteLine("   New dog");
            Console.WriteLine("      name: {0}", shepherd.dog.name);
            Console.WriteLine("      age: {0}", shepherd.dog.age);

            if (shepherd.dog.bowl != null)
            {
                Console.WriteLine("      New bowl");
                Console.WriteLine("         mark: {0}", shepherd.dog.bowl.mark);
                Console.WriteLine("         size: {0}", shepherd.dog.bowl.size);
            }
            
            if (shepherd.dog.fleas != null)
            {
                Console.WriteLine("      New fleas");

                foreach (Flea flea in shepherd.dog.fleas)
                {
                    Console.WriteLine("      flea");
                    Console.WriteLine("         nick: {0}", flea.nick);
                    Console.WriteLine("         jump level: {0}", flea.jumpLevel);
                }
            }

            Console.WriteLine("   New sheep");

            foreach (Sheep sheep in shepherd.sheep)
            {
                Console.WriteLine("   sheep");
                Console.WriteLine("      name: {0}", sheep.name);
                Console.WriteLine("      wool quality: {0}", sheep.woolQuality);

                if (sheep.label != null)
                {
                    Console.WriteLine("      New label");

                    Console.WriteLine("         nr: {0}", sheep.label.nr);
                }
            }

            Console.WriteLine("   New alps");

            foreach (Alp alp in shepherd.alps)
            {
                Console.WriteLine("   alp");
                Console.WriteLine("      name: {0}", alp.name);
                Console.WriteLine("      area: {0}", alp.area);
            }

            Console.WriteLine();
        }
    }

    [Table("shepherds")]
    class Shepherd
    {
        [Column("name")]
        public string name { get; set; }

        [OneToOne]
        public Dog dog { get; set; }

        [OneToMany]
        public List<Sheep> sheep { get; set; } = new List<Sheep>();

        [ManyToMany]
        public List<Alp> alps { get; set; } = new List<Alp>();

        public Shepherd( string name)
        {
            this.name = name;
        }

        public void SetDog(Dog dog)
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
        [Column("name")]
        public string name { get; set; }

        [Column("age")]
        public double age { get; set; }

        [OneToOne]
        public Bowl bowl { get; set; }

        [OneToMany]
        public List<Flea> fleas { get; set; } = new List<Flea>();

        public void AddFlea(Flea flea)
        {
            this.fleas.Add(flea);
        }

        public void SetBowl(Bowl bowl)
        {
            this.bowl = bowl;
        }

        public Dog(string name, double age)
        {
            this.name = name;
            this.age = age;
        }
    }

    [Table("fleas")]
    class Flea
    {
        [Column("nickname")]
        public string nick { get; set; }

        [Column("jump_level")]
        public double jumpLevel { get; set; }

        public Flea(string nick, double jumpLevel)
        {
            this.nick = nick;
            this.jumpLevel = jumpLevel;
        }
    }


    [Table("bowls")]
    class Bowl
    {
        [Column("mark")]
        public string mark { get; set; }

        [Column("size")]
        public int size { get; set; }

        public Bowl(string mark, int size)
        {
            this.mark = mark;
            this.size = size;
        }
    }


    [Table("sheep")]
    class Sheep
    {
        [Column("name")]
        public string name { get; set; }

        [Column("wool_quality")]
        public double woolQuality { get; set; }

        [OneToOne]
        public Label label { get; set; }

        public void SetLabel(Label label)
        {
            this.label = label;
        }

        public Sheep(string name, double woolQuality)
        {
            this.name = name;
            this.woolQuality = woolQuality;
        }
    }

    [Table("labels")]
    class Label
    {
        [Column("num")]
        public int nr { get; set; }

        public Label(int nr)
        {
            this.nr = nr;
        }
    }

    [Table("alps")]
    class Alp
    {
        [Column("name")]
        public string name { get; set; }

        [Column("area")]
        public double area { get; set; }

        public Alp(string name, double area)
        {
            this.name = name;
            this.area = area;
        }
    }
}
