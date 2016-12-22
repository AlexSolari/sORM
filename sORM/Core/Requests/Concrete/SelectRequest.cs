using sORM.Core.Conditions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace sORM.Core.Requests.Concrete
{
    public class SelectRequest : IRequestWithResponse, IConditionalRequest
    {
        public IList<ICondition> Conditions { get; set; }
        private Type responseType;
        private Type targetType;
        
        private bool usePaging = false;
        private string fieldToOrder;
        private bool asc;

        private bool useOrdering = false;
        private int pageSize;
        private int pageCount;

        private bool onlyCount = false;

        public SelectRequest()
        {
            Conditions = new List<ICondition>();
        }

        public SelectRequest(bool onlyCount)
        {
            this.onlyCount = onlyCount;
            SetResponseType<int>();
            Conditions = new List<ICondition>();
        }

        public SelectRequest(string fieldToOrder, bool asc)
        {
            useOrdering = true;
            this.fieldToOrder = fieldToOrder;
            this.asc = asc;
            Conditions = new List<ICondition>();
        }
        public SelectRequest(int pageSize, int pageCount)
        {
            usePaging = true;
            this.pageSize = pageSize;
            this.pageCount = pageCount;
            Conditions = new List<ICondition>();
        }

        public SelectRequest(int pageSize, int pageCount, string fieldToOrder, bool asc)
        {
            usePaging = true;
            this.pageSize = pageSize;
            this.pageCount = pageCount;

            useOrdering = true;
            this.fieldToOrder = fieldToOrder;
            this.asc = asc;
            Conditions = new List<ICondition>();
        }

        public Type GetResponseType()
        {
            return responseType;
        }

        public void SetResponseType<TType>()
        {
            responseType = typeof(TType);
        }

        public void SetResponseType(Type type)
        {
            responseType = type;
        }

        public void SetTargetType<TType>()
        {
            targetType = typeof(TType);
        }

        public void SetTargetType(Type type)
        {
            targetType = type;
        }

        public System.Data.IDbCommand BuildSql()
        {
            var map = SimpleORM.Current.Mappings[targetType];

            var request = "SELECT " + ((onlyCount) ? "COUNT(*)" : "*") + " FROM [" + targetType.Name + "] ";

            if (Conditions.Count > 0)
            {
                var last = Conditions.Last();

                request += " WHERE ";
                foreach (var item in Conditions)
                {
                    request += item.BuildSql();

                    if (!last.Equals(item))
                        request += " AND ";
                }
            }

            if (useOrdering)
            {
                request += " ORDER BY " + fieldToOrder;
                if (!asc)
                {
                    request += " DESC ";
                }
            }
            else if (!onlyCount)
            {
                request += " ORDER BY " + SimpleORM.Current.Mappings[targetType].Data.First().Key.Name;
            }

            if (usePaging)
            {
                request += " OFFSET " + pageSize * pageCount + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY ";
            }

            var command = new SqlCommand(request, SimpleORM.Current.Requests.connection.Connection as SqlConnection);

            foreach (RequestCondition item in Conditions)
            {
                foreach (var parameter in item.Parameters)
                {
                    object value;
                    if (parameter.Value == null)
                    {
                        value = "NULL";
                    }
                    else if (parameter.Value is string || parameter.Value is Guid || parameter.Value is DateTime)
                    {
                        value = parameter.Value.ToString();
                    }
                    else if (parameter.Value is bool)
                    {
                        value = parameter.Value;
                    }
                    else if (parameter.Value is XmlDocument)
                    {
                        value = ((XmlDocument)parameter.Value).InnerXml;
                    }
                    else
                    {
                        value = parameter.Value.ToString();
                    }
                    command.Parameters.Add(parameter.Key, map.GetSqlType(item.Field)).Value = value;
                }
            }

            return command;
        }

        public void AddCondition(ICondition condition)
        {
            Conditions.Add(condition);
        }
    }
}
