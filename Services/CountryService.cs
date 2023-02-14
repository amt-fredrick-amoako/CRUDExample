using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountryService : ICountryService
    {
        private readonly List<Country> countries;

        public CountryService(bool initialize = true)
        {
            countries = new List<Country>();
            if (initialize)
            {
                countries.AddRange(new List<Country>()
                {
                        new Country { CountryID = Guid.Parse("C703058F-7CB0-43D3-B90B-66973F0AF19F"), CountryName = "USA"},
                        new Country {CountryID = Guid.Parse("BC86E026-FADA-482E-AC32-2979E01658ED"), CountryName = "UK"},
                        new Country { CountryID = Guid.Parse("56DD8B92-B09F-4FE7-89A8-86D34A10E220"), CountryName = "Germany"},
                        new Country { CountryID = Guid.Parse("AC0B0BC6-6E06-4347-8A71-43FA0F4B0A72"), CountryName = "Switzerland" },
                        new Country { CountryID = Guid.Parse("912936C6-2B61-4FF2-90E7-827B9814C470"), CountryName = "Canada"}
                });


            }
        }

        public CountryResponse AddCountry(CountryAddRequest? countryDTO)
        {
            //Performing Validation Logic
            //if argument is null
            if (countryDTO == null) throw new ArgumentNullException(nameof(countryDTO));

            //if name is null
            if (countryDTO.CountryName == null) throw new ArgumentException(nameof(countryDTO.CountryName));

            //if name already exists in the collection
            if (countries.Where(country => country.CountryName == countryDTO.CountryName).Count() > 0)
                throw new ArgumentException("Name of the Country already exists");


            //convert from DTO to Model
            Country country = countryDTO.ToCountry();
            //generate id for country object
            country.CountryID = Guid.NewGuid();
            //add country object to countries list
            countries.Add(country);
            //return response DTO
            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetCountryList()
        {
            return countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryId(Guid? CountryID)
        {
            if (CountryID == null) return null;

            Country? country_response_from_list = countries.FirstOrDefault(country => country.CountryID == CountryID);
            if (country_response_from_list == null) return null;

            return country_response_from_list.ToCountryResponse();
        }
    }
}