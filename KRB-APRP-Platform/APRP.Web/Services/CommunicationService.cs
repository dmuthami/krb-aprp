using APRP.Web.ViewModels.Account;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels.ResponseTypes;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net.Mail;
using System.Net;

namespace APRP.Web.Services
{
    public class CommunicationService : ControllerBase, ICommunicationService
    {
        private readonly ICommunicationRepository _manageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;

        private CultureInfo _cultures = new CultureInfo("en-US");
        public IConfiguration Configuration { get; }

        public CommunicationService(ICommunicationRepository manageRepository, IUnitOfWork unitOfWork
            , ILogger<CommunicationService> logger,
            IEmailSender emailSender, IConfiguration configuration)
        {
            _manageRepository = manageRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailSender = emailSender;
            Configuration = configuration;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CommunicationResponse> SendEmail2(SendEmailModel sendEmailModel)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                //Check if need to send email by GMail
                string mykey = Configuration["UseGmail"];
                bool flag;
                bool result = Boolean.TryParse(mykey, out flag);
                if (result)
                {
                    try
                    {
                        //Get Port
                        string portFlag = Configuration.GetSection("GmailSettings")["port"];
                        int port;
                        result = int.TryParse(portFlag, out port);

                        //Get Email
                        string emailFlag = Configuration.GetSection("GmailSettings")["Email"];

                        //Get app password
                        string passwordFlag = Configuration.GetSection("GmailSettings")["Password"];

                        using (var message = new MailMessage())
                        {
                            message.To.Add(new MailAddress(sendEmailModel.Email, "To Name"));
                            message.From = new MailAddress("krb.web.rms@gmail.com", "KRB Admin");
                            //message.CC.Add(new MailAddress(sendEmail.CC, "CC Name"));
                            //message.Bcc.Add(new MailAddress("bcc@email.com", "BCC Name"));
                            message.Subject = sendEmailModel.Subject;
                            message.Body = sendEmailModel.HTMLMessage;
                            message.IsBodyHtml = true;

                            using (var client = new SmtpClient("smtp.gmail.com"))
                            {
                                client.Port = port;
                                client.Credentials = new NetworkCredential(emailFlag, passwordFlag);
                                client.EnableSsl = true;
                                client.Send(message);
                            }
                        }
                        authResponse.StatusCode = Ok().StatusCode.ToString(_cultures);
                        authResponse.Response = "You have successfully sent out an Email";
                        return new CommunicationResponse(Ok(authResponse));
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"CommunicationService.SendEmail2 Error: {Environment.NewLine}");
                        authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                        authResponse.Response = Ex.Message.ToString(_cultures);
                        return new CommunicationResponse(BadRequest(authResponse));
                    }
                }
                else
                {
                    try
                    {
                        await _emailSender.SendEmailAsync(
                         sendEmailModel.Email,
                         sendEmailModel.Subject,
                         sendEmailModel.HTMLMessage).ConfigureAwait(false);

                        authResponse.StatusCode = Ok().StatusCode.ToString(_cultures);
                        authResponse.Response = "You have successfully sent out an Email";
                        return new CommunicationResponse(Ok(authResponse));

                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"CommunicationService.SendEmail2 Error: {Environment.NewLine}");
                        authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                        authResponse.Response = Ex.Message.ToString(_cultures);
                        return new CommunicationResponse(BadRequest(authResponse));
                    }
                }



            }
            catch (Exception Ex)
            {
                _logger.LogError($"CommunicationRepository.SendEmail2 Index Error{Ex.Message.ToString(_cultures)}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message.ToString(_cultures);
                return new CommunicationResponse(BadRequest(authResponse));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CommunicationResponse> SendSMS(ApplicationUser applicationUser, string message)
        {
            try
            {
                var iActionResult = await _manageRepository.SendSMS(applicationUser, message).ConfigureAwait(false);
                //await _unitOfWork.CompleteAsync();

                return new CommunicationResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CommunicationService.SendEmail Error: {Environment.NewLine}");
                return new CommunicationResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }
    }
}
