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

            MsSqlConnection connection = new MsSqlConnection(remoteConfig);
            connection.ConnectAndOpen();

            string testSelectQuery = "SELECT * FROM dbo.Players;";
            string testInsertQuery = "INSERT INTO dbo.Players (PlayerID,Name,Surname,Age,Nick) VALUES (4,'Gerard','Pique',33,'Lion');";
            string testDeleteQuery = "DELETE FROM dbo.Players WHERE PlayerID = 4;";

            string output = connection.ExecuteSelectQuery(testSelectQuery);
            Console.WriteLine(output);

            connection.ExecuteQuery(testInsertQuery);
            output = connection.ExecuteSelectQuery(testSelectQuery);
            Console.WriteLine(output);

            connection.ExecuteQuery(testDeleteQuery);
            output = connection.ExecuteSelectQuery(testSelectQuery);
            Console.WriteLine(output);

            connection.Dispose();

        }
    }
}
