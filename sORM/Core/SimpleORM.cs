using sORM.Core.Conditions;
using sORM.Core.Mappings;
using sORM.Core.Mappings.Exceptions;
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
    /// <summary>
    /// CRUD ORM system.
    /// Use "Current" property to access methods.
    /// </summary>
    public class SimpleORM
    {
        #region Singleton
        private static SimpleORM _instance;

        private SimpleORM()
	    {

	    }

        /// <summary>
        /// Gets current instance of sORM
        /// </summary>
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

        /// <summary>
        /// Adds a listener that will be called on request to database.
        /// </summary>
        /// <param name="action">Callback that will recieve SQL query that beign executed.</param>
        public void AddOnRequestListener(Action<string> action)
        {
            if (Requests == null)
                throw new NotInitializedException();

            Requests.AddOnRequestListener(action);
        }

        /// <summary>
        /// Initializes sORM and creates mappings for annotated classes.
        /// </summary>
        /// <param name="connectionString">Connection string to database</param>
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

        /// <summary>
        /// Updates existing entity or creates new one if does not exist in database.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="obj">Entity being updated/created</param>
        public void CreateOrUpdate<T>(T obj)
        {
            if (Requests == null)
                throw new NotInitializedException();

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

        /// <summary>
        /// Deletes target entity from database.
        /// </summary>
        /// <param name="obj">Entity being deleted</param>
        public void Delete(object obj)
        {
            if (Requests == null)
                throw new NotInitializedException();

            var request = new DeleteRequest(obj);
            Requests.Execute(request);
        }

        /// <summary>
        /// Deletes all entities of target type that are matching condition.
        /// All entities will be deleted if no condition passed.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="condition">Condition to match entities on</param>
        public void Delete<T>(ICondition condition = null)
        {
            if (Requests == null)
                throw new NotInitializedException();

            var request = new DeleteRequest(typeof(T));
            if (condition != null)
                request.AddCondition(condition);
            Requests.Execute(request);
        }

        /// <summary>
        /// Gets entities of choosen type that are matching condition with pagination.
        /// All entities will be returned if no condition passed.
        /// All entities at once will be returned if no options passed.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="condition">Condition to match entities on</param>
        /// <param name="options">Pagination options</param>
        /// <returns>Collection of entities</returns>
        public IEnumerable<T> Get<T>(ICondition condition = null, DataEntityListLoadOptions options = null)
        {
            if (Requests == null)
                throw new NotInitializedException();

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

        /// <summary>
        /// Gets number of entities that are matching condition.
        /// Returns total number of entities if no condition passed.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="condition">Condition to match entities on</param>
        /// <returns>Number of entities</returns>
        public int Count<T>(ICondition condition = null)
        {
            if (Requests == null)
                throw new NotInitializedException();

            var request = new SelectRequest(true);

            request.SetTargetType<T>();

            if (condition != null)
                request.AddCondition(condition);

            var result = Requests.Execute<int>(request);

            return result.First();
        }
    }
}
