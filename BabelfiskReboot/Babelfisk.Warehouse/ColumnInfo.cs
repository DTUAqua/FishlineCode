using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Warehouse
{
    public class ColumnInfo
    {
        public string ColumnName
        {
            get;
            set;
        }

        public int? CharacterMaxLength
        {
            get;
            set;
        }

        public byte? NumericPrecision
        {
            get;
            set;
        }

        public int? NumericScale
        {
            get;
            set;
        }

        public string IsNullable
        {
            get;
            set;
        }
    }
}
