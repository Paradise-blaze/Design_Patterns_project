using System;
using System.Collections.Generic;
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

        public void OpenConnection()
        {
            this._msSqlConnection.ConnectAndOpen();
        }

        public void CloseConnection()
        {
            this._msSqlConnection.Dispose();
        }

        public void CreateTable(Type objectType)
        {

        }

        public void CreateTable(Type objectType, List<PropertyInfo> columnsBasedOnProperties)
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

            Dragon d = new Dragon(200, "Something", 20, 10);

            Console.WriteLine(_dataMapper.ConvertObjectNameToString(d));
            
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
                TryInherit(lastMembersOfInheritanceHierarchy, mode);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void TryInherit(List<Object> lastMembersOfInheritanceHierarchy, int mode)
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
