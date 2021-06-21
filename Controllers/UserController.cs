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
            return View(new List<User>());
        }
        public IActionResult Find(string name)
        {
            var users = new List<User>();
            using (var session = sessionFactory.OpenStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    users = session.Query<User>().Where(u => u.Name == name).OrderBy(user => user.Id).ToList();
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
            var user = new User();
            using (var session = sessionFactory.OpenStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    user.Id = updatedUser.Id;
                    user = session.Get<User>(user.Id);

                    if(updatedUser.Name == null || updatedUser.Name == "")
                    {
                        updatedUser.Name = session.Get<User>(user.Id).Name;
                    }
                    if(updatedUser.Weight == 0)
                    {
                        updatedUser.Name = session.Get<User>(user.Id).Name;
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