using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Requests
{
    public class RequestProcessor
    {
        public bool LoggingEnabled = false;

        private DataContext connection = null;
        private string connectionString;

        public RequestProcessor(string connectionString)
        {
            this.connectionString = connectionString;

            connection = new DataContext(connectionString);
        }

        public void Initialize()
        {
            foreach (var map in SimpleORM.Current.Mappings.Values)
            {
                var result = connection.ExecuteQuery<int?>("SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '" + map.Name + "'").FirstOrDefault();

                if (result.HasValue && result == 0)
                {
                    StringBuilder createTableCommand = new StringBuilder();
                    createTableCommand.Append("CREATE TABLE " + map.Name + "(");
                    foreach (var column in map.Data.Values)
                    {
                        createTableCommand.Append(column);
                        createTableCommand.Append(",");
                    }
                    createTableCommand.Append(")");
                    connection.ExecuteCommand(createTableCommand.ToString());
                }

            }
        }

        public IEnumerable Execute(IRequestWithResponse request)
        {
            var sql = request.BuildSql();
            OnRequest(sql);
            return connection.ExecuteQuery(request.GetResponseType(), sql);
        }

        public void Execute(IRequest request)
        {
            var sql = request.BuildSql();
            OnRequest(sql);
            connection.ExecuteCommand(sql);
        }

        protected void OnRequest(string sql)
        {
            if (LoggingEnabled)
                Log(sql);
        }

        private void Log(string sql)
        {
            using (var writer = new System.IO.StreamWriter("Log["+DateTime.Now.ToShortDateString().Replace("/", ".")+"].txt", true))
            {
                writer.WriteLine(DateTime.Now.ToString() + " | Request:");
                writer.WriteLine(sql);
                writer.WriteLine("===================================");

            }
        }

        
    }
}
