using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using NHibernate;

using Task15.Models;
using Task15.NHibernate;

namespace Task15.Controllers
{
    public class UserController : Controller
    {
        private DBContextBindings dBContextBindings;
        private DBContext dbcontext;
        public ISessionFactory sessionFactory;
        public List<User> usersList;
        public UserController()
        {
            dBContextBindings = new DBContextBindings("localhost", 5432, "postgres", "postgres", "root");
            dbcontext = new DBContext();
            usersList = new List<User>();
            sessionFactory = dbcontext.CreateSessionFactory(dBContextBindings);
        }
        public async Task<IActionResult> GetAll()
        {
            IQueryable<User> users = null;
            using (var session = sessionFactory.OpenStatelessSession())
            {
                await Task.Run(() => users = session.Query<User>().OrderBy(user => user.Id));
                usersList = users.ToList();
                session.Close();
            }

            return View(usersList);
        }
        public IActionResult AddIndex()
        {
            return View();
        }
        public async Task<IActionResult> Add(User user)
        {
            user.Age = DateTime.Now.Year - user.BirthDay.Year;

            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    await Task.Run(() => session.Save(user));
                    await Task.Run(() => transaction.Commit());
                }
                session.Close();
            }


            return RedirectToAction("AddIndex", "User");
        }
        public async Task<IActionResult> Delete(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var user = session.Get<User>(id);
                using (var transaction = session.BeginTransaction())
                {
                    await Task.Run(() => session.Delete(user));
                    await Task.Run(() => transaction.Commit());
                }
                session.Close();
            }

            return RedirectToAction("GetAll", "User");
        }
        public IActionResult FindIndex()
        {
            return View();
        }
        public async Task<IActionResult> Find(string login, string name,
            float salary, string salaryOption, string sex)
        {
            if (sex == "Any") sex = default(string);

            IQueryable<User> users = null;

            using (var session = sessionFactory.OpenStatelessSession())
            {
                switch (salaryOption)
                {
                    case "More Than":
                        await Task.Run(() => users = session.Query<User>()
                            .Where(u => name == default(string) || u.Name.Contains(name))
                            .Where(u => login == default(string) || u.Login.Contains(login))
                            .Where(u => salary == default(float) || u.Salary > salary)
                            .Where(u => sex == default(string) || u.Sex == sex)
                            .OrderBy(user => user.Id));
                        break;
                    case "Less Than":
                        await Task.Run(() => users = session.Query<User>()
                            .Where(u => name == default(string) || u.Name.Contains(name))
                            .Where(u => login == default(string) || u.Login.Contains(login))
                            .Where(u => salary == default(float) || u.Salary < salary)
                            .Where(u => sex == default(string) || u.Sex == sex)
                            .OrderBy(user => user.Id));
                        break;
                    default:
                        await Task.Run(() => users = session.Query<User>()
                            .Where(u => name == default(string) || u.Name.Contains(name))
                            .Where(u => login == default(string) || u.Login.Contains(login))
                            .Where(u => salary == default(float) || u.Salary == salary)
                            .Where(u => sex == default(string) || u.Sex == sex)
                            .OrderBy(user => user.Id));
                        break;
                };
                usersList = users.ToList();
                session.Close();
            }
            return View(usersList);
        }
        public async Task<IActionResult> UpdateIndex(int id)
        {
            var user = new User();
            using (var session = sessionFactory.OpenStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    await Task.Run(() => user = session.Query<User>().Where(u => u.Id == id).FirstOrDefault());
                    await Task.Run(() => transaction.Commit());
                }
                session.Close();
            }

            return View(user);
        }
        public async Task<IActionResult> Update(User updatedUser)
        {
            using (var session = sessionFactory.OpenStatelessSession())
            {
                var user = session.Get<User>(updatedUser.Id);
                user.Id = updatedUser.Id;

                if (updatedUser.Login == default(string) || updatedUser.Login == "")
                {
                    updatedUser.Login = user.Login;
                }
                if (updatedUser.Name == default(string) || updatedUser.Name == "")
                {
                    updatedUser.Name = user.Name;
                }
                if (updatedUser.Salary == default(float))
                {
                    updatedUser.Salary = user.Salary;
                }
                if (updatedUser.Age == default(int))
                {
                    updatedUser.Age = user.Age;
                }
                if (updatedUser.BirthDay == default(DateTime))
                {
                    updatedUser.BirthDay = user.BirthDay;
                }
                user = updatedUser;

                using (var transaction = session.BeginTransaction())
                {

                    await Task.Run(() => session.Update(user));
                    await Task.Run(() => transaction.Commit());
                }
                session.Close();
            }

            return RedirectToAction("Find", "User");
        }
    }
}