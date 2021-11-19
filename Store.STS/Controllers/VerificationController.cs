using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Store.STS.Extensions;
using Store.STS.Model;
using Store.STS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Store.STS.Controllers
{
    [ApiController]
    [Route("api/verification")]
    public class VerificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DataProtectorTokenProvider<ApplicationUser> _dataProtectorTokenProvider;
        private readonly PhoneNumberTokenProvider<ApplicationUser> _phoneNumberTokenProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        public VerificationController(
            IConfiguration configuration,
            DataProtectorTokenProvider<ApplicationUser> dataProtectorTokenProvider,
            PhoneNumberTokenProvider<ApplicationUser> phoneNumberTokenProvider,
            UserManager<ApplicationUser> userManager,
            ILogger<VerificationController> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _dataProtectorTokenProvider = dataProtectorTokenProvider
                ?? throw new ArgumentNullException(nameof(_dataProtectorTokenProvider));

            _phoneNumberTokenProvider = phoneNumberTokenProvider
                ?? throw new ArgumentNullException(nameof(_phoneNumberTokenProvider));

            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LiginViewMoled model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                _logger.LogWarning($"On post login model error: {message}");
                return BadRequest(ModelState);
            }

            var matched = model.PhoneNumber.CheckPhone();
            if (!matched.Success)
                return BadRequest("wrong_number");
            var user = await GetOrCreateUser(model.PhoneNumber);
            if (user is null)
                return BadRequest("user_not_created"); 
            var (verificationToken, result) = await SendSmsRequest(model.PhoneNumber, user, token);

            if (!result)
            {
                _logger.LogWarning("Sms send error");
                return BadRequest("sms_send_error");
            }

            var resendToken = await _dataProtectorTokenProvider.GenerateAsync("resend_token", _userManager, user);
            var body = GetBody(verificationToken, resendToken);
            _logger.LogInformation($"Sending sms ok, token: {verificationToken}");
            return Accepted(body);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] ResendViewModel model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                _logger.LogWarning($"On put login model error: {message}");
                return BadRequest(ModelState);
            }

            var user = await GetUser(model.PhoneNumber);
            if (user is null)
                return BadRequest("user_not_created");
            if (!await _dataProtectorTokenProvider.ValidateAsync("resend_token", model.ResendToken, _userManager, user))
            {
                _logger.LogWarning("Invalid resend token");
                return BadRequest("Invalid resend token");
            }

            var (verifyToken, result) = await SendSmsRequest(model.PhoneNumber, user, token);

            if (!result)
            {
                _logger.LogWarning("Sending sms failed");
                return BadRequest("Sending sms failed");
            }

            var newResendToken = await _dataProtectorTokenProvider.GenerateAsync("resend_token", _userManager, user);
            var body = GetBody(verifyToken, newResendToken);
            _logger.LogInformation($"Resending token ok, token: {verifyToken}");
            return Accepted(body);
        }
        private async Task<ApplicationUser> GetUser(string phoneNumber)
        {
            var normPhone = _userManager.NormalizeName(phoneNumber);
            var user = await _userManager.FindByNameAsync(normPhone);
            _logger.LogInformation("Returning existing user: {username}, ", phoneNumber);
            return user;
        }
        private async Task<ApplicationUser> GetOrCreateUser(string phoneNumber)
        {
            var normalizedPhoneNumber = _userManager.NormalizeName(phoneNumber);
            var user = await _userManager.FindByNameAsync(normalizedPhoneNumber);
            if (user is null)
            {
                user = phoneNumber.NewVerificationTokenUser();
                var resultCreation = await _userManager.CreateAsync(user);
                if (resultCreation != IdentityResult.Success)
                {
                    foreach (var error in resultCreation.Errors)
                    {

                    }
                    _logger.LogWarning("User creation failed: {username}, reason: invalid user", phoneNumber);
                    return null;
                }
                user = await _userManager.FindByNameAsync(normalizedPhoneNumber);
                _logger.LogInformation("User creation ok: {username}", phoneNumber);
                await _userManager.AddClaimsAsync(user, new List<Claim>
                    {
                        new Claim(Claims.Subject, user.Id),
                        new Claim(Claims.Username, user.PhoneNumber),
                        new Claim(Claims.Role, "client"),
                        new Claim(Claims.Scope, "mobile-api")
                    }); ;
            }
            _logger.LogInformation("Returning existing user: {username}, ", phoneNumber);
            return user;
        }
        private async Task<(string VerifyToken, bool Result)> SendSmsRequest(string phoneNumber,
            ApplicationUser user, CancellationToken token)
        {
            var verifyToken = await _phoneNumberTokenProvider.GenerateAsync("verification_token", _userManager, user);
            if (_configuration["SendRealSms"] == bool.TrueString)
            {
                // var result = await _smsService.SendAsync(phoneNumber, $"{verifyToken}", token);
                _logger.LogInformation("'SendRealSms' enabled, sending token is true");
                return (verifyToken, true);
            }
            else
                _logger.LogInformation("'SendRealSms' disabled, sending token is false");
            return (verifyToken, true);
        }

        private Dictionary<string, string> GetBody(string verifyToken, string resendToken)
        {
            var body = new Dictionary<string, string> { { "resend_token", resendToken } };
            if (_configuration["ReturnVerifyTokenForTesting"] == bool.TrueString)
            {
                _logger.LogInformation("'ReturnVerifyTokenForTesting' enabled, sending token is true");
                body.Add("verification_token", verifyToken);
            }
            else _logger.LogInformation("'ReturnVerifyTokenForTesting' disabled, sending token is false");

            return body;
        }
    }
}
