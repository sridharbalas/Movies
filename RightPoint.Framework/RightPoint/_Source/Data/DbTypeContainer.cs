using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace RightPoint.Data
{
    internal class DbTypeContainer
    {
        public readonly DbType? DbType = null;
        public readonly SqlDbType? SqlDbType = null;

        public DbTypeContainer(DbType dbType)
        {
            DbType = dbType;
        }

        public DbTypeContainer(SqlDbType sqlDbType)
        {
            SqlDbType = sqlDbType;
        }

        public bool IsSqlDbType
        {
            get { return SqlDbType != null; }
        }

        public bool IsDbType
        {
            get { return DbType != null; }
        }
    }
}
