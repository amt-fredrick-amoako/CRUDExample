using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    /*
    public class ResponseHeaderActionFilter : IActionFilter, IOrderedFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        private readonly string Key;
        private readonly string Value;

        public int Order { get; set; } // used to set the order of execution

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string key, string value, int order)
        {
            _logger = logger;
            Key = key;
            Value = value;
            Order = order;
        }

        //before
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuting));
        }

        //after
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuted));

            context.HttpContext.Response.Headers[Key] = Value;
        }

    } */
    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        private string? Key { get; set; }
        private string? Value { get; set; }
        private int Order { get; set; }

        public ResponseHeaderFilterFactoryAttribute(string key, string value, int order)
        {
            Key = key;
            Value = value;
            Order = order;
        }
        // Controller -> FilterFactory -> Filter
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            ResponseHeaderActionFilter filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
            //return filter object
            filter.Key = Key;
            filter.Value = Value;
            filter.Order = Order;
            return filter;
        }
    }

    /*if you need to provide a service in the constructor of the filter class
     * 1. make the filter class constructor parameterless
     * 2. make the factory class parameterless constructor as well
     * 3. assign values using properties and not fields
     * 4. use the IService provider to get required service from the ioc container
     */

    public class ResponseHeaderActionFilter : /*ActionFilterAttribute*/ IAsyncActionFilter, IOrderedFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        //private readonly string _key;
        //private readonly string _value;
        public string? Key { get; set; }
        public string? Value { get; set; }

        public int Order { get; set; } // used to set the order of execution

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger/*string key, string value, int order*/)
        {
            _logger = logger;
            //Key = key;
            //Value = value;
            //Order = order;
        }


        //override virtual methods of base class
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method - before", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

            await next(); //calls next filter or action method

            _logger.LogInformation("{FilterName}.{MethodName} method - after", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

            context.HttpContext.Response.Headers[Key] = Value;
        }
    }
}
