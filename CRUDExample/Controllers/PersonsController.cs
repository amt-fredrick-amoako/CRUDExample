using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personService;
        private readonly ICountryService _countryService;

        public PersonsController(IPersonsService personService, ICountryService countryService)
        {
            _personService = personService;
            _countryService = countryService;
        }


        [Route("[action]")]
        [Route("/")]
        public async Task<IActionResult> Index(string searchBy,
            string searchString,
            string sortBy = nameof(PersonResponse.PersonName),
            SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            //Search
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "DOB" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryName), "Country" },
                { nameof(PersonResponse.Address), "Address" },
            };
            List<PersonResponse> persons = await _personService.GetFilteredPersons(searchBy, searchString);
            ViewBag.SearchBy = searchBy;
            ViewBag.SearchString = searchString;

            //Sort
            List<PersonResponse> sortedPersons = await _personService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder.ToString();

            return View(sortedPersons);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countryService.GetCountryList();
            ViewBag.CountryList = countries.Select(country => new SelectListItem
            {
                Text = country.CountryName,
                Value = country.CountryId.ToString()
            });

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> coutries = await _countryService.GetCountryList();
                ViewBag.CountryList = coutries;
                ViewBag.Errors = ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage).ToList();

                return View();
            }

            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);

            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse personResponse = await _personService.GetPersonByPersonId(personID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            List<CountryResponse> countries = await _countryService.GetCountryList();
            ViewBag.CountryList = countries.Select(country => new SelectListItem
            {
                Text = country.CountryName,
                Value = country.CountryId.ToString()
            });

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse person = await _personService.GetPersonByPersonId(personUpdateRequest.PersonID);
            if (person == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                PersonResponse updatedPerson = await _personService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else
            {
                List<CountryResponse> coutries = await _countryService.GetCountryList();
                ViewBag.CountryList = coutries;
                ViewBag.Errors = ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage).ToList();
            }

            return View();
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? personResponse = await _personService.GetPersonByPersonId(personID);
            if (personResponse == null)
                return RedirectToAction("Index");

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateResult)
        {
            PersonResponse? personResponse = await _personService.GetPersonByPersonId(personUpdateResult.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");

            await _personService.DeletePerson(personUpdateResult.PersonID);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> PersonsPDF()
        {
            //Get list of persons
            List<PersonResponse> persons = await _personService.GetAllPersons();


            //Return view as PDF
            return new ViewAsPdf("PersonsPDF", persons, ViewData)
            {
                PageMargins = new Margins()
                {
                    Top = 20, 
                    Right= 20,
                    Bottom = 20,
                    Left = 20
                }, 

                PageOrientation = Orientation.Landscape
            };

            /*
             * Install Rotativa package
             * Write action method to return a new object of ViewAsPDF with the following in the constructor as parameters
             * 1. View
             * 2. Object to pass into the view
             * 3. ViewData
             * 4. Property of name PageMargins should be configured
             * 5. Property of name PageOrientation should be set to either portrait of landscape
             */
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream = await _personService.GetPersonsCSV();
            return File(memoryStream, "application/octet-stream", "persons.csv");

            /* Steps
             * initialize an obj of memoryStream to hold reference to the GetPersonsCSV method of the personsService type
             * return FileResult "File" with the memory stream obj, content-type, and name of file as parameters to the constructor
             */

        }
        
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personService.GetPersonsExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");

            /* Steps
             * initialize an obj of memoryStream to hold reference to the GetPersonsExcel method of the personsService type
             * return FileResult "File" with the memory stream obj, content-type, and name of file as parameters to the constructor
             */

        }
    }
}
