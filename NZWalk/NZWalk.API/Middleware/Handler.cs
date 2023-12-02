using System.Net;

namespace NZWalk.API.Middlewares
{
    public class Handler
    {
        private readonly ILogger<Handler> logger;
        private readonly RequestDelegate next;

        public Handler(ILogger<Handler> logger,
            RequestDelegate next)
        {
            this.logger = logger;
            this.next = next;
        }


        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();

                //Log this Exception
                logger.LogError(ex, $"{errorId} : {ex.Message}");


                //Return A Custom error response
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var error = new
                {
                    Id = errorId,
                    ErrorMessage = "Something went wrong! we are looking into resolving this."
                };
                await httpContext.Response.WriteAsJsonAsync(error);

            }
        }
    }
}
