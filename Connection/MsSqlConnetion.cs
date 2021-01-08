using System;
using System.Data;
using System.Data.SqlClient;


namespace Design_Patterns_project.Connection
{
    class MsSqlConnection : IDisposable
    {
        private static readonly MsSqlConnection instance = new MsSqlConnection();
        private SqlConnection connection;
        private MsSqlConnectionConfig config;


        private MsSqlConnection()
        {
            this.connection = new SqlConnection();
        }

        public MsSqlConnection(MsSqlConnectionConfig config)
        {
            this.connection = new SqlConnection();
            this.config = config;
        }

        public static MsSqlConnection getInstance()
        {
            return instance;
        }

        public void SetConfiguration(MsSqlConnectionConfig config)
        {
            this.config = config;
        }


        public void ConnectAndOpen()
        {
            this.connection.ConnectionString = config.CreateConnectionString();
            this.connection.Open();
        }
        

        public void Dispose()
        {
            this.connection.Close();
            this.connection.Dispose();
            
        }

        public string ExecuteSelectCommand(string sqlQuery){

            SqlCommand command = new SqlCommand(sqlQuery,this.connection);
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

            // add records
            while(dataReader.Read()){
                output += ReadSingleRow((IDataRecord)dataReader,colsAmount,colWidht);
            }

            return output;
        }

        private static string ReadSingleRow(IDataRecord record, int colsAmount, int colWidht)
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
    } 

}