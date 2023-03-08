using AutoFixture;
using Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System.Linq.Expressions;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsGetService _personGetService;
        private readonly IPersonsAdderService _personAdderService;
        private readonly IPersonsSortService _personSortService;
        private readonly IPersonsDeleteService _personDeleteService;
        private readonly IPersonsUpdateService _personUpdateService;
        //private readonly ICountryService countryService;

        private readonly IPersonsRepository _personsRepository; //Holds the mocked methods
        private readonly Mock<IPersonsRepository> _personsRepositoryMock; //Helps define dummy method implementations

        private readonly IFixture fixture;

        private readonly ITestOutputHelper testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            #region Old code out

            //var countriesInitialData = new List<Country>(); //initialize an empty list of fake data store
            //var personsInitialData = new List<Person>(); //initialize an empty list of fake data store

            //DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
            //new DbContextOptionsBuilder<ApplicationDbContext>().Options); //Mock the database
            //ApplicationDbContext dbContext = dbContextMock.Object; //request object of the database

            //countryService = new CountryService(null);


            //dbContextMock.CreateDbSetMock(dbSet => dbSet.Countries, countriesInitialData);
            //dbContextMock.CreateDbSetMock(dbSet => dbSet.Persons, personsInitialData);
            #endregion

            /*Mocking the repository so that the test class doesn't access it's methods directly
             * Create a new object ofthe _personsRepositoryMock and assign it's object to the 
             * object _personsRepository
             */

            fixture = new Fixture();
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object; //creates a dummy object of the repository
            var diagnosticContextMock = new Mock<IDiagnosticContext>();
            var loggerMock = new Mock<ILogger<PersonsGetService>>();

            _personGetService = new PersonsGetService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object); //injects the dummy object of the mocked repository, in effect the dummy methods executes instead of the real one
            _personAdderService = new PersonsAddService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object); //injects the dummy object of the mocked repository, in effect the dummy methods executes instead of the real one
            _personUpdateService = new PersonsUpdateService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object); //injects the dummy object of the mocked repository, in effect the dummy methods executes instead of the real one
            _personSortService = new PersonsSortService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object); //injects the dummy object of the mocked repository, in effect the dummy methods executes instead of the real one
            _personDeleteService = new PersonsDeleteService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object); //injects the dummy object of the mocked repository, in effect the dummy methods executes instead of the real one

            this.testOutputHelper = testOutputHelper;

        }

        /// <summary>
        /// Create new person method
        /// </summary>
        /// <returns>returns a new list of person response</returns>
        private async Task<List<PersonResponse>> CreatePerson()
        {
            //Arrange
            //CountryAddRequest country_request_one = fixture.Create<CountryAddRequest>();
            //CountryAddRequest country_request_two = fixture.Create<CountryAddRequest>();

            //CountryResponse country_response_one = await countryService.AddCountry(country_request_one);
            //CountryResponse country_response_two = await countryService.AddCountry(country_request_two);

            //PersonAddRequest person_request_one = fixture.Build<PersonAddRequest>()
            //    .With(person => person.PersonName, "Kweku")
            //    .With(person => person.Address, "P.V. Obeng Bypass")
            //    .With(person => person.Email, "kweku@example.com")
            //    .With(person => person.CountryID, country_response_one.CountryId)
            //    .Create();
            //PersonAddRequest person_request_two = fixture.Build<PersonAddRequest>()
            //    .With(person => person.PersonName, "Koo Nimo")
            //    .With(person => person.Address, "P.V. Obeng Bypass")
            //    .With(person => person.Email, "koonimo@example.com")
            //    .With(person => person.CountryID, country_response_two.CountryId)
            //    .Create();

            //PersonAddRequest person_request_one = new PersonAddRequest
            //{
            //    PersonName = "Kweku",
            //    Email = "kweku@example.com",
            //    Address = "P.V. Obeng Bypass",
            //    CountryID = country_response_one.CountryId,
            //    DateOfBirth = DateTime.Parse("11-29-1995"),
            //    Gender = GenderOptions.Male,
            //    ReceiveNewsLetters = false
            //};
            //PersonAddRequest person_request_two = new PersonAddRequest
            //{
            //    PersonName = "Kofi",
            //    Email = "kofi@example.com",
            //    Address = "P.V. Obeng Bypass",
            //    CountryID = country_response_two.CountryId,
            //    DateOfBirth = DateTime.Parse("06-26-1998"),
            //    Gender = GenderOptions.Male,
            //    ReceiveNewsLetters = true
            //};

            //List<PersonAddRequest> person_request_list = new List<PersonAddRequest>
            //{
            //    person_request_one, person_request_two
            //};

            List<Person> persons = new List<Person>()
            {
                //Avoid circular reference with nagivation properties by supplying Country as null
                fixture.Build<Person>().With(person => person.Email, "kwekuea1@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea2@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea3@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea4@example.org").With(person => person.Country, null as Country).Create(),

            };

            List<PersonResponse> person_response_list_expected = persons.Select(person => person.ToPersonResponse()).ToList();


            //List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            //foreach (PersonAddRequest personAddRequest in person_request_list)
            //{
            //    PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
            //    person_response_list_from_add.Add(personResponse);
            //}

            return person_response_list_expected;
        }

        #region AddPerson

        [Fact]
        //Throw ArgumentNullException when PersonAddRequest is supplied as null
        public async Task AddPerson_NullPerson_To_ThrowArgumentNullException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            Func<Task> action = async () =>
            {
                await _personAdderService.AddPerson(personAddRequest);
            };
            //using fluent assertions
            action.Should().ThrowAsync<ArgumentNullException>();

            //await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personService.AddPerson(personAddRequest));
        }

        [Fact]
        //Throw ArgumentException when PersonName is null
        public async Task AddPerson_PersonName_IsNull_To_ThrowArgumentException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = fixture.Build<PersonAddRequest>()
                .With(person => person.PersonName, null as string)
                .Create();

            Person person = personAddRequest.ToPerson();
            //When the personsRepository.AddPerson is called, it has to return the same "person" object
            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act

            Func<Task> action = async () =>
            {
                await _personAdderService.AddPerson(personAddRequest);
            };
            action.Should().ThrowAsync<ArgumentException>();

            //await Assert.ThrowsAsync<ArgumentException>(async () => await _personService.AddPerson(personAddRequest));
        }

        [Fact]
        //Throws no error and insert PersonAddRequest into PersonResponse successfully with a newly generated ID
        public async Task AddPerson_FullPersonDetails_ToBeSuccesful()
        {
            //Arrange
            //PersonAddRequest? personAddRequest = new PersonAddRequest
            //{
            //    PersonName = "Kofi",
            //    Email = "kofi@example.com",
            //    Address = "P.V.Obeng ByPass",
            //    CountryID = Guid.NewGuid(),
            //    Gender = GenderOptions.Male,
            //    DateOfBirth = DateTime.Parse("1998-06-26"),
            //    ReceiveNewsLetters = true
            //}; 

            //using AutoFixture for the tests
            //Arrange
            PersonAddRequest? personAddRequest = fixture.Build<PersonAddRequest>()
                .With(person => person.Email, "kofi@example.com")
                .Create();

            Person person = personAddRequest.ToPerson(); //converts personAddRequest to person object
            PersonResponse person_response_expected = person.ToPersonResponse();

            //if supplied any argument value to the AddPerson, is must return the same value
            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            //Act
            PersonResponse personResponse_from_add = await _personAdderService.AddPerson(personAddRequest);
            person_response_expected.PersonID = personResponse_from_add.PersonID;
            //List<PersonResponse> personResponse_from_getall = await _personService.GetAllPersons();

            //Assert
            personResponse_from_add.PersonID.Should().NotBe(Guid.Empty);
            personResponse_from_add.Should().Be(person_response_expected);
            //personResponse_from_getall.Should().Contain(personResponse_from_add);

            //Assert.True(personResponse_from_add.PersonID != Guid.Empty);
            //Assert.Contains(personResponse_from_add, personResponse_from_getall);
        }
        #endregion

        #region GetPersonByID
        //Return null as PersonResponse when PersonID is provided as null
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
        {
            //Arrange
            Guid? PersonID = null;

            //Act
            PersonResponse? person_response_from_get = await _personGetService.GetPersonByPersonId(PersonID);


            //Assert
            //Assert.Null(person_response_from_get);
            person_response_from_get.Should().BeNull();
        }

        //when supplied a valid person id, the valid person details should be returned as a PersonResponse obj
        [Fact]
        public async void GetPersonByPersonID_WithPersonID_ToBeSuccesful()
        {
            //Arrange
            //use countryResponseDTO's countryID property as a foreign key
            //CountryAddRequest country_request = fixture.Create<CountryAddRequest>();
            //CountryResponse country_response = await countryService.AddCountry(country_request);

            //PersonAddRequest person_request = fixture.Build<PersonAddRequest>()
            //    .With(person => person.Email, "koo@example.com")
            //    .Create();

            Person person = fixture.Build<Person>()
                .With(person => person.Email, "koo@example.com")
                .With(person => person.Country, null as Country)
                .Create();

            PersonResponse personResponse_expected = person.ToPersonResponse();


            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);
            //Act
            //var person_response_from_add = await _personService.AddPerson(person_request);
            var person_response_from_get = await _personGetService.GetPersonByPersonId(person.PersonID);


            //Assert
            //Assert.Equal(person_response_from_add, person_response_from_get);

            person_response_from_get.Should().Be(personResponse_expected);
        }
        #endregion

        #region GetAllPersons
        //The GetAllPersons method must return an empty list by default
        [Fact]
        public async Task GetAllPersons_ToBeEmptyList()
        {
            //Arrange
            var persons = new List<Person>();
            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);
            //Act
            List<PersonResponse> person_from_get = await _personGetService.GetAllPersons();

            //Assert
            //Assert.Empty(person_from_get);

            person_from_get.Should().BeEmpty();
        }

        [Fact]
        //Create and add a few instances  of persons and calls GetAllPersons method.this should return the same persons added
        public async Task GetALLPersons_AddFewPersons_ToBeSuccessful()
        {
            //Arrange
            //CountryAddRequest country_request_one = fixture.Create<CountryAddRequest>();
            //CountryAddRequest country_request_two = fixture.Create<CountryAddRequest>();
            //CountryResponse country_response_one = await countryService.AddCountry(country_request_one);
            //CountryResponse country_response_two = await countryService.AddCountry(country_request_two);
            List<Person> persons = new List<Person>()
            {
                //Avoid circular reference with nagivation properties by supplying Country as null
                fixture.Build<Person>().With(person => person.Email, "kwekuea1@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea2@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea3@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea4@example.org").With(person => person.Country, null as Country).Create(),

            };

            #region Pervious Commented Out
            //PersonAddRequest person_request_one = fixture.Build<PersonAddRequest>().With(person => person.Email, "koo@example.com").Create();
            //PersonAddRequest person_request_two = fixture.Build<PersonAddRequest>().With(person => person.Email, "koonimo@example.com").Create();

            //List<PersonAddRequest> person_request_list = new List<PersonAddRequest>
            //{
            //    person_request_one, person_request_two
            //};

            //create a list of person response dto list to hold reference to response from add
            //List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            ////loop over person request list and creates an add request on each item
            ////items are pushed into the person response dto in person_response_list_from_add
            //foreach (PersonAddRequest personAddRequest in person_request_list)
            //{
            //    PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
            //    person_response_list_from_add.Add(personResponse);
            //}
            #endregion

            List<PersonResponse> person_response_list_expected = persons.Select(person => person.ToPersonResponse()).ToList();

            //Prints values of personresponsedto from add request
            this.testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                this.testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Act
            //Get all persons in form of PersonResponseDTO
            List<PersonResponse> person_list_from_get = await _personGetService.GetAllPersons();

            //Assert
            //Checks equality on each single object of person_response_list_from_add in person_list_from_get
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //    Assert.Contains(person_response_from_add, person_list_from_get);
            //}

            person_list_from_get.Should().BeEquivalentTo(person_response_list_expected);

            //Print values of the person response from get
            this.testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in person_list_from_get)
            {
                this.testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
        }
        #endregion

        #region GetFilteredPersons
        [Fact]
        //Search term "PersonName" should return all persons when provided as empty strings
        public async Task GetFilteredPerson_EmptySearchText_ToBeSuccessful()
        {
            //Arrange
            //List<PersonResponse> person_response_list_from_add = await CreatePerson();
            List<Person> persons = new List<Person>()
            {
                //Avoid circular reference with nagivation properties by supplying Country as null
                fixture.Build<Person>().With(person => person.Email, "kwekuea1@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea2@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea3@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea4@example.org").With(person => person.Country, null as Country).Create(),

            };

            List<PersonResponse> person_response_list_expected = persons.Select(person => person.ToPersonResponse()).ToList();

            //Prints values of personresponsedto from add request
            this.testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                this.testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);


            //Act
            //Get all persons in form of PersonResponseDTO
            List<PersonResponse> person_list_from_search = await _personGetService.GetFilteredPersons(nameof(Person.PersonName), "");

            //Assert
            //Checks equality on each single object of person_response_list_from_add in person_list_from_get
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //    Assert.Contains(person_response_from_add, person_list_from_search);
            //}

            person_list_from_search.Should().BeEquivalentTo(person_response_list_expected);

            //Print values of the person response from get
            this.testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in person_list_from_search)
            {
                this.testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
        }

        //First we will add few persons and search based on person name with some search string
        //it should return a few persons
        [Fact]
        //Search term "PersonName" should return all persons when provided as empty strings
        public async Task GetFilteredPerson_NonEmptySearchString_ToBeSuccessful()
        {
            #region Comment out

            //Arrange
            //List<PersonResponse> person_response_list_from_add = await CreatePerson();

            //Prints values of personresponsedto from add request
            //this.testOutputHelper.WriteLine("Expected: ");
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //    this.testOutputHelper.WriteLine(person_response_from_add.ToString());
            //}

            //Act
            //Get all persons in form of PersonResponseDTO
            //List<PersonResponse> person_list_from_search = _personService.GetFilteredPersons(nameof(Person.PersonName), "of");
            //List<PersonResponse> person_list_from_search_email = await _personService.GetFilteredPersons(nameof(Person.Email), "ku");

            //Assert
            //Checks equality on each single object of person_response_list_from_add in person_list_from_get
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //    if (person_response_from_add.PersonName != null)
            //    {
            //        if (person_response_from_add.PersonName.Contains("ku", StringComparison.OrdinalIgnoreCase))
            //        {
            //            Assert.Contains(person_response_from_add, person_list_from_search_email);
            //        }
            //    }
            //}

            //person_list_from_search_email.Should().OnlyContain(person => person.PersonName.Contains("ku", StringComparison.OrdinalIgnoreCase));

            //Print values of the person response from get
            //this.testOutputHelper.WriteLine("Actual: ");
            //foreach (PersonResponse person_response_from_get in person_list_from_search_email)
            //{
            //    this.testOutputHelper.WriteLine(person_response_from_get.ToString());
            //}

            #endregion

            //Search based on person name with some search string. It should return the matching persons

            List<Person> persons = new List<Person>()
            {
                //Avoid circular reference with nagivation properties by supplying Country as null
                fixture.Build<Person>().With(person => person.Email, "kwekuea1@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea2@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea3@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea4@example.org").With(person => person.Country, null as Country).Create(),

            };

            List<PersonResponse> person_response_list_expected = persons.Select(person => person.ToPersonResponse()).ToList();

            //Prints values of personresponsed to from add request
            this.testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                this.testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);


            //Act
            //Get all persons in form of PersonResponseDTO
            List<PersonResponse> person_list_from_search = await _personGetService.GetFilteredPersons(nameof(Person.PersonName), "sa");



            person_list_from_search.Should().BeEquivalentTo(person_response_list_expected);

            //Print values of the person response from get
            this.testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in person_list_from_search)
            {
                this.testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
        }

        #endregion

        #region GetSortedPersons
        [Fact]
        //When we sort personName is desc order, it should return a list of persons in desc order
        public async Task GetSortedPersons_DESC_ToBeSuccessful()
        {
            //Arrange
            //List<PersonResponse> person_response_list_from_add = await CreatePerson();
            List<Person> persons = new List<Person>()
            {
                //Avoid circular reference with nagivation properties by supplying Country as null
                fixture.Build<Person>().With(person => person.Email, "kwekuea1@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea2@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea3@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea4@example.org").With(person => person.Country, null as Country).Create(),

            };

            List<PersonResponse> person_response_list_expected = persons.Select(person => person.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Prints values of personresponsedto from add request
            this.testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                this.testOutputHelper.WriteLine(person_response_from_add.ToString());
            }


            List<PersonResponse> allPersons = await _personGetService.GetAllPersons();


            //Act
            //Get all persons in form of PersonResponseDTO
            //List<PersonResponse> person_list_from_search = _personService.GetFilteredPersons(nameof(Person.PersonName), "of");
            List<PersonResponse> person_list_from_sort = await _personSortService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            //Print values of the person response from get
            this.testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_sort in person_list_from_sort)
            {
                this.testOutputHelper.WriteLine(person_response_from_sort.ToString());
            }

            //person_response_list_from_add = person_response_list_from_add.OrderByDescending(person => person.PersonName).ToList();


            //Assert
            //Checks equality on each single object of person_response_list_from_add in person_list_from_sort
            //for (int i = 0; i < person_response_list_from_add.Count; i++)
            //{
            //    Assert.Equal(person_response_list_from_add[i], person_list_from_sort[i]);
            //}

            //person_list_from_sort.Should().BeEquivalentTo(person_response_list_from_add);
            //BeEquivalentTo iterates over collections and compares them, can be used but better approach with BeInDescendingOrder is better
            person_list_from_sort.Should().BeInDescendingOrder(person => person.PersonName);
        }

        [Fact]
        //When we sort personName is asc order, it should return a list of persons in desc order
        public async Task GetSortedPersons_ASC_ToBeSuccessful()
        {
            #region Old
            //Arrange
            //List<PersonResponse> person_response_list_from_add = await CreatePerson();
            //List<PersonResponse> allPersons = await _personService.GetAllPersons();

            //person_response_list_from_add = person_response_list_from_add.OrderBy(person => person.PersonName).ToList();

            ////Prints values of personresponsedto from add request
            //this.testOutputHelper.WriteLine("Expected: ");
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //    this.testOutputHelper.WriteLine(person_response_from_add.ToString());
            //}

            ////Act
            ////Get all persons in form of PersonResponseDTO
            ////List<PersonResponse> person_list_from_search = _personService.GetFilteredPersons(nameof(Person.PersonName), "of");
            //List<PersonResponse> person_list_from_sort = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.ASC);

            ////Print values of the person response from get
            //this.testOutputHelper.WriteLine("Actual: ");
            //foreach (PersonResponse person_response_from_sort in person_list_from_sort)
            //{
            //    this.testOutputHelper.WriteLine(person_response_from_sort.ToString());
            //}


            ////Assert
            ////Checks equality on each single object of person_response_list_from_add in person_list_from_sort
            ////for (int i = 0; i < person_response_list_from_add.Count; i++)
            ////{
            ////    Assert.Equal(person_response_list_from_add[i], person_list_from_sort[i]);
            ////}

            ////BeInAscendingOrder should be used
            //person_response_list_from_add.Should().BeInAscendingOrder(person => person.PersonName);
            #endregion

            //Arrange
            //List<PersonResponse> person_response_list_from_add = await CreatePerson();
            List<Person> persons = new List<Person>()
            {
                //Avoid circular reference with nagivation properties by supplying Country as null
                fixture.Build<Person>().With(person => person.Email, "kwekuea1@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea2@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea3@example.org").With(person => person.Country, null as Country).Create(),
                fixture.Build<Person>().With(person => person.Email, "kwekuea4@example.org").With(person => person.Country, null as Country).Create(),

            };

            List<PersonResponse> person_response_list_expected = persons.Select(person => person.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Prints values of personresponsedto from add request
            this.testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                this.testOutputHelper.WriteLine(person_response_from_add.ToString());
            }


            List<PersonResponse> allPersons = await _personGetService.GetAllPersons();


            //Act
            //Get all persons in form of PersonResponseDTO
            //List<PersonResponse> person_list_from_search = _personService.GetFilteredPersons(nameof(Person.PersonName), "of");
            List<PersonResponse> person_list_from_sort = await _personSortService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.ASC);

            //Print values of the person response from get
            this.testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_sort in person_list_from_sort)
            {
                this.testOutputHelper.WriteLine(person_response_from_sort.ToString());
            }

            person_list_from_sort.Should().BeInAscendingOrder(person => person.PersonName);
        }
        #endregion

        #region UpdatePerson
        //when we supply null as the personUpdateRequest, it should throw 
        //ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;
            //Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    //Act
            //    await _personService.UpdatePerson(person_update_request);
            //});

            //Act
            Func<Task> action = async () =>
            {
                await _personUpdateService.UpdatePerson(person_update_request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //When the ID of the person is new, it should throw Argument exception
        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            //Arrange
            //PersonUpdateRequest? person_update_request = new PersonUpdateRequest
            //{
            //    PersonID = Guid.NewGuid(),
            //};

            PersonUpdateRequest? person_update_request = fixture.Build<PersonUpdateRequest>().Create();

            ////Assert
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    //Act
            //    await _personService.UpdatePerson(person_update_request);
            //});

            //Act
            Func<Task> action = async () =>
            {
                await _personUpdateService.UpdatePerson(person_update_request);
            };
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When person name is null it should throw Argument Exception
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentNullException()
        {
            //Arrange
            //CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();
            //;CountryAddRequest countryAddRequest = new CountryAddRequest
            //{
            //    CountryName = "USA"
            //};

            //CountryResponse countryResponse = await countryService.AddCountry(countryAddRequest);

            //PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
            //    .With(person => person.PersonName, "Kweku")
            //    .With(person => person.Email, "Kweku@example.org")
            //    .With(person => person.CountryID, countryResponse.CountryId)
            //    .Create();

            Person person = fixture.Build<Person>()
                .With(person => person.PersonName, null as string)
                .With(person => person.Email, "Kweku@example.org")
                .With(person => person.Country, null as Country)
                .With(person => person.Gender, "male")
                .Create();
            //PersonAddRequest personAddRequest = new PersonAddRequest
            //{
            //    PersonName = "Kweku",
            //    CountryID = countryResponse.CountryId,
            //    Email = "kweku@example.com",
            //    DateOfBirth = DateTime.Parse("11-29-1995"),
            //    Address = "Manhattan",
            //    Gender = GenderOptions.Male,
            //};

            //PersonResponse personResponse_from_add = await _personService.AddPerson(personAddRequest);
            PersonResponse personResponse_from_add = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = personResponse_from_add.ToPersonUpdateRequest();

            //person_update_request.PersonName = null;

            //Asset
            //await Assert.ThrowsAsync<ArgumentException>(async () => { await _personService.UpdatePerson(person_update_request); });

            //Act
            Func<Task> action = async () =>
            {
                await _personUpdateService.UpdatePerson(person_update_request);
            };
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_PersonNameFullDetails_ToBeSuccessful()
        {
            //Arrange
            //CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();

            //CountryResponse countryResponse = await countryService.AddCountry(countryAddRequest);

            //PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
            //    .With(person => person.PersonName, "Kweku")
            //    .With(person => person.Email, "Kweku@example.org")
            //    .With(person => person.CountryID, countryResponse.CountryId)
            //    .Create();

            //PersonResponse personResponse_from_add = await _personService.AddPerson(personAddRequest);
            //person_update_request.PersonName = "Anokye";
            //person_update_request.Email = "kweku@protonmail.com";
            //Act
            //PersonResponse personResponse_from_get = await _personService.GetPersonByPersonId(person_update_request.PersonID);

            //Asset
            //Assert.Equal(personResponse_from_get, personResponse_from_update);


            Person person = fixture.Build<Person>()
            .With(temp => temp.Email, "someone@example.com")
            .With(temp => temp.Country, null as Country)
            .With(temp => temp.Gender, "Male")
            .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

            _personsRepositoryMock
             .Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
             .ReturnsAsync(person);

            _personsRepositoryMock
             .Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
             .ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_update = await _personUpdateService.UpdatePerson(person_update_request);

            //Assert
            person_response_from_update.Should().Be(person_response_expected);

        }
        #endregion


        #region DeletePerson
        //if you supply an valid personID, it should return true
        [Fact]
        public async Task DeletePerson_validPersonID_ToBeSuccessful()
        {
            //Arrange
            //CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();

            //CountryResponse country_response_from_add = await countryService.AddCountry(countryAddRequest);

            //PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
            //    .With(person => person.PersonName, "Kweku")
            //    .With(person => person.Email, "Kweku@example.org")
            //    .With(person => person.CountryID, country_response_from_add.CountryId)
            //    .Create();
            //PersonResponse personResponse_from_add = await _personService.AddPerson(personAddRequest);

            Person person = fixture.Build<Person>()
                .With(person => person.PersonName, "Kweku")
                .With(person => person.Email, "kweku@example.com")
                .With(person => person.Country, null as Country)
                .With(person => person.Gender, "Male")
                .Create();

            _personsRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(true);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);
            //Act
            bool isDeleted = await _personDeleteService.DeletePerson(person.PersonID);

            //Assert.True(isDeleted);
            isDeleted.Should().BeTrue();

        }

        //if you supply an invalid personID, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID_ToThrowError()
        {
            //Arrange
            //CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();

            //CountryResponse country_response_from_add = await countryService.AddCountry(countryAddRequest);

            //PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
            //    .With(person => person.PersonName, "Kweku")
            //    .With(person => person.Email, "Kweku@example.org")
            //    .With(person => person.CountryID, country_response_from_add.CountryId)
            //    .Create();

            Person person = fixture.Build<Person>()
                .With(person => person.PersonName, "Kweku")
                .With(person => person.Email, "kweku@example.com")
                .With(person => person.Country, null as Country)
                .With(person => person.Gender, "Male")
                .Create();

            //PersonResponse personResponse_from_add = await _personService.AddPerson(personAddRequest);
            _personsRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(false);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            bool isDeleted = await _personDeleteService.DeletePerson(Guid.NewGuid());

            //Assert.False(isDeleted);
            isDeleted.Should().BeFalse();

        }
        #endregion

    }
}
