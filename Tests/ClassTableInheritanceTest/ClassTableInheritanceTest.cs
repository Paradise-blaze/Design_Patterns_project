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

            DataManager wizardManager = new DataManager("LAPTOP-BHF7G1P9", "ClassTableInheritanceTest");
            //DataManager wizardManager = new DataManager("DESKTOP-HVUO0CP", "TestDB");

            ArchMage archMage1 = new ArchMage(1, "Gandalf", 1, 2.32, 1, 106);
            ArchMage archMage2 = new ArchMage(1, "Vilgefortz", 1, 2.29, 2, 104);
            ArchMage archMage3 = new ArchMage(1, "Blacki", 1, 2.61, 3, 111);
            ArchMage archMage4 = new ArchMage(1, "Detmold", 1, 2.05, 4, 91);
            DarkMage darkMage1 = new DarkMage(1, "Saruman", 1, 2.26, 1, 89);
            DarkMage darkMage2 = new DarkMage(1, "Dark Wizard", 1, 1.73, 2, 70);
            Mage mage1 = new Mage(1, "Radagast", 1, 67);
            Mage mage2 = new Mage(1, "Merlin", 2, 86);
            Character character1 = new Character(1, "Bilbo Baggins");
            Character character2 = new Character(2, "Dijkstra");
            Character character3 = new Character(3, "King Arthur");
            List<Object> mages = new List<Object>() { archMage1, darkMage1 };

            //create and inherit
            wizardManager.Inherit(mages, 1);

            //insert
            wizardManager.Insert(archMage1);
            wizardManager.Insert(archMage2);
            wizardManager.Insert(archMage3);
            wizardManager.Insert(archMage4);
            wizardManager.Insert(darkMage1);
            wizardManager.Insert(darkMage2);
            wizardManager.Insert(mage1);
            wizardManager.Insert(mage2);
            wizardManager.Insert(character1);
            wizardManager.Insert(character2);
            wizardManager.Insert(character3);

            //select
            List<SqlCondition> selectConditions = new List<SqlCondition> { SqlCondition.LowerThan("id", 6) };
            string select = wizardManager.Select(typeof(ArchMage), selectConditions);
            Console.WriteLine('\n' + select + '\n');

            //delete
            wizardManager.Delete(archMage2);
            List<SqlCondition> conditions = new List<SqlCondition> { SqlCondition.LowerThan("id", 2) };
            wizardManager.Delete("darkMage", conditions);

            //select
            select = wizardManager.Select(typeof(ArchMage), selectConditions);
            Console.WriteLine('\n' + select + '\n');

            //update
            List<Tuple<string, Object>> valuesToSet = new List<Tuple<string, object>> { new Tuple<string, Object>("years_of_experience", 99) };
            List<SqlCondition> updateConditions = new List<SqlCondition> { SqlCondition.Equals("id", 3) };
            wizardManager.Update(typeof(ArchMage), valuesToSet, updateConditions);

            //select
            select = wizardManager.Select(typeof(ArchMage), selectConditions);
            Console.WriteLine('\n'+select+'\n');

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
        double spellSkills { get; set; }

        public Mage(int id, string name, int mageID, double spellSkills) : base(id, name)
        {
            this.mageID = mageID;
            this.spellSkills = spellSkills;
        }
    }

    class ArchMage : Mage
    {
        [PKey()]
        [Column("id")]
        int archMageID { get; set; }

        [Column("years_of_experience")]
        int yearsOfExperience { get; set; }

        public ArchMage(int id, string name, int mageID, double spellSkills, int archMageID, int yearsOfExperience)
            : base(id, name, mageID, spellSkills)
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

        public DarkMage(int id, string name, int mageID, double spellSkills, int darkMageID, double necromancy) 
            : base(id, name, mageID, spellSkills)
        {
            this.darkMageID = darkMageID;
            this.necromancy = necromancy;
        }
    }
}
