using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data.SqlClient;
using System.Linq;
using System.Collections;
using Design_Patterns_project.Attributes;

namespace orm.Mapper
{

    class PropertiesMapper
    {
        public string GetTableName(Object t)
        {
            TableAttribute attr = (TableAttribute)Attribute.GetCustomAttribute(t.GetType(), typeof(TableAttribute));

            if (attr == null)
            {
                Console.WriteLine("The attribute was not found.");
                return ConvertObjectNameToString(t);
            }
            else
            {
                return attr._tableName;
            }
        }

        public List<string> GetColumnName(Object t)
        {
            List<string> list = new List<string> { };
            Type type = t.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            PropertyInfo[] props = type.GetProperties(bindingFlags);

            foreach (PropertyInfo prp in props)
            {
                MethodInfo strGetter = prp.GetGetMethod(nonPublic: true);
                object[] att = prp.GetCustomAttributes(typeof(ColumnAttribute), false);
                var val = strGetter.Invoke(t, null);

                if (att.Length == 0)
                {
                    string columnName = ConvertObjectNameToString(prp.Name);
                    list.Add(columnName); ;
                }
                else
                {
                    foreach (ColumnAttribute atr in att)
                    {
                        list.Add(atr._columnName);
                    }
                }
            }

            return list;
        }

        public List<Tuple<string, object>> GetColumnAndValue(Object obj)
        {
            List<Tuple<string, object>> list = new List<Tuple<string, object>> { };
            Type type = obj.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            PropertyInfo[] props = type.GetProperties(bindingFlags);
            
            foreach (PropertyInfo prp in props)
            {
                MethodInfo strGetter = prp.GetGetMethod(nonPublic: true);
                var val = strGetter.Invoke(obj, null);
                object[] att = prp.GetCustomAttributes(typeof(ColumnAttribute), false);

                if (att.Length == 0)
                    continue;

                string columnName;
                ColumnAttribute att1 = (ColumnAttribute)att[0];

                if (att1._columnName == null)
                {
                    columnName = ConvertObjectNameToString(prp.Name);
                }
                else
                {
                    columnName = att1._columnName;
                }

                object[] oneToOneAtt = prp.GetCustomAttributes(typeof(OneToOneAttribute), false);

                if (oneToOneAtt.Length != 0)
                {
                    if (val != null)
                    {
                        var forgeinKey = findPrimaryKey(val);
                        val = forgeinKey;
                    }
                    else
                    {
                        val = "null";

                    }
                }

                list.Add(new Tuple<string, object>(columnName, val));
            }

            return list;
        }

        public object findPrimaryKey(object obj)
        {
            object primaryKey;
            Type type = obj.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            PropertyInfo[] props = type.GetProperties(bindingFlags);
            
            foreach (PropertyInfo prp in props)
            {
                MethodInfo strGetter = prp.GetGetMethod(nonPublic: true);

                primaryKey = strGetter.Invoke(obj, null);
                object[] att = prp.GetCustomAttributes(typeof(PKeyAttribute), false);
                if (att.Length != 0)
                {
                    return primaryKey;
                }
            }

            return null;
        }
        public string FindPrimaryKeyFieldName(object obj)
        {
            object primaryKey;
            Type type = obj.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            PropertyInfo[] props = type.GetProperties(bindingFlags);
            
            foreach (PropertyInfo prp in props)
            {
                MethodInfo strGetter = prp.GetGetMethod(nonPublic: true);
                primaryKey = strGetter.Invoke(obj, null);
                object[] att = prp.GetCustomAttributes(typeof(PKeyAttribute), false);

                if (att.Length != 0)
                {
                    string columnName;
                    object[] attColumn = prp.GetCustomAttributes(typeof(ColumnAttribute), false);
                    ColumnAttribute att1 = (ColumnAttribute)attColumn[0];

                    if (att1._columnName == null)
                    {
                        columnName = ConvertObjectNameToString(prp.Name);
                    }
                    else
                    {
                        columnName = att1._columnName;
                    }

                    return columnName;
                }
            }

            return null;
        }
        public string ConvertObjectNameToString(Object t)
        {
            string nameWithNamespaces = t.ToString();
            int appearanceOfLastFullStop = -1;

            for (int i = nameWithNamespaces.Length - 1; i > 0; i--)
            {
                if (nameWithNamespaces[i] == '.')
                {
                    appearanceOfLastFullStop = i;
                }
            }

            string nameWithoutNamespaces = nameWithNamespaces.Substring(appearanceOfLastFullStop + 1);

            return nameWithoutNamespaces;
        }


        public Dictionary<string, object> CreateDictionaryFromTable(SqlDataReader reader)
        {
            Dictionary<string, object> columnNameAndItsValue = new Dictionary<string, object>();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columnNameAndItsValue.Add(reader.GetName(i), reader[i]);
                }
            }

            return columnNameAndItsValue;
        }

        public object GetValueOfForeignKey(PropertyInfo prp, SqlDataReader reader)
        {
            Dictionary<string, object> columnNameAndItsValue = CreateDictionaryFromTable(reader);
            reader.Close();

            if (columnNameAndItsValue.Count == 0)
            {
                return null;
            }

            object[] attColumn = prp.GetCustomAttributes(typeof(ColumnAttribute), false);

            if (attColumn.Length != 0)
            {
                string columnNameInObject;

                foreach (ColumnAttribute atr in attColumn)
                {
                    if (atr._columnName == null)
                    {
                        columnNameInObject = ConvertObjectNameToString(prp.Name);
                    }
                    else
                    {
                        columnNameInObject = atr._columnName;
                    }

                    return columnNameAndItsValue[columnNameInObject];
                }
            }

            return null;
        }

        public object MapTableIntoObject(object obj, SqlDataReader reader)
        {

            Dictionary<string, object> columnNameAndItsValue = CreateDictionaryFromTable(reader);
            reader.Close();
            Type type = obj.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            PropertyInfo[] props = type.GetProperties(bindingFlags);

            foreach (PropertyInfo prp in props)
            {
                MethodInfo strGetter = prp.GetGetMethod(nonPublic: true);
                object[] attColumn = prp.GetCustomAttributes(typeof(ColumnAttribute), false);
                object[] attOneToOne = prp.GetCustomAttributes(typeof(OneToOneAttribute), false);

                if (attColumn.Length == 0)
                {
                    string columnName = ConvertObjectNameToString(prp.Name);
                }

                else
                {
                    if (attOneToOne.Length != 0) { continue; }

                    string columnNameInObject;

                    foreach (ColumnAttribute atr in attColumn)
                    {
                        if (atr._columnName == null)
                        {
                            columnNameInObject = ConvertObjectNameToString(prp.Name);
                        }
                        else
                        {
                            columnNameInObject = atr._columnName;
                        }

                        var value = columnNameAndItsValue[columnNameInObject];
                        prp.SetValue(obj, value, null);
                    }
                }
            }

            return obj;
        }

        public object SetCertainListField(object parent, object children, PropertyInfo prp)
        {
            IList childTmp = children as IList;
            IList list = Activator.CreateInstance(prp.PropertyType) as IList;

            foreach (var it in childTmp)
            {
                list.Add(it);
            }

            prp.SetValue(parent, list, null);

            return parent;
        }

        public object SetCertainField(object parent, object child, PropertyInfo prp)
        {
            prp.SetValue(parent, child, null);

            return parent;
        }
    }

}