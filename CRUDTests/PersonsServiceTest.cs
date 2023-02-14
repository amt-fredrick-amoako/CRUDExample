using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.Common.DataCollection;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;
        private readonly ICountryService countryService;

        private readonly ITestOutputHelper testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            countryService = new CountryService(new PeopleDbContext(new DbContextOptionsBuilder<PeopleDbContext>().Options));
            _personService = new PersonsService(new PeopleDbContext(new DbContextOptionsBuilder<PeopleDbContext>().Options), countryService);
            this.testOutputHelper = testOutputHelper;
        }

        /// <summary>
        /// Create new person method
        /// </summary>
        /// <returns>returns a new list of person response</returns>
        private List<PersonResponse> CreatePerson()
        {
            //Arrange
            CountryAddRequest country_request_one = new CountryAddRequest { CountryName = "Ghana" };
            CountryAddRequest country_request_two = new CountryAddRequest { CountryName = "USA" };

            CountryResponse country_response_one = countryService.AddCountry(country_request_one);
            CountryResponse country_response_two = countryService.AddCountry(country_request_two);

            PersonAddRequest person_request_one = new PersonAddRequest
            {
                PersonName = "Kweku",
                Email = "kweku@example.com",
                Address = "P.V. Obeng Bypass",
                CountryID = country_response_one.CountryId,
                DateOfBirth = DateTime.Parse("11-29-1995"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false
            };
            PersonAddRequest person_request_two = new PersonAddRequest
            {
                PersonName = "Kofi",
                Email = "kofi@example.com",
                Address = "P.V. Obeng Bypass",
                CountryID = country_response_two.CountryId,
                DateOfBirth = DateTime.Parse("06-26-1998"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> person_request_list = new List<PersonAddRequest>
            {
                person_request_one, person_request_two
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest personAddRequest in person_request_list)
            {
                PersonResponse personResponse = _personService.AddPerson(personAddRequest);
                person_response_list_from_add.Add(personResponse);
            }

            return person_response_list_from_add;
        }

        #region AddPerson

        [Fact]
        //Throw ArgumentNullException when PersonAddRequest is supplied as null
        public void AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            Assert.Throws<ArgumentNullException>(() => _personService.AddPerson(personAddRequest));
        }

        [Fact]
        //Throw ArgumentException when PersonName is null
        public void AddPerson_PersonName_IsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest { PersonName = null };

            //Act
            Assert.Throws<ArgumentException>(() => _personService.AddPerson(personAddRequest));
        }

        [Fact]
        //Throws no error and insert PersonAddRequest into PersonResponse successfully with a newly generated ID
        public void AddPerson_PersonDetails_IsValid()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest
            {
                PersonName = "Kofi",
                Email = "kofi@example.com",
                Address = "P.V.Obeng ByPass",
                CountryID = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("1998-06-26"),
                ReceiveNewsLetters = true
            };
            //Act
            PersonResponse personResponse_from_add = _personService.AddPerson(personAddRequest);
            List<PersonResponse> personResponse_from_getall = _personService.GetAllPersons();

            //Assert
            Assert.True(personResponse_from_add.PersonID != Guid.Empty);
            Assert.Contains(personResponse_from_add, personResponse_from_getall);
        }
        #endregion

        #region GetPersonByID
        //Return null as PersonResponse when PersonID is provided as null
        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? PersonID = null;

            //Act
            PersonResponse? person_response_from_get = _personService.GetPersonByPersonId(PersonID);

            //Assert
            Assert.Null(person_response_from_get);
        }

        //when supplied a valid person id, the valid person details should be returned as a PersonResponse obj
        [Fact]
        public void GetPersonByPersonID_WithPersonID()
        {
            //Arrange
            //use countryResponseDTO's countryID property as a foreign key
            CountryAddRequest country_request = new CountryAddRequest { CountryName = "Ghana" };
            CountryResponse country_response = countryService.AddCountry(country_request);

            PersonAddRequest person_request = new PersonAddRequest
            {
                PersonName = "Kweku",
                Email = "kweku@example.com",
                Address = "P.V. Obeng Bypass",
                CountryID = country_response.CountryId,
                DateOfBirth = DateTime.Parse("11-29-1995"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false
            };

            //Act
            var person_response = _personService.AddPerson(person_request);

            var person_byID_response = _personService.GetPersonByPersonId(person_response.PersonID);

            //Assert
            Assert.Equal(person_response, person_byID_response);
        }
        #endregion

        #region GetAllPersons
        //The GetAllPersons method must return an empty list by default
        [Fact]
        public void GetAllPersons_Empty()
        {
            //Act
            List<PersonResponse> person_from_get = _personService.GetAllPersons();

            //Assert
            Assert.Empty(person_from_get);
        }

        [Fact]
        //Create and add a few instances  of persons and calls GetAllPersons method.this should return the same persons added
        public void GetALLPersons_AddFewPersons()
        {
            //Arrange
            CountryAddRequest country_request_one = new CountryAddRequest { CountryName = "Ghana" };
            CountryAddRequest country_request_two = new CountryAddRequest { CountryName = "USA" };
            CountryResponse country_response_one = countryService.AddCountry(country_request_one);
            CountryResponse country_response_two = countryService.AddCountry(country_request_two);
            PersonAddRequest person_request_one = new PersonAddRequest
            {
                PersonName = "Kweku",
                Email = "kweku@example.com",
                Address = "P.V. Obeng Bypass",
                CountryID = country_response_one.CountryId,
                DateOfBirth = DateTime.Parse("11-29-1995"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false
            };
            PersonAddRequest person_request_two = new PersonAddRequest
            {
                PersonName = "Kofi",
                Email = "kofi@example.com",
                Address = "P.V. Obeng Bypass",
                CountryID = country_response_two.CountryId,
                DateOfBirth = DateTime.Parse("06-26-1998"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };

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
                PersonResponse personResponse = _personService.AddPerson(personAddRequest);
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
            List<PersonResponse> person_list_from_get = _personService.GetAllPersons();

            //Assert
            //Checks equality on each single object of person_response_list_from_add in person_list_from_get
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, person_list_from_get);
            }

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
        public void GetFilteredPerson_EmptySearchText()
        {
            //Arrange
            List<PersonResponse> person_response_list_from_add = CreatePerson();

            //Prints values of personresponsedto from add request
            this.testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                this.testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            //Get all persons in form of PersonResponseDTO
            List<PersonResponse> person_list_from_search = _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            //Assert
            //Checks equality on each single object of person_response_list_from_add in person_list_from_get
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, person_list_from_search);
            }

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
        public void GetFilteredPerson_NonEmptySearchString()
        {
            //Arrange
            List<PersonResponse> person_response_list_from_add = CreatePerson();

            //Prints values of personresponsedto from add request
            this.testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                this.testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            //Get all persons in form of PersonResponseDTO
            //List<PersonResponse> person_list_from_search = _personService.GetFilteredPersons(nameof(Person.PersonName), "of");
            List<PersonResponse> person_list_from_search_email = _personService.GetFilteredPersons(nameof(Person.Email), "ku");

            //Assert
            //Checks equality on each single object of person_response_list_from_add in person_list_from_get
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                if (person_response_from_add.PersonName != null)
                {
                    if (person_response_from_add.PersonName.Contains("ku", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(person_response_from_add, person_list_from_search_email);
                    }
                }
            }

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
        public void GetSortedPersons_DESC()
        {
            //Arrange
            List<PersonResponse> person_response_list_from_add = CreatePerson();
            List<PersonResponse> allPersons = _personService.GetAllPersons();

            //Prints values of personresponsedto from add request
            this.testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                this.testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            //Get all persons in form of PersonResponseDTO
            //List<PersonResponse> person_list_from_search = _personService.GetFilteredPersons(nameof(Person.PersonName), "of");
            List<PersonResponse> person_list_from_sort = _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            //Print values of the person response from get
            this.testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_sort in person_list_from_sort)
            {
                this.testOutputHelper.WriteLine(person_response_from_sort.ToString());
            }

            person_response_list_from_add = person_response_list_from_add.OrderByDescending(person => person.PersonName).ToList();


            //Assert
            //Checks equality on each single object of person_response_list_from_add in person_list_from_sort
            for (int i = 0; i < person_response_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_list_from_add[i], person_list_from_sort[i]);
            }
        }

        [Fact]
        //When we sort personName is asc order, it should return a list of persons in desc order
        public void GetSortedPersons_ASC()
        {
            //Arrange
            List<PersonResponse> person_response_list_from_add = CreatePerson();
            List<PersonResponse> allPersons = _personService.GetAllPersons();

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
            List<PersonResponse> person_list_from_sort = _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.ASC);

            //Print values of the person response from get
            this.testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_sort in person_list_from_sort)
            {
                this.testOutputHelper.WriteLine(person_response_from_sort.ToString());
            }


            //Assert
            //Checks equality on each single object of person_response_list_from_add in person_list_from_sort
            for (int i = 0; i < person_response_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_list_from_add[i], person_list_from_sort[i]);
            }
        }
        #endregion

        #region UpdatePerson
        //when we supply null as the personUpdateRequest, it should throw 
        //ArgumentNullException
        [Fact]
        public void UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;
            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _personService.UpdatePerson(person_update_request);
            });
        }

        //When the ID of the person is new, it should throw Argument exception
        [Fact]
        public void UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = new PersonUpdateRequest
            {
                PersonID = Guid.NewGuid(),
            };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personService.UpdatePerson(person_update_request);
            });
        }

        //When person name is null it should throw Argument Exception
        [Fact]
        public void UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest
            {
                CountryName = "USA"
            };

            CountryResponse countryResponse = countryService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                PersonName = "Kweku",
                CountryID = countryResponse.CountryId,
                Email = "kweku@example.com",
                DateOfBirth = DateTime.Parse("11-29-1995"),
                Address = "Manhattan",
                Gender = GenderOptions.Male,
            };

            PersonResponse personResponse_from_add = _personService.AddPerson(personAddRequest);

            PersonUpdateRequest person_update_request = personResponse_from_add.ToPersonUpdateRequest();

            person_update_request.PersonName = null;

            //Asset
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personService.UpdatePerson(person_update_request);
            });
        }
        //First, add a new person and try to update the person name and email
        [Fact]
        public void UpdatePerson_PersonNameFullDetails()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest
            {
                CountryName = "USA"
            };

            CountryResponse countryResponse = countryService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                PersonName = "Kweku",
                CountryID = countryResponse.CountryId,
                Address = "La Brea",
                DateOfBirth = DateTime.Parse("1995-11-29"),
                Email = "kweku@example.com",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };

            PersonResponse personResponse_from_add = _personService.AddPerson(personAddRequest);

            PersonUpdateRequest person_update_request = personResponse_from_add.ToPersonUpdateRequest();

            person_update_request.PersonName = "Anokye";
            person_update_request.Email = "kweku@protonmail.com";

            //Act
            PersonResponse personResponse_from_update = _personService.UpdatePerson(person_update_request);
            PersonResponse personResponse_from_get = _personService.GetPersonByPersonId(person_update_request.PersonID);

            //Asset
            Assert.Equal(personResponse_from_get, personResponse_from_update);
            
        }
        #endregion

        #region DeletePerson
        //if you supply an valid personID, it should return true
        [Fact]
        public void DeletePerson_validPersonID()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest
            {
                CountryName = "USA",
            };

            CountryResponse country_response_from_add = countryService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                PersonName = "Freddie",
                Address = "1440 Topping Ave",
                CountryID = country_response_from_add.CountryId,
                DateOfBirth = DateTime.Parse("11-29-1996"),
                Email = "fredrickamoako@gmail.com",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            PersonResponse personResponse_from_add = _personService.AddPerson(personAddRequest);

            //Act
            bool isDeleted = _personService.DeletePerson(personResponse_from_add.PersonID);

            Assert.True(isDeleted);

        }

        //if you supply an invalid personID, it should return false
        [Fact]
        public void DeletePerson_InvalidPersonID()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest
            {
                CountryName = "USA",
            };

            CountryResponse country_response_from_add = countryService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                PersonName = "Freddie",
                Address = "1440 Topping Ave",
                CountryID = country_response_from_add.CountryId,
                DateOfBirth = DateTime.Parse("11-29-1996"),
                Email = "fredrickamoako@gmail.com",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            PersonResponse personResponse_from_add = _personService.AddPerson(personAddRequest);

            //Act
            bool isDeleted = _personService.DeletePerson(Guid.NewGuid());

            Assert.False(isDeleted);

        }
        #endregion

    }
}
