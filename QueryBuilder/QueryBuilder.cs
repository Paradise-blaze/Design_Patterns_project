using System;
using System.Collections.Generic;
using Design_Patterns_project.SqlCondition;

namespace orm.Query
{
    class QueryBuilder
    {

        public QueryBuilder() { }

        public string createInsertQuery(string tableName, List<Tuple<string, object>> columns)
        {
            string returnQuery = "INSERT INTO " + tableName + "(";
            foreach (Tuple<string, object> it in columns)
            {
                returnQuery += it.Item1 + ", ";
            }
            returnQuery = returnQuery.Remove(returnQuery.Length - 2);
            returnQuery += ") VALUES (";


            foreach (Tuple<string, object> it in columns)
            {
                if (it.Item2.GetType() == typeof(string))
                {
                    if (it.Item2.Equals("null"))
                    {
                        returnQuery += "null, ";
                    }
                    else
                        returnQuery += "'" + it.Item2 + "'" + ", ";
                }
                else
                {
                    returnQuery += it.Item2 + ", ";
                }
            }
            returnQuery = returnQuery.Remove(returnQuery.Length - 2);
            returnQuery += ");";

            return returnQuery;
        }

        public static readonly Dictionary<Type, string> CsTypesToSql = new Dictionary<Type, string>()
        {
            {typeof(System.Int64),"bigint"},
            {typeof(System.Byte[]),"binary"},
            {typeof(System.Boolean),"bit"},
            {typeof(System.String),"varchar(255)" },
            {typeof(System.Char[]),"varchar(255)" },
            {typeof(System.DateTime),"date" },
            {typeof(System.Decimal),"decimal" },
            {typeof(System.Double),"float" },
            {typeof(System.Int32),"int" },
        };

        public string createCreateTableQuery(string tableName, List<Tuple<string, object>> columns, object primaryKey)
        {
            string returnQuery = "IF NOT EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo." + tableName + "') and TYPE in (N'U')) BEGIN";
            returnQuery += " CREATE TABLE " + tableName + "(";

            foreach (Tuple<string, object> it in columns)
            {
                returnQuery += it.Item1 + " ";
                returnQuery += CsTypesToSql[it.Item2.GetType()];
                if (primaryKey.Equals(it.Item1))
                {
                    returnQuery += " PRIMARY KEY,   ";
                }
                else
                    returnQuery += ", ";
            }

            returnQuery = returnQuery.Remove(returnQuery.Length - 2);
            returnQuery += ") END;";

            return returnQuery;
        }

        public string createUpdateQuery(string tableName, List<Tuple<string, object>> valuesToSet, List<SqlCondition> SqlCondition)
        {
            string returnQuery = "UPDATE " + tableName + " SET ";
            foreach (Tuple<string, object> it in valuesToSet)
            {
                returnQuery += it.Item1 + "=";
                if (it.Item2.GetType() == typeof(string))
                {
                    returnQuery += "'" + it.Item2 + "'" + ", ";
                }
                else
                {
                    returnQuery += it.Item2 + ", ";
                }
            }
            returnQuery = returnQuery.Remove(returnQuery.Length - 2);
            returnQuery += generateWhereClause(SqlCondition);
            return returnQuery;
        }

        public string createDeleteQuery(string tableName, List<SqlCondition> listOfSqlCondition)
        {
            string query = "DELETE FROM " + tableName + generateWhereClause(listOfSqlCondition);
            return query;
        }

        public string createSelectByIdQuery(string tableName, object id, string primaryKeyName)
        {
            string result = "SELECT * FROM " + tableName + " WHERE " + tableName + "." + primaryKeyName + "=" + id + ";";
            return result;
        }

        public string createSelectQuery(string tablename, List<SqlCondition> listOfSqlCondition)
        {
            string query = "SELECT * FROM " + tablename + generateWhereClause(listOfSqlCondition);
            return query;
        }

        public string generateWhereClause(List<SqlCondition> listOfSqlCondition)
        {
            string whereClause = " WHERE ";
            foreach (SqlCondition c in listOfSqlCondition)
            {
                whereClause += c.generateString() + " AND ";
            }
            whereClause = whereClause.Remove(whereClause.Length - 5);
            whereClause += ";";
            return whereClause;
        }

    }
}
