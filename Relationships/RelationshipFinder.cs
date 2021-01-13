using System;
using System.Collections.Generic;
using Design_Patterns_project.Attributes;
using System.Reflection;
using System.Linq;

namespace Design_Patterns_project.Relationships
{
    class RelationshipFinder
    {
        private List<Relationship> FindRelationship(Type instanceType, RelationshipKind kind)
        {
            List<Relationship> oneToOneRelationships = new List<Relationship>();
            BindingFlags bindingFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            MemberInfo[] memberArray = instanceType.GetFields(bindingFlag)
                                            .Cast<MemberInfo>()
                                            .Concat(instanceType.GetProperties(bindingFlag))
                                            .Where(val => !(val.Name[0] == '<'))
                                            .ToArray();

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

            foreach (var member in memberArray)
            {
                Object[] attributes = member.GetCustomAttributes(attr, false);

                if (attributes.Length != 0)
                {
                    Type first = instanceType;
                    
                    Relationship oneToOneRelationship = new Relationship(first, member, kind);
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
