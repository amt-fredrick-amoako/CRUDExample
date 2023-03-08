using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonsDeleteService
    {
        
        /// <summary>
        /// Deletes a person based on the given person id
        /// </summary>
        /// <param name="personId">PersonID to delete</param>
        /// <returns>returns true, if the deletion is sucecssful, otherwise false</returns>
        Task<bool> DeletePerson(Guid? personId);

    }
}
