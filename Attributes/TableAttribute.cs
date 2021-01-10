using System;

namespace Design_Patterns_project.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class TableAttribute : Attribute
    {
        public string m_tableName {private set; get; }
        
        public TableAttribute(string tableName = null)
        {
            m_tableName = tableName;
        }
    }
}