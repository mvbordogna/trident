using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Trident.EFCore
{
    /// <summary>
    /// Class DataParameterExtensions provides helper extension methods for converting variables to consumable
    /// SQL objects
    /// </summary>
    public static class DataParameterExtensions
    {
        /// <summary>
        /// Converts a primitive object to a SqlParameter.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static IDbDataParameter ToSqlParameter(this object obj, string parameterName)
        {
            var sqlDbType = GetSqlDbType(obj);
            var parameter = new SqlParameter(parameterName, sqlDbType);
            parameter.Value = obj;

            return parameter;
        }

        /// <summary>
        /// Converts a DataTable to a SqlParameter.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parameterName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static IDbDataParameter ToSqlParameter(this DataTable obj, string parameterName, string typeName)
        {
            var sqlDbType = SqlDbType.Structured;
            var parameter = new SqlParameter(parameterName, sqlDbType);
            parameter.Value = obj;
            parameter.TypeName = typeName;

            return parameter;
        }

        /// <summary>
        /// Converts a collection of <code>long</code> to a TVP SqlParameter for the [system].[IdsType] UDT.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static IDbDataParameter ToIdsTypeTVP(this IEnumerable<long> obj, string parameterName)
        {
            var idType = new DataTable();
            idType.Columns.Add("Id", typeof(long));
            if (obj != null)
            {
                foreach (long val in obj)
                {
                    idType.Rows.Add(val);
                }
            }

            var parameter = new SqlParameter(parameterName, SqlDbType.Structured);
            parameter.TypeName = "[system].[IdsType]";
            parameter.Value = idType;

            return parameter;
        }

        /// <summary>
        /// Helper method used to convert primitive data types to SqlDbType.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static SqlDbType GetSqlDbType(object obj)
        {
            switch (obj)
            {
                case int _:
                    return SqlDbType.Int;
                case long _:
                    return SqlDbType.BigInt;
                case string _:
                    return SqlDbType.NVarChar;
                case bool _:
                    return SqlDbType.Bit;
                case DateTime _:
                    return SqlDbType.DateTime;
                case DateTimeOffset _:
                    return SqlDbType.DateTimeOffset;
                default:
                    throw new NotImplementedException($"The primitive type { obj?.GetType() } is not mapped to a SqlDbType");
            }
        }
    }
}
