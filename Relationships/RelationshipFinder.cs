using System;
using System.Collections.Generic;
using Design_Patterns_project.Attributes;
using System.Reflection;

namespace Design_Patterns_project.Relationships
{
    class RelationshipFinder
    {
        private List<Relationship> FindRelationship(Object instance, RelationshipKind kind)
        {
            List<Relationship> oneToOneRelationships = new List<Relationship>();
            PropertyInfo[] propertiesArray = DataMapper.GetTypeProperties(instance.GetType());

            Type attr = kind switch
            {
                RelationshipKind.OneToOne => typeof(OneToOneAttribute),
                RelationshipKind.OneToMany => typeof(OneToManyAttribute),
                RelationshipKind.ManyToMany => typeof(ManyToManyAttribute),
                _ => null,
            };

            foreach (var property in propertiesArray)
            {
                Object[] attributes = property.GetCustomAttributes(attr, false);

                if (attributes.Length != 0)
                {
                    Relationship oneToOneRelationship = new Relationship(instance, property, kind);
                    oneToOneRelationships.Add(oneToOneRelationship);
                }
            }

            return oneToOneRelationships;
        }

        public List<Relationship> FindOneToOne(Object instance)
        {
            return FindRelationship(instance, RelationshipKind.OneToOne);
        }

        public List<Relationship> FindOneToMany(Object instance)
        {
            return FindRelationship(instance, RelationshipKind.OneToMany);
        }

        public List<Relationship> FindManyToMany(Object instance)
        {
            return FindRelationship(instance, RelationshipKind.ManyToMany);
        }
    }
}
