using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonsGetService
    {

        /// <summary>
        /// Gets all PersonResponseDTO
        /// </summary>
        /// <returns>Returns a list of PersonResponseDTO</returns>
        Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// Gets person response obj based on person id
        /// </summary>
        /// <param name="id">Id of person to look for</param>
        /// <returns>Matching person object</returns>
        Task<PersonResponse?> GetPersonByPersonId(Guid? id);

        /// <summary>
        /// Returns all person objects that matches with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Returns all matching persons based on the given search field and string</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string searchString);

        /// <summary>
        /// Returns persons as csv
        /// </summary>
        /// <returns>Returns the memory stream with csv data</returns>
        Task<MemoryStream> GetPersonsCSV();

        /// <summary>
        /// Returns persons as excel
        /// </summary>
        /// <returns>memory stream with persons as excel</returns>
        Task<MemoryStream> GetPersonsExcel();
    }
}
