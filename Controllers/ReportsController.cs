using System;
using System.IO;
using System.Text;
using System.Web;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

using NHibernate;

using Task15.Models;
using Task15.NHibernate;

namespace Task15.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private DBcontext dbcontext;
        private ISessionFactory sessionFactory;
        public ReportsController(IWebHostEnvironment appEnvironment)
        {
            dbcontext = new DBcontext();
            sessionFactory = dbcontext.CreateSessionFactory();
            _appEnvironment = appEnvironment;
        }
        public async Task<IActionResult> ReportAll()
        {
            var users = new List<User>();

            using (var session = sessionFactory.OpenStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    users = session.Query<User>().OrderBy(user => user.Id).ToList();
                    transaction.Commit();
                }
                session.Close();
            }
            sessionFactory.Close();

            using (StreamWriter sw = new StreamWriter("Reports/Report.txt", false, System.Text.Encoding.UTF8))
            {
                foreach (var user in users)
                {
                    await sw.WriteLineAsync($"{user.Id}\t\t{user.Name}\t\t\t{user.Sex}\t\t{user.Login}\t\t{user.Salary}\t\t{user.Age}\t\t{user.BirthDay}");
                }
            }

            string filePath = Path.Combine(_appEnvironment.ContentRootPath, "Reports/Report.txt");
            string fileType = "text/txt";
            string fileName = "Report.txt";
            return PhysicalFile(filePath, fileType, fileName);
        }
    }
}