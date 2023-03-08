using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonsAdderService
    {
        /// <summary>
        /// Adds a new PersonDTO and responds with a PersonResponse type
        /// </summary>
        /// <param name="person">PersonAddRequest</param>
        /// <returns>Returns PersonResponse in the form of the same person details along with newly generated PersonID</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? person);

    }
}
