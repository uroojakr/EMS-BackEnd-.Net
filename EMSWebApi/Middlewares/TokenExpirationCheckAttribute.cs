using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

public class TokenExpirationCheckFilter : IAsyncActionFilter
{
    private readonly ILogger<TokenExpirationCheckFilter> _logger;

    public TokenExpirationCheckFilter(ILogger<TokenExpirationCheckFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeaderValue))
        {
            var token = authHeaderValue.First()!.Split(" ").Last(); // for bearer scheme
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenSplits = token.Split('.');
                    var tokenPayload = tokenSplits[1];
                    var padding = tokenPayload.Length % 4;
                    if (padding > 0)
                    {
                        tokenPayload += new string('=', 4 - padding);
                    }

                    var tokenBytes = Convert.FromBase64String(tokenPayload);
                    var tokenString = System.Text.Encoding.UTF8.GetString(tokenBytes);

                    var jwtToken = tokenHandler.ReadJwtToken(tokenString);

                    if (jwtToken.ValidTo < DateTime.UtcNow)
                    {
                        _logger.LogWarning("Token expired.");
                        context.Result = new UnauthorizedResult();
                        return;
                    } //jwt method on events
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking token expiration.");
                }
            }
        }

        await next();
    }
}