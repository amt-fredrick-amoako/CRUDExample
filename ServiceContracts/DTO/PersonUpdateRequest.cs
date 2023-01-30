using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO that contains person details to update
    /// </summary>
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage ="Person ID is required")]
        public Guid PersonID { get; set; }
        [Required(ErrorMessage = "Person name is required")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Email is required"),
            EmailAddress(ErrorMessage = "Email should be in the right format")]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }


        /// <summary>
        /// Converts the current object of PersonAddRequest to a new object of Person type
        /// </summary>
        /// <returns>Person Obj</returns>
        public Person ToPerson()
        {
            return new Person
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                Address = Address,
                CountryID = CountryID,
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }
}
