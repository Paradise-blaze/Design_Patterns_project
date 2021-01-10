using System;
using System.Data;
using System.Data.SqlClient;


namespace Design_Patterns_project.Connection
{
    class MsSqlConnection : IDisposable
    {
        private static readonly MsSqlConnection _instance = new MsSqlConnection();
        private SqlConnection _connection;
        private MsSqlConnectionConfig _config;


        private MsSqlConnection()
        {
            this._connection = new SqlConnection();
        }

        public MsSqlConnection(MsSqlConnectionConfig config)
        {
            this._connection = new SqlConnection();
            this._config = config;
        }

        public static MsSqlConnection GetsInstance()
        {
            return _instance;
        }

        public void SetConfiguration(MsSqlConnectionConfig config)
        {
            this._config = config;
        }


        public void ConnectAndOpen()
        {
            this._connection.ConnectionString = _config.CreateConnectionString();
            this._connection.Open();
        }
        

        public void Dispose()
        {
            this._connection.Close();
            this._connection.Dispose();
            
        }


        // SELECT
        public string ExecuteSelectQuery(string sqlQuery){

            SqlCommand command = new SqlCommand(sqlQuery,this._connection);
            SqlDataReader dataReader = command.ExecuteReader();

            string output = "";
            int colsAmount = dataReader.FieldCount;
            int colWidht = 12;
   
            // add header
            for(int i=0; i<colsAmount; i++){
                string colName = dataReader.GetName(i);
                output += colName;
                for(int j=0; j<(colWidht - colName.Length); j++){
                    output += " ";
                }
                output += "|";  
            }

            output += "\n";
            for(int j=0; j<(colsAmount*colWidht)+colsAmount; j++){output += "-";}
            output += "\n";

            // add records
            while(dataReader.Read()){
                output += ReadSingleRow((IDataRecord)dataReader,colsAmount,colWidht);
            }

            return output;
        }

        private string ReadSingleRow(IDataRecord record, int colsAmount, int colWidht)
        {   
            string recordString = "";
    
            for(int i=0; i<colsAmount; i++){
                string item = record[i].ToString().Trim(' ');
                recordString += item;
                for(int j=0; j<(colWidht - item.Length); j++){
                    recordString += " ";
                }
                recordString += "|";  
            }
            return recordString+"\n";
        }


        // INSERT,UPDATE,DROP,DELETE, etc.
        public void ExecuteQuery(string sqlQuery){
            SqlCommand command = new SqlCommand(sqlQuery,this._connection);
            int num = command.ExecuteNonQuery();
            Console.WriteLine("Num of edited rows: "+num);
        }

    }

}