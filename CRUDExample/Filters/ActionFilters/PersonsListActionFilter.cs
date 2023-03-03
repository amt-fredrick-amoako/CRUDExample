using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace CRUDExample.Filters.ActionFilters
{

    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //To do: add before logic here
            _logger.LogInformation("{FilterName}.{ActionName} executed", nameof(PersonsListActionFilter),nameof(OnActionExecuted));
            //typecase the context's controller to personsController before you're able to reach the ViewData Object
            PersonsController personsController = (PersonsController)context.Controller;

            IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

            if (parameters != null)
            {
                if (parameters.ContainsKey("searchBy"))
                    personsController.ViewData["SearchBy"] = Convert.ToString(parameters["searchBy"]);

                if (parameters.ContainsKey("searchString"))
                    personsController.ViewData["SearchString"] = Convert.ToString(parameters["searchString"]);

                if (parameters.ContainsKey("sortBy"))
                    personsController.ViewData["SortBy"] = Convert.ToString(parameters["sortBy"]);

                if (parameters.ContainsKey("sortOrder"))
                    personsController.ViewData["SortOrder"] = Convert.ToString(parameters["sortOrder"]);
            }

            //shift search to action method to simplify code in the controller
            personsController.ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "DOB" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryName), "Country" },
                { nameof(PersonResponse.Address), "Address" },
            };
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //available everywhere, so this can be made available in OnActionExecuted Filter
            context.HttpContext.Items["arguments"] = context.ActionArguments;
            //To do: add before logic here
            _logger.LogInformation("{FilterName}.{ActionName} executed", nameof(PersonsListActionFilter), nameof(OnActionExecuting));

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                //validate the searchBy parameter value
                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryID),
                        nameof(PersonResponse.Address)
                    };

                    //reset the searchBy parameter value
                    if (searchByOptions.Any(option => option == searchBy) == false)
                    {
                        _logger.LogInformation("searchBy actual value {searchBy}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        _logger.LogInformation("searchBy updated value {searchBy}", context.ActionArguments["searchBy"]);
                    }
                }

            }
        }
    }
}

/*Notes:
 * ActionFilters can be used to
 * 1. Validate models before working with them
 * 2. Supply values to the ViewData or ViewBag for the UI
 * 3. Pass parameters as response headers etc
 */
