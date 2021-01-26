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

            FourWheeledVehicle car1 = new FourWheeledVehicle(1, 250.21, 8.12);
            FourWheeledVehicle car2 = new FourWheeledVehicle(2, 245.15, 7.91);
            FourWheeledVehicle car3 = new FourWheeledVehicle(3, 234.59, 8.65);
            TwoWheeledVehicle motorbike1 = new TwoWheeledVehicle(1, 154.15, 18.91);
            TwoWheeledVehicle motorbike2 = new TwoWheeledVehicle(2, 166.67, 20.51);
            TwoWheeledVehicle motorbike3 = new TwoWheeledVehicle(3, 165.51, 21.51);
            Vehicle truck1 = new Vehicle(1, 105.51);
            Vehicle truck2 = new Vehicle(2, 99.81);
            Vehicle truck3 = new Vehicle(3, 101.74);
            Vehicle truck4 = new Vehicle(4, 106.12);
            Vehicle truck5 = new Vehicle(5, 113.95);
            Vehicle bike1 = new Vehicle(6, 51.66);
            Vehicle bike2 = new Vehicle(7, 48.91);
            List<Object> vehicles = new List<Object>() { car1, motorbike1 };
            vehicleManager.Inherit(vehicles, 2);
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

            List<SqlCondition> conditions = new List<SqlCondition> { SqlCondition.GreaterThan("size", 8) };
            vehicleManager.Delete("FourWheeledVehicle", conditions);
            vehicleManager.Delete(motorbike2);

            Console.WriteLine("Utter success");
        }
    }

    class Vehicle
    {
        [PKey()]
        [Column()]
        int id { get; set; }

        [Column()]
        double velocity { get; set; }

        public Vehicle(int id, double velocity)
        {
            this.id = id;
            this.velocity = velocity;
        }
    }

    class FourWheeledVehicle : Vehicle
    {
        [Column()]
        double size { get; set; }

        public FourWheeledVehicle(int id, double velocity, double size) : base(id, velocity)
        {
            this.size = size;
        }
    }

    class TwoWheeledVehicle : Vehicle
    {
        [Column()]
        double acceleration { get; set; }

        public TwoWheeledVehicle(int id, double velocity, double acceleration) : base(id, velocity)
        {
            this.acceleration = acceleration;
        }
    }
}
