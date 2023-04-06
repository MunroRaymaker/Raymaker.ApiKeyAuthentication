using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiKeyAuthentication.Authentication;

[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
public class ApiKeyAuthFilter : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if(!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult("API Key is missing");
            return;
        }

        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var apiKey = configuration.GetValue<string>(AuthConstants.ApiKeySectionName);

        
        var extractedApiKeySpan = MemoryMarshal.Cast<char, byte>(extractedApiKey.ToString().AsSpan());
        var apiKeySpan = MemoryMarshal.Cast<char, byte>(apiKey.AsSpan());
        
        if(!CryptographicOperations.FixedTimeEquals(apiKeySpan, extractedApiKeySpan))
        {
            context.Result = new UnauthorizedObjectResult("Invalid API Key");
            return;    
        }
    }
}
