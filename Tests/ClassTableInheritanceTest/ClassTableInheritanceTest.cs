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
            // szymon -> "LAPTOP-BHF7G1P9", "Test" databases (like tests directories)

            DataManager wizardManager = new DataManager("LAPTOP-BHF7G1P9", "ClassTableInheritanceTest");

            ArchMage archMage = new ArchMage(1, "Gandalf", 1, 2.32, 1, 106);
            DarkMage darkMage = new DarkMage(2, "Saruman", 2, 2.26, 89);
            List<Object> mages = new List<Object>() { archMage, darkMage };
            wizardManager.Inherit(mages, 1);

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
            this.archMageID = mageID;
            this.yearsOfExperience = yearsOfExperience;
        }
    }

    class DarkMage : Mage
    {
        [Column()]
        double necromancy { get; set; }

        public DarkMage(int id, string name, int mageID, double spell_skills, double necromancy) : base(id, name, mageID, spell_skills)
        {
            this.necromancy = necromancy;
        }
    }
}
