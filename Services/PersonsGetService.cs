using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Globalization;
using OfficeOpenXml;
using System.Drawing;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
using Exceptions;

namespace Services
{
    public class PersonsGetService : IPersonsGetService
    {
        #region Comment on Initial approach
        ////fake a data store for person obj type
        //private readonly List<Person> _db;
        ////fake injecting ICountriesService
        //private readonly ICountryService countryService;

        ////contructor initialization
        //public PersonsService(bool initialize = true)
        //{
        //    //Fake data storage
        //    _db = new List<Person>();
        //    countryService = new CountryService();
        //    if (initialize)
        //    {
        //        _db.Add(new Person
        //        {
        //            PersonName = "Fredrick Amoako",
        //            PersonID = Guid.Parse("B93A901E-2289-4ACE-AD52-0F7B90CB9775"),
        //            Email = "fredrickamoako@example.com",
        //            DateOfBirth = DateTime.Parse("11-30-1997"),
        //            Gender = "Male",
        //            CountryID = Guid.Parse("C703058F-7CB0-43D3-B90B-66973F0AF19F"),
        //            Address = "1660 Topping Ave",
        //            ReceiveNewsLetters = true,

        //        });

        //        _db.Add(new Person
        //        {
        //            PersonName = "Maxwell Amoako Antwi",
        //            PersonID = Guid.Parse("C82D7817-A2D9-4A68-B457-C39FF14EC61E"),
        //            Email = "antwimaxwell@example.com",
        //            DateOfBirth = DateTime.Parse("06-26-1999"),
        //            Gender = "Male",
        //            CountryID = Guid.Parse("C703058F-7CB0-43D3-B90B-66973F0AF19F"),
        //            Address = "1660 Topping Ave",
        //            ReceiveNewsLetters = true,

        //        });

        //        _db.Add(new Person
        //        {
        //            PersonName = "Ellen Amoako Dankwah",
        //            PersonID = Guid.Parse("2DADEABE-343D-4C7F-97A5-7B9ED0D979A3"),
        //            Email = "ellenamoakod@example.com",
        //            DateOfBirth = DateTime.Parse("07-20-1998"),
        //            Gender = "Female",
        //            CountryID = Guid.Parse("C703058F-7CB0-43D3-B90B-66973F0AF19F"),
        //            Address = "LA",
        //            ReceiveNewsLetters = true,

        //        });

        //        _db.Add(new Person
        //        {
        //            PersonName = "Kingsley Kwarteng",
        //            PersonID = Guid.Parse("9911211C-A294-4323-8BF7-6F4FD4B86F53"),
        //            Email = "kingsleykwarteng@example.com",
        //            DateOfBirth = DateTime.Parse("08-11-1996"),
        //            Gender = "Male",
        //            CountryID = Guid.Parse("BC86E026-FADA-482E-AC32-2979E01658ED"),
        //            Address = "Milton Keynes",
        //            ReceiveNewsLetters = false,

        //        });

        //        _db.Add(new Person
        //        {
        //            PersonName = "Owura",
        //            PersonID = Guid.Parse("B8080B29-8D47-489C-BDA5-5019A28F6226"),
        //            Email = "owura@example.com",
        //            DateOfBirth = DateTime.Parse("12-02-2001"),
        //            Gender = "Male",
        //            CountryID = Guid.Parse("56DD8B92-B09F-4FE7-89A8-86D34A10E220"),
        //            Address = "Hamburg",
        //            ReceiveNewsLetters = false,

        //        });

        //        _db.Add(new Person
        //        {
        //            PersonName = "Janet Dwomoh",
        //            PersonID = Guid.Parse("AF458C6F-DF10-4F40-BCF0-56A7558EC00E"),
        //            Email = "jdwomoh@example.com",
        //            DateOfBirth = DateTime.Parse("05-03-1998"),
        //            Gender = "Female",
        //            CountryID = Guid.Parse("912936C6-2B61-4FF2-90E7-827B9814C470"),
        //            Address = "Ontario",
        //            ReceiveNewsLetters = true,

        //        });


        //    }
        //}

        #endregion

        //fake a data store for person obj type
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetService> _logger;
        private readonly IDiagnosticContext _diagnosticContext; //Diagnotic context private field
        //fake injecting ICountriesService


        //contructor initialization
        public PersonsGetService(IPersonsRepository personsRepository, ILogger<PersonsGetService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
        /*Redundant*/
        //reusable method to get country by id and convert to personResponseDTO
        //private PersonResponse ConvertPersonToPersonResponse(Person person)
        //{
        //    PersonResponse personResponse = person.ToPersonResponse();
        //    personResponse.CountryName = person.Country?.CountryName; //access property directly
        //    //personResponse.CountryName = countryService.GetCountryByCountryId(person.CountryID)?.CountryName;
        //    return personResponse;
        //}

        public virtual async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonsService");
            //using stored procedures instead
            //SELECT * FROM Persons
            //return _db.sp_GetAllPersons().Select(person => ConvertPersonToPersonResponse(person)).ToList();
            var persons = await _personsRepository.GetAllPersons();
            return persons.Select(person => person.ToPersonResponse()).ToList();
        }

        public virtual async Task<PersonResponse?> GetPersonByPersonId(Guid? id)
        {
            /* Algorithm
             * 1. Check for null value in ID, throw error if null.
             * 2. If not null, get matching person from data store.
             * 3. If found convert matching person to PersonResponeDTO.
             * 4. Return the PersonResponseDTO object.
             */

            if (id == null) return null;
            Person? person = await _personsRepository.GetPersonByPersonID(id.Value);
            if (person == null) return null;

            return person.ToPersonResponse() ?? null;

        }

        public virtual async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredPersons of Persons Service");
            List<Person> persons;

            using (Operation.Time("Time for filtered persons from database")) //record time taken by the method execution
            {
                persons = searchBy switch
                {
                    nameof(PersonResponse.PersonName) =>
                       await _personsRepository.GetFilteredPersons(person =>
                       person.PersonName.Contains(searchString)),

                    nameof(PersonResponse.Email) =>
                            await _personsRepository.GetFilteredPersons(person =>
                            person.Email.Contains(searchString)),

                    nameof(PersonResponse.Address) =>
                                await _personsRepository.GetFilteredPersons(person =>
                                person.Address.Contains(searchString)),

                    nameof(PersonResponse.CountryID) =>
                                    await _personsRepository.GetFilteredPersons(person =>
                                    person.Country.CountryName.Contains(searchString)),

                    nameof(PersonResponse.Gender) =>
                                        await _personsRepository.GetFilteredPersons(person =>
                                        person.Gender.Contains(searchString)),

                    nameof(PersonResponse.DateOfBirth) =>
                                await _personsRepository.GetFilteredPersons(person =>
                                person.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),
                    _ => await _personsRepository.GetAllPersons()
                };
            }// end of using block

            _diagnosticContext.Set("Persons", persons); //add diagnotic context to log diagnostics, requires the Serilog.Extensions.Hosting from PM

            return (from person in persons select person.ToPersonResponse()).ToList();
        }

        public virtual async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortedOrder)
        {
            _logger.LogInformation("GetSortedPersons method of Persons Service");
            if (string.IsNullOrEmpty(sortBy)) return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortedOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Age).ToList(),

                (nameof(PersonResponse.CountryName), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.CountryName), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

                _ => allPersons
            };

            return sortedPersons;
        }

        public virtual async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdate)
        {
            if (personUpdate == null)
                throw new ArgumentNullException(nameof(personUpdate));

            //validation
            ValidationHelper.ModelValidation(personUpdate);

            //Convert to Person
            Person updatePerson = personUpdate.ToPerson();
            if (updatePerson == null)
                throw new ArgumentException("Given person id doesn't exist");

            //call the stored procedure method to update person object
            //_db.sp_UpdatePerson(updatePerson);

            //get matching person obj to update
            Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personUpdate.PersonID);
            if (matchingPerson == null)
                throw new InvalidPersonIDException("Given person id doesn't exist");

            //update all details
            matchingPerson.PersonName = personUpdate.PersonName;
            matchingPerson.Address = personUpdate.Address;
            matchingPerson.DateOfBirth = personUpdate.DateOfBirth;
            matchingPerson.CountryID = personUpdate.CountryID;
            matchingPerson.ReceiveNewsLetters = personUpdate.ReceiveNewsLetters;
            matchingPerson.Email = personUpdate.Email;
            matchingPerson.Gender = personUpdate.Gender.ToString();

            await _personsRepository.UpdatePerson(matchingPerson);
            return matchingPerson.ToPersonResponse();

            //convert person object to person response and return it
            //return updatePerson.ToPersonResponse();
        }

        public virtual async Task<bool> DeletePerson(Guid? personId)
        {
            if (personId == null)
                throw new ArgumentNullException(nameof(personId));
            Person? person = await _personsRepository.GetPersonByPersonID(personId.Value);
            if (person == null)
                return false;


            //_db.sp_DeletePerson(person.PersonID);
            return await _personsRepository.DeletePersonByPersonID(personId.Value);
        }

        public virtual async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream(); //initialize obj of memoryStreeam
            StreamWriter streamWriter = new StreamWriter(memoryStream); //initialize object of streamWriter to write into memoryStream obj

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);


            CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration); // initialize obj of csvWriter to write into the streamWriter

            //Manually configure headers and data using the WriteField property as shown below
            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
            csvWriter.NextRecord();

            List<PersonResponse> persons = await GetAllPersons();
            //Manually loop through values in the persons obj and generate the csv as demonstrated below
            foreach (PersonResponse person in persons)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                if (person.DateOfBirth.HasValue)
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Gender);
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.ReceiveNewsLetters);
                csvWriter.NextRecord();
                csvWriter.Flush();
            }

            // few lines with auto generated csv
            //CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true); // initialize obj of csvWriter to write into the streamWriter

            //csvWriter.WriteHeader<PersonResponse>(); //Write properties as headers
            //csvWriter.NextRecord(); // moves to a new line

            //await csvWriter.WriteRecordsAsync(persons); //1, abc, ....

            memoryStream.Position = 0; //moves pointer to the beginning of the stream
            return memoryStream;
            /* Steps
             * Create a memory stream object, this will act as a store for your stream
             * Create a stream writer object, this will write into your memory stream
             * Create an object of the CsvHelper with constructor filled with streamwrite obj, cultureinfo.invariantculture and leave open set to true
             * Use property WriteHeader of the csvWrite obj to write property names of model class as headers
             * Use NextRecord property to move to  a new line
             * Load objects from database into a list of the obj type
             * Use the WriteRecord method to write values or properties of each obj in the list seperated by commas
             * Set memoryStream position to 0
             * return memory stream
             */
        }

        public virtual async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream(); //Create an in memory stream
            using ExcelPackage excelPackage = new ExcelPackage(memoryStream); //Use properties in excel package with obj of memoryStream
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet"); //Create a workbook, and add a worksheet and save it in worksheet return type

            /*Create Headers for the first row in the worksheet*/
            worksheet.Cells["A1"].Value = "Person Name";
            worksheet.Cells["B1"].Value = "Email";
            worksheet.Cells["C1"].Value = "Date of Birth";
            worksheet.Cells["D1"].Value = "Age";
            worksheet.Cells["E1"].Value = "Gender";
            worksheet.Cells["F1"].Value = "Country";
            worksheet.Cells["G1"].Value = "Address";
            worksheet.Cells["H1"].Value = "Receive News Letters";

            /*set style properties using the Excel Range to specify the range of rows that should be affected*/
            using ExcelRange headerCells = worksheet.Cells["A1:H1"];
            headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            headerCells.Style.Font.Bold = true;



            int row = 2;
            List<PersonResponse> persons = await GetAllPersons();//get a list of persons

            /*Iterate over the list and assign values to the cells*/
            foreach (PersonResponse person in persons)
            {
                worksheet.Cells[row, 1].Value = person.PersonName;
                worksheet.Cells[row, 2].Value = person.Email;
                if (person.DateOfBirth.HasValue)
                    worksheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 4].Value = person.Age;
                worksheet.Cells[row, 5].Value = person.Gender;
                worksheet.Cells[row, 6].Value = person.CountryName;
                worksheet.Cells[row, 7].Value = person.Address;
                worksheet.Cells[row, 8].Value = person.ReceiveNewsLetters;



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
