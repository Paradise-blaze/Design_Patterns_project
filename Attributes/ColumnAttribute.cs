using System;

namespace Design_Patterns_project.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class ColumnAttribute : Attribute
    {
        public string _columnName {private set; get;}

        public ColumnAttribute(string columnName = null)
        {
            _columnName = columnName;
        }
    }
}