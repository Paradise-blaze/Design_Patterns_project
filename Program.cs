using System;
using Design_Patterns_project.Connection;


namespace Design_Patterns_project
{
    class Program
    {
        static void Main(string[] args)
        {

            //remote -> "den1.mssql7.gear.host", "DPTest", "dptest", "Me3JyhRLOg-_"
            //local -> "DESKTOP-HVUO0CP", "TestDB"

            MsSqlConnectionConfig remoteConfig = new MsSqlConnectionConfig("den1.mssql7.gear.host", "DPTest", "dptest", "Me3JyhRLOg-_");
            MsSqlConnectionConfig localConfig = new MsSqlConnectionConfig("DESKTOP-HVUO0CP", "TestDB");
            MsSqlConnection connection = new MsSqlConnection(localConfig);
            connection.ConnectAndOpen();
            string query = "SELECT * FROM dbo.Players;";
            string output = connection.ExecuteSelectCommand(query);
            Console.WriteLine(output);

        }
    }
}
