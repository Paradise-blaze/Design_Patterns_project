using System;

namespace Design_Patterns_project.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class ColumnAttribute : Attribute
    {
        public string m_columnName {private set; get;}

        public ColumnAttribute(string columnName = null)
        {
            m_columnName = columnName;
        }
    }
}