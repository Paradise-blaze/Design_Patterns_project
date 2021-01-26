﻿using System;
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

            FourWheeledVehicle car = new FourWheeledVehicle(1, 250.21, 8.12);
            TwoWheeledVehicle motorbike = new TwoWheeledVehicle(1, 166.67, 20.51);
            Vehicle truck = new Vehicle(1, 105.51);
            Vehicle bike = new Vehicle(2, 51.66);
            List<Object> vehicles = new List<Object>() { car, motorbike };
            vehicleManager.Inherit(vehicles, 2);
            vehicleManager.Insert(car);
            vehicleManager.Insert(motorbike);
            vehicleManager.Insert(truck);
            vehicleManager.Insert(bike);

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
