using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountryService _countriesService;

        //constructor
        public CountriesServiceTest()
        {
            _countriesService = new CountryService(new PeopleDbContext(new DbContextOptionsBuilder<PeopleDbContext>().Options));
        }

        #region AddCountry
        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public void AddCountry_Null_Country()
        {
            //Arrange
            CountryAddRequest request = null;
            //Assert
            Assert.Throws<ArgumentNullException>(() => /*Act*/_countriesService.AddCountry(request));
        }
        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public void AddCountry_Country_NameIsNull()
        {
            //Arrange
            CountryAddRequest request = new CountryAddRequest
            {
                CountryName = null
            };
            //Assert
            Assert.Throws<ArgumentException>(() => /*Act*/_countriesService.AddCountry(request));
        }
        //When the CountryName is duplicate, it should throw ArguementException
        [Fact]
        public void AddCountry_Country_IsDuplicate()
        {
            //Arrange
            CountryAddRequest request = new CountryAddRequest
            {
                CountryName = "USA"
            };

            CountryAddRequest request1 = new CountryAddRequest
            {
                CountryName = "USA"
            };
            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                /*Act*/
                _countriesService.AddCountry(request);
                _countriesService.AddCountry(request1);
            });
        }
        //When you supply the right CountryName, it should insert(add) the Country to the existing list of countries
        [Fact]
        public void AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest request = new CountryAddRequest
            {
                CountryName = "Japan"
            };

            CountryResponse response = /*Act*/_countriesService.AddCountry(request);
            List<CountryResponse> countries_from_GetAllCountries = _countriesService.GetCountryList();

            //Assert
            Assert.True(response.CountryId != Guid.Empty);
            Assert.Contains(response, countries_from_GetAllCountries);
        }
        #endregion

        #region GetAllCountries

        [Fact]
        //The list of countries by default should be emtpy 
        public void GetAllCountries_EmptyList()
        {
            //Act
            List<CountryResponse> actual_countries_from_response_list = _countriesService.GetCountryList();

            //Assert
            Assert.Empty(actual_countries_from_response_list);
        }

        [Fact]
        //Should return added countries
        public void GetAllCountries_AddFewCountries()
        {
            //Arrange
            //created a new list of countryAddDTO
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>
            {
                new CountryAddRequest {CountryName = "USA"},
                new CountryAddRequest {CountryName = "UK"}
            };

            //Act
            //created a list of CountryResponseDTO
            List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();

            foreach (CountryAddRequest country_request in country_request_list)
            {
                //Loops through CountryAddDTO and adds them to CountryResponseDTO
                countries_list_from_add_country
                    .Add(_countriesService.AddCountry(country_request));
            }

            //Gets all the list from the GetCountryList and stores them as 
            //CountryResponseDTO list
            List<CountryResponse> actualCountryResponseList = _countriesService.GetCountryList();

            //read each response from country_list_from_add_country
            foreach (CountryResponse expected_country in countries_list_from_add_country)
            {
                //Assert
                //Verifies that countries_list_from_add_country is in actualCountryResponseList
                Assert.Contains(expected_country, actualCountryResponseList);
            }
        }

        #endregion

        #region GetCountryByID

        [Fact]
        //Should return null when supplied a null CountryID as a CountryResponseDTO
        public void GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;

            //Act
            CountryResponse? country_response_from_getmethod= _countriesService.GetCountryByCountryId(countryID);

            //Assert
            Assert.Null(country_response_from_getmethod);
        }

        [Fact]
        //Should return the matching country to the CountryID as a CountryResponseDTO
        public void GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? country_add_request = new CountryAddRequest
            {
                CountryName = "Ghana"
            };

            CountryResponse country_response_from_addrequest = _countriesService.AddCountry(country_add_request);

            //Act
            CountryResponse country_response_from_get = _countriesService.GetCountryByCountryId(country_response_from_addrequest.CountryId);

            //Assert
            Assert.Equal(country_response_from_addrequest, country_response_from_get);
        }


        #endregion

    }
}
