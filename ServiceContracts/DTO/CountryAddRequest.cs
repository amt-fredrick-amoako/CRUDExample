using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{

    public class CountryAddRequest
    {
        /// <summary>
        /// Represents Country Model
        /// DTO houses models to abstract access by the external layer directly
        /// This is what the Controller or xUnit will send as an argument
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Creates an object of the Country class
        /// Converts CountryAddRequest object to Country object
        /// </summary>
        /// <returns>
        /// Returns a new object of the Country class
        /// </returns>
        public Country ToCountry()
        {
            return new Country { CountryName = this.CountryName };
        }
    }
}
