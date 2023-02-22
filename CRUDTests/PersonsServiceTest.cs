using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;
using AutoFixture;
using FluentAssertions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;
        private readonly ICountryService countryService;
        private readonly IFixture fixture;

        private readonly ITestOutputHelper testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            var countriesInitialData = new List<Country>(); //initialize an empty list of fake data store
            var personsInitialData = new List<Person>(); //initialize an empty list of fake data store

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options); //Mock the database
            ApplicationDbContext dbContext = dbContextMock.Object; //request object of the database

            countryService = new CountryService(dbContext);
            _personService = new PersonsService(dbContext, countryService);


            dbContextMock.CreateDbSetMock(dbSet => dbSet.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(dbSet => dbSet.Persons, personsInitialData);
            this.testOutputHelper = testOutputHelper;
            fixture = new Fixture();
        }

        /// <summary>
        /// Create new person method
        /// </summary>
        /// <returns>returns a new list of person response</returns>
        private async Task<List<PersonResponse>> CreatePerson()
        {
            //Arrange
            CountryAddRequest country_request_one = fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_two = fixture.Create<CountryAddRequest>();

            CountryResponse country_response_one = await countryService.AddCountry(country_request_one);
            CountryResponse country_response_two = await countryService.AddCountry(country_request_two);

            PersonAddRequest person_request_one = fixture.Build<PersonAddRequest>()
                .With(person => person.PersonName, "Kweku")
                .With(person => person.Address, "P.V. Obeng Bypass")
                .With(person => person.Email, "kweku@example.com")
                .With(person => person.CountryID, country_response_one.CountryId)
                .Create();
            PersonAddRequest person_request_two = fixture.Build<PersonAddRequest>()
                .With(person => person.PersonName, "Koo Nimo")
                .With(person => person.Address, "P.V. Obeng Bypass")
                .With(person => person.Email, "koonimo@example.com")
                .With(person => person.CountryID, country_response_two.CountryId)
                .Create();

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

            List<PersonAddRequest> person_request_list = new List<PersonAddRequest>
            {
                person_request_one, person_request_two
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest personAddRequest in person_request_list)
            {
                PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
                person_response_list_from_add.Add(personResponse);
            }

            return person_response_list_from_add;
        }

        #region AddPerson

        [Fact]
        //Throw ArgumentNullException when PersonAddRequest is supplied as null
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);
            };
            //using fluent assertions
            action.Should().ThrowAsync<ArgumentNullException>();

            //await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personService.AddPerson(personAddRequest));
        }

        [Fact]
        //Throw ArgumentException when PersonName is null
        public async Task AddPerson_PersonName_IsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = fixture.Build<PersonAddRequest>()
                .With(person => person.PersonName, null as string)
                .Create();

            //Act

            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);
            };
            action.Should().ThrowAsync<ArgumentException>();

            //await Assert.ThrowsAsync<ArgumentException>(async () => await _personService.AddPerson(personAddRequest));
        }

        [Fact]
        //Throws no error and insert PersonAddRequest into PersonResponse successfully with a newly generated ID
        public async Task AddPerson_PersonDetails_IsValid()
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
            PersonAddRequest? personAddRequest = fixture.Build<PersonAddRequest>()
                .With(person => person.Email, "kofi@example.com")
                .Create();
            //Act
            PersonResponse personResponse_from_add = await _personService.AddPerson(personAddRequest);
            List<PersonResponse> personResponse_from_getall = await _personService.GetAllPersons();

            //Assert
            personResponse_from_add.PersonID.Should().NotBe(Guid.Empty);
            personResponse_from_getall.Should().Contain(personResponse_from_add);

            //Assert.True(personResponse_from_add.PersonID != Guid.Empty);
            //Assert.Contains(personResponse_from_add, personResponse_from_getall);
        }
        #endregion

        #region GetPersonByID
        //Return null as PersonResponse when PersonID is provided as null
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? PersonID = null;

            //Act
            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonId(PersonID);


            //Assert
            //Assert.Null(person_response_from_get);
            person_response_from_get.Should().BeNull();
        }

        //when supplied a valid person id, the valid person details should be returned as a PersonResponse obj
        [Fact]
        public async void GetPersonByPersonID_WithPersonID()
        {
            //Arrange
            //use countryResponseDTO's countryID property as a foreign key
            CountryAddRequest country_request = fixture.Create<CountryAddRequest>();
            CountryResponse country_response = await countryService.AddCountry(country_request);

            PersonAddRequest person_request = fixture.Build<PersonAddRequest>()
                .With(person => person.Email, "koo@example.com")
                .Create();

            //Act
            var person_response_from_add = await _personService.AddPerson(person_request);

            var person_response_from_get = await _personService.GetPersonByPersonId(person_response_from_add.PersonID);

            //Assert
            //Assert.Equal(person_response_from_add, person_response_from_get);

            person_response_from_get.Should().Be(person_response_from_add);
        }
        #endregion

        #region GetAllPersons
        //The GetAllPersons method must return an empty list by default
        [Fact]
        public async Task GetAllPersons_Empty()
        {
            //Act
            List<PersonResponse> person_from_get = await _personService.GetAllPersons();

            //Assert
            //Assert.Empty(person_from_get);

            person_from_get.Should().BeEmpty();
        }

        [Fact]
        //Create and add a few instances  of persons and calls GetAllPersons method.this should return the same persons added
        public async Task GetALLPersons_AddFewPersons()
        {
            //Arrange
            CountryAddRequest country_request_one = fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_two = fixture.Create<CountryAddRequest>();
            CountryResponse country_response_one = await countryService.AddCountry(country_request_one);
            CountryResponse country_response_two = await countryService.AddCountry(country_request_two);
            PersonAddRequest person_request_one = fixture.Build<PersonAddRequest>().With(person => person.Email, "koo@example.com").Create();
            PersonAddRequest person_request_two = fixture.Build<PersonAddRequest>().With(person => person.Email, "koonimo@example.com").Create();

            List<PersonAddRequest> person_request_list = new List<PersonAddRequest>
            {
                person_request_one, person_request_two
            };

            //create a list of person response dto list to hold reference to response from add
            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            //loop over person request list and creates an add request on each item
            //items are pushed into the person response dto in person_response_list_from_add
            foreach (PersonAddRequest personAddRequest in person_request_list)
            {
                PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
                person_response_list_from_add.Add(personResponse);
            }

            //Prints values of personresponsedto from add request
            this.testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                this.testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            //Get all persons in form of PersonResponseDTO
            List<PersonResponse> person_list_from_get = await _personService.GetAllPersons();

            //Assert
            //Checks equality on each single object of person_response_list_from_add in person_list_from_get
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //    Assert.Contains(person_response_from_add, person_list_from_get);
            //}

            person_response_list_from_add.Should().BeEquivalentTo(person_list_from_get);

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
        public async Task GetFilteredPerson_EmptySearchText()
        {
            //Arrange
            List<PersonResponse> person_response_list_from_add = await CreatePerson();

            //Prints values of personresponsedto from add request
            this.testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                this.testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            //Get all persons in form of PersonResponseDTO
            List<PersonResponse> person_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            //Assert
            //Checks equality on each single object of person_response_list_from_add in person_list_from_get
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //    Assert.Contains(person_response_from_add, person_list_from_search);
            //}

            person_list_from_search.Should().BeEquivalentTo(person_response_list_from_add);

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
        public async Task GetFilteredPerson_NonEmptySearchString()
        {
            //Arrange
            List<PersonResponse> person_response_list_from_add = await CreatePerson();

            //Prints values of personresponsedto from add request
            this.testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                this.testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            //Get all persons in form of PersonResponseDTO
            //List<PersonResponse> person_list_from_search = _personService.GetFilteredPersons(nameof(Person.PersonName), "of");
            List<PersonResponse> person_list_from_search_email = await _personService.GetFilteredPersons(nameof(Person.Email), "ku");

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

            person_list_from_search_email.Should().OnlyContain(person => person.PersonName.Contains("ku", StringComparison.OrdinalIgnoreCase));

            //Print values of the person response from get
            this.testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in person_list_from_search_email)
            {
                this.testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
        }

        #endregion

        #region GetSortedPersons
        [Fact]
        //When we sort personName is desc order, it should return a list of persons in desc order
        public async Task GetSortedPersons_DESC()
        {
            //Arrange
            List<PersonResponse> person_response_list_from_add = await CreatePerson();
            List<PersonResponse> allPersons = await _personService.GetAllPersons();

            //Prints values of personresponsedto from add request
            this.testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                this.testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            //Get all persons in form of PersonResponseDTO
            //List<PersonResponse> person_list_from_search = _personService.GetFilteredPersons(nameof(Person.PersonName), "of");
            List<PersonResponse> person_list_from_sort = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            //Print values of the person response from get
            this.testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_sort in person_list_from_sort)
            {
                this.testOutputHelper.WriteLine(person_response_from_sort.ToString());
            }

            person_response_list_from_add = person_response_list_from_add.OrderByDescending(person => person.PersonName).ToList();


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
        public async Task GetSortedPersons_ASC()
        {
            //Arrange
            List<PersonResponse> person_response_list_from_add = await CreatePerson();
            List<PersonResponse> allPersons = await _personService.GetAllPersons();

            person_response_list_from_add = person_response_list_from_add.OrderBy(person => person.PersonName).ToList();

            //Prints values of personresponsedto from add request
            this.testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                this.testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            //Get all persons in form of PersonResponseDTO
            //List<PersonResponse> person_list_from_search = _personService.GetFilteredPersons(nameof(Person.PersonName), "of");
            List<PersonResponse> person_list_from_sort = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.ASC);

            //Print values of the person response from get
            this.testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_sort in person_list_from_sort)
            {
                this.testOutputHelper.WriteLine(person_response_from_sort.ToString());
            }


            //Assert
            //Checks equality on each single object of person_response_list_from_add in person_list_from_sort
            //for (int i = 0; i < person_response_list_from_add.Count; i++)
            //{
            //    Assert.Equal(person_response_list_from_add[i], person_list_from_sort[i]);
            //}

            //BeInAscendingOrder should be used
            person_response_list_from_add.Should().BeInAscendingOrder(person => person.PersonName);
        }
        #endregion

        #region UpdatePerson
        //when we supply null as the personUpdateRequest, it should throw 
        //ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
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
                await _personService.UpdatePerson(person_update_request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //When the ID of the person is new, it should throw Argument exception
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
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
                await _personService.UpdatePerson(person_update_request);
            };
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When person name is null it should throw Argument Exception
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();
            //;CountryAddRequest countryAddRequest = new CountryAddRequest
            //{
            //    CountryName = "USA"
            //};

            CountryResponse countryResponse = await countryService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
                .With(person => person.PersonName, "Kweku")
                .With(person => person.Email, "Kweku@example.org")
                .With(person => person.CountryID, countryResponse.CountryId)
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

            PersonResponse personResponse_from_add = await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest person_update_request = personResponse_from_add.ToPersonUpdateRequest();

            person_update_request.PersonName = null;

            //Asset
            //await Assert.ThrowsAsync<ArgumentException>(async () => { await _personService.UpdatePerson(person_update_request); });

            //Act
            Func<Task> action = async () =>
            {
                await _personService.UpdatePerson(person_update_request);
            };
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_PersonNameFullDetails()
        {
            //Arrange
            CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse = await countryService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
                .With(person => person.PersonName, "Kweku")
                .With(person => person.Email, "Kweku@example.org")
                .With(person => person.CountryID, countryResponse.CountryId)
                .Create();

            PersonResponse personResponse_from_add = await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest person_update_request = personResponse_from_add.ToPersonUpdateRequest();

            person_update_request.PersonName = "Anokye";
            person_update_request.Email = "kweku@protonmail.com";

            //Act
            PersonResponse personResponse_from_update = await _personService.UpdatePerson(person_update_request);
            PersonResponse personResponse_from_get = await _personService.GetPersonByPersonId(person_update_request.PersonID);

            //Asset
            //Assert.Equal(personResponse_from_get, personResponse_from_update);

            personResponse_from_update.Should().Be(personResponse_from_get);

        }
        #endregion

        #region DeletePerson
        //if you supply an valid personID, it should return true
        [Fact]
        public async Task DeletePerson_validPersonID()
        {
            //Arrange
            CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();

            CountryResponse country_response_from_add = await countryService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
                .With(person => person.PersonName, "Kweku")
                .With(person => person.Email, "Kweku@example.org")
                .With(person => person.CountryID, country_response_from_add.CountryId)
                .Create();
            PersonResponse personResponse_from_add = await _personService.AddPerson(personAddRequest);

            //Act
            bool isDeleted = await _personService.DeletePerson(personResponse_from_add.PersonID);

            //Assert.True(isDeleted);
            isDeleted.Should().BeTrue();

        }

        //if you supply an invalid personID, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Arrange
            CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();

            CountryResponse country_response_from_add = await countryService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
                .With(person => person.PersonName, "Kweku")
                .With(person => person.Email, "Kweku@example.org")
                .With(person => person.CountryID, country_response_from_add.CountryId)
                .Create();

            PersonResponse personResponse_from_add = await _personService.AddPerson(personAddRequest);

            //Act
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            Assert.False(isDeleted);
            isDeleted.Should().BeFalse();

        }
        #endregion

    }
}
