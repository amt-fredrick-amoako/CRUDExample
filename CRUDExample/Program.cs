using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;
using Serilog;
using CRUDExample.Filters.ActionFilters;
using CRUDExample;
using CRUDExample.Middleware;

var builder = WebApplication.CreateBuilder(args);
/*
configure logging providers, choose what you want and what you don't want 
builder.Host.ConfigureLogging(logging => {
    logging.ClearProviders();
    logging.AddDebug();
    logging.AddConsole();
    logging.AddEventLog();
});
*/

//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration logger) =>
{
    logger.ReadFrom.Configuration(context.Configuration)//read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services);// read out current app's services and make them available to serilog
});

builder.Services.ConfigureServices(builder.Configuration);


var app = builder.Build();

app.UseSerilogRequestLogging();//add serilogrequest loggin to the middleware chain

//Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PersonsDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware();
}

app.UseHttpLogging();
//app.Logger.LogDebug("Debug LogLevel");
//app.Logger.LogInformation("Information LogLevel");
//app.Logger.LogError("Error LogLevel");
//app.Logger.LogTrace("Trace LogLevel");
//app.Logger.LogCritical("Critical LogLevel");
//app.Logger.LogWarning("Warning - Log");

if (builder.Environment.IsEnvironment("Test") == false)
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");//add file to middleware chain

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();


app.Run();

public partial class Program { } // make the Program class accessible programmatically
