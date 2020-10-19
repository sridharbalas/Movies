using System;
using System.Data;
using System.Data.SqlClient;

namespace RightPoint.Data
{
    /// <summary>
    /// Summary description for DbUtility.
    /// </summary>
    public sealed class DbUtility
    {
        private DbUtility()
        {
        }

        /// <summary>
        /// Creates an IDbCommand for the passed query text using the passed connection.
        /// </summary>
        /// <param name="dbConnection">An IDbConnection that is already opened.</param>
        /// <param name="queryText">The query text that this IDbCommand object will execute.</param>
        /// <returns></returns>
        public static IDbCommand CreateQueryTextCommand(IDbConnection dbConnection, string queryText)
        {
            IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.Text;
            dbCommand.CommandText = queryText;

            return (dbCommand);
        }

        /// <summary>
        /// Creates an IDbCommand for the passed stored procedure using the passed connection.
        /// </summary>
        /// <param name="dbConnection">An IDbConnection that is already opened.</param>
        /// <param name="storedProcedureName">The stored procedure that this IDbCommand object will execute.</param>
        /// <returns></returns>
        public static IDbCommand CreateStoredProcedureCommand(IDbConnection dbConnection, string storedProcedureName)
        {
            IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.StoredProcedure;
            dbCommand.CommandText = storedProcedureName;
            if (dbConnection.ConnectionTimeout > dbCommand.CommandTimeout)
            {
                dbCommand.CommandTimeout = dbConnection.ConnectionTimeout;
            }
            return (dbCommand);
        }

        /// <summary>
        /// Adds a parameter for the return value to the passed command object.
        /// </summary>
        /// <param name="dbCommand"></param>
        public static void AddReturnParameter(IDbCommand dbCommand)
        {
#if DOTNET_STANDARD
            // MySql doesn't support return parameters.
            if (dbCommand is MySql.Data.MySqlClient.MySqlCommand)
                return;
#endif
            IDbDataParameter param = dbCommand.CreateParameter();

            param.ParameterName = "@RETURN_VALUE";
            param.DbType = DbType.Int32;
            param.Direction = ParameterDirection.ReturnValue;

            dbCommand.Parameters.Add(param);
        }

        private static IDbDataParameter CreateParameter(IDbCommand dbCommand, string parameterName,
                                                         DbTypeContainer parameterType, int parameterLength,
                                                         object parameterValue, ParameterDirection parameterDirection)
        {
            IDbDataParameter param;

            param = dbCommand.CreateParameter();
            param.ParameterName = parameterName;

            if (parameterType.IsDbType == true)
                param.DbType = parameterType.DbType.GetValueOrDefault(DbType.Object);
            else
                ((SqlParameter)param).SqlDbType = parameterType.SqlDbType.GetValueOrDefault(SqlDbType.Variant);

            if (parameterLength != Int32.MinValue)
            {
                param.Size = parameterLength;
            }

            param.Value = parameterValue;
            param.Direction = parameterDirection;

            return (param);
        }

        private static void AddParameter(IDbCommand dbCommand, string parameterName, DbTypeContainer parameterType,
                                          int parameterLength, object parameterValue,
                                          ParameterDirection parameterDirection)
        {
            dbCommand.Parameters.Add(
                CreateParameter(dbCommand, parameterName, parameterType, parameterLength, parameterValue,
                                 parameterDirection));
        }

        /// <summary>
        /// Adds a parameter and value to the passed command object.
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterType"></param>
        /// <param name="parameterLength"></param>
        /// <param name="parameterValue"></param>
        public static void AddInputParameter(IDbCommand dbCommand, string parameterName, DbType parameterType,
                                              int parameterLength, object parameterValue)
        {
            AddParameter(dbCommand, parameterName, new DbTypeContainer(parameterType), parameterLength, parameterValue,
                          ParameterDirection.Input);
        }

        public static void AddInputParameter(IDbCommand dbCommand, string parameterName, SqlDbType parameterType,
                                      int parameterLength, object parameterValue)
        {
            AddParameter(dbCommand, parameterName, new DbTypeContainer(parameterType), parameterLength, parameterValue,
                          ParameterDirection.Input);
        }

        public static void AddInputParameter(IDbCommand dbCommand, string parameterName, DbType parameterType,
                                              object parameterValue)
        {
            AddInputParameter(dbCommand, parameterName, parameterType, Int32.MinValue, parameterValue);
        }

        /// <summary>
        /// Adds an output parameter to the passed command object.
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterType"></param>
        /// <param name="parameterLength"></param>
        public static void AddOutputParameter(IDbCommand dbCommand, string parameterName, DbType parameterType,
                                               int parameterLength)
        {
            AddParameter(dbCommand, parameterName, new DbTypeContainer(parameterType), parameterLength, null, ParameterDirection.Output);
        }

        public static void AddOutputParameter(IDbCommand dbCommand, string parameterName, DbType parameterType)
        {
            AddOutputParameter(dbCommand, parameterName, parameterType, Int32.MinValue);
        }

        /// <summary>
        /// Adds an input/output parameter to the passed command object.
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterType"></param>
        /// <param name="parameterLength"></param>
        public static void AddInputOutputParameter(IDbCommand dbCommand, string parameterName, DbType parameterType,
                                                    int parameterLength)
        {
            AddParameter(dbCommand, parameterName, new DbTypeContainer(parameterType), parameterLength, null, ParameterDirection.InputOutput);
        }

        public static void AddInputOutputParameter(IDbCommand dbCommand, string parameterName, DbType parameterType)
        {
            AddInputOutputParameter(dbCommand, parameterName, parameterType, Int32.MinValue);
        }

        public static int GetReturnCode(IDbCommand dbCommand)
        {
#if DOTNET_STANDARD
            // MySql Procs do not support a return parameter.
            if (dbCommand is MySql.Data.MySqlClient.MySqlCommand)
                return 0;
            else
#endif
                return (int)(((System.Data.IDbDataParameter)dbCommand.Parameters[0]).Value == System.DBNull.Value ? null : ((System.Data.IDbDataParameter)dbCommand.Parameters[0]).Value);
        }

        public static bool IsExceptionAPrimaryKeyViolation(System.Data.SqlClient.SqlException ex)
        {
            bool isPrimaryKeyViolation = false;

            foreach (System.Data.SqlClient.SqlError error in ex.Errors)
            {
                if (error.Number == 2627)
                    isPrimaryKeyViolation = true;
            }

            return isPrimaryKeyViolation;
        }

        public static bool IsExceptionAForeignKeyViolation(System.Data.SqlClient.SqlException ex)
        {
            bool isForeignKeyViolation = false;

            foreach (System.Data.SqlClient.SqlError error in ex.Errors)
            {
                if (error.Number == 547)
                    isForeignKeyViolation = true;
            }

            return isForeignKeyViolation;
        }
    }
}