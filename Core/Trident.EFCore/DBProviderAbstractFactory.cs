#if (NET20 || NET35 || NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471 || NET472)
#define NETFUL
#endif

using Trident.Data.Contracts;
using System.Data.Common;
using System;
using System.Reflection;
using System.IO;
using Microsoft.Data.SqlClient;

namespace Trident.EFCore
{
    public class DBProviderAbstractFactory : IDBProviderAbstractFactory
    {
        private static string UnsupportedProvidedMessage = "The data provider {0} is not supported";
        private static string UnableToretriveDbProvidedMessage = "The data provider {0} was unable to be retrieved.";

        public DbProviderFactory GetFactory(string provider)
        {
#if NETFULL
            return DbProviderFactories.GetFactory(providerName);
#else
            var lowerProvider = provider.ToLower();

            if (lowerProvider == "system.data.sqlclient")
                return GetDbProviderFactory(DataAccessProviderTypes.SqlServer);
            if (lowerProvider == "system.data.sqlite" || lowerProvider == "microsoft.data.sqlite")
                return GetDbProviderFactory(DataAccessProviderTypes.SqLite);
            if (lowerProvider == "mysql.data.mysqlclient" || lowerProvider == "mysql.data")
                return GetDbProviderFactory(DataAccessProviderTypes.MySql);
            if (lowerProvider == "npgsql")
                return GetDbProviderFactory(DataAccessProviderTypes.PostgreSql);

            throw new NotSupportedException(string.Format(UnsupportedProvidedMessage, provider));
#endif
        }

        private static DbProviderFactory GetDbProviderFactory(DataAccessProviderTypes type)
        {
            if (type == DataAccessProviderTypes.SqlServer)
                return SqlClientFactory.Instance; // this library has a ref to SqlClient so this works

            if (type == DataAccessProviderTypes.SqLite)
            {
                // SqLite support in .NET Standard available now
                return GetDbProviderFactory("System.Data.SQLite.SQLiteFactory", "System.Data.SQLite");
                //#if NETFULL
                //#else
                //                return GetDbProviderFactory("Microsoft.Data.Sqlite.SqliteFactory", "Microsoft.Data.Sqlite");
                //#endif
            }
            if (type == DataAccessProviderTypes.MySql)
                return GetDbProviderFactory("MySql.Data.MySqlClient.MySqlClientFactory", "MySql.Data");
            if (type == DataAccessProviderTypes.PostgreSql)
                return GetDbProviderFactory("Npgsql.NpgsqlFactory", "Npgsql");
#if NETFULL
            if (type == DataAccessProviderTypes.OleDb)
                return System.Data.OleDb.OleDbFactory.Instance;
            if (type == DataAccessProviderTypes.SqlServerCompact)
                return DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0");                
#endif

            throw new NotSupportedException(string.Format(UnsupportedProvidedMessage, type.ToString()));
        }

        private static object GetStaticProperty(string typeName, string property)
        {
            Type type = GetTypeFromName(typeName);
            if (type == null)
                return null;

            return GetStaticProperty(type, property);
        }

        private static Type GetTypeFromName(string typeName, string assemblyName = null)
        {
            var type = Type.GetType(typeName, false);
            if (type != null)
                return type;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // try to find manually
            foreach (Assembly asm in assemblies)
            {
                type = asm.GetType(typeName, false);

                if (type != null)
                    break;
            }
            if (type != null)
                return type;

            // see if we can load the assembly
            if (!string.IsNullOrEmpty(assemblyName))
            {
                var a = LoadAssembly(assemblyName);
                if (a != null)
                {
                    type = Type.GetType(typeName, false);
                    if (type != null)
                        return type;
                }
            }

            return null;
        }

        private static Assembly LoadAssembly(string assemblyName)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch { }

            if (assembly != null)
                return assembly;

            if (File.Exists(assemblyName))
            {
                assembly = Assembly.LoadFrom(assemblyName);
                if (assembly != null)
                    return assembly;
            }
            return null;
        }

        private static object GetStaticProperty(Type type, string property)
        {
            object result = null;
            try
            {
                result = type.InvokeMember(property, BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty, null, type, null);
            }
            catch
            {
                return null;
            }

            return result;
        }

        private static DbProviderFactory GetDbProviderFactory(string dbProviderFactoryTypename, string assemblyName)
        {
            var instance = GetStaticProperty(dbProviderFactoryTypename, "Instance");
            if (instance == null)
            {
                var a = LoadAssembly(assemblyName);
                if (a != null)
                    instance = GetStaticProperty(dbProviderFactoryTypename, "Instance");
            }

            if (instance == null)
                throw new InvalidOperationException(string.Format(UnableToretriveDbProvidedMessage, dbProviderFactoryTypename));

            return instance as DbProviderFactory;
        }
    }



    public enum DataAccessProviderTypes
    {
        SqlServer,
        SqLite,
        MySql,
        PostgreSql,

#if NETFULL
        OleDb,
        SqlServerCompact
#endif
    }
}