using System;

namespace Design_Patterns_project.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    sealed class ManyToManyAttribute : Attribute
    {        
        public string _manyToManyTableName{private set; get;}
        public ManyToManyAttribute(string manyToManyTableName)
        {
            _manyToManyTableName = manyToManyTableName;
        }
    }
}