using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using Design_Patterns_project.Relationships;
using Design_Patterns_project.SqlCommands;
using Design_Patterns_project.Connection;

namespace Design_Patterns_project
{
    class DataManager
    {
        DataMapper _dataMapper = new DataMapper();
        MsSqlConnection _msSqlConnection;
        QueryBuilder _queryBuilder = new QueryBuilder();
        //WorkUnit _workUnit = new WorkUnit();
        TableInheritance _tableInheritance = new TableInheritance();
        RelationshipFinder _relationshipFinder = new RelationshipFinder();

        public DataManager() { } //for tests only
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

        public string GetMergedNames(string name1, string name2)
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
            string tableName = _dataMapper.GetTableName(instance);
            string query;

            if (parentTableName.Equals(""))
            {
                query = _queryBuilder.CreateCreateTableQuery(tableName, columnsAndValuesList, primaryKeyName);
            }
            else
            {
                Dictionary<string, string> tableAndForeignKey = new Dictionary<string, string> { {parentTableName, foreignKeyName } };
                query = _queryBuilder.CreateCreateTableQuery(tableName, columnsAndValuesList, primaryKeyName, tableAndForeignKey);
            }
            /*
            _msSqlConnection.ConnectAndOpen();
            _msSqlConnection.ExecuteQuery(query);
            _msSqlConnection.Dispose();
            */
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

                    CreateTable(valueList[0], parentTableName, primaryKeyName);
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
                    
                    string memberTableName = _dataMapper.GetTableName(secondInstance);
                    string memberTableKeyName = _dataMapper.FindPrimaryKeyFieldName(secondInstance);
                    string mergedTablesName = GetMergedNames(tableName, memberTableName);

                    List<Tuple<string, Object>> foreignKeys = _dataMapper.GetAssociationTable(instance, secondInstance);
                    Dictionary<string, string> tablesAndForeignKeys = new Dictionary<string, string> { { tableName, primaryKeyName }, { memberTableName, memberTableKeyName } };

                    CreateAssociationTable(mergedTablesName, tablesAndForeignKeys, foreignKeys);
                }
            }
        }

        private void CreateAssociationTable(string tableName, Dictionary<string, string> tablesAndForeignKeys, List<Tuple<string, Object>> foreignKeys)
        {
            string query = _queryBuilder.CreateCreateTableQuery(tableName, foreignKeys, "", tablesAndForeignKeys);
            /*
            _msSqlConnection.ConnectAndOpen();
            _msSqlConnection.ExecuteQuery(query);
            _msSqlConnection.Dispose();
            */
        }

        private void CreateTable(Type objectType, List<PropertyInfo> columnsBasedOnProperties)
        {
            //string query = _queryBuilder.GetCreateQuery(name, fList);
            //_databaseConnection.ExecuteCreateCommand(query);
            /*FieldInfo[] fieldArray = name.GetType().GetFields();
            foreach (var attr in fieldArray[0].GetCustomAttributes())
                Console.WriteLine(attr);
            Console.WriteLine();

            List<Relationship> oneToMany = this._relationshipFinder.FindOneToMany(objectType);

            foreach (var rel in oneToMany)
            {
                rel.PrintInfo();
            }*/
            
        }

        public void Select()
        {
            
        }

        public void Insert()
        {

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
