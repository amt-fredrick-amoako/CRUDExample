using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using EntityFrameworkCoreMock;
using Moq;
using FluentAssertions;
using AutoFixture;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountryService _countriesService;
        private readonly IFixture fixture;

        //constructor
        public CountriesServiceTest()
        {
            //Mock the actual database.
            var countriesInitialData = new List<Country>();
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options
                );
            var dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(dbSet => dbSet.Countries, countriesInitialData);

            _countriesService = new CountryService(null);

            fixture = new Fixture();
        }

        #region AddCountry
        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_Null_Country()
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
            //Assert
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    /*Act*/
            //    await _countriesService.AddCountry(request);
            //    await _countriesService.AddCountry(request1);
            //});

            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(request);
                await _countriesService.AddCountry(request1);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }
        //When you supply the right CountryName, it should insert(add) the Country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest request = fixture.Build<CountryAddRequest>()
                .With(country => country.CountryName, "Japan")
                .Create();

            CountryResponse response = /*Act*/await _countriesService.AddCountry(request);
            List<CountryResponse> countries_from_GetAllCountries = await _countriesService.GetCountryList();

            //Assert
            //Assert.True(response.CountryId != Guid.Empty);
            //Assert.Contains(response, countries_from_GetAllCountries);
            countries_from_GetAllCountries.Should().Contain(response);
            response.CountryId.Should().NotBe(Guid.Empty);
        }
        #endregion

        #region GetAllCountries

        [Fact]
        //The list of countries by default should be emtpy 
        public async Task GetAllCountries_EmptyList()
        {
            //Act
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
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>
            {
                fixture.Build<CountryAddRequest>()
                .With(country => country.CountryName, "USA")
                .Create(),
                //////////////////////////////////////////////
                fixture.Build < CountryAddRequest >()
                .With(country => country.CountryName, "UK")
                .Create()
            };

            //Act
            //created a list of CountryResponseDTO
            List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();

            foreach (CountryAddRequest country_request in country_request_list)
            {
                //Loops through CountryAddDTO and adds them to CountryResponseDTO
                countries_list_from_add_country
                    .Add(await _countriesService.AddCountry(country_request));
            }

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
            CountryAddRequest? country_add_request = fixture.Build<CountryAddRequest>()
                .With(country => country.CountryName, "Ghana")
                .Create();

            CountryResponse country_response_from_addrequest = await _countriesService.AddCountry(country_add_request);

            //Act
            CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryId(country_response_from_addrequest.CountryId);

            ////Assert
            //Assert.Equal(country_response_from_addrequest, country_response_from_get);

            country_response_from_get.Should().Be(country_response_from_addrequest);
        }


        #endregion

    }
}
