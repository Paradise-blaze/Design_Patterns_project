using System;

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
        private Object _firstMember;
        private Object _secondMember;
        RelationshipKind _kind;

        Relationship(Object first, Object second, RelationshipKind kind)
        {
            this._firstMember = first;
            this._secondMember = second;
            this._kind = kind;
        }
    }
}
