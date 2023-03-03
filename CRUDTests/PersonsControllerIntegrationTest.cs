using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace CRUDTests
{
    //Request IClassFixture from `xUnit` to create a new object of our CustomWebApplicationFactory
    public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient; //Predefined but should be created based on the custom web application factory, request made by this client should be received by it

        public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        #region Index
        [Fact]
        public async Task Index_ToReturnView()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await _httpClient.GetAsync("/Persons/Index");

            //Assert
            response.Should().BeSuccessful();
            //Read content of the response body
            string responseBody = await response.Content.ReadAsStringAsync();

            //create htmldocument obj
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(responseBody);//load html from the response body
            var document = html.DocumentNode;//create dom from the html
            document.QuerySelectorAll("table.persons").Should().NotBeNullOrEmpty();//select the table with class "persons" and asserts
        }
        #endregion
    }
}
