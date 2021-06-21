using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using System;

using Task15.Models;

namespace Task15.NHibernate
{
    public class DBcontext
    {
        public ISessionFactory CreateSessionFactory()
        {
            return Fluently
                .Configure()
                    .Database(
                        PostgreSQLConfiguration.Standard
                        .ConnectionString(c =>
                            c.Host("localhost")
                            .Port(5432)
                            .Database("postgres")
                            .Username("postgres")
                            .Password("root")))
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<User>())
                    .ExposeConfiguration(TreatConfiguration)
                .BuildSessionFactory();
        }
        private static void TreatConfiguration(Configuration configuration)
        {
            // dump sql file for debug
            Action<string> updateExport = x =>
            {
                using (var file = new System.IO.FileStream(@"update.sql", System.IO.FileMode.Append, System.IO.FileAccess.ReadWrite))
                using (var sw = new System.IO.StreamWriter(file))
                {
                    sw.Write(x);
                    sw.Close();
                }
            };
            var update = new SchemaUpdate(configuration);
            update.Execute(updateExport, true);
        }
    }
}