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
        public Type _firstMember { private set; get; }
        public MemberInfo _secondMember { private set; get; }
        public RelationshipKind _kind { private set; get; }

        public Relationship(Type first, MemberInfo second, RelationshipKind kind)
        {
            this._firstMember = first;
            this._secondMember = second;
            this._kind = kind;
        }

        public Type GetSecondMemberType() // determine whether it's field or property
        {
            if (this._secondMember.MemberType == MemberTypes.Field)
                return typeof(FieldInfo);
            else // this._secondMember.MemberType == MemberTypes.Property
                return typeof(PropertyInfo);
        }

        public Type GetSecondType()
        {
            Type type = GetSecondMemberType();

            if (type == typeof(FieldInfo))
                return ((FieldInfo)this._secondMember).FieldType;

            else // type == typeof(PropertyInfo)
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
