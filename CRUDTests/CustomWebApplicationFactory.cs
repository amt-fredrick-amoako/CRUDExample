using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CRUDTests
{
    /*Create a CustomWebApplicationFactory that inherits from the WebApplicationFactory
     * Override the ConfigureWebHost method
     * Use the Configure services method to configure services in it's lambda expression
     * This can be used to remove previous existing services and re-add what you want
     */
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                //Request the DbContextOptions of the ApplicationDbContext in the services collection
                //Return type is of type ServiceDescriptor which represents the type of the service and it's lifetime
                var descriptor = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);//Remove existing descriptor if any exists
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("DatabaseForTesting");
                });

            });
        }
    }
}
