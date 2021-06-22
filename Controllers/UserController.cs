using System;
using System.Linq;
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
        private DBcontext dbcontext;
        private ISessionFactory sessionFactory;
        public List<User> usersList;
        public UserController()
        {
            dbcontext =  new DBcontext();
            usersList = new List<User>();
            sessionFactory = dbcontext.CreateSessionFactory();
        }
        public async Task<IActionResult> GetAll()
        {
            var users = new List<User>();
            using (var session = sessionFactory.OpenStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    await Task.Run(() => users = session.Query<User>().OrderBy(user => user.Id).ToList());
                    await Task.Run(() => transaction.Commit());
                }
                session.Close();
            }
            sessionFactory.Close();

            usersList = users;
            return View(users);
        }
        public IActionResult AddIndex()
        {
            return View();
        }
        public async Task<IActionResult> Add(User user)
        {
            user.Age = DateTime.Now.Year - user.BirthDay.Year;

            sessionFactory = dbcontext.CreateSessionFactory();

            using(var session = sessionFactory.OpenSession())
            {
                using(var transaction = session.BeginTransaction())
                {
                    await Task.Run(() => session.Save(user));
                    await Task.Run(() => transaction.Commit());
                }
                session.Close();
            }
            sessionFactory.Close();

            return RedirectToAction("AddIndex", "User");
        }
        public async Task<IActionResult> Delete(int id)
        {
            sessionFactory = dbcontext.CreateSessionFactory();

            using(var session = sessionFactory.OpenSession())
            {
                using(var transaction = session.BeginTransaction())
                {
                    var user = session.Get<User>(id);
                    await Task.Run(() => session.Delete(user));
                    await Task.Run(() => transaction.Commit());
                }
                session.Close();
            }
            sessionFactory.Close();

            return RedirectToAction("GetAll", "User");
        }
        public IActionResult FindIndex()
        {
            return View();
        }
        public async Task<IActionResult> Find(string login, string name, float salary)
        {
            var users = new List<User>();
            using (var session = sessionFactory.OpenStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    await Task.Run(() => users = session.Query<User>().Where(u => u.Login == login).OrderBy(user => user.Id).ToList());
                    await Task.Run(() => transaction.Commit());
                }
                session.Close();
            }
            sessionFactory.Close();

            usersList = users;
            return View(users);
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
            sessionFactory.Close();

            return View(user);
        }
        public async Task<IActionResult> Update(User updatedUser)
        {
            using (var session = sessionFactory.OpenStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var user =  session.Get<User>(updatedUser.Id);
                    user.Id = updatedUser.Id;

                    if(updatedUser.Login == default(string) || updatedUser.Login == "")
                    {
                        updatedUser.Login = user.Login;
                    }
                    if(updatedUser.Name == default(string) || updatedUser.Name == "")
                    {
                        updatedUser.Name = user.Name;
                    }
                    if(updatedUser.Salary == default(float))
                    {
                        updatedUser.Salary = user.Salary;
                    }
                    if(updatedUser.Age == default(int))
                    {
                        updatedUser.Age = user.Age;
                    }
                    if(updatedUser.BirthDay == default(DateTime))
                    {
                        updatedUser.BirthDay = user.BirthDay;
                    }

                    user = updatedUser;
                    await Task.Run(() => session.Update(user));
                    await Task.Run(() => transaction.Commit());
                }
                session.Close();
            }
            sessionFactory.Close();

            return RedirectToAction("UpdateIndex", "User");
        }
    }
}