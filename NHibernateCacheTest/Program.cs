using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Caches.SysCache2;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Transform;
using System;

namespace NHibernateCacheTest
{
    /// <summary>
    /// Please, make sure that there is a testDb on your SQlEXPRESS server or change the connection string.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Configuration config = GenerateMapping();
            SchemaExport schema = new SchemaExport(config);

            CreateDatabase(schema);

            using (ISessionFactory sessionFactory = config.BuildSessionFactory())
            {
                using (ISession session = sessionFactory.OpenSession())
                {
                    Seed(session);

                    try
                    {
                        TestQuery(session);

                        TestQuery(session, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            Console.ReadLine();
        }


        public static void TestQuery(ISession session, bool withCache = false)
        {
            Console.WriteLine("Start testing. Cache is turned " + (withCache ? "ON" : "OFF"));
            EntityDto result = null;

            var query = session.QueryOver<Entity>()
                .SelectList(l => l
                    .Select(x => x.Id).WithAlias(() => result.Id)
                    .Select(x => x.Name).WithAlias(() => result.Name))
                .TransformUsing(Transformers.AliasToBean<EntityDto>());

            if (withCache)
            {
                query.Cacheable();
            }

            var list = query.List<EntityDto>();

            foreach (EntityDto entity in list)
            {
                Console.WriteLine(entity);
            }
            Console.WriteLine();
        }

        #region Database helper methods
        public static Configuration GenerateMapping()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["testDB"].ConnectionString;

            var config = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connectionString).DefaultSchema("dbo"))
                .Cache(c => c.UseQueryCache().ProviderClass<SysCacheProvider>())
                .Mappings(m => m.FluentMappings.Conventions.Setup(x => x.Add(AutoImport.Never())).AddFromAssemblyOf<EntityMap>())
                .BuildConfiguration();

            return config;
        }
        public static void CreateDatabase(SchemaExport schema)
        {
            //drop database 
            schema.Drop(false, true);
            //re-create database
            schema.Create(false, true);

            Console.WriteLine("Database has been created.");
        }
        public static void Seed(ISession session)
        {
            for (int i = 0; i < 10; i++)
            {
                session.Save(new Entity($"TestEntity_{i}"));
            }

            Console.WriteLine("Seeding has been finished");
        }
        #endregion      
    }
}
