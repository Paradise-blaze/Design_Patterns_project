﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Linq;
using Design_Patterns_project.Attributes;

namespace Design_Patterns_project
{
    class DataMapper
    {
        public static PropertyInfo[] GetTypeProperties(Type type)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            PropertyInfo[] typeProperties = type.GetProperties(bindingFlags);

            return typeProperties;
        }

        public static PropertyInfo[] GetTypeAllProperties(Type type)
        {
            PropertyInfo[] typeProperties = new PropertyInfo[] { };
            Type currentType = type;

            while (currentType != typeof(Object))
            {
                PropertyInfo[] currentProperties = GetTypeProperties(currentType);
                typeProperties = typeProperties.Concat(currentProperties).ToArray();
                currentType = currentType.BaseType;
            }

            return typeProperties;
        }

        public string GetTableName(Type objectType)
        {
            TableAttribute tableAttribute = (TableAttribute)Attribute.GetCustomAttribute(objectType, typeof(TableAttribute));

            if (tableAttribute == null)
            {
                return objectType.Name;
            }
            else
            {
                if (tableAttribute._tableName == null)
                {
                    return objectType.Name;
                }
                return tableAttribute._tableName;
            }
        }

        public List<Tuple<string, Object>> GetColumnsAndValues(Object instance, bool isInherited = false)
        {
            List<Tuple<string, Object>> list = new List<Tuple<string, Object>> { };
            Type instanceType = instance.GetType();
            PropertyInfo[] properties;

            if (isInherited)
            {
                properties = GetTypeAllProperties(instanceType);
            }
            else
            {
                properties = GetTypeProperties(instanceType);
            }

            foreach (PropertyInfo property in properties)
            {
                MethodInfo strGetter = property.GetGetMethod(nonPublic: true);
                var value = strGetter.Invoke(instance, null);
                string columnName;
                ColumnAttribute columnAttribute = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute), false);

                if (columnAttribute == null)
                {
                    continue;
                }

                if (columnAttribute._columnName == null)
                {
                    columnName = property.Name;
                }
                else
                {
                    columnName = columnAttribute._columnName;
                }

                OneToOneAttribute oneToOneAttribute = (OneToOneAttribute)property.GetCustomAttribute(typeof(OneToOneAttribute), false);
                OneToManyAttribute oneToManyAttribute = (OneToManyAttribute)property.GetCustomAttribute(typeof(OneToManyAttribute), false);
                ManyToManyAttribute manyToManyAttribute = (ManyToManyAttribute)property.GetCustomAttribute(typeof(ManyToManyAttribute), false);

                if (oneToOneAttribute == null && oneToManyAttribute == null && manyToManyAttribute == null)
                {
                    list.Add(new Tuple<string, Object>(columnName, value));
                }
            }

            return list;
        }

        public List<Tuple<string, Object>> GetInheritedColumnsAndValues(List<PropertyInfo> inheritedProperties)
        {
            List<Tuple<string, Object>> list = new List<Tuple<string, object>>();

            foreach (var property in inheritedProperties)
            {
                Type propertyType = property.PropertyType;
                string columnName;
                ColumnAttribute columnAttribute = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute), false);

                if (columnAttribute == null)
                {
                    continue;
                }

                if (columnAttribute._columnName == null)
                {
                    columnName = property.Name;
                }
                else
                {
                    columnName = columnAttribute._columnName;
                }

                OneToOneAttribute oneToOneAttribute = (OneToOneAttribute)property.GetCustomAttribute(typeof(OneToOneAttribute), false);
                OneToManyAttribute oneToManyAttribute = (OneToManyAttribute)property.GetCustomAttribute(typeof(OneToManyAttribute), false);
                ManyToManyAttribute manyToManyAttribute = (ManyToManyAttribute)property.GetCustomAttribute(typeof(ManyToManyAttribute), false);

                if (oneToOneAttribute == null && oneToManyAttribute == null && manyToManyAttribute == null)
                {
                    list.Add(new Tuple<string, Object>(columnName, propertyType));
                }
            }

            return list;
        }

        public Dictionary<string, Object> CreateDictionaryFromRecord(IDataRecord record)
        {
            Dictionary<string, Object> columnNamesAndTheirValues = new Dictionary<string, Object>();

            for (int i = 0; i < record.FieldCount; i++)
            {
                columnNamesAndTheirValues.Add(record.GetName(i), record[i]);
            }

            return columnNamesAndTheirValues;
        }
    }

}