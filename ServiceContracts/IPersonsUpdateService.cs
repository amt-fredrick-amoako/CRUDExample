using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonsUpdateService
    {
        /// <summary>
        /// Updates the person details based on the given person ID
        /// </summary>
        /// <param name="personUpdate">Person details to update, with person id</param>
        /// <returns>Person obj after update</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdate);

    }
}
