using CRUDExample.Filters.ActionFilters;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace CRUDExample
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ResponseHeaderActionFilter>();

            //Add controllers and views as services
            //Configure global filter
            services.AddControllersWithViews(options =>
            {
                //options.Filters.Add<ResponseHeaderActionFilter>(); //Global filter added but parameters cannot be added like this
                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>(); //Creates a service provider with services from provided service collection

                options.Filters.Add(new ResponseHeaderActionFilter(logger /*,"My-Key-From-Global", "My-Value-From-Global", 2*/)
                {
                    Key = "My-Key-From-Global",
                    Value = "My-Value-From-Global",
                    Order = 2
                });
            });

            //Add PersonsService and CountriesService to the IoC
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IPersonsGetService, PersonsGetterServiceWithFewExcelFields>();
            services.AddScoped<PersonsGetService, PersonsGetService>();

            services.AddScoped<IPersonsAdderService, PersonsAddService>();
            services.AddScoped<IPersonsSortService, PersonsSortService>();
            services.AddScoped<IPersonsUpdateService, PersonsUpdateService>();
            services.AddScoped<IPersonsDeleteService, PersonsDeleteService>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            //Add PersonsDbContext to the IoC
            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddTransient<PersonsListActionFilter>();

            //logging http request and response
            services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties
                | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });

            return services;
        }
    }
}
