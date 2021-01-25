using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
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
            string primaryKeyName = _dataMapper.FindPrimaryKeyFieldName(instance);
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
            
            _msSqlConnection.ConnectAndOpen();
            _msSqlConnection.ExecuteQuery(query);
            _msSqlConnection.Dispose();
            
            // foreign key mapping
            List<Relationship> oneToOne = _relationshipFinder.FindOneToOne(instance);
            List<Relationship> oneToMany = _relationshipFinder.FindOneToMany(instance);
            // association table mapping
            List<Relationship> manyToMany = _relationshipFinder.FindManyToMany(instance);

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
                    string memberTableKeyName = _dataMapper.FindPrimaryKeyFieldName(secondInstance);
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
            
            _msSqlConnection.ConnectAndOpen();
            _msSqlConnection.ExecuteQuery(query);
            _msSqlConnection.Dispose();
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

            _msSqlConnection.ConnectAndOpen();
            _msSqlConnection.ExecuteQuery(query);
            _msSqlConnection.Dispose();
        }

        public void Select() //void temporary, need to return something
        {
            
        }

        public void Insert(Object obj, Tuple<string,object> parentKey = null)
        {
            string tableName = _dataMapper.GetTableName(obj.GetType());
            List<Tuple<string, object>> columnsAndValuesList = _dataMapper.GetColumnsAndValues(obj);


            object primaryKey = _dataMapper.FindPrimaryKey(obj); 
            object primaryKeyName = _dataMapper.FindPrimaryKeyFieldName(obj);
            
            // relationships lists
            List<Relationship> oneToOne = _relationshipFinder.FindOneToOne(obj);
            List<Relationship> oneToMany = _relationshipFinder.FindOneToMany(obj);
            List<Relationship> manyToMany = _relationshipFinder.FindManyToMany(obj);

            QueryBuilder query = new QueryBuilder();
            string insertQuery;

            if (parentKey != null){
                columnsAndValuesList.Add(parentKey);
                insertQuery = query.CreateInsertQuery(tableName,columnsAndValuesList);
            }else{
                insertQuery = query.CreateInsertQuery(tableName,columnsAndValuesList);
            }
            

            _msSqlConnection.ConnectAndOpen();
            _msSqlConnection.ExecuteQuery(insertQuery);
            _msSqlConnection.Dispose();


            if(oneToOne.Count != 0){
                foreach (var relation in oneToOne)
                {
                    PropertyInfo propertyObj = relation._secondMember;
                    MethodInfo getter = propertyObj.GetGetMethod(nonPublic: true);
                    Object secondMemberObject = getter.Invoke(obj, null);
                    Tuple<string,object> parentKeyTuple = new Tuple<string,object>( tableName+(string)primaryKeyName, primaryKey);
                    
                    Insert(secondMemberObject, parentKeyTuple);
                }
            }

            if(oneToMany.Count != 0){
                foreach (var relation in oneToMany)
                {
                    PropertyInfo propertyObj = relation._secondMember;
                    MethodInfo getter = propertyObj.GetGetMethod(nonPublic: true);
                    Object secondMemberObject = getter.Invoke(obj, null);
                    IList secondMemberObjectList = secondMemberObject as IList;

                    foreach (var item in secondMemberObjectList)
                    {
                        Tuple<string,object> parentKeyTuple = new Tuple<string,object>( tableName+(string)primaryKeyName, primaryKey);
                        Insert(item,parentKeyTuple);
                    }
                }
            }

            if(manyToMany.Count != 0){
                foreach (var relation in manyToMany)
                {

                    PropertyInfo propertyObj = relation._secondMember;
                    MethodInfo getter = propertyObj.GetGetMethod(nonPublic: true);
                    Object secondMemberObject = getter.Invoke(obj, null);
                    IList secondMemberObjectList = secondMemberObject as IList;

                    foreach (var item in secondMemberObjectList)
                    {
                        Object secondMemberKey = _dataMapper.FindPrimaryKey(item);
                        Object secondMemberKeyName = _dataMapper.FindPrimaryKeyFieldName(item);
                        string secondMemberTableName = _dataMapper.GetTableName(item.GetType());

                        Insert(item);

                        Tuple<string,object> oneTableKey = new Tuple<string,object>(tableName+primaryKeyName,primaryKey);
                        Tuple<string,object> secondTableKey = new Tuple<string,object>(secondMemberTableName+secondMemberKeyName,secondMemberKey);
                        List<Tuple<string, object>> keysAndValues = new List<Tuple<string, object>>{oneTableKey,secondTableKey};

                        string associationTableName = GetMergedNames((string)tableName, (string)secondMemberTableName);
                        
                        string intoAssocTableInsertQuery = query.CreateInsertQuery(associationTableName,keysAndValues);

                        _msSqlConnection.ConnectAndOpen();
                        _msSqlConnection.ExecuteQuery(intoAssocTableInsertQuery);
                        _msSqlConnection.Dispose();

                    }
                }
            }

        }

        public void Delete() 
        { 
        
        }

        public void Update()
        {

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
                    Type mainType = _tableInheritance.GetMainType(lastMembersOfInheritanceHierarchy[0]);
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
    }
}
