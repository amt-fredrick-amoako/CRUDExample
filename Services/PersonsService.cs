using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        //fake a data store for person obj type
        private readonly List<Person> _persons;
        //fake injecting ICountriesService
        private readonly ICountryService countryService;

        //contructor initialization
        public PersonsService(bool initialize = true)
        {
            //Fake data storage
            _persons = new List<Person>();
            countryService = new CountryService();
            if (initialize)
            {
                _persons.Add(new Person
                {
                    PersonName = "Fredrick Amoako",
                    PersonID = Guid.Parse("B93A901E-2289-4ACE-AD52-0F7B90CB9775"),
                    Email = "fredrickamoako@example.com",
                    DateOfBirth = DateTime.Parse("11-30-1997"),
                    Gender = "Male",
                    CountryID = Guid.Parse("C703058F-7CB0-43D3-B90B-66973F0AF19F"),
                    Address = "1660 Topping Ave",
                    ReceiveNewsLetters = true,

                });

                _persons.Add(new Person
                {
                    PersonName = "Maxwell Amoako Antwi",
                    PersonID = Guid.Parse("C82D7817-A2D9-4A68-B457-C39FF14EC61E"),
                    Email = "antwimaxwell@example.com",
                    DateOfBirth = DateTime.Parse("06-26-1999"),
                    Gender = "Male",
                    CountryID = Guid.Parse("C703058F-7CB0-43D3-B90B-66973F0AF19F"),
                    Address = "1660 Topping Ave",
                    ReceiveNewsLetters = true,

                });

                _persons.Add(new Person
                {
                    PersonName = "Ellen Amoako Dankwah",
                    PersonID = Guid.Parse("2DADEABE-343D-4C7F-97A5-7B9ED0D979A3"),
                    Email = "ellenamoakod@example.com",
                    DateOfBirth = DateTime.Parse("07-20-1998"),
                    Gender = "Female",
                    CountryID = Guid.Parse("C703058F-7CB0-43D3-B90B-66973F0AF19F"),
                    Address = "LA",
                    ReceiveNewsLetters = true,

                });

                _persons.Add(new Person
                {
                    PersonName = "Kingsley Kwarteng",
                    PersonID = Guid.Parse("9911211C-A294-4323-8BF7-6F4FD4B86F53"),
                    Email = "kingsleykwarteng@example.com",
                    DateOfBirth = DateTime.Parse("08-11-1996"),
                    Gender = "Male",
                    CountryID = Guid.Parse("BC86E026-FADA-482E-AC32-2979E01658ED"),
                    Address = "Milton Keynes",
                    ReceiveNewsLetters = false,

                });

                _persons.Add(new Person
                {
                    PersonName = "Owura",
                    PersonID = Guid.Parse("B8080B29-8D47-489C-BDA5-5019A28F6226"),
                    Email = "owura@example.com",
                    DateOfBirth = DateTime.Parse("12-02-2001"),
                    Gender = "Male",
                    CountryID = Guid.Parse("56DD8B92-B09F-4FE7-89A8-86D34A10E220"),
                    Address = "Hamburg",
                    ReceiveNewsLetters = false,

                });

                _persons.Add(new Person
                {
                    PersonName = "Janet Dwomoh",
                    PersonID = Guid.Parse("AF458C6F-DF10-4F40-BCF0-56A7558EC00E"),
                    Email = "jdwomoh@example.com",
                    DateOfBirth = DateTime.Parse("05-03-1998"),
                    Gender = "Female",
                    CountryID = Guid.Parse("912936C6-2B61-4FF2-90E7-827B9814C470"),
                    Address = "Ontario",
                    ReceiveNewsLetters = true,

                });


            }
        }

        //reusable method to get country by id and convert to personResponseDTO
        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.CountryName = countryService.GetCountryByCountryId(person.CountryID)?.CountryName;
            return personResponse;
        }

        public PersonResponse AddPerson(PersonAddRequest? person)
        {
            //validate personAddRequestDTO
            if (person == null) throw new ArgumentNullException(nameof(person));
            //if (string.IsNullOrEmpty(person.PersonName)) throw new ArgumentException("Person name is required");

            //model validations
            ValidationHelper.ModelValidation(person);

            //convert and store the personAddRequestDTO to Person obj type and list respectively
            Person newPerson = person.ToPerson();
            newPerson.PersonID = Guid.NewGuid();
            _persons.Add(newPerson);

            return ConvertPersonToPersonResponse(newPerson);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _persons.Select(person => ConvertPersonToPersonResponse(person)).ToList();
        }

        public PersonResponse? GetPersonByPersonId(Guid? id)
        {
            /* Algorithm
             * 1. Check for null value in ID, throw error if null.
             * 2. If not null, get matching person from data store.
             * 3. If found convert matching person to PersonResponeDTO.
             * 4. Return the PersonResponseDTO object.
             */

            if (id == null) return null;
            Person? person = _persons.FirstOrDefault(person => person.PersonID == id);
            if (person == null) return null;

            return ConvertPersonToPersonResponse(person) ?? null;

        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
                return matchingPersons;
            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.PersonName) ?
                    person.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.Email) ?
                    person.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(person => (person.DateOfBirth != null) ? person.DateOfBirth.ToString()
                    .Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.Gender) ?
                    person.Gender.Contains(searchString, StringComparison.InvariantCultureIgnoreCase) : true)).ToList();
                    break;
                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.Address) ?
                    person.Address.Contains(searchString, StringComparison.InvariantCultureIgnoreCase) : true)).ToList();
                    break;


                default:
                    matchingPersons = allPersons; break;
            }
            return matchingPersons;
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortedOrder)
        {
            if (string.IsNullOrEmpty(sortBy)) return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortedOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Age).ToList(),

                (nameof(PersonResponse.CountryName), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.CountryName), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

                _ => allPersons
            };

            return sortedPersons;
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdate)
        {
            if (personUpdate == null)
                throw new ArgumentNullException(nameof(personUpdate));

            //validation
            ValidationHelper.ModelValidation(personUpdate);

            //get matching person obj to update
            Person? matchingPerson = _persons.FirstOrDefault(person => person.PersonID == personUpdate.PersonID);
            if (matchingPerson == null)
                throw new ArgumentException("Given person id doesn't exist");

            //update all details
            matchingPerson.PersonName = personUpdate.PersonName;
            matchingPerson.Address = personUpdate.Address;
            matchingPerson.DateOfBirth = personUpdate.DateOfBirth;
            matchingPerson.CountryID = personUpdate.CountryID;
            matchingPerson.ReceiveNewsLetters = personUpdate.ReceiveNewsLetters;
            matchingPerson.Email = personUpdate.Email;
            matchingPerson.Gender = personUpdate.Gender.ToString();

            return ConvertPersonToPersonResponse(matchingPerson);
        }

        public bool DeletePerson(Guid? personId)
        {
            if(personId == null)
                throw new ArgumentNullException(nameof(personId));
            Person person = _persons.FirstOrDefault(person => person.PersonID == personId);
            if (person == null)
                return false;
            _persons.RemoveAll(person => person.PersonID == personId);
            return true;
        }

        
    }
}
