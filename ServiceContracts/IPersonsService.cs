using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonsService
    {
        /// <summary>
        /// Adds a new PersonDTO and responds with a PersonResponse type
        /// </summary>
        /// <param name="person">PersonAddRequest</param>
        /// <returns>Returns PersonResponse in the form of the same person details along with newly generated PersonID</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? person);

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
        /// Returns sorted list of persons
        /// </summary>
        /// <param name="allPersons">Represents list of persons to sort</param>
        /// <param name="sortBy">Name of the property (key), based on which the persons should be sorted</param>
        /// <param name="sortedOrder">ASC, DESC</param>
        /// <returns>Returns sorted persons as a list of person response</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortedOrder);

        /// <summary>
        /// Updates the person details based on the given person ID
        /// </summary>
        /// <param name="personUpdate">Person details to update, with person id</param>
        /// <returns>Person obj after update</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdate);


        /// <summary>
        /// Deletes a person based on the given person id
        /// </summary>
        /// <param name="personId">PersonID to delete</param>
        /// <returns>returns true, if the deletion is sucecssful, otherwise false</returns>
        Task<bool> DeletePerson(Guid? personId);

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
