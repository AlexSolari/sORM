using sORM.Core.Conditions;
using sORM.Core.Mappings;
using sORM.Core.Requests;
using sORM.Core.Requests.Concrete;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core
{
    public class SimpleORM
    {
        #region Singleton
        private static SimpleORM _instance;

        private SimpleORM()
	    {

	    }

        public static SimpleORM Current 
        {
            get
            {
                if (_instance == null)
                    _instance = new SimpleORM();

                return _instance;
            }
        }
        #endregion


        internal Dictionary<Type, Map> Mappings = new Dictionary<Type, Map>();

        internal RequestProcessor Requests = null;

        internal MapBinder Mapper = new MapBinder();

        public void AddOnRequestListener(Action<string> action)
        {
            Requests.AddOnRequestListener(action);
        }

        public void Initialize(string connectionString)
        {
            Requests = new RequestProcessor(connectionString);

            var types = Assembly.GetCallingAssembly().DefinedTypes;
            foreach (var type in types)
            {
                if (type.GetCustomAttribute(typeof(DataModelAttribute)) != null)
                {
                    Mapper.Map(type);
                }
            }

            Requests.Initialize();

            var prepareRequest = new SqlRequest("IF EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'DatabaseHealthCheck') AND type in (N'P', N'PC')) " +
                    "DROP PROCEDURE DatabaseHealthCheck;");
            var healthCheckRequest = new CreateProcedureRequest(@"
            SELECT  @@ServerName AS ServerName ,
                    DB_NAME() AS DBName ,
                    OBJECT_NAME(ddius.object_id) AS TableName ,
                    SUM(ddius.user_seeks + ddius.user_scans + ddius.user_lookups) AS  Reads ,
                    SUM(ddius.user_updates) AS Writes ,
                    SUM(ddius.user_seeks + ddius.user_scans + ddius.user_lookups
                        + ddius.user_updates) AS [Reads&Writes] ,
                    ( SELECT    DATEDIFF(s, create_date, GETDATE()) / 86400.0
                      FROM      master.sys.databases
                      WHERE     name = 'tempdb'
                    ) AS SampleDays ,
                    ( SELECT    DATEDIFF(s, create_date, GETDATE()) AS SecoundsRunnig
                      FROM      master.sys.databases
                      WHERE     name = 'tempdb'
                    ) AS SampleSeconds
            FROM    sys.dm_db_index_usage_stats ddius
                    INNER JOIN sys.indexes i ON ddius.object_id = i.object_id
                                                 AND i.index_id = ddius.index_id
            WHERE    OBJECTPROPERTY(ddius.object_id, 'IsUserTable') = 1
                    AND ddius.database_id = DB_ID()
            GROUP BY OBJECT_NAME(ddius.object_id)
            ORDER BY [Reads&Writes] DESC;
            ", "DatabaseHealthCheck");

            Requests.Execute(prepareRequest);
            Requests.Execute(healthCheckRequest);
        }

        public void CreateOrUpdate<T>(T obj)
        {
            var map = Mappings[typeof(T)];
            var isCreate = false;

            var checkIsExistRequest = new SelectRequest(true);
            checkIsExistRequest.SetTargetType(obj.GetType());

            isCreate = Count<T>(Condition.Equals(map.PrimaryKeyName, typeof(T).GetProperty(map.PrimaryKeyName).GetValue(obj))) == 0;

            IRequest request;

            if (isCreate)
            {
                request = new CreateRequest(obj);
            }
            else
            {
                request = new UpdateRequest(obj);
            }

            Requests.Execute(request);
        }

        public void Delete(object obj)
        {
            var request = new DeleteRequest(obj);
            Requests.Execute(request);
        }

        public void Delete<T>(ICondition condition = null)
        {
            var request = new DeleteRequest(typeof(T));
            if (condition != null)
                request.AddCondition(condition);
            Requests.Execute(request);
        }

        public IEnumerable<T> Get<T>(ICondition condition = null, DataEntityListLoadOptions options = null)
        {
            SelectRequest request;

            if (options == null)
            {
                request = new SelectRequest();
            }
            else if (options.PageSize > 0 && !string.IsNullOrWhiteSpace(options.OrderBy))
            {
                request = new SelectRequest(options.PageSize, options.PageNumber, options.OrderBy, options.OrderAsc);
            }
            else if (!string.IsNullOrWhiteSpace(options.OrderBy))
            {
                request = new SelectRequest(options.OrderBy, options.OrderAsc);
            }
            else 
            {
                request = new SelectRequest(options.PageSize, options.PageNumber);
            }

            if (condition != null)
                request.AddCondition(condition);

            request.SetTargetType<T>();
            request.SetResponseType<T>();

            return Requests.Execute<T>(request);
        }

        public int Count<T>(ICondition condition = null)
        {
            var request = new SelectRequest(true);

            request.SetTargetType<T>();

            if (condition != null)
                request.AddCondition(condition);

            var result = Requests.Execute<int>(request);

            return result.First();
        }
    }
}
