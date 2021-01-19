using System;
using System.Reflection;
using System.Collections.Generic;

namespace Design_Patterns_project.Relationships
{
    enum RelationshipKind
    {
        OneToOne,
        OneToMany,
        ManyToMany
    }

    class Relationship
    {
        public Object _firstMember { private set; get; }
        public PropertyInfo _secondMember { private set; get; }
        public RelationshipKind _kind { private set; get; }

        public Relationship(Object first, PropertyInfo second, RelationshipKind kind)
        {
            this._firstMember = first;
            this._secondMember = second;
            this._kind = kind;
        }

        public Type GetSecondType()
        {
            return ((PropertyInfo)this._secondMember).PropertyType;
        }

        public string GetSecondName()
        {
            return this._secondMember.Name;
        }

        public bool IsSecondList()
        {
            Type type = GetSecondType();

            return type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public Type GetListParameter()
        {
            Type type = GetSecondType();

            if (IsSecondList())
                return type.GetGenericArguments()[0];

            return null;
        }
        public void PrintInfo()
        {
            Console.WriteLine(this._firstMember);
            Console.WriteLine(GetSecondType());
            Console.WriteLine(GetSecondType().Name);
            Console.WriteLine(GetListParameter().Name);
            Console.WriteLine(GetSecondName());
            Console.WriteLine();
        }

        
    }
}
