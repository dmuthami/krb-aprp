using Dapper;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Extensions;
using APRP.Web.Persistence.Contexts;
using APRP.Web.ViewModels.ResponseTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace APRP.Web.Persistence.Repositories
{
    public class CommunicationRepository : BaseRepository, ICommunicationRepository
    {
        private IConfiguration Configuration { get; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly ISmsSender _smsSender;
        private readonly IEmailSender _emailSender;
        private readonly IMessageOutService _messageOutService;

        // creating object of CultureInfo 
        private CultureInfo _cultures = new CultureInfo("en-US");

        public CommunicationRepository(AppDbContext context, UserManager<ApplicationUser> userManager,
             IConfiguration configuration, ILogger<CommunicationRepository> logger,
              IEmailSender emailSender,
             ISmsSender smsSender, IMessageOutService messageOutService) : base(context)
        {
            _userManager = userManager;
            Configuration = configuration;
            _logger = logger;
            _smsSender = smsSender;
            _emailSender = emailSender;
            _messageOutService = messageOutService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> SendSMS(ApplicationUser applicationUser, string message)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                //send code
                //var message = "Your security code is: " + Code;
                //check if allowed to use Twilio
                bool useTwilio = false;
                bool resultCode = bool.TryParse(Configuration.GetSection("TwilioSettings")["Use_Twilio"], out useTwilio);
                if (useTwilio == true)
                {
                    try
                    {
                        await _smsSender.SendSmsAsync(applicationUser.PhoneNumber, message).ConfigureAwait(false);
                        authResponse.StatusCode = Ok().StatusCode.ToString(_cultures);
                        authResponse.Response = Ok().ToString();
                        return Ok(authResponse);
                    }
                    catch (Exception)
                    {
                        //Use Diafaan as an alternative
                        int ret = await WriteMessageOut(applicationUser.PhoneNumber, message).ConfigureAwait(false);
                        authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                        authResponse.Response = message;
                        return BadRequest(authResponse);
                    }
                }
                else
                {
                    //Use Diafaan
                    //write to message out table

                    int returnInssert = await WriteMessageOut(applicationUser.PhoneNumber, message).ConfigureAwait(false);
                    authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                    authResponse.Response = message;
                    return BadRequest(authResponse);
                }


            }
            catch (Exception Ex)
            {
                _logger.LogError($"Manage.AddPhone Index Error{Ex.Message}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message.ToString(_cultures);
                return BadRequest(authResponse);
            }
        }

        #region Diafaan
        private int WriteMessageOutOGN(string phoneNumber, string msg)
        {
            string connString = null;
            string substring = "";
            IDbConnection dbConnection = null;
            var whichDBMS = Configuration.GetConnectionString("DBMS"); //Read Config File
            if (whichDBMS == "PSQL")
            {
                connString = Configuration.GetConnectionString("PSQLDapperConnection");
                dbConnection = new NpgsqlConnection(connString);
                substring = "INSERT INTO 'MessageOut' ('MessageTo',  'MessageText','IsSent','IsRead')";

            }
            else
            {
                connString = Configuration.GetConnectionString("DapperConnection");
                dbConnection = new SqlConnection(connString);
                substring = "INSERT INTO MessageOut (MessageTo,  MessageText,IsSent,IsRead)";
            }

            using (IDbConnection dbConn = dbConnection)
            {
                string str;
                int issent = 0;
                int isread = 0;
                str = substring + " VALUES('" + phoneNumber + "'" + "," +
                    "'" + msg + "'" + "," +
                    "" + issent + "" + "," +
                    "" + isread + "" +
                    ")";

                dbConnection.Open();
                MessageOut messageOut = new MessageOut();

                int ret = dbConnection.Execute(str, messageOut);
                return ret;
                //return new JsonResult(list);
                //return Json(list);

            }
        }

        private async Task<int> WriteMessageOut(string phoneNumber, string msg)
        {
            int ret = 0;

            MessageOut messageOut = new MessageOut();
            messageOut.MessageTo = phoneNumber;
            messageOut.MessageText = msg;
            messageOut.IsSent = 0;
            messageOut.IsRead = 0;

            var messageOutResp = await _messageOutService.AddAsync(messageOut).ConfigureAwait(false);
            if (messageOutResp.Success == true)
            {
                ret = 0;
            }
            else
            {
                ret = 1;
            }
            return ret;
        }
        #endregion

    }
}
