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

        private string GetMergedNames(string name1, string name2)
        {
            return string.Compare(name1, name2) < 0 ? name1 + name2 : name2 + name1;
        }

        public void CreateTable(Object instance)
        {
            CreateTable(instance, "", "");
        }

        private void CreateTable(Object instance, string parentTableName, string foreignKeyName)
        {
            List<Tuple<string, Object>> columnsAndValuesList = _dataMapper.GetColumnsAndValues(instance);
            string primaryKeyName = _dataMapper.FindPrimaryKeyFieldName(instance.GetType());
            string tableName = _dataMapper.GetTableName(instance.GetType());
            string query;

            if (parentTableName.Equals(""))
            {
                query = _queryBuilder.CreateCreateTableQuery(tableName, columnsAndValuesList, primaryKeyName);
            }
            else
            {
                Object foreignKey = _dataMapper.FindPrimaryKey(instance);
                Dictionary<string, Tuple<string, Object>> tableAndForeignKey = new Dictionary<string, Tuple<string, Object>>
                { {parentTableName, new Tuple<string, Object> (foreignKeyName, foreignKey)} };
                query = _queryBuilder.CreateCreateTableQuery(tableName, columnsAndValuesList, primaryKeyName, tableAndForeignKey);
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
                    CreateTable(value, tableName, primaryKeyName);
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

                    CreateTable(valueList[0], tableName, primaryKeyName);
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
                    var secondInstance = valueList[0];

                    string memberTableName = _dataMapper.GetTableName(secondInstance.GetType());
                    string memberTableKeyName = _dataMapper.FindPrimaryKeyFieldName(secondInstance.GetType());
                    string mergedTablesName = GetMergedNames(tableName, memberTableName);
                    Object firstPrimaryKey = _dataMapper.FindPrimaryKey(instance);
                    Object secondPrimaryKey = _dataMapper.FindPrimaryKey(secondInstance);

                    Dictionary<string, Tuple<string, Object>> tablesAndForeignKeys = new Dictionary<string, Tuple<string, Object>> {
                        { tableName, new Tuple<string, Object>(primaryKeyName, firstPrimaryKey) },
                        { memberTableName, new Tuple<string, Object> (memberTableKeyName, secondPrimaryKey) } };

                    CreateTable(secondInstance);
                    CreateAssociationTable(mergedTablesName, tablesAndForeignKeys);
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
            string tableName = _dataMapper.GetTableName(objectType);

            PropertyInfo primaryProperty = inheritedProperties.Find(x => x.GetCustomAttribute(typeof(PKeyAttribute), false) != null);
            string primaryKeyName = primaryProperty != null ?
                (((ColumnAttribute)primaryProperty.GetCustomAttribute(typeof(ColumnAttribute), false))._columnName ?? primaryProperty.Name)
                : "";
            string query = _queryBuilder.CreateCreateTableQuery(tableName, columnsBasedOnProperties, primaryKeyName);

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

        public List<Object> Select(Type type, List<SqlCondition> listOfSqlCondition)
        {
            _msSqlConnection.ConnectAndOpen();
            List<Object> result = SelectWithOpenConnection(type, listOfSqlCondition);
            _msSqlConnection.Dispose();

            return result;
        }


        private List<Object> SelectWithOpenConnection(Type type, List<SqlCondition> listOfSqlCondition)
        {
            List<Object> selectedObjects = new List<Object> {};
            string tableName = _dataMapper.GetTableName(type);

            if (!_msSqlConnection.CheckIfTableExists(tableName))
            {
                Type rootHierarchyType = _tableInheritance.GetMainType(type);
                tableName = _dataMapper.GetTableName(rootHierarchyType);
            }

            string selectQuery = _queryBuilder.CreateSelectQuery(tableName, listOfSqlCondition);
            SqlDataReader dataReader = _msSqlConnection.ExecuteObjectSelect(selectQuery);
            
            while(dataReader.Read())
            {
                IDataRecord record = (IDataRecord)dataReader;
                Object[] parameters = FetchDataFromRecord(type, record);
                Object o = Activator.CreateInstance(type, parameters);
                selectedObjects.Add(o);
            }

            dataReader.Close();

            List<Relationship> oneToOne = _relationshipFinder.FindOneToOne(type);
            List<Relationship> oneToMany = _relationshipFinder.FindOneToMany(type);
            List<Relationship> manyToMany = _relationshipFinder.FindManyToMany(type);

            foreach (var obj in selectedObjects)
            {
                foreach (var relationship in oneToOne)
                {
                    PropertyInfo property = relationship._secondMember;
                    Type childType = property.PropertyType;
                    string primaryKeyName = _dataMapper.FindPrimaryKeyFieldName(type);
                    var primaryKey = _dataMapper.FindPrimaryKey(obj);
                    List<SqlCondition> condition = new List<SqlCondition> { SqlCondition.Equals(tableName+primaryKeyName, primaryKey) };
                    List<Object> children = SelectWithOpenConnection(childType, condition);
                    Object child = null;

                    if (children.Count() != 0)
                    {
                        child = children[0];
                    }

                    property.SetValue(obj, child, null);
                }

                foreach (var relationship in oneToMany)
                {
                    PropertyInfo property = relationship._secondMember;
                    Type childType = property.PropertyType.GetGenericArguments()[0];
                    string primaryKeyName = _dataMapper.FindPrimaryKeyFieldName(type);
                    var primaryKey = _dataMapper.FindPrimaryKey(obj);
                    List<SqlCondition> condition = new List<SqlCondition> { SqlCondition.Equals(tableName + primaryKeyName, primaryKey) };
                    List<Object> children = SelectWithOpenConnection(childType, condition);

                    IList childTmp = children as IList;
                    IList list = Activator.CreateInstance(property.PropertyType) as IList;
                    
                    foreach (var it in childTmp)
                    {
                        list.Add(it);
                    }

                    property.SetValue(obj, list, null);
                }

                foreach (var relationship in manyToMany)
                {
                    PropertyInfo property = relationship._secondMember;
                    Type childType = property.PropertyType.GetGenericArguments()[0];
                    string childTableName = _dataMapper.GetTableName(childType);
                    string childPrimaryKeyName = _dataMapper.FindPrimaryKeyFieldName(type);
                    string primaryKeyName = _dataMapper.FindPrimaryKeyFieldName(type);
                    var primaryKey = _dataMapper.FindPrimaryKey(obj);

                    string associationTable = GetMergedNames(tableName, childTableName);
                    List<Object> childPrimaryKeys = SelectFromAssociationTable(associationTable, tableName + primaryKeyName, primaryKey);
                    List<Object> children = new List<object>();

                    foreach(var childPK in childPrimaryKeys)
                    {
                        List<SqlCondition> condition = new List<SqlCondition> { SqlCondition.Equals(childPrimaryKeyName, childPK) };
                        Object child = SelectWithOpenConnection(childType, condition)[0];
                        children.Add(child);
                    }

                    IList childTmp = children as IList;
                    IList list = Activator.CreateInstance(property.PropertyType) as IList;

                    foreach (var it in childTmp)
                    {
                        list.Add(it);
                    }

                    property.SetValue(obj, list, null);
                }
            }

            return selectedObjects;
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
            Console.WriteLine(primaryKey);
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
            if (obj != null){

                string tableName = _dataMapper.GetTableName(obj.GetType());
                List<Tuple<string, object>> columnsAndValuesList;
                object primaryKey;
                object primaryKeyName;

                if (_msSqlConnection.CheckIfTableExists(tableName))
                {
                    // concrete table inheritance
                    if ((_msSqlConnection.GetColumnNamesFromTable(tableName)).Count == (DataMapper.GetTypeAllProperties(obj.GetType())).Length)
                    {
                        columnsAndValuesList = _dataMapper.GetColumnsAndValues(obj, true);
                        primaryKey = _dataMapper.FindPrimaryKey(obj, true);
                        primaryKeyName = _dataMapper.FindPrimaryKeyFieldName(obj.GetType(), true);
                    }
                    // class table inheritance or normal insert on single class
                    else
                    {
                        columnsAndValuesList = _dataMapper.GetColumnsAndValues(obj);
                        primaryKey = _dataMapper.FindPrimaryKey(obj);
                        primaryKeyName = _dataMapper.FindPrimaryKeyFieldName(obj.GetType());
                    }
                }
                // single table inheritance
                else
                {
                    Type rootHierarchyType = _tableInheritance.GetMainType(obj.GetType());
                    tableName = _dataMapper.GetTableName(rootHierarchyType);
                    columnsAndValuesList = _dataMapper.GetColumnsAndValues(obj, true);
                    primaryKey = _dataMapper.FindPrimaryKey(obj, true);
                    primaryKeyName = _dataMapper.FindPrimaryKeyFieldName(obj.GetType(), true);
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
                        Object secondMemberObject = getter.Invoke(obj, null);
                        Tuple<string, object> parentKeyTuple = new Tuple<string, object>(tableName + (string)primaryKeyName, primaryKey);

                        Insert(secondMemberObject, parentKeyTuple);
                    }
                }

                if (oneToMany.Count != 0)
                {
                    foreach (var relation in oneToMany)
                    {
                        PropertyInfo propertyObj = relation._secondMember;
                        MethodInfo getter = propertyObj.GetGetMethod(nonPublic: true);
                        Object secondMemberObject = getter.Invoke(obj, null);
                        IList secondMemberObjectList = secondMemberObject as IList;

                        foreach (var item in secondMemberObjectList)
                        {
                            Tuple<string, object> parentKeyTuple = new Tuple<string, object>(tableName + (string)primaryKeyName, primaryKey);
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
                        Object secondMemberObject = getter.Invoke(obj, null);
                        IList secondMemberObjectList = secondMemberObject as IList;

                        foreach (var item in secondMemberObjectList)
                        {
                            Object secondMemberKey = _dataMapper.FindPrimaryKey(item);
                            string secondMemberKeyName = _dataMapper.FindPrimaryKeyFieldName(item.GetType());
                            string secondMemberTableName = _dataMapper.GetTableName(item.GetType());

                            Insert(item);

                            Tuple<string, object> oneTableKey = new Tuple<string, object>(tableName + primaryKeyName, primaryKey);
                            Tuple<string, object> secondTableKey = new Tuple<string, object>(secondMemberTableName + secondMemberKeyName, secondMemberKey);
                            string associationTableName = GetMergedNames((string)tableName, (string)secondMemberTableName);

                            _msSqlConnection.ConnectAndOpen();
                            List<Object> valuesFromAssociationTable = SelectFromAssociationTable(associationTableName, secondMemberTableName + secondMemberKeyName, secondMemberKey); ;
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
            string primaryKeyName;
            Object primaryKey;
            string tableName = _dataMapper.GetTableName(obj.GetType());

            if (_msSqlConnection.CheckIfTableExists(tableName))
            {
                // concrete table inheritance
                if ((_msSqlConnection.GetColumnNamesFromTable(tableName)).Count == (DataMapper.GetTypeAllProperties(obj.GetType())).Length)
                {
                    primaryKey = _dataMapper.FindPrimaryKey(obj, true);
                    primaryKeyName = _dataMapper.FindPrimaryKeyFieldName(obj.GetType(), true);
                }
                // class table inheritance or normal insert on single class
                else
                {
                    primaryKey = _dataMapper.FindPrimaryKey(obj);
                    primaryKeyName = _dataMapper.FindPrimaryKeyFieldName(obj.GetType());
                }
            }
            // single table inheritance
            else
            {
                Type rootHierarchyType = _tableInheritance.GetMainType(obj.GetType());
                tableName = _dataMapper.GetTableName(rootHierarchyType);
                primaryKey = _dataMapper.FindPrimaryKey(obj, true);
                primaryKeyName = _dataMapper.FindPrimaryKeyFieldName(obj.GetType(), true);
            }

            List<SqlCondition> listOfCriteria = new List<SqlCondition> { SqlCondition.Equals(primaryKeyName, primaryKey) };
            string query = _queryBuilder.CreateDeleteQuery(tableName, listOfCriteria);

            PerformQuery(query);
        }

        public void Delete(string tableName, List<SqlCondition> listOfCriteria)
        {
            String query = _queryBuilder.CreateDeleteQuery(tableName, listOfCriteria);
            PerformQuery(query);
        }

        public void Update(Type type, List<Tuple<string, object>> valuesToSet, List<SqlCondition> conditions)
        {
            string tableName = _dataMapper.GetTableName(type);

            if (!_msSqlConnection.CheckIfTableExists(tableName))
            {
                Type rootHierarchyType = _tableInheritance.GetMainType(type);
                tableName = _dataMapper.GetTableName(rootHierarchyType);
            }

            string updateQuery = _queryBuilder.CreateUpdateQuery(tableName,valuesToSet, conditions);
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
                Console.WriteLine(e.Message);
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
