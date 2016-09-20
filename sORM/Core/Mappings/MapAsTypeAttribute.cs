﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Mappings
{
    public enum DataType
    {
        Int,
        String,
        Text,
        Bool,
        Float,
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class MapAsTypeAttribute : Attribute
    {
        public string Type;

        public MapAsTypeAttribute(DataType type)
        {
            switch (type)
            {
                case DataType.Int:
                    Type = "int";
                    break;
                case DataType.String:
                    Type = "varchar(MAX)";
                    break;
                case DataType.Text:
                    Type = "text";
                    break;
                case DataType.Bool:
                    Type = "bit";
                    break;
                case DataType.Float:
                    Type = "real";
                    break;
                default:
                    break;
            }
        }
    }
}
