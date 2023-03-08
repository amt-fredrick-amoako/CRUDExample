using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using Serilog;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PersonsGetServiceChild : PersonsGetService
    {
        public PersonsGetServiceChild(IPersonsRepository personsRepository, ILogger<PersonsGetServiceChild> logger, IDiagnosticContext diagnosticContext) : base(personsRepository, logger, diagnosticContext)
        {
            
        }

        public async override Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream(); //Create an in memory stream
            using ExcelPackage excelPackage = new ExcelPackage(memoryStream); //Use properties in excel package with obj of memoryStream
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet"); //Create a workbook, and add a worksheet and save it in worksheet return type

            /*Create Headers for the first row in the worksheet*/
            worksheet.Cells["A1"].Value = "Person Name";
            worksheet.Cells["B1"].Value = "Age";
            worksheet.Cells["C1"].Value = "Gender";


            /*set style properties using the Excel Range to specify the range of rows that should be affected*/
            using ExcelRange headerCells = worksheet.Cells["A1:C1"];
            headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            headerCells.Style.Font.Bold = true;



            int row = 2;
            List<PersonResponse> persons = await GetAllPersons();//get a list of persons

            /*Iterate over the list and assign values to the cells*/
            foreach (PersonResponse person in persons)
            {
                worksheet.Cells[row, 1].Value = person.PersonName;
                worksheet.Cells[row, 2].Value = person.Age;
                worksheet.Cells[row, 3].Value = person.Gender;




                row++;
            }

            //using ExcelRange nameCells = worksheet.Cells[$"A2:A{row}"];
            //nameCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            //nameCells.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);


            worksheet.Cells[$"A1:H{row}"].AutoFitColumns(); //

            await excelPackage.SaveAsync();

            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
