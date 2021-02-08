using System;
using System.Collections.Generic;
using Design_Patterns_project.Attributes;
using Design_Patterns_project;

namespace ConcreteTableInheritanceTest
{
    class ConcreteTableInheritanceTest
    {
        public static void Main(string[] args)
        {
            // remote -> "den1.mssql7.gear.host", "DPTest", "dptest", "Me3JyhRLOg-_"

            // local 
            // maciopelo -> "DESKTOP-HVUO0CP", "TestDB"
            // szymon -> "LAPTOP-BHF7G1P9", "ConcreteTableInheritanceTest"

            DataManager vehicleManager = new DataManager("LAPTOP-BHF7G1P9", "ConcreteTableInheritanceTest");
            //DataManager vehicleManager = new DataManager("DESKTOP-HVUO0CP", "ConcreteTableInheritanceTest");

            FourWheeledVehicle car1 = new FourWheeledVehicle(250.21, 8.12);
            FourWheeledVehicle car2 = new FourWheeledVehicle(245.15, 7.91);
            FourWheeledVehicle car3 = new FourWheeledVehicle(234.59, 8.65);
            TwoWheeledVehicle motorbike1 = new TwoWheeledVehicle(154.15, 18.91);
            TwoWheeledVehicle motorbike2 = new TwoWheeledVehicle(166.67, 20.51);
            TwoWheeledVehicle motorbike3 = new TwoWheeledVehicle(165.51, 21.51);
            Vehicle truck1 = new Vehicle(105.51);
            Vehicle truck2 = new Vehicle(99.81);
            Vehicle truck3 = new Vehicle(101.74);
            Vehicle truck4 = new Vehicle(106.12);
            Vehicle truck5 = new Vehicle(113.95);
            Vehicle bike1 = new Vehicle(51.66);
            Vehicle bike2 = new Vehicle(48.91);
            List<Object> vehicles = new List<Object>() { car1, motorbike1 };

            //create and inherit
            vehicleManager.Inherit(vehicles, 2);

            //insert
            vehicleManager.Insert(car1);
            vehicleManager.Insert(car2);
            vehicleManager.Insert(car3);
            vehicleManager.Insert(motorbike1);
            vehicleManager.Insert(motorbike2);
            vehicleManager.Insert(motorbike3);
            vehicleManager.Insert(truck1);
            vehicleManager.Insert(truck2);
            vehicleManager.Insert(truck3);
            vehicleManager.Insert(truck4);
            vehicleManager.Insert(truck5);
            vehicleManager.Insert(bike1);
            vehicleManager.Insert(bike2);

            //update
            truck3.velocity = 109.25;
            car1.size = 8.92;
            vehicleManager.Update(truck3);
            vehicleManager.Update(car1);

            //delete
            vehicleManager.Delete(truck5);

            //Test for relation-object mapping
            FourWheeledVehicle newVehicle = (FourWheeledVehicle)vehicleManager.Select(typeof(FourWheeledVehicle), 1);

            Console.WriteLine("\nNew vehicle");
            Console.WriteLine("    velocity: {0}", newVehicle.velocity);
            Console.WriteLine("    size: {0}", newVehicle.size);

            Console.WriteLine("Utter success");
        }
    }

    [Table()]
    class Vehicle
    {
        [Column()]
        public double velocity { get; set; }

        public Vehicle(double velocity)
        {
            this.velocity = velocity;
        }
    }

    [Table()]
    class FourWheeledVehicle : Vehicle
    {
        [Column()]
        public double size { get; set; }

        public FourWheeledVehicle(double velocity, double size) : base(velocity)
        {
            this.size = size;
        }
    }

    [Table()]
    class TwoWheeledVehicle : Vehicle
    {
        [Column()]
        public double acceleration { get; set; }

        public TwoWheeledVehicle(double velocity, double acceleration) : base(velocity)
        {
            this.acceleration = acceleration;
        }
    }
}
