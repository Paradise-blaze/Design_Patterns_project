using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data.SqlClient;
using System.Collections;
using Design_Patterns_project.Attributes;

namespace Design_Patterns_project
{
    class DataMapper
    {
        public static PropertyInfo[] GetTypeProperties(Type type)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            PropertyInfo[] typeProperties = type.GetProperties(bindingFlags);

            return typeProperties;
        }

        public string GetTableName(Object instance)
        {
            TableAttribute tableAttribute = (TableAttribute)Attribute.GetCustomAttribute(instance.GetType(), typeof(TableAttribute));

            if (tableAttribute == null)
            {
                return instance.GetType().Name;
            }
            else
            {
                return tableAttribute._tableName;
            }
        }

        public List<string> GetColumnNamesFromObject(Object instance)
        {
            List<string> columnNames = new List<string>();
            Type instanceType = instance.GetType();
            PropertyInfo[] instanceProperties = GetTypeProperties(instanceType);

            foreach (PropertyInfo property in instanceProperties)
            {
                Object[] columnAttributes = property.GetCustomAttributes(typeof(ColumnAttribute), false);

                if (columnAttributes.Length == 0) //change this code (deal with names and attribute existence)
                {
                    string columnName = property.Name;
                    columnNames.Add(columnName);
                }
                else
                {
                    ColumnAttribute columnAttribute = (ColumnAttribute)columnAttributes[0];
                    columnNames.Add(columnAttribute._columnName);
                }
            }

            return columnNames;
        }

        public List<Tuple<string, Object>> GetColumnsAndValues(Object instance)
        {
            List<Tuple<string, Object>> list = new List<Tuple<string, Object>> { };
            Type instanceType = instance.GetType();
            PropertyInfo[] properties = GetTypeProperties(instanceType);
            
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

                //foreign key mapping and association table mapping will solve this problem
                if (oneToOneAttribute == null && oneToManyAttribute == null && manyToManyAttribute == null)
                {
                    list.Add(new Tuple<string, Object>(columnName, value));
                }
            }

            return list;
        }

        public Object FindPrimaryKey(Object instance)
        {
            Object primaryKey;
            Type instanceType = instance.GetType();
            PropertyInfo[] properties = GetTypeProperties(instanceType);
            
            foreach (PropertyInfo property in properties)
            {
                Object[] att = property.GetCustomAttributes(typeof(PKeyAttribute), false);

                if (att.Length != 0)
                {
                    MethodInfo strGetter = property.GetGetMethod(nonPublic: true);
                    primaryKey = strGetter.Invoke(instance, null);
                    return primaryKey;
                }
            }

            return null;
        }

        public string FindPrimaryKeyFieldName(Object instance)
        {
            Type instanceType = instance.GetType();
            PropertyInfo[] properties = GetTypeProperties(instanceType);
            
            foreach (PropertyInfo property in properties)
            {
                Object[] pKeyAttributes = property.GetCustomAttributes(typeof(PKeyAttribute), false);

                if (pKeyAttributes.Length != 0)
                {
                    string columnName;
                    Object[] columnAttributes = property.GetCustomAttributes(typeof(ColumnAttribute), false);
                    ColumnAttribute columnAttribute = (ColumnAttribute)columnAttributes[0];

                    if (columnAttribute._columnName == null)
                    {
                        columnName = property.Name;
                    }
                    else
                    {
                        columnName = columnAttribute._columnName;
                    }

                    return columnName;
                }
            }

            return null;
        }

        public Dictionary<string, Object> CreateDictionaryFromTable(SqlDataReader reader)
        {
            Dictionary<string, Object> columnNamesAndTheirValues = new Dictionary<string, Object>();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columnNamesAndTheirValues.Add(reader.GetName(i), reader[i]);
                }
            }

            return columnNamesAndTheirValues;
        }

        public Object GetValueOfForeignKey(PropertyInfo property, SqlDataReader reader)
        {
            Dictionary<string, Object> columnNamesAndTheirValues = CreateDictionaryFromTable(reader);
            reader.Close();

            if (columnNamesAndTheirValues.Count == 0)
            {
                return null;
            }

            Object[] columnAttributes = property.GetCustomAttributes(typeof(ColumnAttribute), false);

            if (columnAttributes.Length != 0)
            {
                string columnNameInObject;
                ColumnAttribute columnAttribute = (ColumnAttribute)columnAttributes[0];
                
                if (columnAttribute._columnName == null)
                {
                    columnNameInObject = property.Name;
                }
                else
                {
                    columnNameInObject = columnAttribute._columnName;
                }

                return columnNamesAndTheirValues[columnNameInObject];
            }

            return null;
        }

        public Object MapTableIntoObject(Object instance, SqlDataReader reader)
        {
            Dictionary<string, Object> columnNamesAndTheirValues = CreateDictionaryFromTable(reader);
            reader.Close();
            Type instanceType = instance.GetType();
            PropertyInfo[] properties = GetTypeProperties(instanceType);

            foreach (PropertyInfo property in properties)
            {
                Object[] columnAttributes = property.GetCustomAttributes(typeof(ColumnAttribute), false);
                Object[] oneToOneAttributes = property.GetCustomAttributes(typeof(OneToOneAttribute), false);

                if (columnAttributes.Length == 0)
                {
                    string columnName = property.Name;
                }

                else
                {
                    if (oneToOneAttributes.Length != 0) { continue; }

                    string columnNameInObject;

                    foreach (ColumnAttribute atr in columnAttributes)
                    {
                        if (atr._columnName == null)
                        {
                            columnNameInObject = property.Name;
                        }
                        else
                        {
                            columnNameInObject = atr._columnName;
                        }

                        var value = columnNamesAndTheirValues[columnNameInObject];
                        property.SetValue(instance, value, null);
                    }
                }
            }

            return instance;
        }

        public Object SetCertainListField(Object parent, Object children, PropertyInfo property)
        {
            IList childTmp = children as IList;
            IList list = Activator.CreateInstance(property.PropertyType) as IList;

            foreach (var it in childTmp)
            {
                list.Add(it);
            }

            property.SetValue(parent, list, null);

            return parent;
        }

        public Object SetCertainField(Object parent, Object child, PropertyInfo property)
        {
            property.SetValue(parent, child, null);

            return parent;
        }
    }

}