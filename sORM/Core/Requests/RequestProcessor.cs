using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace sORM.Core.Requests
{
    public class RequestProcessor
    {
        private List<Action<string>> Listeners = new List<Action<string>>();
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
                    createTableCommand.Append("CREATE TABLE [" + map.Name + "](");
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

        public IEnumerable<T> Execute<T>(IRequestWithResponse request)
        {
            var sql = request.BuildSql();
            OnRequest(sql);

            var rows = new List<Dictionary<string, object>>();
            using (DbCommand command = connection.Connection.CreateCommand())
            {
                command.CommandText = sql;
                connection.Connection.Open();

                using (DbDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                            row.Add(reader.GetName(i), reader.GetValue(i));

                        rows.Add(row);
                    }
                }
            }

            var objects = new List<T>();

            foreach (var row in rows)
            {
                var obj = Activator.CreateInstance<T>();

                foreach (var column in row)
                {
                    var prop = obj.GetType().GetProperty(column.Key);

                    if (prop == null && typeof(T) == typeof(int))
                    {
                        obj = (T)column.Value;
                    }
                    else if (prop.PropertyType == typeof(string))
                    {
                        prop.SetValue(obj, column.Value.ToString());
                    }
                    else if (prop.PropertyType == typeof(int))
                    {
                        var tmp = 0;
                        int.TryParse(column.Value.ToString(), out tmp);
                        prop.SetValue(obj, tmp);
                    }
                    else if (prop.PropertyType == typeof(float))
                    {
                        var tmp = 0f;
                        float.TryParse(column.Value.ToString(), out tmp);
                        prop.SetValue(obj, tmp);
                    }
                    else if (prop.PropertyType == typeof(bool))
                    {
                        prop.SetValue(obj, column.Value.ToString().Equals("1"));
                    }
                    else if (prop.PropertyType == typeof(XmlDocument))
                    {
                        var xml = new XmlDocument();
                        xml.LoadXml(column.Value.ToString());
                        prop.SetValue(obj, xml);
                    }
                    else if (prop.PropertyType == typeof(Guid))
                    {
                        prop.SetValue(obj, Guid.Parse(column.Value.ToString()));
                    }
                    else if (prop.PropertyType == typeof(DateTime))
                    {
                        prop.SetValue(obj, DateTime.Parse(column.Value.ToString()));
                    }
                }

                objects.Add(obj);
            }

            return objects;
        }

        public void Execute(IRequest request)
        {
            var sql = request.BuildSql();
            OnRequest(sql);
            connection.ExecuteCommand(sql);
        }

        protected void OnRequest(string sql)
        {
            Listeners.ForEach(listener => listener(sql));
        }

        public void AddOnRequestListener(Action<string> action)
        {
            Listeners.Add(action);
        }
    }
}
