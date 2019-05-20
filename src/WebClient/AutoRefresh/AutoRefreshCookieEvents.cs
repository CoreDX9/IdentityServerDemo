using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebClient.AutoRefresh
{
    public class AutoRefreshCookieEvents : CookieAuthenticationEvents
    {
        private readonly IOptionsSnapshot<OpenIdConnectOptions> _oidcOptions;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly ISystemClock _clock;
        private readonly AutoRefreshOptions _refreshOptions;

        public AutoRefreshCookieEvents(
            IOptions<AutoRefreshOptions> refreshOptions,
            IOptionsSnapshot<OpenIdConnectOptions> oidcOptions,
            IAuthenticationSchemeProvider schemeProvider,
            IHttpClientFactory httpClientFactory,
            ILogger<AutoRefreshOptions> logger,
            ISystemClock clock)
        {
            _refreshOptions = refreshOptions.Value;
            _oidcOptions = oidcOptions;
            _schemeProvider = schemeProvider;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _clock = clock;
        }

        // important: this is just a POC at this point - it misses any thread synchronization. Will add later.
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var tokens = context.Properties.GetTokens();
            if (tokens == null || !tokens.Any())
            {
                _logger.LogDebug("No tokens found in cookie properties. SaveTokens must be enabled for automatic token refresh.");
                return;
            }

            var refreshToken = tokens.SingleOrDefault(t => t.Name == OpenIdConnectParameterNames.RefreshToken);
            if (refreshToken == null)
            {
                _logger.LogWarning("No refresh token found in cookie properties. A refresh token must be requested and SaveTokens must be enabled.");
                return;
            }

            var expiresAt = tokens.SingleOrDefault(t => t.Name == "expires_at");
            if (expiresAt == null)
            {
                _logger.LogWarning("No expires_at value found in cookie properties.");
                return;
            }

            var dtExpires = DateTimeOffset.Parse(expiresAt.Value, CultureInfo.InvariantCulture);
            var dtRefresh = dtExpires.Subtract(_refreshOptions.RefreshBeforeExpiration);

            if (dtRefresh < _clock.UtcNow)
            {
                var oidcOptions = await GetOidcOptionsAsync();
                var configuration = await oidcOptions.ConfigurationManager.GetConfigurationAsync(default(CancellationToken));

                var tokenClient = _httpClientFactory.CreateClient("tokenClient");

                var response = await tokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    Address = configuration.TokenEndpoint,

                    ClientId = oidcOptions.ClientId,
                    ClientSecret = oidcOptions.ClientSecret,
                    RefreshToken = refreshToken.Value
                });

                if (response.IsError)
                {
                    _logger.LogWarning("Error refreshing token: {error}", response.Error);
                    return;
                }

                context.Properties.UpdateTokenValue("access_token", response.AccessToken);
                context.Properties.UpdateTokenValue("refresh_token", response.RefreshToken);

                var newExpiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(response.ExpiresIn);
                context.Properties.UpdateTokenValue("expires_at", newExpiresAt.ToString("o", CultureInfo.InvariantCulture));

                await context.HttpContext.SignInAsync(context.Principal, context.Properties);
            }
        }

        private async Task<OpenIdConnectOptions> GetOidcOptionsAsync()
        {
            if (string.IsNullOrEmpty(_refreshOptions.Scheme))
            {
                var scheme = await _schemeProvider.GetDefaultChallengeSchemeAsync();
                return _oidcOptions.Get(scheme.Name);
            }
            else
            {
                return _oidcOptions.Get(_refreshOptions.Scheme);
            }
        }
    }
}