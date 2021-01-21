using System;

namespace Design_Patterns_project.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    sealed class ManyToManyAttribute : Attribute
    {        
        public ManyToManyAttribute(){}
    }
}