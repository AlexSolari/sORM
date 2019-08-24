using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Mappings
{
    /// <summary>
    /// Marks type as a data model, what will result in creation a table in database for this entity type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class DataModelAttribute : Attribute
    {

    }
}
