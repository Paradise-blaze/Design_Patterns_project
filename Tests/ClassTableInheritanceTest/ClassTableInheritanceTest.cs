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
            //DataManager wizardManager = new DataManager("DESKTOP-HVUO0CP", "ClassTableInheritanceTest");

            ArchMage archMage1 = new ArchMage("Gandalf", 2.32, 106);
            ArchMage archMage2 = new ArchMage("Vilgefortz", 2.29, 104);
            ArchMage archMage3 = new ArchMage("Blacki", 2.61, 111);
            ArchMage archMage4 = new ArchMage("Detmold", 2.05, 91);
            DarkMage darkMage1 = new DarkMage("Saruman", 2.26, 89);
            DarkMage darkMage2 = new DarkMage("Dark Wizard", 1.73, 70);
            Mage mage1 = new Mage("Radagast", 67);
            Mage mage2 = new Mage("Merlin", 86);
            Character character1 = new Character("Bilbo Baggins");
            Character character2 = new Character("Dijkstra");
            Character character3 = new Character("King Arthur");
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

            //update
            character2.name = "Sigi Reuven";
            archMage1.yearsOfExperience = 110;
            wizardManager.Update(character2);
            wizardManager.Update(archMage1);

            //delete
            wizardManager.Delete(archMage2);

            Console.WriteLine("Utter success");
        }
    }

    [Table()]
    class Character
    {
        [Column()]
        public string name { get; set; }

        public Character(string name)
        {
            this.name = name;
        }
    }

    [Table()]
    class Mage : Character
    {
        [Column("spell_skills")]
        public double spellSkills { get; set; }

        public Mage(string name, double spellSkills) : base(name)
        {
            this.spellSkills = spellSkills;
        }
    }

    [Table()]
    class ArchMage : Mage
    {
        [Column("years_of_experience")]
        public int yearsOfExperience { get; set; }

        public ArchMage(string name, double spellSkills, int yearsOfExperience)
            : base(name, spellSkills)
        {
            this.yearsOfExperience = yearsOfExperience;
        }
    }

    [Table()]
    class DarkMage : Mage
    {
        [Column()]
        public double necromancy { get; set; }

        public DarkMage(string name, double spellSkills, double necromancy) 
            : base(name, spellSkills)
        {
            this.necromancy = necromancy;
        }
    }
}
