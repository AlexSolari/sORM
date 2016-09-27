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


        public Dictionary<Type, Map> Mappings = new Dictionary<Type, Map>();

        protected RequestProcessor Requests = null;

        protected MapBinder Mapper = new MapBinder();

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
        }

        public void CreateOrUpdate<T>(T obj)
            where T : DataEntity
        {
            var isCreate = false;

            var checkIsExistRequest = new SelectRequest(true);
            checkIsExistRequest.SetTargetType(obj.GetType());

            isCreate = Count<T>(Condition.Equals("DataId", obj.DataId)) == 0;

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

        public void Delete(DataEntity obj)
        {
            var request = new DeleteRequest(obj);
            Requests.Execute(request);
        }

        public void Delete<T>(ICondition condition = null)
            where T : DataEntity
        {
            var request = new DeleteRequest(typeof(T));
            if (condition != null)
                request.AddCondition(condition);
            Requests.Execute(request);
        }

        public IEnumerable<T> Get<T>(ICondition condition = null, DataEntityListLoadOptions options = null)
            where T : DataEntity
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
            where T : DataEntity
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
