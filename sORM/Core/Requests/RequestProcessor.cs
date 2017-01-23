using sORM.Core.Requests.Concrete;
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
        public DataContext connection = null;
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
                    foreach (var column in map.Data)
                    {
                        var fieldReferences = false;

                        foreach (var reference in map.References)
                        {
                            if (reference.Value.Value == column.Key.Name)
                            {
                                fieldReferences = true;
                                break;
                            }
                        }

                        var valueToAppend = column.Value;
                        if (fieldReferences)
                        {
                            valueToAppend = valueToAppend.Replace("MAX", "500");
                        }

                        if (map.SecondaryKeyNames.Contains(column.Key.Name))
                        {
                            valueToAppend = valueToAppend.Replace("MAX", "500");

                            valueToAppend += " UNIQUE NOT NULL";
                        }

                        if (map.PrimaryKeyName == column.Key.Name && !map.SecondaryKeyNames.Contains(column.Key.Name))
                        {
                            valueToAppend += " NOT NULL";
                        }

                        valueToAppend += ",";

                        createTableCommand.Append(valueToAppend);
                    }
                    createTableCommand.Append(")");

                    var request = new SqlRequest(createTableCommand.ToString());
                    Execute(request);
                }
            }

            BuildReferences();
        }

        private void BuildReferences()
        {
            foreach (var map in SimpleORM.Current.Mappings.Values)
            {
                var resultForeigns = connection.ExecuteQuery<int?>("SELECT count(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_TYPE = 'PRIMARY KEY' AND TABLE_NAME = '"+ map.Name + "'").FirstOrDefault();

                if (resultForeigns.HasValue && resultForeigns == 0)
                {
                    StringBuilder createPrimaryKeyCommand = new StringBuilder();
                    createPrimaryKeyCommand.Append("ALTER TABLE [" + map.Name + "] ADD PRIMARY KEY (");
                    createPrimaryKeyCommand.Append(map.PrimaryKeyName);
                    createPrimaryKeyCommand.Append(")");

                    var request = new SqlRequest(createPrimaryKeyCommand.ToString());
                    Execute(request);
                }
            }

            foreach (var map in SimpleORM.Current.Mappings.Values)
            {
                foreach (var reference in map.References)
                {
                    var referencedTypeMap = SimpleORM.Current.Mappings[reference.Key];
                    var referencedTableName = referencedTypeMap.Name;
                    var referencedPropName = reference.Value.Key;
                    var keyPropName = reference.Value.Value;

                    var resultForeigns = connection.ExecuteQuery<int?>("SELECT count(*) FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE [TABLE_NAME] = '"+ map.Name + "' AND [COLUMN_NAME] = '"+ keyPropName + "'").FirstOrDefault();

                    if (resultForeigns.HasValue && resultForeigns == 0)
                    {
                        StringBuilder createPrimaryKeyCommand = new StringBuilder();
                        createPrimaryKeyCommand.Append("ALTER TABLE [" + map.Name + "] ADD FOREIGN  KEY (");
                        createPrimaryKeyCommand.Append(keyPropName);
                        createPrimaryKeyCommand.Append(") REFERENCES [");
                        createPrimaryKeyCommand.Append(referencedTableName);
                        createPrimaryKeyCommand.Append("](");
                        createPrimaryKeyCommand.Append(referencedPropName);
                        createPrimaryKeyCommand.Append(") ON DELETE CASCADE");

                        var request = new SqlRequest(createPrimaryKeyCommand.ToString());
                        Execute(request);
                    }
                }
            }
        }

        public IEnumerable<T> Execute<T>(IRequestWithResponse request)
        {
            var rows = new List<Dictionary<string, object>>();
            using (var command = request.BuildSql())
            {
                connection.Connection.Open();
                try
                {
                    OnRequest(command.CommandText);

                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
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
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    connection.Connection.Close();
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
                    else
                    {
                        prop.SetValue(obj, column.Value);
                    }
                }

                objects.Add(obj);
            }

            return objects;
        }

        public void Execute(IRequest request)
        {
            connection.Connection.Open();
            try
            {

                using (var command = request.BuildSql())
                {
                    OnRequest(command.CommandText);
                    command.ExecuteReader();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                connection.Connection.Close();
            }
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
