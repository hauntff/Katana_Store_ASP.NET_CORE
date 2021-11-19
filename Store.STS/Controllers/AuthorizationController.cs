using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Store.STS.Extensions;
using Store.STS.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Store.STS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly PhoneNumberTokenProvider<ApplicationUser> _phoneNumberTokenProvider;
        private readonly IOpenIddictScopeManager _scopeManager;

        private readonly ILogger _logger;

        public AuthorizationController(
            PhoneNumberTokenProvider<ApplicationUser> phoneNumberTokenProvider,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOpenIddictScopeManager scopeManager,
            ILogger<AuthorizationController> logger)
        {
            _phoneNumberTokenProvider = phoneNumberTokenProvider
                                           ?? throw new ArgumentNullException(nameof(phoneNumberTokenProvider));
            _scopeManager = scopeManager;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _logger = logger;
        }
        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return Challenge(
                    authenticationSchemes: CookieAuthenticationDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                            Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                    });
            }

            var claims = new List<Claim>
                {
                    new Claim(OpenIddictConstants.Claims.Subject, result.Principal.Identity.Name),
                    new Claim("some claim", "some value").SetDestinations(OpenIddictConstants.Destinations.AccessToken),
                    new Claim(OpenIddictConstants.Claims.Email, "some@email").SetDestinations(OpenIddictConstants.Destinations.IdentityToken)
                };

            var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            claimsPrincipal.SetScopes(request.GetScopes());

            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpPost("~/connect/token")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
            var user = await GetUser(request.Username);
            if (user is null)
                return BadRequest("user_not_exist");

            ClaimsPrincipal claimsPrincipal;
            if (request.IsClientCredentialsGrantType())
            {

                var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());

                identity.AddClaim("some-claim", "some-value", OpenIddictConstants.Destinations.AccessToken);

                claimsPrincipal = new ClaimsPrincipal(identity);

                claimsPrincipal.SetScopes(request.GetScopes());
            }

            else if (request.IsAuthorizationCodeGrantType())
            {
                claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            }

            else if (request.IsRefreshTokenGrantType())
            {
                claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            }
            else if (request.IsVerificationTokenGrantType())
                return await TokenVerificationGrantType(request, user);
            else
                throw new InvalidOperationException("The specified grant type is not supported.");
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("~/connect/userinfo")]
        public async Task<IActionResult> Userinfo()
        {
            var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

            return Ok(new
            {
                Name = claimsPrincipal.GetClaim(OpenIddictConstants.Claims.Subject),
                Occupation = "Developer",
                Age = 43
            });
        }

        private async Task<IActionResult> TokenVerificationGrantType(OpenIddictRequest request, ApplicationUser user)
        {
            if (!request.TryGetParameter("verification_token", out var verificationTokenPrm))
                return BadRequest("'verification_token' is not exist in parameters");
            if (verificationTokenPrm.Value is null)
                return BadRequest("'verification_token' is empty");
            var matched = request.Username.CheckPhone();
            if (!matched.Success)
                return BadRequest("wrong_number");
            var verificationToken = verificationTokenPrm.Value?.ToString();
            if (string.IsNullOrWhiteSpace(verificationToken))
                return BadRequest("invalid_token");
            var tokenValidated =
                await _phoneNumberTokenProvider.ValidateAsync("verification_token", verificationToken, _userManager, user);
            if (!tokenValidated)
            {
                _logger.LogInformation("Authentication failed for token: {token}, reason: invalid token", verificationToken);
                return Unauthorized("invalid_token");
            }
            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var claims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in claims.Where(p => p.Type != Claims.Scope))
                identity.AddClaim(claim);
            identity.AddClaim(new Claim(Claims.ClientId, request.ClientId ?? throw new InvalidOperationException()));

            var claimsPrincipal = new ClaimsPrincipal(identity);
            var dbScopes = claims.Where(p => p.Type == Claims.Scope);
            var userScopes = new List<string>
            {
                Scopes.Roles,
                Scopes.OfflineAccess,
                Scopes.Email,
                Scopes.Profile,
                Scopes.Phone
            };
            foreach (var scope in dbScopes)
                userScopes.Add(scope.Value);
            var requestScopes = request.GetScopes();
            if (requestScopes.Any())
                userScopes.AddRange(requestScopes);
            claimsPrincipal.SetScopes(userScopes);
            var scopes = ImmutableArray.Create(userScopes.ToArray());
            var resources = _scopeManager.ListResourcesAsync(scopes);
            var userResources = new List<string>();
            await foreach (var resource in resources)
                userResources.Add(resource);
            claimsPrincipal.SetResources(userResources);
            var result = SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            return result;
        }

        private async Task<ApplicationUser> GetUser(string phoneNumber)
        {
            var normalizedPhoneNumber = _userManager.NormalizeName(phoneNumber);
            var user = await _userManager.FindByNameAsync(normalizedPhoneNumber);
            if (user is not null)
                _logger.LogInformation("Returning existing user: {username}, ", phoneNumber);
            return user;
        }
    }
}
