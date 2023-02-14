using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO used as a return type for most of CountriesService methods
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryId { get; set; }
        public string CountryName { get; set; }

        //Compare the current object to another object of CountryResponse type and returns true if both have the same values and false otherwise
        public override bool Equals(object? obj)
        {
            if (obj == null) return false; // return false if the obj parameter is null
            if(obj.GetType() != typeof(CountryResponse)) return false; //return false if parameter is not of the same type as the CountryResponse

            CountryResponse country_to_compare = obj as CountryResponse;
            return this.CountryId == country_to_compare.CountryId && this.CountryName == country_to_compare.CountryName;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    public static class CountryExtension
    {
        public static CountryResponse ToCountryResponse(this Country response)
        {
            return new CountryResponse { CountryId = response.CountryID, CountryName = response.CountryName };
        }
    }
}
