﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Mappings.Exceptions
{
    internal class KeyPropertyNotFoundException : Exception
    {
        public KeyPropertyNotFoundException(Type type) : base("Key property is not defined for class " + type.FullName)
        {

        }
    }
}
