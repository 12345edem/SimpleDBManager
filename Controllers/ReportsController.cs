using System;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

using Task15.Reporters;
using Task15.Models;

namespace Task15.Controllers
{
    public class ReportsController : Controller
    {
        private UserController userController = new UserController();
        public IActionResult ReportIndex()
        {
            return View();
        }
        public async Task<IActionResult> GetAll()
        {
            var reporter = new Reporter<User>("ReportDir");

            await Task.Run(() => userController.GetAll());
            return reporter.CreateReport(userController.usersList, "ReportAll", "Список всех сотрудников на: ");
        }
        public async Task<IActionResult> GetUniqueSalary()
        {
            var reporter = new Reporter<User>("ReportDir");
            var salaries = new List<User>();

            await Task.Run(() => userController.GetAll());
            var users = userController.usersList;

            var uniqueSalaries = users.Where(u => u.Name != null).GroupBy(u => u.Salary).Select(u => u.FirstOrDefault()).ToList();

            return reporter.CreateReport(uniqueSalaries, "ReportUniqueSalary", "Список сотрудников с уникальными зарплатами на: ");
        }
        public async Task<IActionResult> GetUniqueAges()
        {
            var reporter = new Reporter<ReportAgeModel>("ReportDir");
            var ages = new List<ReportAgeModel>();

            await Task.Run(() => userController.GetAll());
            var users = userController.usersList;

            var uniqueAges = users.Select(u => u.Age).Distinct();
            
            int i = 1;
            foreach(var uniqueAge in uniqueAges)
            {
                ages.Add( new ReportAgeModel(){Name=$"Age id {i}", Age=uniqueAge});
                i += 1;
            }

            return reporter.CreateReport(ages, "ReportUnoqueAges", "Список уникальных возрастов на: ");
        }
        public async Task<IActionResult> GetMaxAndMinSalary()
        {
            var reporter = new Reporter<ReportSalaryModel>("ReportDir");
            var salaries = new List<ReportSalaryModel>();

            await Task.Run(() => userController.GetAll());
            var users = userController.usersList;

            salaries.Add( new ReportSalaryModel(){Name = "Minimun salary", Salary =  users.Min(u => u.Salary)});
            salaries.Add( new ReportSalaryModel(){Name = "Maximum salary", Salary =  users.Max(u => u.Salary)});

            return reporter.CreateReport(salaries, "ReportMaxAndMinSalary", "Минимальная и максимальная зарплаты: ");
        }
        public async Task<IActionResult> GetSummarySalary()
        {
            var reporter = new Reporter<ReportSalaryModel>("ReportDir");
            var salary = new List<ReportSalaryModel>();

            await Task.Run(() => userController.GetAll());
            var users = userController.usersList;

            salary.Add( new ReportSalaryModel(){Name="Summary salary", Salary = users.Sum(u => u.Salary)});

            return reporter.CreateReport(salary, "ReportSummarySalary", "Суммарная зарплата на: ");
        }
        public async Task<IActionResult> GetSalary21And25()
        {
            var reporter = new Reporter<ReportSalaryModel>("ReportDir");
            var salary = new List<ReportSalaryModel>();

            await Task.Run(() => userController.GetAll());
            var users = userController.usersList;

            var salariesBetween = users.Where(u => u.Age >= 21 && u.Age <= 25).Sum(u => u.Salary);
            salary.Add( new ReportSalaryModel(){Name="Summary salary of worker between 21 and 25 years old",Salary=salariesBetween});

            return reporter.CreateReport(salary, "ReportSummarySalaryBetween21And25", "Суммарная зарплата рабочих возрастом от 21 до 25 лет на: ");
        }
        public async Task<IActionResult> GetAverageSalary()
        {
            var reporter = new Reporter<ReportSalaryModel>("ReportDir");
            var salary = new List<ReportSalaryModel>();

            await Task.Run(() => userController.GetAll());
            var users = userController.usersList;

            salary.Add( new ReportSalaryModel(){Name="Average salary", Salary=users.Average(u => u.Salary)});

            return reporter.CreateReport(salary, "ReportAverageSalary", "Средняя зарплата на: ");
        }
        public async Task<IActionResult> GetAverageAge()
        {
            var reporter = new Reporter<ReportAgeModel>("ReportDir");
            var age = new List<ReportAgeModel>();

            await Task.Run(() => userController.GetAll());
            var users = userController.usersList;

            age.Add( new ReportAgeModel(){Name="Average age", Age = users.Average(u => u.Age)});

            return reporter.CreateReport(age, "ReportAverageAge", "Средний возраст на: ");
        }
    }
}