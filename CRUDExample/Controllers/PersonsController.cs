using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

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
        public IActionResult Index(string searchBy,
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
            List<PersonResponse> persons = _personService.GetFilteredPersons(searchBy, searchString);
            ViewBag.SearchBy = searchBy;
            ViewBag.SearchString = searchString;

            //Sort
            List<PersonResponse> sortedPersons = _personService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder.ToString();

            return View(sortedPersons);
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult Create()
        {
            List<CountryResponse> countries = _countryService.GetCountryList();
            ViewBag.CountryList = countries.Select(country => new SelectListItem
            {
                Text = country.CountryName,
                Value = country.CountryId.ToString()
            });

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> coutries = _countryService.GetCountryList();
                ViewBag.CountryList = coutries;
                ViewBag.Errors = ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage).ToList();

                return View();
            }

            PersonResponse personResponse = _personService.AddPerson(personAddRequest);

            return RedirectToAction("Index", "Persons");
        }
    }
}
