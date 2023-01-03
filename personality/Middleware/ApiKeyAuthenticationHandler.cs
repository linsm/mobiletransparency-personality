using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions> {
    
    private readonly IApiKeyService apiKeyService;
    private const string ApiKeyHeaderName = "ApiKey";

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IApiKeyService apiKeyService) : base(options, logger, encoder, clock)
    {
        if(apiKeyService == null) {
            throw new ArgumentNullException(nameof(apiKeyService));
        }
        this.apiKeyService = apiKeyService;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaders))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }
        string? apiKey = apiKeyHeaders.FirstOrDefault();

        if (!apiKeyHeaders.Any() || string.IsNullOrWhiteSpace(apiKey))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        ApiKey? validatedApiKey = apiKeyService.VerifyApiKey(apiKey);
        if(validatedApiKey != null) {
            IList<Claim> claims = new List<Claim>();
            foreach(Role role in validatedApiKey.roles) {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }          

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            AuthenticationTicket authenticationTicket = new AuthenticationTicket(claimsPrincipal, ApiKeyAuthenticationOptions.DefaultScheme);
            return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
        }
        return Task.FromResult(AuthenticateResult.NoResult());
    }
}
