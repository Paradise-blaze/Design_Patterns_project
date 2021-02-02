using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace Design_Patterns_project.Connection
{
    class MsSqlConnection : IDisposable
    {
        private SqlConnection _connection;
        private MsSqlConnectionConfig _config;

        public SqlConnection GetConnection()
        {
            return this._connection;
        }

        private MsSqlConnection()
        {
            this._connection = new SqlConnection();
        }

        public MsSqlConnection(MsSqlConnectionConfig config)
        {
            this._connection = new SqlConnection();
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

        public SqlDataReader ExecuteObjectSelect(string sqlQuery, string tableName){

            ConnectAndOpen();
            SqlCommand command = new SqlCommand(sqlQuery, this._connection);
            SqlDataReader dataReader = command.ExecuteReader();
            return dataReader;
        }





        // SELECT
        public string ExecuteSelectQuery(string sqlQuery, string tableName){
            ConnectAndOpen();
            SqlCommand command = new SqlCommand(sqlQuery, this._connection);
            Dispose();
            List<string> columnNames = GetColumnNamesFromTable(tableName);
            int maxLength = columnNames.Max(x => x.Length);

            try
            {
                ConnectAndOpen();
                SqlDataReader dataReader = command.ExecuteReader();

                string output = tableName + '\n';
                int colsAmount = dataReader.FieldCount;
                int colWidht = maxLength;
    
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
                dataReader.Close();
                Dispose();

                return output;

            }catch(SqlException ex){
                return HandleSqlException(ex);
            }
            catch(Exception ex){
                return HandleOtherException(ex);
            }
             
        }


        // INSERT,UPDATE,DROP,DELETE, etc.
        public void ExecuteQuery(string sqlQuery){
            
            try{
                SqlCommand command = new SqlCommand(sqlQuery,this._connection);
                int num = command.ExecuteNonQuery();
                Console.WriteLine("Num of edited rows: "+num);

            }catch(SqlException ex){
                Console.WriteLine(HandleSqlException(ex));

            }catch(Exception ex){    
                Console.WriteLine(HandleOtherException(ex)); 
            }
        }

        // Check if table of given name exists
        public bool CheckIfTableExists(string tableName){
            string sqlQuery = "SELECT CASE WHEN OBJECT_ID('dbo."+tableName+"', 'U') IS NOT NULL THEN 1 ELSE 0 END;";
            SqlCommand command = new SqlCommand(sqlQuery,this._connection);
            ConnectAndOpen();
            int result = (Int32)command.ExecuteScalar();
            Dispose();
            return (result == 1);
        }


        // Return list of column names of given table 
        public List<string> GetColumnNamesFromTable(string tableName){

            List<string> columns = new List<string>();

            if (this.CheckIfTableExists(tableName)){
                string sqlQuery = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '"+tableName+"'";
                
                SqlCommand command = new SqlCommand(sqlQuery,this._connection);
                ConnectAndOpen();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    IDataRecord record = (IDataRecord)reader;
                    columns.Add(String.Format("{0}",record[0]));
                }

                reader.Close();
                Dispose();
            }

            return columns;
        }


        public string HandleSqlException(SqlException ex){
            StringBuilder errorMessages = new StringBuilder();
            for (int i = 0; i < ex.Errors.Count; i++)
                {
                    errorMessages.Append("\nHandled exception \n"+
                        "Index #" + i + "\n" +
                        "Message: " + ex.Errors[i].Message + "\n" +
                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                        "Source: " + ex.Errors[i].Source + "\n" +
                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                }
                return errorMessages.ToString();
        }


        public string HandleOtherException(Exception ex){
            StringBuilder errorMessages = new StringBuilder();
            errorMessages.Append("\nHandled exception \n"+
                    "Message: "+ex.Message+"\n"+
                    "Source: " +ex.Source+"\n");
            return errorMessages.ToString();
        }

    }

}