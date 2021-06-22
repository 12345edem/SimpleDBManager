using System;
using System.IO;
using System.Text;
using System.Web;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

using NHibernate;

using iTextSharp.text;
using iTextSharp.text.pdf;

using Task15.Models;
using Task15.Controllers;
using Task15.NHibernate;

namespace Task15.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private DBcontext dbcontext;
        private ISessionFactory sessionFactory;
        private List<User> users;
        private UserController userController;
        private readonly int FontSize = 8;
        public ReportsController(IWebHostEnvironment appEnvironment)
        {
            dbcontext = new DBcontext();
            users = new List<User>();
            userController = new UserController();
            sessionFactory = dbcontext.CreateSessionFactory();
            _appEnvironment = appEnvironment;

            System.Text.EncodingProvider encoding = System.Text.CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(encoding);
        }
        public IActionResult Report()
        {
            users = GetAllUsersData().Result;
            var fileName = DataToPdf("Report", "Список всех работников на").Result;
            return Download(fileName);
        }
        public async Task<List<User>> GetAllUsersData()
        {
            await Task.Run(() => userController.GetAll());
            return userController.usersList;
        }
        private DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            dataTable.TableName = typeof(T).FullName;
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
        public async Task<string> DataToPdf(string fileName, string info)
        {
            var usersTable = CreateDataTable<User>(users);
            var file = new Document();

            file = new Document();
            PdfWriter.GetInstance(file, new FileStream($"Reports/{fileName}.pdf", FileMode.Create));
            file.Open();

            BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font font = new iTextSharp.text.Font(baseFont, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.NORMAL);
            font.Size = FontSize;

            PdfPTable table = new PdfPTable(usersTable.Columns.Count);
            PdfPCell cell = new PdfPCell(new Phrase($"{info} {System.DateTime.Now}", font));

            cell.Colspan = usersTable.Columns.Count;
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            table.AddCell(cell);

            await Task.Run(() =>
            {

                for (int j = 0; j < usersTable.Columns.Count; j++)
                {
                    cell = new PdfPCell(new Phrase(new Phrase(usersTable.Columns[j].ColumnName, font)));
                    cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                }

                for (int j = 0; j < usersTable.Rows.Count; j++)
                {
                    for (int k = 0; k < usersTable.Columns.Count; k++)
                    {
                        table.AddCell(new Phrase(usersTable.Rows[j][k].ToString(), font));
                    }
                }
            });


            file.Add(table);
            file.Close();
            return fileName;
        }
        public IActionResult Download(string fileName)
        {
            var file = Path.Combine(_appEnvironment.ContentRootPath, "Reports/", $"{fileName}.pdf");
            var dowload = File(System.IO.File.ReadAllBytes(file), "application/octet-stream", $"{fileName}.pdf");
            
            System.IO.File.Delete(file);
            return dowload;
        }
    }
}