using System;
using System.Collections.Generic;
using Design_Patterns_project.Attributes;
using Design_Patterns_project;

namespace ClassTableInheritanceTest
{
    class ClassTableInheritanceTest
    {
        public static void Main(string[] args)
        {
            // remote -> "den1.mssql7.gear.host", "DPTest", "dptest", "Me3JyhRLOg-_"

            // local 
            // maciopelo -> "DESKTOP-HVUO0CP", "TestDB"
            // szymon -> "LAPTOP-BHF7G1P9", "ClassTableInheritanceTest"

            // DataManager wizardManager = new DataManager("LAPTOP-BHF7G1P9", "TestDB");
            DataManager wizardManager = new DataManager("DESKTOP-HVUO0CP", "TestDB");

            ArchMage archMage1 = new ArchMage(1, "Gandalf", 1, 2.32, 1, 106);
            ArchMage archMage2 = new ArchMage(1, "Vilgefortz", 1, 2.29, 2, 104);
            DarkMage darkMage1 = new DarkMage(1, "Saruman", 1, 2.26, 1, 89);
            DarkMage darkMage2 = new DarkMage(1, "Dark Wizard", 1, 1.73, 2, 70);
            Mage mage1 = new Mage(1, "Radagast", 1, 67);
            Mage mage2 = new Mage(1, "Merlin", 2, 86);
            Character character1 = new Character(1, "Bilbo Baggins");
            Character character2 = new Character(2, "Dijkstra");
            Character character3 = new Character(3, "King Arthur");
            List<Object> mages = new List<Object>() { archMage1, darkMage1 };

            wizardManager.Inherit(mages, 1);

            wizardManager.Insert(archMage1);
            wizardManager.Insert(archMage2);
            wizardManager.Insert(darkMage1);
            wizardManager.Insert(darkMage2);
            wizardManager.Insert(mage1);
            wizardManager.Insert(mage2);
            wizardManager.Insert(character1);
            wizardManager.Insert(character2);
            wizardManager.Insert(character3);

            wizardManager.Delete(archMage2);
            List<SqlCondition> conditions = new List<SqlCondition> { SqlCondition.LowerThan("id", 2) };
            wizardManager.Delete("darkMage", conditions);

            Console.WriteLine("Utter success");
        }
    }

    class Character
    {
        [PKey()]
        [Column()]
        int id { get; set; }

        [Column()]
        string name { get; set; }

        public Character(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }

    class Mage : Character
    {
        [PKey()]
        [Column()]
        int mageID { get; set; }

        [Column("spell_skills")]
        double spell_skills { get; set; }

        public Mage(int id, string name, int mageID, double spell_skills) : base(id, name)
        {
            this.mageID = mageID;
            this.spell_skills = spell_skills;
        }
    }

    class ArchMage : Mage
    {
        [PKey()]
        [Column("id")]
        int archMageID { get; set; }

        [Column("years_of_experience")]
        int yearsOfExperience { get; set; }

        public ArchMage(int id, string name, int mageID, double spell_skills, int archMageID, int yearsOfExperience)
            : base(id, name, mageID, spell_skills)
        {
            this.archMageID = archMageID;
            this.yearsOfExperience = yearsOfExperience;
        }
    }

    class DarkMage : Mage
    {
        [PKey()]
        [Column("id")]
        int darkMageID { get; set; }

        [Column()]
        double necromancy { get; set; }

        public DarkMage(int id, string name, int mageID, double spell_skills, int darkMageID, double necromancy) 
            : base(id, name, mageID, spell_skills)
        {
            this.darkMageID = darkMageID;
            this.necromancy = necromancy;
        }
    }
}
