using System;

namespace Design_Patterns_project.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    sealed class ManyToManyAttribute : Attribute
    {        
        public string m_manyToManyTableName{private set; get;}
        public ManyToManyAttribute(string manyToManyTableName)
        {
            m_manyToManyTableName = manyToManyTableName;
        }
    }
}