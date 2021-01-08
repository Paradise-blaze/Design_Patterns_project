using System;
using System.Collections.Generic;
using System.Text;

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

        public void CreateTable()
        {

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
    }
}
