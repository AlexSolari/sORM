using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Mappings.Exceptions
{
    internal class NotInitializedException : Exception
    {
        public NotInitializedException() : base("Instance of sORM was not initialized")
        {

        }
    }
}
