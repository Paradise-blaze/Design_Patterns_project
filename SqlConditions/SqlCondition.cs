using System;
using System.Collections.Generic;
using System.Text;

namespace Design_Patterns_project.SqlConditions
{
    public class SqlCondition
    {

        private string _sqlOperator;
        protected string field;
        protected object value;
        public SqlCondition(string _sqlOperator, string field, object value)
        {
            this._sqlOperator = _sqlOperator;
            this.field = field;
            this.value = value;
        }


        public string generateString()
        {
            if (value.GetType() == typeof(string))
            {
                return field + _sqlOperator + '"' + value + '"';
            }
            return field + _sqlOperator + value.ToString();
        }

        public static SqlCondition greaterThan(string fieldName, object value)
        {
            SqlCondition SqlCondition = new SqlCondition(">", fieldName, value);
            return SqlCondition;
        }
        public static SqlCondition lowerThan(string fieldName, object value)
        {
            SqlCondition SqlCondition = new SqlCondition("<", fieldName, value);
            return SqlCondition;
        }
        public static SqlCondition equals(string fieldName, object value)
        {
            SqlCondition SqlCondition = new SqlCondition("=", fieldName, value);
            return SqlCondition;
        }
        public static SqlCondition notEquals(string fieldName, object value)
        {
            SqlCondition SqlCondition = new SqlCondition("!=", fieldName, value);
            return SqlCondition;
        }


    }


}