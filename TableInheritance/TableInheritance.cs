using System;
using System.Collections.Generic;
using System.Reflection;

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
            {
                currentType = currentType.BaseType;
            }

            return currentType;
        }

        public List<PropertyInfo> InheritSingle(List<Object> inheritanceHierarchyLeaves)
        {
            List<PropertyInfo> allPropertiesInHierarchy = new List<PropertyInfo>();
            List<Type> visitedTypes = new List<Type>();

            foreach (Object leaf in inheritanceHierarchyLeaves)
            {
                AddSingleInheritanceMember(leaf.GetType(), allPropertiesInHierarchy, visitedTypes);
            }

            return allPropertiesInHierarchy;
        }

        public void AddSingleInheritanceMember(Type leafType, List<PropertyInfo> allProperties, List<Type> visited)
        {
            PropertyInfo[] currentProperties = DataMapper.GetTypeProperties(leafType);

            if (!visited.Contains(leafType))
            {
                foreach (var property in currentProperties)
                {
                    if (!allProperties.Contains(property))
                    {
                        allProperties.Add(property);
                    }
                }

                visited.Add(leafType);

                if (leafType.BaseType != typeof(Object)) // Object as a parent 
                {
                    AddSingleInheritanceMember(leafType.BaseType, allProperties, visited);
                }
            }
        }

        public Dictionary<Type, List<PropertyInfo>> InheritClass(List<Object> inheritanceHierarchyLeaves)
        {
            Dictionary<Type, List<PropertyInfo>> typesAndTheirProperties = new Dictionary<Type, List<PropertyInfo>>();

            foreach (var leaf in inheritanceHierarchyLeaves)
            {
                AddClassInheritanceMember(leaf.GetType(), typesAndTheirProperties);
            }

            return typesAndTheirProperties;
        }

        public void AddClassInheritanceMember(Type leafType, Dictionary<Type, List<PropertyInfo>> typesAndProperties)
        {
            if (!typesAndProperties.ContainsKey(leafType))
            {
                List<PropertyInfo> propertiesList = new List<PropertyInfo>(DataMapper.GetTypeProperties(leafType));

                typesAndProperties.Add(leafType, propertiesList);
                
                if (leafType.BaseType != typeof(Object))
                {
                    AddClassInheritanceMember(leafType.BaseType, typesAndProperties);
                }
            }
        }

        public Dictionary<Type, List<PropertyInfo>> InheritConcrete(List<Object> inheritanceHierarchyLeaves)
        {
            Dictionary<Type, List<PropertyInfo>> typesAndTheirProperties = new Dictionary<Type, List<PropertyInfo>>();

            foreach (var leaf in inheritanceHierarchyLeaves)
            {
                Dictionary<Type, List<PropertyInfo>> oneBranchTypesAndProperties = new Dictionary<Type, List<PropertyInfo>>();
                AddConcreteInheritanceMember(leaf.GetType(), oneBranchTypesAndProperties);

                foreach (var pair in oneBranchTypesAndProperties)
                    if (!typesAndTheirProperties.ContainsKey(pair.Key))
                        typesAndTheirProperties.Add(pair.Key, pair.Value);
            }

            return typesAndTheirProperties;
        }

        public void AddConcreteInheritanceMember(Type leafType, Dictionary<Type, List<PropertyInfo>> oneBranchTypesAndProperties)
        {
            List<PropertyInfo> propertiesList = new List<PropertyInfo>(DataMapper.GetTypeProperties(leafType));

            foreach (var pair in oneBranchTypesAndProperties)
            {
                foreach (var member in propertiesList)
                {
                    pair.Value.Add(member);
                }
            }
            
            if (!oneBranchTypesAndProperties.ContainsKey(leafType))
            {
                oneBranchTypesAndProperties.Add(leafType, propertiesList);
            }

            if (leafType.BaseType != typeof(Object))
            {
                AddConcreteInheritanceMember(leafType.BaseType, oneBranchTypesAndProperties);
            }
        }
    }
}
