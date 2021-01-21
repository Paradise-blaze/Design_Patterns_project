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
    }
}
