using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing the person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Adds a person instance to the data store
        /// </summary>
        /// <param name="person">Person instance to add</param>
        /// <returns>The person instance after adding it to the data store</returns>
        Task<Person> AddPerson(Person person);


        /// <summary>
        /// Gets all persons in the data store
        /// </summary>
        /// <returns>A list of person instances from the data store</returns>
        Task<List<Person>> GetAllPersons();


        /// <summary>
        /// Gets matching person instance based on the person id from the data storec
        /// </summary>
        /// <param name="personID">Guid representing ID of the person to look for</param>
        /// <returns>A person instance or null</returns>
        Task<Person?> GetPersonByPersonID(Guid personID);


        /// <summary>
        /// Returns all persons that matches a given expression
        /// </summary>
        /// <param name="predicate">Linq expression to check</param>
        /// <returns>all matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);


        /// <summary>
        /// Takes a person Id finds the matching person and removes it fromt the data store
        /// </summary>
        /// <param name="personID">ID of the person object needed to be removed from the data store</param>
        /// <returns>true or false whether person was found and deleted or not</returns>
        Task<bool> DeletePersonByPersonID(Guid personID);


        /// <summary>
        /// Updates a person instance with the given person instance's ID
        /// </summary>
        /// <param name="person">instance of the person that has the updated values</param>
        /// <returns>returns the updated person instance</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
