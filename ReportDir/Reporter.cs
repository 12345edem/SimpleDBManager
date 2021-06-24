using System;
using System.IO;
using System.Web;
using System.Text;
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
using Task15.NHibernate;
using Task15.Controllers;

namespace Task15.Reporters
{
    public class Reporter<TData>
    {
        private DBContextBindings dBContextBindings;
        private DBContext dbcontext;
        UserController userController = new UserController();
        private ISessionFactory sessionFactory;
        private readonly int FontSize = 8;
        public string SaveDirectory;
        public Reporter(string saveDir)
        {
            SaveDirectory = saveDir;
            dBContextBindings = new DBContextBindings("localhost", 5432, "postgres", "postgres", "root");
            dbcontext = new DBContext();
            sessionFactory = dbcontext.CreateSessionFactory(dBContextBindings);

            System.Text.EncodingProvider encoding = System.Text.CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(encoding);
        }
        public FileResult CreateReport(List<TData> data, string filename, string header)
        {
            var fileName = DataToPdf(data, filename, header).Result;

            var preparedFile = Path.Combine(SaveDirectory + "/", $"{fileName}.pdf");
            var download =  userController.File(System.IO.File.ReadAllBytes(preparedFile), "application/octet-stream", $"{fileName}.pdf");
            System.IO.File.Delete(preparedFile);
            
            return download; 
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
        private async Task<string> DataToPdf(List<TData> data, string fileName, string header)
        {
            var usersTable = CreateDataTable<TData>(data);

            var file = new Document();

            file = new Document();
            PdfWriter.GetInstance(file, new FileStream($"{SaveDirectory}/{fileName}.pdf", FileMode.Create));
            file.Open();

            BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font font = new iTextSharp.text.Font(baseFont, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.NORMAL);
            font.Size = FontSize;


            PdfPTable table = new PdfPTable(usersTable.Columns.Count);
            PdfPCell cell = new PdfPCell(new Phrase($"{header} {System.DateTime.Now}", font));

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
    }
}