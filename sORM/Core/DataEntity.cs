using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core
{
    public abstract class DataEntity
    {
        public abstract string DataId { get; set; }

        public DataEntity()
        {
            DataId = Guid.NewGuid().ToString();
        }
    }
}
