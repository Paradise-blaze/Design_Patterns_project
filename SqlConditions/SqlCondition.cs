using System;

namespace Design_Patterns_project
{
    public class SqlCondition
    {

        private string _sqlOperator;
        protected string _field;
        protected Object _value;
        public SqlCondition(string _sqlOperator, string field, Object value)
        {
            this._sqlOperator = _sqlOperator;
            this._field = field;
            this._value = value;
        }

        public string GenerateString()
        {
            if (_value.GetType() == typeof(string))
            {
                return _field + _sqlOperator + '"' + _value + '"';
            }

            return _field + _sqlOperator + _value.ToString();
        }

        public static SqlCondition GreaterThan(string fieldName, Object value)
        {
            SqlCondition SqlCondition = new SqlCondition(">", fieldName, value);
            return SqlCondition;
        }
        public static SqlCondition LowerThan(string fieldName, Object value)
        {
            SqlCondition SqlCondition = new SqlCondition("<", fieldName, value);
            return SqlCondition;
        }
        public static SqlCondition Equals(string fieldName, Object value)
        {
            SqlCondition SqlCondition = new SqlCondition("=", fieldName, value);
            return SqlCondition;
        }
        public static SqlCondition NotEquals(string fieldName, Object value)
        {
            SqlCondition SqlCondition = new SqlCondition("!=", fieldName, value);
            return SqlCondition;
        }


    }


}