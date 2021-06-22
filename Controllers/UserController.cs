using System;
using System.Linq;
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
        public UserController()
        {
            dbcontext =  new DBcontext();
            sessionFactory = dbcontext.CreateSessionFactory();
        }
        public IActionResult GetAll()
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

            return View(users);
        }
        public IActionResult AddIndex()
        {
            return View();
        }
        public IActionResult Add(User user)
        {
            user.Age = DateTime.Now.Year - user.BirthDay.Year;

            sessionFactory = dbcontext.CreateSessionFactory();

            using(var session = sessionFactory.OpenSession())
            {
                using(var transaction = session.BeginTransaction())
                {
                    session.Save(user);
                    transaction.Commit();
                }
                session.Close();
            }
            sessionFactory.Close();
            return RedirectToAction("AddIndex", "User");
        }
        public IActionResult Delete(int id)
        {
            sessionFactory = dbcontext.CreateSessionFactory();

            using(var session = sessionFactory.OpenSession())
            {
                using(var transaction = session.BeginTransaction())
                {
                    var user = session.Get<User>(id);
                    session.Delete(user);
                    transaction.Commit();
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
        public IActionResult Find(string login)
        {
            var users = new List<User>();
            using (var session = sessionFactory.OpenStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    users = session.Query<User>().Where(u => u.Login == login).OrderBy(user => user.Id).ToList();
                    transaction.Commit();
                }
                session.Close();
            }
            sessionFactory.Close();
            return View(users);
        }
        public IActionResult UpdateIndex(int id)
        {
            var user = new User();
            using (var session = sessionFactory.OpenStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    user = session.Query<User>().Where(u => u.Id == id).FirstOrDefault();
                    transaction.Commit();
                }
                session.Close();
            }
            sessionFactory.Close();
            return View(user);
        }
        public IActionResult Update(User updatedUser)
        {
            using (var session = sessionFactory.OpenStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var user = session.Get<User>(updatedUser.Id);
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
                    session.Update(user);
                    transaction.Commit();
                }
                session.Close();
            }
            sessionFactory.Close();

            return RedirectToAction("GetAll", "User");
        }
    }
}