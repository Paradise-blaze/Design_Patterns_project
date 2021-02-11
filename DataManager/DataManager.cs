using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Data.SqlClient;
using Design_Patterns_project.Relationships;
using Design_Patterns_project.SqlCommands;
using Design_Patterns_project.Connection;
using Design_Patterns_project.Attributes;

namespace Design_Patterns_project
{
    class DataManager
    {
        DataMapper _dataMapper = new DataMapper();
        MsSqlConnection _msSqlConnection;
        QueryBuilder _queryBuilder = new QueryBuilder();
        TableInheritance _tableInheritance = new TableInheritance();
        RelationshipFinder _relationshipFinder = new RelationshipFinder();
        Dictionary<Type, Dictionary<int, Object>> _dataDictionary = new Dictionary<Type, Dictionary<int, Object>>();

        public DataManager(string serverName, string databaseName, string user, string password)
        {
            MsSqlConnectionConfig config = new MsSqlConnectionConfig(serverName, databaseName, user, password);
            this._msSqlConnection = new MsSqlConnection(config);
        }

        public DataManager(string serverName, string databaseName)
        {
            MsSqlConnectionConfig config = new MsSqlConnectionConfig(serverName, databaseName);
            this._msSqlConnection = new MsSqlConnection(config);
        }

        private int FindMinimumAvailableID(Type type, bool single = false)
        {
            Type instanceType;

            if (single)
            {
                instanceType = _tableInheritance.GetMainType(type);
            }
            else
            {
                instanceType = type;
            }

            if (!_dataDictionary.ContainsKey(instanceType))
            {
                return 1;
            }
            else
            {
                List<int> keyList = new List<int>(_dataDictionary[instanceType].Keys);
                keyList.Sort();
                int availableMinimum = keyList.Max() + 1;

                for(int i = 0; i < keyList.Count - 1; i++)
                {
                    if(keyList[i+1] - keyList[i] != 1)
                    {
                        return keyList[i] + 1;
                    }
                }

                return availableMinimum;
            }
        }

        private void AddPrimaryKey(Object instance, bool single = false)
        {
            Type type;

            if (single)
            {
                type = _tableInheritance.GetMainType(instance.GetType());
            }
            else
            {
                type = instance.GetType();
            }

            int availableMinimum = FindMinimumAvailableID(type);

            if (!_dataDictionary.ContainsKey(type))
            {
                _dataDictionary.Add(type, new Dictionary<int, object> { { availableMinimum, instance } });
            }
            else
            {
                _dataDictionary[type].Add(availableMinimum, instance);
            }
        }

        private int FindPrimaryKey(Object instance, bool single = false)
        {
            if (single)
            {
                Type rootHierarchyType = _tableInheritance.GetMainType(instance.GetType());
                if (_dataDictionary.ContainsKey(rootHierarchyType))
                {
                    KeyValuePair<int, Object> obj = _dataDictionary[rootHierarchyType].FirstOrDefault(x => x.Value == instance);

                    if (!obj.Equals(default(KeyValuePair<int, Object>)))
                    {
                        return obj.Key;
                    }

                    return -1;
                }

                return -1;
            }
            else
            {
                if (_dataDictionary.ContainsKey(instance.GetType()))
                {
                    KeyValuePair<int, Object> obj = _dataDictionary[instance.GetType()].FirstOrDefault(x => x.Value == instance);

                    if (!obj.Equals(default(KeyValuePair<int, Object>)))
                    {
                        return obj.Key;
                    }

                    return -1;
                }

                return -1;
            }
        }

        private void DeletePrimaryKey(Object instance, bool single = false)
        {
            Type type;

            if (single)
            {
                type = _tableInheritance.GetMainType(instance.GetType());
            }
            else
            {
                type = instance.GetType();
            }

            if (_dataDictionary.ContainsKey(type))
            {
                int key = FindPrimaryKey(instance);
                _dataDictionary[type].Remove(key);
            }
        }

        private string GetMergedNames(string name1, string name2)
        {
            return string.Compare(name1, name2) < 0 ? name1 +"_"+ name2 : name2 +"_"+ name1;
        }


        private void AddForeignKey(Object mainInstance, Object childInstance)
        {
            string mainTableName = _dataMapper.GetTableName(mainInstance.GetType());
            string childTableName = _dataMapper.GetTableName(childInstance.GetType());
            string columnName = childTableName + "_id";
            int childPrimaryKey = FindMinimumAvailableID(childInstance.GetType());
            string addColumnQuery =  _queryBuilder.CreateAddColumnQuery(mainTableName,columnName,childPrimaryKey.GetType());
            string foreignKeyQuery = _queryBuilder.CreateAddForeignKeyQuery(mainTableName, columnName, "id", childTableName);

            PerformQuery(addColumnQuery);
            PerformQuery(foreignKeyQuery);
        }

        public void CreateTable(Object instance)
        {
            CreateTable(instance, "", "");
        }

        private void CreateTable(Object instance, string parentTableName, string foreignKeyName)
        {
            if (instance != null)
            {

                List<Tuple<string, Object>> columnsAndValuesList = _dataMapper.GetColumnsAndValues(instance);
                int primaryKey = FindMinimumAvailableID(instance.GetType());
                columnsAndValuesList.Add(new Tuple<string, Object>("id", primaryKey));
                string tableName = _dataMapper.GetTableName(instance.GetType());
                string query;

                if (parentTableName.Equals(""))
                {
                    query = _queryBuilder.CreateCreateTableQuery(tableName, columnsAndValuesList, "id");
                }
                else
                {
                    int foreignKey = FindMinimumAvailableID(instance.GetType());
                    Dictionary<string, Tuple<string, Object>> tableAndForeignKey = new Dictionary<string, Tuple<string, Object>>
                { {parentTableName, new Tuple<string, Object> (foreignKeyName, foreignKey)} };
                    query = _queryBuilder.CreateCreateTableQuery(tableName, columnsAndValuesList, "id", tableAndForeignKey);
                }

                PerformQuery(query);

                // foreign key mapping
                List<Relationship> oneToOne = _relationshipFinder.FindOneToOne(instance.GetType());
                List<Relationship> oneToMany = _relationshipFinder.FindOneToMany(instance.GetType());

                // association table mapping
                List<Relationship> manyToMany = _relationshipFinder.FindManyToMany(instance.GetType());

                if (oneToOne.Count != 0)
                {
                    foreach (var relation in oneToOne)
                    {
                        PropertyInfo property = relation._secondMember;
                        MethodInfo strGetter = property.GetGetMethod(nonPublic: true);
                        Object value = strGetter.Invoke(instance, null);
                        if (value != null)
                        {
                            CreateTable(value);
                            AddForeignKey(instance, value);
                        }
                    }
                }

                if (oneToMany.Count != 0)
                {
                    foreach (var relation in oneToMany)
                    {
                        PropertyInfo property = relation._secondMember;
                        MethodInfo strGetter = property.GetGetMethod(nonPublic: true);
                        var values = strGetter.Invoke(instance, null);
                        IList valueList = values as IList;

                        if (valueList.Count != 0)
                        {
                            CreateTable(valueList[0], tableName, "id");
                        }
                    }
                }

                if (manyToMany.Count != 0)
                {
                    foreach (var relation in manyToMany)
                    {
                        PropertyInfo property = relation._secondMember;
                        MethodInfo strGetter = property.GetGetMethod(nonPublic: true);
                        var values = strGetter.Invoke(instance, null);
                        IList valueList = values as IList;

                        if (valueList.Count != 0)
                        {
                            var secondInstance = valueList[0];
                            string memberTableName = _dataMapper.GetTableName(secondInstance.GetType());
                            string mergedTablesName = GetMergedNames(tableName, memberTableName);
                            int firstPrimaryKey = FindMinimumAvailableID(instance.GetType());
                            int secondPrimaryKey = FindMinimumAvailableID(secondInstance.GetType());

                            Dictionary<string, Tuple<string, Object>> tablesAndForeignKeys = new Dictionary<string, Tuple<string, Object>> {
                            { tableName, new Tuple<string, Object>("id", firstPrimaryKey) },
                            { memberTableName, new Tuple<string, Object> ("id", secondPrimaryKey) } };

                            CreateTable(secondInstance);
                            CreateAssociationTable(mergedTablesName, tablesAndForeignKeys);
                        }
                    }
                }
            }
        }

        private void CreateAssociationTable(string tableName, Dictionary<string, Tuple<string, Object>> tablesAndForeignKeys)
        {
            string query = _queryBuilder.CreateCreateTableQuery(tableName, null, "", tablesAndForeignKeys);

            PerformQuery(query);
        }

        private void CreateTable(Type objectType, List<PropertyInfo> inheritedProperties)
        {
            List<Tuple<string, Object>> columnsBasedOnProperties = _dataMapper.GetInheritedColumnsAndValues(inheritedProperties);
            columnsBasedOnProperties.Add(new Tuple<string, object>("id", 0));
            string tableName = _dataMapper.GetTableName(objectType);
            string query = _queryBuilder.CreateCreateTableQuery(tableName, columnsBasedOnProperties, "id");

            PerformQuery(query);
        }

        public string SelectAsString(Type type, List<SqlCondition> listOfSqlCondition)
        {
            string tableName = _dataMapper.GetTableName(type);

            if (!_msSqlConnection.CheckIfTableExists(tableName))
            {
                Type rootHierarchyType = _tableInheritance.GetMainType(type);
                tableName = _dataMapper.GetTableName(rootHierarchyType);
            }

            string selectQuery = _queryBuilder.CreateSelectQuery(tableName, listOfSqlCondition);
            string selectQueryOutput = _msSqlConnection.ExecuteSelectQuery(selectQuery, tableName);

            return selectQueryOutput;
        }

        public Object Select(Type type, int id)
        {
            List<SqlCondition> listOfSqlCondition = new List<SqlCondition> { SqlCondition.Equals("id", id) };
            _msSqlConnection.ConnectAndOpen();
            Object result = SelectWithOpenConnection(type, listOfSqlCondition, id);
            _msSqlConnection.Dispose();

            return result;
        }


        private Object SelectWithOpenConnection(Type type, List<SqlCondition> listOfSqlCondition, int primaryKey)
        {
            Object selectedObject;
            string tableName = _dataMapper.GetTableName(type);

            if (!_msSqlConnection.CheckIfTableExists(tableName))
            {
                Type rootHierarchyType = _tableInheritance.GetMainType(type);
                tableName = _dataMapper.GetTableName(rootHierarchyType);

                if (!_msSqlConnection.CheckIfTableExists(tableName))
                {
                    Console.WriteLine("Handled exception");
                    Console.WriteLine("This table is not present in database!");
                    Console.WriteLine("If single table inheritance is used then the root hierarchy table is not present!\n");

                    return null;
                }
            }

            if (primaryKey < 0)
            {
                return null;
            }

            string selectQuery = _queryBuilder.CreateSelectQuery(tableName, listOfSqlCondition);
            SqlDataReader dataReader = _msSqlConnection.ExecuteObjectSelect(selectQuery);

            if (!dataReader.HasRows)
            {
                return null;
            }

            dataReader.Read();

            IDataRecord record = (IDataRecord)dataReader;
            Object[] parameters = FetchDataFromRecord(type, record);
            selectedObject = Activator.CreateInstance(type, parameters);

            dataReader.Close();

            List<Relationship> oneToOne = _relationshipFinder.FindOneToOne(type);
            List<Relationship> oneToMany = _relationshipFinder.FindOneToMany(type);
            List<Relationship> manyToMany = _relationshipFinder.FindManyToMany(type);

            foreach (var relationship in oneToOne)
            {
                PropertyInfo property = relationship._secondMember;
                Type childType = property.PropertyType;
                string childTableName = _dataMapper.GetTableName(childType);
                int foreignKey = -1;

                if (_msSqlConnection.CheckIfTableExists(childTableName))
                {
                    foreignKey = GetForeignKeyFromTable(tableName, childTableName, primaryKey);
                }                   

                List<SqlCondition> condition = new List<SqlCondition> { SqlCondition.Equals("id", foreignKey) };
                Object child = SelectWithOpenConnection(childType, condition, foreignKey);

                property.SetValue(selectedObject, child, null);
            }

            foreach (var relationship in oneToMany)
            {
                PropertyInfo property = relationship._secondMember;
                Type childType = property.PropertyType.GetGenericArguments()[0];
                string childTableName = _dataMapper.GetTableName(childType);
                List<Object> children = new List<Object>();
                List<int> foreignKeys = GetForeignKeysFromTable(childTableName, tableName, primaryKey);
                    
                foreach (int fk in foreignKeys)
                {
                    List<SqlCondition> childCondition = new List<SqlCondition> { SqlCondition.Equals("id", fk) };
                    children.Add(SelectWithOpenConnection(childType, childCondition, fk));
                }

                IList childTmp = children as IList;
                IList list = Activator.CreateInstance(property.PropertyType) as IList;
                    
                foreach (var it in childTmp)
                {
                    list.Add(it);
                }

                property.SetValue(selectedObject, list, null);
            }

            foreach (var relationship in manyToMany)
            {
                PropertyInfo property = relationship._secondMember;
                Type childType = property.PropertyType.GetGenericArguments()[0];
                string childTableName = _dataMapper.GetTableName(childType);

                string associationTable = GetMergedNames(tableName, childTableName);

                if (!_msSqlConnection.CheckIfTableExists(associationTable))
                {
                    continue;
                }

                List<Object> childPrimaryKeys = SelectFromAssociationTable(associationTable, tableName + "_id", primaryKey);
                List<Object> children = new List<object>();

                foreach(var childPK in childPrimaryKeys)
                {
                    List<SqlCondition> condition = new List<SqlCondition> { SqlCondition.Equals("id", childPK) };
                    Object child = SelectWithOpenConnection(childType, condition, (int)childPK);
                    children.Add(child);
                }

                IList childTmp = children as IList;
                IList list = Activator.CreateInstance(property.PropertyType) as IList;

                foreach (var it in childTmp)
                {
                    list.Add(it);
                }

                property.SetValue(selectedObject, list, null);
            }

            return selectedObject;
        }

        private int GetForeignKeyFromTable(string tableName, string childTableName, int primaryKey)
        {
            List<SqlCondition> foreignKeyConditions = new List<SqlCondition> { SqlCondition.Equals("id", primaryKey) };
            string foreignKeyQuery = _queryBuilder.CreateSelectQuery(tableName, foreignKeyConditions);
            SqlDataReader foreignKeyReader = _msSqlConnection.ExecuteObjectSelect(foreignKeyQuery);

            foreignKeyReader.Read();

            IDataRecord record = (IDataRecord)foreignKeyReader;
            Dictionary<string, Object> columnsAndValues = _dataMapper.CreateDictionaryFromRecord(record);
            
            foreignKeyReader.Close();

            if ((columnsAndValues[childTableName + "_id"]).GetType() == typeof(System.DBNull))
            {
                return -1;
            }

            return (int)columnsAndValues[childTableName + "_id"];
        }

        private List<int> GetForeignKeysFromTable(string childTableName, string parentTableName, int primaryKey)
        {
            List<int> foreignKeys = new List<int>();

            if (!_msSqlConnection.CheckIfTableExists(childTableName))
            {
                return foreignKeys;
            }

            List<SqlCondition> foreignKeyConditions = new List<SqlCondition> { SqlCondition.Equals(parentTableName+"_id", primaryKey) };
            string foreignKeyQuery = _queryBuilder.CreateSelectQuery(childTableName, foreignKeyConditions);
            SqlDataReader foreignKeyReader = _msSqlConnection.ExecuteObjectSelect(foreignKeyQuery);

            while(foreignKeyReader.Read())
            {
                IDataRecord record = (IDataRecord)foreignKeyReader;
                Dictionary<string, Object> columnsAndValues = _dataMapper.CreateDictionaryFromRecord(record);
                foreignKeys.Add((int)columnsAndValues["id"]);
            }

            foreignKeyReader.Close();

            return foreignKeys;
        }

        private Object[] FetchDataFromRecord(Type type, IDataRecord record)
        {
            ConstructorInfo constructor = type.GetConstructors()[0];
            Dictionary<string, Object> columnsAndValues = _dataMapper.CreateDictionaryFromRecord(record);
            IEnumerable<string> parameterNames = (new List<ParameterInfo>(constructor.GetParameters()).Select(x => x.Name));
            PropertyInfo[] properties = DataMapper.GetTypeAllProperties(type);
            Object[] parameters = new Object[parameterNames.Count()];

            foreach (var property in properties)
            {
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

                if (columnsAndValues.ContainsKey(columnName))
                {
                    int index = parameterNames.ToList().IndexOf(property.Name);
                    parameters[index] = columnsAndValues[columnName];
                }
            }
            
            return parameters;
        }

        private List<Object> SelectFromAssociationTable(string tableName, string primaryKeyName, Object primaryKey)
        {
            List<Object> selectedObjects = new List<Object> { };
            List<SqlCondition> listOfSqlCondition = new List<SqlCondition>{ SqlCondition.Equals(primaryKeyName, primaryKey) };
            string selectQuery = _queryBuilder.CreateSelectQuery(tableName, listOfSqlCondition);
            SqlDataReader dataReader = _msSqlConnection.ExecuteObjectSelect(selectQuery);

            while(dataReader.Read())
            {
                IDataRecord record = (IDataRecord)dataReader;

                if (record.GetName(0) == primaryKeyName)
                {
                    selectedObjects.Add(record[1]);
                }
                else
                {
                    selectedObjects.Add(record[0]);
                }
            }

            dataReader.Close();

            return selectedObjects;
        }

        public void Insert(Object obj, Tuple<string, object> parentKey = null)
        {
            if (obj != null)
            {
                string tableName = _dataMapper.GetTableName(obj.GetType());
                List<Tuple<string, object>> columnsAndValuesList;
                int primaryKey;

                if (_msSqlConnection.CheckIfTableExists(tableName))
                {
                    // concrete table inheritance

                    if ((_msSqlConnection.GetColumnNamesFromTable(tableName)).Count - 1 == (DataMapper.GetTypeAllProperties(obj.GetType())).Length)
                    {
                        columnsAndValuesList = _dataMapper.GetColumnsAndValues(obj, true);
                        primaryKey = FindMinimumAvailableID(obj.GetType());
                        columnsAndValuesList.Add(new Tuple<string, Object>("id", primaryKey));
                        AddPrimaryKey(obj);
                    }
                    // class table inheritance or normal insert on single class
                    else
                    {
                        columnsAndValuesList = _dataMapper.GetColumnsAndValues(obj);
                        primaryKey = FindMinimumAvailableID(obj.GetType());
                        columnsAndValuesList.Add(new Tuple<string, Object>("id", primaryKey));
                        AddPrimaryKey(obj);
                    }
                }
                // single table inheritance
                else
                {
                    Type rootHierarchyType = _tableInheritance.GetMainType(obj.GetType());
                    tableName = _dataMapper.GetTableName(rootHierarchyType);
                    columnsAndValuesList = _dataMapper.GetColumnsAndValues(obj, true);
                    primaryKey = FindMinimumAvailableID(obj.GetType(), true);
                    columnsAndValuesList.Add(new Tuple<string, Object>("id", primaryKey));
                    AddPrimaryKey(obj, true);
                }

                // relationships lists
                List<Relationship> oneToOne = _relationshipFinder.FindOneToOne(obj.GetType());
                List<Relationship> oneToMany = _relationshipFinder.FindOneToMany(obj.GetType());
                List<Relationship> manyToMany = _relationshipFinder.FindManyToMany(obj.GetType());

                string insertQuery;

                if (parentKey != null)
                {
                    columnsAndValuesList.Add(parentKey);
                    insertQuery = _queryBuilder.CreateInsertQuery(tableName, columnsAndValuesList);
                }
                else
                {
                    insertQuery = _queryBuilder.CreateInsertQuery(tableName, columnsAndValuesList);
                }

                PerformQuery(insertQuery);

                if (oneToOne.Count != 0)
                {
                    foreach (var relation in oneToOne)
                    {
                        PropertyInfo propertyObj = relation._secondMember;
                        MethodInfo getter = propertyObj.GetGetMethod(nonPublic: true);
                        Object child = getter.Invoke(obj, null);
                        if (child != null)
                        {
                            int childPrimaryKey = FindMinimumAvailableID(child.GetType());
                            string childTableName = _dataMapper.GetTableName(child.GetType());

                            List<Tuple<string, object>> valuesToSet = new List<Tuple<string, object>>
                            {new Tuple<string,object>(childTableName + "_id", childPrimaryKey)};

                            List<SqlCondition> updateConditions = new List<SqlCondition> { SqlCondition.Equals("id", primaryKey) };

                            Insert(child);
                            Update(obj.GetType(), valuesToSet, updateConditions);
                        }
                    }
                }

                if (oneToMany.Count != 0)
                {
                    foreach (var relation in oneToMany)
                    {
                        PropertyInfo propertyObj = relation._secondMember;
                        MethodInfo getter = propertyObj.GetGetMethod(nonPublic: true);
                        Object child = getter.Invoke(obj, null);
                        IList childList = child as IList;

                        foreach (var item in childList)
                        {
                            Tuple<string, object> parentKeyTuple = new Tuple<string, object>(tableName +"_id", primaryKey);
                            Insert(item, parentKeyTuple);
                        }
                    }
                }

                if (manyToMany.Count != 0)
                {
                    foreach (var relation in manyToMany)
                    {
                        PropertyInfo propertyObj = relation._secondMember;
                        MethodInfo getter = propertyObj.GetGetMethod(nonPublic: true);
                        Object child = getter.Invoke(obj, null);
                        IList childList = child as IList;

                        foreach (var item in childList)
                        {
                            int secondMemberKey = FindMinimumAvailableID(item.GetType());
                            string childTableName = _dataMapper.GetTableName(item.GetType());

                            Insert(item);

                            Tuple<string, object> oneTableKey = new Tuple<string, object>(tableName +"_id", primaryKey);
                            Tuple<string, object> secondTableKey = new Tuple<string, object>(childTableName +"_id", secondMemberKey);
                            string associationTableName = GetMergedNames((string)tableName, (string)childTableName);

                            _msSqlConnection.ConnectAndOpen();
                            List<Object> valuesFromAssociationTable = SelectFromAssociationTable(associationTableName, childTableName +"_id", secondMemberKey); ;
                            _msSqlConnection.Dispose();

                            if (!valuesFromAssociationTable.Contains(primaryKey))
                            {
                                List<Tuple<string, object>> keysAndValues = new List<Tuple<string, object>> { oneTableKey, secondTableKey };

                                string intoAssocTableInsertQuery = _queryBuilder.CreateInsertQuery(associationTableName, keysAndValues);

                                PerformQuery(intoAssocTableInsertQuery);
                            }
                        }
                    }
                }

            }
            
        }

        public void Delete(Object obj)
        {
            int primaryKey;
            string tableName = _dataMapper.GetTableName(obj.GetType());

            if (_msSqlConnection.CheckIfTableExists(tableName))
            {
                // concrete table inheritance, class table inheritance or normal insert on single class
                primaryKey = FindPrimaryKey(obj);
                DeletePrimaryKey(obj);
            }
            // single table inheritance
            else
            {
                Type rootHierarchyType = _tableInheritance.GetMainType(obj.GetType());
                tableName = _dataMapper.GetTableName(rootHierarchyType);
                primaryKey = FindPrimaryKey(obj, true);
                DeletePrimaryKey(obj, true);
            }

            List<SqlCondition> listOfCriteria = new List<SqlCondition> { SqlCondition.Equals("id", primaryKey) };
            string query = _queryBuilder.CreateDeleteQuery(tableName, listOfCriteria);

            PerformQuery(query);
        }

        public void Update(Object instance)
        {
            int primaryKey;
            List<Tuple<string, Object>> valuesToSet;
            string tableName = _dataMapper.GetTableName(instance.GetType());

            if (!_msSqlConnection.CheckIfTableExists(tableName))
            {
                primaryKey = FindPrimaryKey(instance, true);
                valuesToSet = _dataMapper.GetColumnsAndValues(instance, true);
            }
            else
            {
                primaryKey = FindPrimaryKey(instance);
                valuesToSet = _dataMapper.GetColumnsAndValues(instance);
            }

            List<SqlCondition> updateConditions = new List<SqlCondition> { SqlCondition.Equals("id", primaryKey) };

            Update(instance.GetType(), valuesToSet, updateConditions);
        }

        private void Update(Type type, List<Tuple<string, object>> valuesToSet, List<SqlCondition> conditions)
        {
            string tableName = _dataMapper.GetTableName(type);

            if (!_msSqlConnection.CheckIfTableExists(tableName))
            {
                Type rootHierarchyType = _tableInheritance.GetMainType(type);
                tableName = _dataMapper.GetTableName(rootHierarchyType);
            }

            string updateQuery = _queryBuilder.CreateUpdateQuery(tableName, valuesToSet, conditions);
            PerformQuery(updateQuery);
        }

        public void Inherit(List<Object> lastMembersOfInheritanceHierarchy, int mode)
        {
            try
            {
                TryToInherit(lastMembersOfInheritanceHierarchy, mode);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message+"\n");
            }
        }

        public void TryToInherit(List<Object> lastMembersOfInheritanceHierarchy, int mode)
        {
            switch (mode)
            {
                case 0: //SingleInheritance
                    List<PropertyInfo> memberList = _tableInheritance.InheritSingle(lastMembersOfInheritanceHierarchy);
                    Type mainType = _tableInheritance.GetMainType((lastMembersOfInheritanceHierarchy[0]).GetType());
                    CreateTable(mainType, memberList);

                    break;
                case 1: //ClassInheritance
                    Dictionary<Type, List<PropertyInfo>> typeMap = _tableInheritance.InheritClass(lastMembersOfInheritanceHierarchy);

                    foreach (var pair in typeMap)
                    {
                        CreateTable(pair.Key, pair.Value);
                    }

                    break;
                case 2: //ConcreteInheritance
                    Dictionary<Type, List<PropertyInfo>> singleMap = _tableInheritance.InheritConcrete(lastMembersOfInheritanceHierarchy);

                    foreach (var pair in singleMap)
                    {
                        CreateTable(pair.Key, pair.Value);
                    }

                    break;
                default:
                    throw new ArgumentException("Incorrect value", nameof(mode));
            }
        }

        public void PerformQuery(string query)
        {
            _msSqlConnection.ConnectAndOpen();
            _msSqlConnection.ExecuteQuery(query);
            _msSqlConnection.Dispose();
        }


    }
}
