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
    public class PersonsAddService : IPersonsAdderService
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
        public PersonsAddService(IPersonsRepository personsRepository, ILogger<PersonsGetService> logger, IDiagnosticContext diagnosticContext)
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

        public async Task<PersonResponse> AddPerson(PersonAddRequest? person)
        {
            //validate personAddRequestDTO
            if (person == null) throw new ArgumentNullException(nameof(person));
            //if (string.IsNullOrEmpty(person.PersonName)) throw new ArgumentException("Person name is required");

            //model validations
            ValidationHelper.ModelValidation(person);

            //convert and store the personAddRequestDTO to Person obj type and list respectively
            Person newPerson = person.ToPerson();
            newPerson.PersonID = Guid.NewGuid();
            await _personsRepository.AddPerson(newPerson);

            //using store procedure instead of ef 
            //_db.sp_InsertPerson(newPerson);

            return newPerson.ToPersonResponse();
        }
    }
}
