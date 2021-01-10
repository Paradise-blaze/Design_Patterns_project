using System;
using System.Collections.Generic;

namespace Design_Patterns_project.Relationships
{
    class RelationshipFinder
    {
        public List<Relationship> FindOneToOne() 
        { 
            return new List<Relationship>();
        }

        public List<Relationship> FindOneToMany() 
        {
            return new List<Relationship>(); 
        }

        public List<Relationship> FindManToMany() 
        { 
            return new List<Relationship>(); 
        }
    }
}
