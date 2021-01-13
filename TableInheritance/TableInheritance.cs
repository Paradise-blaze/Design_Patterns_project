using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;

namespace Design_Patterns_project
{
    class TableInheritance
    {
        /*
        These function should be called with every last class of every branch in inheritance hierarchy:
        - InheritSingle
        - InheritClass
        - InheritConcrete
        */

        public Type GetMainType(Object instance)
        {
            Type currentType = instance.GetType();

            while (currentType.BaseType != typeof(Object))
                currentType = currentType.BaseType;

            return currentType;
        }

        public List<MemberInfo> InheritSingle(List<Object> inheritanceHierarchyLeaves)
        {
            List<MemberInfo> memberList = new List<MemberInfo>();
            List<Type> typeList = new List<Type>();

            foreach (Object leaf in inheritanceHierarchyLeaves)
                AddSingleInheritanceMember(leaf.GetType(), memberList, typeList);

            return memberList;
        }

        public void AddSingleInheritanceMember(Type leafType, List<MemberInfo> mList, List<Type> tList)
        {
            BindingFlags bindingFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            MemberInfo[] currentMembers = leafType.GetFields(bindingFlag)
                                            .Cast<MemberInfo>()
                                            .Concat(leafType.GetProperties(bindingFlag))
                                            .Where(val => !(val.Name[0] == '<'))
                                            .ToArray();

            if (!tList.Contains(leafType))
            {
                foreach (var member in currentMembers)
                    if (!mList.Contains(member))
                        mList.Add(member);

                tList.Add(leafType);

                if (leafType.BaseType != typeof(Object)) // Object as a parent
                    AddSingleInheritanceMember(leafType.BaseType, mList, tList);
            }
        }

        public Dictionary<Type, List<MemberInfo>> InheritClass(List<Object> inheritanceHierarchyLeaves)
        {
            Dictionary<Type, List<MemberInfo>> typeMap = new Dictionary<Type, List<MemberInfo>>();

            foreach (var leaf in inheritanceHierarchyLeaves)
                AddClassInheritanceMember(leaf.GetType(), typeMap);

            return typeMap;
        }

        public void AddClassInheritanceMember(Type leafType, Dictionary<Type, List<MemberInfo>> tMap)
        {
            if (!tMap.ContainsKey(leafType))
            {
                BindingFlags bindingFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                MemberInfo[] currentMembers = leafType.GetFields(bindingFlag)
                                                .Cast<MemberInfo>()
                                                .Concat(leafType.GetProperties(bindingFlag))
                                                .Where(val => !(val.Name[0] == '<'))
                                                .ToArray();
                List<MemberInfo> memberList = new List<MemberInfo>(currentMembers);

                tMap.Add(leafType, memberList);
                
                if (leafType.BaseType != typeof(Object))
                    AddClassInheritanceMember(leafType.BaseType, tMap);
            }
        }

        public Dictionary<Type, List<MemberInfo>> InheritConcrete(List<Object> inheritanceHierarchyLeaves)
        {
            Dictionary<Type, List<MemberInfo>> typeMap = new Dictionary<Type, List<MemberInfo>>();

            foreach (var leaf in inheritanceHierarchyLeaves)
            {
                Dictionary<Type, List<MemberInfo>> singleMap = new Dictionary<Type, List<MemberInfo>>();
                AddConcreteInheritanceMember(leaf.GetType(), singleMap);

                foreach (var pair in singleMap)
                    if (!typeMap.ContainsKey(pair.Key))
                        typeMap.Add(pair.Key, pair.Value);
            }

            return typeMap;
        }

        public void AddConcreteInheritanceMember(Type leafType, Dictionary<Type, List<MemberInfo>> tMap)
        {
            BindingFlags bindingFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            MemberInfo[] currentMembers = leafType.GetFields(bindingFlag)
                                            .Cast<MemberInfo>()
                                            .Concat(leafType.GetProperties(bindingFlag))
                                            .Where(val => !(val.Name[0] == '<'))
                                            .ToArray();
            List<MemberInfo> memberList = new List<MemberInfo>(currentMembers);

            foreach (var pair in tMap)
                foreach (var member in memberList)
                    pair.Value.Add(member);
            
            if (!tMap.ContainsKey(leafType))
                tMap.Add(leafType, memberList);

            if (leafType.BaseType != typeof(Object))
                AddConcreteInheritanceMember(leafType.BaseType, tMap);
        }
    }
}
