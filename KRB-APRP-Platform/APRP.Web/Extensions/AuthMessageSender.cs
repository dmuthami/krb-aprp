using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace APRP.Web.Extensions
{
    public class AuthMessageSender : ISmsSender
    {
        // This class is used by the application to send Email and SMS
        // when you turn on two-factor authentication in ASP.NET Identity.
        // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
        public AuthMessageSender(IOptions<SMSoptions> optionsAccessor, IConfiguration configuration)
        {
            Options = optionsAccessor.Value;
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public SMSoptions Options { get; }  // set only via Secret Manager

        private void CheckOption()
        {
            string SMSAccountIdentification = null;
            string SMSAccountPassword = null;
            string SMSAccountFrom = null;

            if (Options.SMSAccountIdentification == null)
            {
                SMSAccountIdentification = Configuration.GetSection("TwilioSettings")["SMSAccountIdentification"];
                Options.SMSAccountIdentification = SMSAccountIdentification;
            }
            if (Options.SMSAccountPassword == null)
            {
                SMSAccountPassword = Configuration.GetSection("TwilioSettings")["SMSAccountPassword"];
                Options.SMSAccountPassword = SMSAccountPassword;
            }
            if (Options.SMSAccountFrom == null)
            {
                SMSAccountFrom = Configuration.GetSection("TwilioSettings")["SMSAccountFrom"];
                Options.SMSAccountFrom = SMSAccountFrom;
            }

        }

        /*Commented out segment for Twilio*/
        public Task SendSmsAsync(string number, string message)
        {
            //Check options
            CheckOption();

            // Plug in your SMS service here to send a text message.
            // Your Account SID from twilio.com/console
            var accountSid = Options.SMSAccountIdentification;
            // Your Auth Token from twilio.com/console
            var authToken = Options.SMSAccountPassword;

            TwilioClient.Init(accountSid, authToken);

            return MessageResource.CreateAsync(
              to: new PhoneNumber(number),
              from: new PhoneNumber(Options.SMSAccountFrom),
              body: message);
        }
    }
}
