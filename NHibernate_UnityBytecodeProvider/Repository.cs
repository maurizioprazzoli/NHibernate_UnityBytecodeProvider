﻿using Microsoft.Practices.Unity;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using Unity.Bytecode;

namespace NHibernate_UnityBytecodeProvider
{
    public class NHConfiguration
    {
        public static ISessionFactory ConfigureNHibernate(Dictionary<string, string> configuration, IUnityContainer container)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource = configuration["DataSource"];
            connectionStringBuilder.UserID = configuration["UserId"];
            connectionStringBuilder.Password = configuration["Password"];
            connectionStringBuilder.InitialCatalog = configuration["Database"];
            connectionStringBuilder.ConnectTimeout = Int32.Parse(configuration["ConnectTimeout"]);

            var dropAndCreateDatabaseSchema = Convert.ToBoolean(configuration["DropAndCreateDatabaseSchema"]);
            var useSecondLevelCache = Convert.ToBoolean(configuration["UseSecondLevelCache"]);
            var useNHibernateSimpleProfiler = Convert.ToBoolean(configuration["UseNHibernateSimpleProfiler"]);

            // Initialize
            Configuration cfg = new Configuration().DataBaseIntegration(db =>
                                                   {
                                                       db.ConnectionString = connectionStringBuilder.ConnectionString;
                                                       db.Dialect<MsSql2008Dialect>();
                                                   });

            //Initializate assembly that contains mapping configuration
            cfg.AddAssembly(Assembly.Load(configuration["ConfigurationAssembly"]));

            if (dropAndCreateDatabaseSchema)
            {
                var schemaExport = new SchemaExport(cfg);
                schemaExport.Drop(false, true);
                schemaExport.Create(false, true);
            }

            if (useSecondLevelCache)
            {
                cfg.SetProperty(NHibernate.Cfg.Environment.UseSecondLevelCache, "true");
                cfg.SetProperty(NHibernate.Cfg.Environment.CacheProvider, "NHibernate.Caches.SysCache.SysCacheProvider, NHibernate.Caches.SysCache");
            }
            else
            {
                cfg.SetProperty(NHibernate.Cfg.Environment.UseSecondLevelCache, "false");
            }

            if (useNHibernateSimpleProfiler)
            {
                cfg.SetProperty(NHibernate.Cfg.Environment.GenerateStatistics, "true");
            }

            NHibernate.Cfg.Environment.BytecodeProvider = new UnityBytecodeProvider(container,
                                                                                    new UnityProxyFactoryFactory(container),
                //new NHibernate.Bytecode.DefaultProxyFactoryFactory(),
                                                                                    new NHibernate.Type.DefaultCollectionTypeFactory());
            // Set Up interception
            var intercepter = new UnityInterceptor();
            cfg.SetInterceptor(intercepter);

            // Create session factory from configuration object
            var sessionFactory = cfg.BuildSessionFactory();

            // Set Up NHibernate profiler
            if (useNHibernateSimpleProfiler)
            {
                NHibernateSimpleProfiler.Profiler.SetSessionFactory = sessionFactory;
            }

            return sessionFactory;
        }
    }
}
