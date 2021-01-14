using System;


namespace Design_Patterns_project.Connection
{
    class MsSqlConnectionConfig
    {
        public string _serverName {get;}
        private string _databaseName;
        private string _user;
        private string _password;


        public MsSqlConnectionConfig(string _serverName, string _databaseName){
            this._serverName = _serverName;
            this._databaseName = _databaseName;
        }

        public MsSqlConnectionConfig(string _serverName, string _databaseName, string _user, string _password ){
            this._serverName = _serverName;
            this._databaseName = _databaseName;
            this._user = _user;
            this._password = _password;
        }

        public string CreateConnectionString(){
            string connectionString = this._user == null ? @"Data Source="+this._serverName+";Initial Catalog="+this._databaseName+"; Integrated Security=True;" :
            "Server=" + this._serverName + ";Database=" + this._databaseName + ";User Id=" +this._user + ";Password=" + this._password + ";MultipleActiveResultSets=true;";
            return connectionString;
        }

    }
}