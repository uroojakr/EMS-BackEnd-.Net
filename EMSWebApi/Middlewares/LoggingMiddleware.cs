
using System.Security.Claims;


namespace EMSWebApi.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // request path does not starts with "/api/auth"
            if (!context.Request.Path.StartsWithSegments("/api/auth"))
            {
                //logging info about the incoming request
                LogRequestInfo(context);

                // capture the response stream
                var originalResponseBodyStream = context.Response.Body;
                using (var responseBodyStream = new MemoryStream()) //disposed when no longer needed
                {
                    context.Response.Body = responseBodyStream;

                    // calling the next middleware in the pipeline
                    await _next(context);

                    // loggign infor about the outgoing response
                    LogResponseInfo(context);

                    // copy the response stream to the original response body
                    responseBodyStream.Seek(0, SeekOrigin.Begin);
                    await responseBodyStream.CopyToAsync(originalResponseBodyStream);
                }
            }
            else
            {
                // if the request path "/api/auth," proceed without logging
                await _next(context);
            }
        }

        private void LogRequestInfo(HttpContext context)
        {
            var request = context.Request;

            // logging request method and path
            _logger.LogInformation($"Request: {request.Method} {request.Path}");

            // logging request headers
            foreach (var header in request.Headers)
            {
                _logger.LogInformation($"Request Header: {header.Key}: {header.Value}");
            }

            // logging user info in jwt
            var user = context.User;
            if (user.Identity!.IsAuthenticated)
            {
                var userName = user.Identity.Name;
                var userRoles = string.Join(", ", user.FindAll(ClaimTypes.Role));
                _logger.LogInformation($"User: {userName}, Roles: {userRoles}");
            }
        }

        private void LogResponseInfo(HttpContext context)
        {
            var response = context.Response;

            // logging response status code and content type
            _logger.LogInformation($"Response Status Code: {response.StatusCode}");
            _logger.LogInformation($"Response Content Type: {response.ContentType}");

            // logging response headers
            foreach (var header in response.Headers)
            {
                _logger.LogInformation($"Response Header: {header.Key}: {header.Value}");
            }
        }
    }
}
