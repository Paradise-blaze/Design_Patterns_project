using System;
using System.Collections.Generic;
using System.Reflection;

namespace Design_Patterns_project
{
    class TableInheritance //future - deal with attributes and relationships
    {
        /*
        These function should be called with every last class of every branch in inheritance hierarchy:
        - InheritSingle
        - InheritClass
        - InheritConcrete
        */

        public Type GetMainType(Object member)
        {
            Type currentType = member.GetType();

            while (currentType.BaseType != typeof(Object))
                currentType = currentType.BaseType;

            return currentType;
        }

        public List<FieldInfo> InheritSingle(List<Object> lastMembers)
        {
            List<FieldInfo> fieldList = new List<FieldInfo>();
            List<Type> typeList = new List<Type>();

            foreach (Object member in lastMembers)
                AddSingleInheritanceMember(member.GetType(), fieldList, typeList);

            return fieldList;
        }

        public void AddSingleInheritanceMember(Type memberType, List<FieldInfo> fList, List<Type> tList)
        {
            FieldInfo[] currentFields = memberType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in currentFields)
                if (!fList.Contains(field))
                    fList.Add(field);

            if (!tList.Contains(memberType))
                tList.Add(memberType);

            if (memberType.BaseType != typeof(Object)) // Object as a parent
                AddSingleInheritanceMember(memberType.BaseType, fList, tList);
        }

        public Dictionary<Type, List<FieldInfo>> InheritClass(List<Object> lastMembers)
        {
            Dictionary<Type, List<FieldInfo>> typeMap = new Dictionary<Type, List<FieldInfo>>();

            foreach (var member in lastMembers)
                AddClassInheritanceMember(member.GetType(), typeMap);

            return typeMap;
        }

        public void AddClassInheritanceMember(Type memberType, Dictionary<Type, List<FieldInfo>> tMap)
        {
            if (!tMap.ContainsKey(memberType))
            {
                List<FieldInfo> fieldList = new List<FieldInfo>(memberType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
                tMap.Add(memberType, fieldList);
                
                if (memberType.BaseType != typeof(Object))
                    AddClassInheritanceMember(memberType.BaseType, tMap);
            }
        }

        public Dictionary<Type, List<FieldInfo>> InheritConcrete(List<Object> lastMembers)
        {
            List<Dictionary<Type, List<FieldInfo>>> typeMapList = new List<Dictionary<Type, List<FieldInfo>>>();

            foreach (var member in lastMembers)
            {
                Dictionary<Type, List<FieldInfo>> typeMap = new Dictionary<Type, List<FieldInfo>>();
                AddConcreteInheritanceMember(member.GetType(), typeMap);
                typeMapList.Add(typeMap);
            }


            Dictionary<Type, List<FieldInfo>> singleMap = new Dictionary<Type, List<FieldInfo>>();

            foreach (var map in typeMapList)
                foreach (var pair in map)
                    if (!singleMap.ContainsKey(pair.Key))
                        singleMap.Add(pair.Key, pair.Value);

            return singleMap;
        }

        public void AddConcreteInheritanceMember(Type memberType, Dictionary<Type, List<FieldInfo>> tMap)
        {
            List<FieldInfo> fieldList = new List<FieldInfo>(memberType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

            foreach (var pair in tMap)
                foreach (var field in fieldList)
                    pair.Value.Add(field);
            
            if (!tMap.ContainsKey(memberType))
                tMap.Add(memberType, fieldList);

            if (memberType.BaseType != typeof(Object))
                AddConcreteInheritanceMember(memberType.BaseType, tMap);
        }
    }
}
