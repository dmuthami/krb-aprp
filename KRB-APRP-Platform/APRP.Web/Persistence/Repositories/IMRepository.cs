using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using APRP.Web.ViewModels.ResponseTypes;
using APRP.Web.Extensions;
using System.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using APRP.Web.ViewModels.Account;

namespace APRP.Web.Persistence.Repositories
{
    public class IMRepository : BaseRepository, IIMRepository
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly ISmsSender _smsSender;
        public IMRepository(AppDbContext context, UserManager<ApplicationUser> userManager,
             IEmailSender emailSender, ILogger<IIMRepository> logger,
             ISmsSender smsSender) : base(context)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _smsSender = smsSender;
        }

        public async Task<IActionResult> ForgotPassword2(InputModel inputModel)
        {
            AuthResponse authResponse = new AuthResponse();

            try
            {
                var user2 = await Task.Run(() => GetUser(inputModel)).ConfigureAwait(false);

                //var user = await _userManager.FindByEmailAsync(inputModel.Email);
                /*I have momentarily disabled the check for a confirmed email account from a user*/
                if (user2 == null /*|| !(await _userManager.IsEmailConfirmedAsync(user))*/)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    authResponse.StatusCode = BadRequest().StatusCode.ToString();
                    authResponse.Response = "User does not exist or is User email is available";
                    return NotFound(authResponse);
                }
                else
                {
                    authResponse.StatusCode = Ok().StatusCode.ToString();
                    authResponse.ApplicationUser = user2;
                    return Ok(authResponse);
                }

            }
            catch (Exception Ex)
            {
                authResponse.StatusCode = BadRequest().StatusCode.ToString();
                authResponse.Response = Ex.Message;
                string msg = "IMRepository.ForgotPassword : \r\n" +
                $" {Ex.Message}";
                _logger.LogError(msg);
                return BadRequest(authResponse);
            }
        }

        private ApplicationUser GetUser(InputModel inputModel)
        {
            var user2 = _context.Users.Where(s =>
                (s.UserName == inputModel.UserName && s.Email == inputModel.Email))
                   .FirstOrDefault();

            return user2;
        }
    }
}
