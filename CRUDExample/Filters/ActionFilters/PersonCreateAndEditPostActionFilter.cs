using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repositories;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CRUDExample.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<PersonCreateAndEditPostActionFilter> _logger;

        public PersonCreateAndEditPostActionFilter(ICountryService countryService, ILogger<PersonCreateAndEditPostActionFilter> logger)
        {
            _countryService = countryService;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("In before logic of PersonsCreatedAndEdit Action filter");

            if (context.Controller is PersonsController personsController) //check if the controller is of the same type as PersonsController, and convert to personsController variable
            {
                //TO DO: before
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> coutries = await _countryService.GetCountryList();
                    personsController.ViewBag.CountryList = coutries.Select(country => new SelectListItem { Text = country.CountryName, Value = country.CountryId.ToString() });
                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage).ToList();
                    var personRequest = context.ActionArguments["personRequest"];
                    context.Result = personsController.View(personRequest); //short-circuits or skips the subsequesnt action filters & action method
                }
                else
                {

                    await next();
                    //TO DO: after
                }
            }
            else
            {
                await next();
            }

            _logger.LogInformation("In after logic of PersonsCreatedAndEdit Action filter");


        }
    }
}
