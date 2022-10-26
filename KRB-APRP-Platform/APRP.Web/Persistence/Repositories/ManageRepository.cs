using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using APRP.Web.ViewModels.ResponseTypes;
using APRP.Web.ViewModels.DTO;
using APRP.Web.Extensions;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Npgsql;
using APRP.Web.Domain.Services;

namespace APRP.Web.Persistence.Repositories
{
    public class ManageRepository : BaseRepository, IManageRepository
    {
        private IConfiguration Configuration { get; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly ISmsSender _smsSender;
        private readonly IMessageOutService _messageOutService;
        public ManageRepository(AppDbContext context, UserManager<ApplicationUser> userManager,
             IConfiguration configuration, ILogger<ManageRepository> logger,
             ISmsSender smsSender, IMessageOutService messageOutService) : base(context)
        {
            _userManager = userManager;
            Configuration = configuration;
            _logger = logger;
            _smsSender = smsSender;
            _messageOutService = messageOutService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddPhone(UserDTO userDTO)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                
                //get the user id of the user added into the system
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(userDTO.Id).ConfigureAwait(false);
                //await _signInManager.SignInAsync(user, isPersistent: false);

                if (applicationUser == null)
                {
                    authResponse.StatusCode = BadRequest().StatusCode.ToString();
                    authResponse.Response = "Your User credentials couldn't be retrieved" +
                        " from the system.";
                    authResponse.Success = false;
                    return BadRequest(authResponse);
                }
                string _PhoneNumber = await _userManager.GetPhoneNumberAsync(applicationUser).ConfigureAwait(false);
                //send code
                var code = await _userManager.GenerateChangePhoneNumberTokenAsync(applicationUser, _PhoneNumber).ConfigureAwait(false);

                var message = "Your security code is: " + code;

                //check if allowed to use Twilio
                string use_Twilio = Configuration.GetSection("TwilioSettings")["Use_Twilio"];
                if (use_Twilio == "True")
                {
                    try
                    {
                        await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(applicationUser).ConfigureAwait(false), message).ConfigureAwait(false);
                        authResponse.StatusCode = Ok().StatusCode.ToString();
                        authResponse.Response = Ok().ToString();
                        return Ok(authResponse);
                    }
                    catch (Exception)
                    {
                        //Use Diafaan as an alternative
                        int ret = await  WriteMessageOut(await _userManager.GetPhoneNumberAsync(applicationUser).ConfigureAwait(false), message).ConfigureAwait(false);
                        authResponse.StatusCode = BadRequest().StatusCode.ToString();
                        authResponse.Response = message;
                        return BadRequest(authResponse);
                    }
                }
                else
                {
                    //Use Diafaan
                    //write to message out table

                    int returnInssert = await WriteMessageOut(_PhoneNumber, message).ConfigureAwait(false);
                    authResponse.StatusCode = BadRequest().StatusCode.ToString();
                    authResponse.Response = message;
                    return BadRequest(authResponse);
                }


            }
            catch (Exception Ex)
            {
                _logger.LogError($"Manage.AddPhone Index Error{Ex.Message}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString();
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        public async Task<IActionResult> VerifyPhone(UserDTO userDTO)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                //get the user id of the user added into the system
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(userDTO.Id).ConfigureAwait(false);
                if (applicationUser == null)
                {
                    authResponse.StatusCode = BadRequest().StatusCode.ToString();
                    authResponse.Response = "User couldnt be retrieved.";
                    return BadRequest(authResponse);
                }

                //verify code
                string _PhoneNumber = await _userManager.GetPhoneNumberAsync(applicationUser).ConfigureAwait(false);
                var result = await _userManager.ChangePhoneNumberAsync(applicationUser, _PhoneNumber, userDTO.Code).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    authResponse.StatusCode = Ok().StatusCode.ToString();
                    authResponse.Response = applicationUser.Id;
                    return Ok(authResponse);
                }
                else
                {
                    authResponse.StatusCode = BadRequest().StatusCode.ToString(); ;
                    authResponse.Response = "Invalid verification code." +
                    "Reenter code received or try signing in again";
                    return BadRequest(authResponse);
                }


            }
            catch (Exception Ex)
            {
                _logger.LogError($"ManageRepository.VerifyPhone Error{Ex.Message}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString();
                authResponse.Response = Ex.Message;
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
                str = substring+" VALUES('" + phoneNumber + "'" + "," +
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
            if (messageOutResp.Success==true)
            {
                ret = 0;
            } else
            {
                ret = 1;
            }
            return ret;
        }
        #endregion
    }
}
