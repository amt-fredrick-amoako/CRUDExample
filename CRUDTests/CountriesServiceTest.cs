using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using EntityFrameworkCoreMock;
using Moq;
using FluentAssertions;
using AutoFixture;
using RepositoryContracts;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountryService _countriesService;
        private readonly ICountriesRepository _countryRepository;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly IFixture fixture;

        //constructor
        public CountriesServiceTest()
        {
            /*
            //Mock the actual database.
            var countriesInitialData = new List<Country>();
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options
                );
            var dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(dbSet => dbSet.Countries, countriesInitialData);

            _countriesService = new CountryService(null);

            fixture = new Fixture();
            */
            fixture = new Fixture();
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countryRepository = _countriesRepositoryMock.Object;
            _countriesService = new CountryService(_countryRepository);

        }

        #region AddCountry
        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_Null_Country_To_ThrowArgumentNullException()
        {
            //Arrange
            CountryAddRequest request = null;
            //Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () => /*Act*/await _countriesService.AddCountry(request));

            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(request);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }
        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_Country_NameIsNull()
        {
            //Arrange
            CountryAddRequest request = fixture.Build<CountryAddRequest>()
                .With(country => country.CountryName, null as string)
                .Create();

            Country country = request.ToCountry();

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);
            //Assert
            //await Assert.ThrowsAsync<ArgumentException>(async () => /*Act*/await _countriesService.AddCountry(request));

            //Using Fluent Assertions
            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }
        //When the CountryName is duplicate, it should throw ArguementException
        [Fact]
        public async Task AddCountry_Country_IsDuplicate()
        {
            //Arrange
            CountryAddRequest request = fixture.Build<CountryAddRequest>()
                .With(country => country.CountryName, "USA")
                .Create();

            CountryAddRequest request1 = fixture.Build<CountryAddRequest>()
                .With(country => country.CountryName, "USA")
                .Create();

            Country country = request.ToCountry();
            Country country1 = request1.ToCountry();

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(null as Country);

            //Assert
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    /*Act*/
            //    await _countriesService.AddCountry(request);
            //    await _countriesService.AddCountry(request1);
            //});

            CountryResponse response_from_add = await _countriesService.AddCountry(request);
            CountryResponse response_from_add1 = await _countriesService.AddCountry(request1);

            Func<Task> action = async () =>
            {
                _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);
                _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(country);

                await _countriesService.AddCountry(request1);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }
        //When you supply the right CountryName, it should insert(add) the Country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            CountryAddRequest country_request = fixture.Create<CountryAddRequest>();
            Country country = country_request.ToCountry();
            CountryResponse country_response = country.ToCountryResponse();

            _countriesRepositoryMock
             .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
             .ReturnsAsync(country);

            _countriesRepositoryMock
             .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
             .ReturnsAsync(null as Country);


            //Act
            CountryResponse country_from_add_country = await _countriesService.AddCountry(country_request);

            country.CountryID = country_from_add_country.CountryId;
            country_response.CountryId = country_from_add_country.CountryId;

            //Assert
            country_from_add_country.CountryId.Should().NotBe(Guid.Empty);
            country_from_add_country.Should().BeEquivalentTo(country_response);
        }
        #endregion

        #region GetAllCountries

        [Fact]
        //The list of countries by default should be emtpy 
        public async Task GetAllCountries_EmptyList()
        {
            //Act
            List<Country> empty_list = new List<Country>();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(empty_list);

            List<CountryResponse> actual_countries_from_response_list = await _countriesService.GetCountryList();
            //Assert
            //Assert.Empty(actual_countries_from_response_list);

            actual_countries_from_response_list.Should().BeEmpty();
        }

        [Fact]
        //Should return added countries
        public async Task GetAllCountries_AddFewCountries()
        {
            //Arrange
            //created a new list of countryAddDTO
            List<Country> countries = new List<Country>
            {
                fixture.Build<Country>()
                .With(country => country.Persons, null as List<Person>)
                .Create(),
                //////////////////////////////////////////////
                fixture.Build < Country >()
                .With(country => country.Persons, null as List < Person >)
                .Create()
            };

            //Act
            //created a list of CountryResponseDTO
            List<CountryResponse> countries_list_from_add_country = countries.Select(country => country.ToCountryResponse()).ToList();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);

            //foreach (CountryAddRequest country_request in country_request_list)
            //{
            //    //Loops through CountryAddDTO and adds them to CountryResponseDTO
            //    countries_list_from_add_country
            //        .Add(await _countriesService.AddCountry(country_request));
            //}

            //Gets all the list from the GetCountryList and stores them as 
            //CountryResponseDTO list
            List<CountryResponse> actualCountryResponseList = await _countriesService.GetCountryList();

            //read each response from country_list_from_add_country
            //foreach (CountryResponse expected_country in countries_list_from_add_country)
            //{
            //    //Assert
            //    //Verifies that countries_list_from_add_country is in actualCountryResponseList
            //    Assert.Contains(expected_country, actualCountryResponseList);
            //}

            actualCountryResponseList.Should().BeEquivalentTo(countries_list_from_add_country);
        }

        #endregion

        #region GetCountryByID

        [Fact]
        //Should return null when supplied a null CountryID as a CountryResponseDTO
        public async Task GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;

            _countriesRepositoryMock.Setup(temp => temp.GetCountryById(It.IsAny<Guid>())).ReturnsAsync(null as Country);

            //Act
            CountryResponse? country_response_from_getmethod = await _countriesService.GetCountryByCountryId(countryID);

            //Assert
            //Assert.Null(country_response_from_getmethod);

            country_response_from_getmethod.Should().BeNull();
        }

        [Fact]
        //Should return the matching country to the CountryID as a CountryResponseDTO
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            Country? country = fixture.Build<Country>()
                .With(country => country.Persons, null as List<Person>)
                .Create();

            CountryResponse countryResponse = country.ToCountryResponse();

            _countriesRepositoryMock.Setup(temp => temp.GetCountryById(It.IsAny<Guid>())).ReturnsAsync(country);

            //Act
            CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryId(country.CountryID);

            ////Assert
            //Assert.Equal(country_response_from_addrequest, country_response_from_get);

            country_response_from_get.Should().Be(countryResponse);
        }


        #endregion

    }
}
