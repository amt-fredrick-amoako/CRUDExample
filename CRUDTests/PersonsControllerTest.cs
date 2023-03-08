using AutoFixture;
using CRUDExample.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests;

public class PersonsControllerTest
{
    private readonly IPersonsGetService _personsGetService;
    private readonly IPersonsSortService _personsSortService;
    private readonly IPersonsAdderService _personsAddService;
    private readonly IPersonsUpdateService _personsUpdateService;
    private readonly IPersonsDeleteService _personsDeleteService;

    private readonly ICountryService _countryService;
    private readonly IFixture _fixture;
    private readonly ILogger<PersonsController> _logger;

    private readonly Mock<ICountryService> _countryServiceMock;

    private readonly Mock<IPersonsGetService> _personsGetServiceMock;
    private readonly Mock<IPersonsSortService> _personsSortServiceMock;
    private readonly Mock<IPersonsAdderService> _personsAddServiceMock;
    private readonly Mock<IPersonsDeleteService> _personsDeleteServiceMock;
    private readonly Mock<IPersonsUpdateService> _personsUpdateServiceMock;

    private readonly Mock<ILogger<PersonsController>> _loggerMock;

    public PersonsControllerTest()
    {
        _personsGetServiceMock = new Mock<IPersonsGetService>();
        _personsSortServiceMock = new Mock<IPersonsSortService>();
        _personsAddServiceMock = new Mock<IPersonsAdderService>();
        _personsUpdateServiceMock = new Mock<IPersonsUpdateService>();
        _personsDeleteServiceMock = new Mock<IPersonsDeleteService>();


        _countryServiceMock = new Mock<ICountryService>();
        _loggerMock = new Mock<ILogger<PersonsController>>();

        _fixture = new Fixture();
        _personsGetService = _personsGetServiceMock.Object;
        _personsAddService = _personsAddServiceMock.Object;
        _personsUpdateService = _personsUpdateServiceMock.Object;
        _personsSortService = _personsSortServiceMock.Object;
        _personsDeleteService = _personsDeleteServiceMock.Object;

        _countryService = _countryServiceMock.Object;
        _logger = _loggerMock.Object;

    }

    #region Index
    [Fact]
    public async Task Index_Should_Return_IndexView_With_Persons_List()
    {
        //Arrange
        List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();
        //var logger = new Mock<ILogger<PersonsController>>();


        PersonsController personsController = new PersonsController(_personsGetService, _countryService, _logger, _personsAddService, _personsSortService, _personsUpdateService,_personsDeleteService);

        _personsGetServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(persons_response_list);
        _personsSortServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(persons_response_list);

        //Act
        IActionResult result = await personsController.Index(_fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<SortOrderOptions>());

        //Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        viewResult.ViewData.Model.Should().BeAssignableTo<List<PersonResponse>>();
        viewResult.ViewData.Model.Should().Be(persons_response_list);

        /*Steps
         * 1. Create an object of the controller
         * 2. Mock the methods that are called inside the controller action method
         * 3. Call the Action method and pass the return value to the IActionResult object
         * 4. Type cast to the appropriate result based on the test requirement
         * 5. Test the model object throught the viewdata
         * 6. Test your response.
         */
    }
    #endregion

    #region Create
    //[Fact]
    //public async Task Create_IfModelErrors_ShouldReturnCreateView()
    //{
    //    //Arrange
    //    PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();
    //    PersonResponse person_response = _fixture.Create<PersonResponse>();
    //    List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();
    //    //var logger = new Mock<ILogger<PersonsController>>();



    //    _countryServiceMock.Setup(temp => temp.GetCountryList()).ReturnsAsync(countries);
    //    _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(person_response);

    //    PersonsController personsController = new PersonsController(_personsService, _countryService, _logger);

    //    //Act
    //    personsController.ModelState.AddModelError("PersonName", "Person Name should not be empty");//add model state error yourself

    //    IActionResult result = await personsController.Create(person_add_request);

    //    //Assert
    //    ViewResult viewResult = Assert.IsType<ViewResult>(result);
    //    viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
    //    viewResult.ViewData.Model.Should().Be(person_add_request);
    //}

    [Fact]
    public async Task Create_IfNoModelErrors_ShouldReturnRedirectToIndex()
    {
        //Arrange
        PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();
        PersonResponse person_response = _fixture.Create<PersonResponse>();
        List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();
        //var logger = new Mock<ILogger<PersonsController>>();



        _countryServiceMock.Setup(temp => temp.GetCountryList()).ReturnsAsync(countries);
        _personsAddServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(person_response);

        PersonsController personsController = new PersonsController(_personsGetService, _countryService, _logger, _personsAddService, _personsSortService, _personsUpdateService, _personsDeleteService);

        //Act
        IActionResult result = await personsController.Create(person_add_request);

        //Assert
        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
        redirectResult.ActionName.Should().Be("Index");
    }
    #endregion


}
