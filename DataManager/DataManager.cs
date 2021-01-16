using System;
using System.Collections.Generic;
using System.Reflection;
using Design_Patterns_project.Relationships;
using orm.Query;
using Design_Patterns_project.Connection;

namespace Design_Patterns_project
{
    class DataManager
    {
        //DataMapper _dataMapper = new DataMapper();
        MsSqlConnection _msSqlConnection;
        QueryBuilder _queryBuilder = new QueryBuilder();
        //WorkUnit _workUnit = new WorkUnit();
        TableInheritance _tableInheritance = new TableInheritance();
        RelationshipFinder _relationshipFinder = new RelationshipFinder();
        /*List<Relationship> _oneToOneRelationships = new List<Relationship>();
        List<Relationship> _oneToManyRelationships = new List<Relationship>();
        List<Relationship> _manyToManyRelationships = new List<Relationship>();*/ //these variables should be in functions

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

        public void CreateTable(Type objectType, List<MemberInfo> fList)
        {
            //string query = _queryBuilder.GetCreateQuery(name, fList);
            //_databaseConnection.ExecuteCreateCommand(query);
            /*FieldInfo[] fieldArray = name.GetType().GetFields();
            foreach (var attr in fieldArray[0].GetCustomAttributes())
                Console.WriteLine(attr);
            Console.WriteLine();*/

            List<Relationship> oneToMany = this._relationshipFinder.FindOneToMany(objectType);

            foreach (var rel in oneToMany)
                rel.PrintInfo();
            
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

        public void Inherit(List<Object> lastMembers, int mode)
        {
            try
            {
                TryInherit(lastMembers, mode);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void TryInherit(List<Object> lastMembers, int mode)
        {
            switch (mode)
            {
                case 0: //SingleInheritance
                    List<MemberInfo> memberList = _tableInheritance.InheritSingle(lastMembers);
                    Type mainType = _tableInheritance.GetMainType(lastMembers[0]);
                    CreateTable(mainType, memberList);
                    
                    break;
                case 1: //ClassInheritance
                    Dictionary<Type, List<MemberInfo>> typeMap = _tableInheritance.InheritClass(lastMembers);

                    foreach (var pair in typeMap)
                        CreateTable(pair.Key, pair.Value);

                    break;
                case 2: //ConcreteInheritance
                    Dictionary<Type, List<MemberInfo>> singleMap = _tableInheritance.InheritConcrete(lastMembers);

                    foreach (var pair in singleMap)
                        CreateTable(pair.Key, pair.Value);

                    break;
                default:
                    throw new ArgumentException("Incorrect value", nameof(mode));
            }
        }
    }
}
