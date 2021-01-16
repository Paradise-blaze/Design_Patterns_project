using System;
using System.Collections.Generic;
using Design_Patterns_project.Attributes;
using System.Reflection;

namespace Design_Patterns_project.Relationships
{
    class RelationshipFinder
    {
        private List<Relationship> FindRelationship(Type instanceType, RelationshipKind kind)
        {
            List<Relationship> oneToOneRelationships = new List<Relationship>();
            BindingFlags bindingFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            PropertyInfo[] propertiesArray = instanceType.GetProperties(bindingFlag);
            Type attr;

            switch (kind)
            {
                case RelationshipKind.OneToOne:
                    attr = typeof(OneToOneAttribute);
                    break;
                case RelationshipKind.OneToMany:
                    attr = typeof(OneToManyAttribute);
                    break;
                case RelationshipKind.ManyToMany:
                    attr = typeof(ManyToManyAttribute);
                    break;
                default:
                    attr = null;
                    break;
            }

            foreach (var property in propertiesArray)
            {
                Object[] attributes = property.GetCustomAttributes(attr, false);

                if (attributes.Length != 0)
                {
                    Type first = instanceType;
                    
                    Relationship oneToOneRelationship = new Relationship(first, property, kind);
                    oneToOneRelationships.Add(oneToOneRelationship);
                }
            }

            return oneToOneRelationships;
        }
        public List<Relationship> FindOneToOne(Type instanceType) 
        {
            return FindRelationship(instanceType, RelationshipKind.OneToOne);
        }

        public List<Relationship> FindOneToMany(Type instanceType) 
        {
            return FindRelationship(instanceType, RelationshipKind.OneToMany);
        }

        public List<Relationship> FindManToMany(Type instanceType) 
        { 
            return FindRelationship(instanceType, RelationshipKind.ManyToMany);
        }
    }
}
