namespace EMSWebApi.Middlewares
{
    public class ConsoleLoggerMiddleware : IMiddleware
    {
        private readonly ILogger<ConsoleLoggerMiddleware> _logger;

        public ConsoleLoggerMiddleware(ILogger<ConsoleLoggerMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next) //constructor
        {
            Console.WriteLine("HelloWorld");
            Console.WriteLine("Before request");
            await next(context);
            Console.WriteLine("After Request");

            Console.WriteLine("Write executed");
        }
    }
}
