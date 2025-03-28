using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace AuthenticationApi.Application.Services
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;

        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;

            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            TwilioClient.Init(accountSid, authToken);
        }

        public async Task SendOtpAsync(SendOtpDTO sendOtpDTO)
        {
            var serviceSid = _configuration["Twilio:VerifyServiceSid"];
            await VerificationResource.CreateAsync(
                to: sendOtpDTO.PhoneNumber,
                channel: "sms",
                pathServiceSid: serviceSid
            );
        }

        public async Task<bool> VerifyOtpAsync(VerifyPhoneDTO verifyPhoneDTO)
        {
            var serviceSid = _configuration["Twilio:VerifyServiceSid"];
            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: verifyPhoneDTO.PhoneNumber,
                code: verifyPhoneDTO.Code,
                pathServiceSid: serviceSid
            );

            return verificationCheck.Status == "approved";
        }
    }
}