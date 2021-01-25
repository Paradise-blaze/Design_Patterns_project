using System;
using System.Collections.Generic;
using System.Reflection;

namespace Design_Patterns_project
{
    class DataManager
    {
        //DataMapper _dataMapper = new DataMapper();
        //DatabaseConnection _databaseConnection = new DatabaseConnection();
        //QueryBuilder _queryBuilder = new QueryBuilder();
        //WorkUnit _workUnit = new WorkUnit();
        TableInheritance _tableInheritance = new TableInheritance();
        Dictionary<Object, Object> _oneToOneRelationships = new Dictionary<Object, Object>();
        Dictionary<Object, Object> _oneToManyRelationships = new Dictionary<Object, Object>();
        Dictionary<Object, Object> _manyToManyRelationships = new Dictionary<Object, Object>();

        public void CreateTable(string name, List<FieldInfo> fList)
        {
            //string query = _queryBuilder.GetCreateQuery(name, fList);
            //_databaseConnection.ExecuteCreateCommand(query);
            /*FieldInfo[] fieldArray = name.GetType().GetFields();
            foreach (var attr in fieldArray[0].GetCustomAttributes())
                Console.WriteLine(attr);
            Console.WriteLine();*/
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
                    List<FieldInfo> fieldList = _tableInheritance.InheritSingle(lastMembers);
                    Type mainType = _tableInheritance.GetMainType(lastMembers[0]);
                    CreateTable(mainType.Name, fieldList);

                    break;
                case 1: //ClassInheritance
                    Dictionary<Type, List<FieldInfo>> typeMap = _tableInheritance.InheritClass(lastMembers);

                    foreach (var pair in typeMap)
                        CreateTable(pair.Key.Name, pair.Value);

                    break;
                case 2: //ConcreteInheritance
                    Dictionary<Type, List<FieldInfo>> singleMap = _tableInheritance.InheritConcrete(lastMembers);

                    foreach (var pair in singleMap)
                        CreateTable(pair.Key.Name, pair.Value);

                    break;
                default:
                    throw new ArgumentException("Incorrect value", nameof(mode));
            }
        }
    }
}
