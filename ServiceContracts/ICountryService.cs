using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country model
    /// </summary>
    public interface ICountryService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">CountryAddRequest DTO</param>
        /// <returns>CountryResponse- Returns the country object after adding 
        /// it  (including newly generated country id)</returns>
        CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// This gets all countries from country response
        /// </summary>
        /// <returns>
        /// Returns a list of country response dto
        /// </returns>
        List<CountryResponse> GetCountryList();

        /// <summary>
        /// Returns a countryResponseDTO based on the given countryId
        /// </summary>
        /// <param name="CountryID">CountryID (guid) to search</param>
        /// <returns>Matching country as CountryResponseDTOc object</returns>
        CountryResponse? GetCountryByCountryId(Guid? CountryID);
    }
}