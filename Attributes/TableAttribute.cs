using System;

namespace Design_Patterns_project.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class TableAttribute : Attribute
    {
        public string _tableName {private set; get; }
        
        public TableAttribute(string tableName = null)
        {
            _tableName = tableName;
        }
    }
}