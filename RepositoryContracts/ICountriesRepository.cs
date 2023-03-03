using Entities;
namespace RepositoryContracts
{
    /// <summary>
    /// Representss data access logic for managing Person entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds a new country object to the data store
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>The country object after adding it to the data store</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Gets all Countries
        /// </summary>
        /// <returns>All countries from the table</returns>
        Task<List<Country>> GetAllCountries();


        /// <summary>
        /// Retrieves a matching country object with the same ID from the table, otherwise it will return null
        /// </summary>
        /// <param name="CountryID"></param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryById(Guid CountryID);


        /// <summary>
        /// Gives a country object based on the given country name
        /// </summary>
        /// <param name="CountryName">name of the country to search</param>
        /// <returns>matching country or null</returns>
        Task<Country?> GetCountryByCountryName(string CountryName);
    }
}