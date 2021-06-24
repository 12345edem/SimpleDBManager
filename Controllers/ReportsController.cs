using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

using Task15.Reporters;

namespace Task15.Controllers
{
    public class ReportsController : Controller
    {
        private Reporter reporter = new Reporter("ReportDir");
        private UserController userController = new UserController();
        public IActionResult ReportIndex()
        {
            return View();
        }
        public async Task<IActionResult> GetAll()
        {
            await Task.Run(() => userController.GetAll());
            return reporter.CreateReport(userController.usersList, "ReportAll", "Список всех сотрудников на: ");
        }
        /*public IActionResult GetUniqueSalary()
        {
            
        }
        public IActionResult GetUniqueAges()
        {

        }
        public IActionResult GetMaxAndMinSalary()
        {

        }
        public IActionResult GetSummarySalary()
        {

        }
        public IActionResult GetSalary21And25()
        {

        }
        public IActionResult GetAverageSalary()
        {

        }
        public IActionResult GetAverageAge()
        {

        }*/
    }
}