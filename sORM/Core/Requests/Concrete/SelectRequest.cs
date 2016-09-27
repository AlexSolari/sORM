using sORM.Core.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string BuildSql()
        {
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

            return request;
        }

        public void AddCondition(ICondition condition)
        {
            Conditions.Add(condition);
        }
    }
}
