using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using System;

using Task15.Models;

namespace Task15.NHibernate
{
    public class DBContext
    {
        public ISessionFactory CreateSessionFactory(DBContextBindings dBContextBindings)
        {
            return Fluently
                .Configure()
                    .Database(
                        PostgreSQLConfiguration.Standard
                        .ConnectionString(c =>
                            c.Host(dBContextBindings.Host)
                            .Port(dBContextBindings.Port)
                            .Database(dBContextBindings.Database)
                            .Username(dBContextBindings.Username)
                            .Password(dBContextBindings.Password)))
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