using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace APRP.Web.Extensions
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, IConfiguration configuration)
        {
            Options = optionsAccessor.Value;
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            string mykey = null;
            if (Options.SendGridKey == null)
            {
                mykey = Configuration.GetSection("SendGridSettings")["SendGridKey"];
            }
            else
            {
                mykey = Options.SendGridKey;
            }
            return Execute(mykey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("fourtyseven.gis@gmail.com", "KRB Admin"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.TrackingSettings = new TrackingSettings
            {
                ClickTracking = new ClickTracking { Enable = false }
            };

            return client.SendEmailAsync(msg);
        }
    }
}
