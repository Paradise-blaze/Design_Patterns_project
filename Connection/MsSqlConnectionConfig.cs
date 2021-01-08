using System;


namespace Design_Patterns_project.Connection
{
    class MsSqlConnectionConfig
    {
        public string serverName {get;}
        private string databaseName;
        private string user;
        private string password;


        public MsSqlConnectionConfig(string serverName, string databaseName){
            this.serverName = serverName;
            this.databaseName = databaseName;
        }

        public MsSqlConnectionConfig(string serverName, string databaseName, string user, string password ){
            this.serverName = serverName;
            this.databaseName = databaseName;
            this.user = user;
            this.password = password;
        }

        public string CreateConnectionString(){
            string connectionString = this.user == null ? @"Data Source="+this.serverName+";Initial Catalog="+this.databaseName+"; Integrated Security=True;" :
            "Server=" + this.serverName + ";Database=" + this.databaseName + ";User Id=" +this.user + ";Password=" + this.password + ";MultipleActiveResultSets=true;";
            return connectionString;
        }

        
        

    }
}