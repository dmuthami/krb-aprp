using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using APRP.Web.ViewModels;
using APRP.Web.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Identity;
using APRP.Web.ViewModels.ResponseTypes;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Persistence.Repositories
{
    public class RegisterRepository : BaseRepository, IRegisterRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private CultureInfo _cultures = new CultureInfo("en-US");

        public RegisterRepository(AppDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<RegisterRepository> logger) : base(context)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<AuthResponse> AddAsync(Register register)
        {
            AuthResponse authResponse = new AuthResponse();
            //check if the user exists
            ApplicationUser userExists = await _userManager.FindByNameAsync(register.Username);
            if (userExists != null)
            {
                authResponse.StatusCode = "400";
                authResponse.Response = "The supplied National ID/KRA Pin Exists in the system." +
                    " Please contact the system administrator";
                authResponse.Success = false;
                return authResponse;
            }

            /*FormatException phone number
             * from say 0700002200 to +254700002200 
             */
            UtilityCustom utilityCustom = new UtilityCustom();
            register.PhoneNumberFormatted = utilityCustom.FormatPhoneNumber(register.PhoneNumber);

            var user = new ApplicationUser
            {
                UserName = register.Username,
                Email = register.Email,
                PhoneNumber = register.PhoneNumberFormatted
            };
            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                //enable two factor authentication
                await _userManager.SetTwoFactorEnabledAsync(user, true);//enable twofactor authentication

                authResponse.StatusCode = "400";
                authResponse.Response = "Ok";
                authResponse.Success = true;
                authResponse.ApplicationUser = user;
                return authResponse;
            }
            else
            {
                authResponse.Response = "Create User Failed";
                authResponse.StatusCode = "400";
                authResponse.result = result;
                authResponse.Success = false;

                return authResponse;
            }
        }

        public async Task<IList<UserListViewModel>> ListAsync()
        {        
            return await Task.Run(() => GetUsers()).ConfigureAwait(false);
        }

        private IList<UserListViewModel> GetUsers()
        {
            IList<UserListViewModel> UsersList = new List<UserListViewModel>();
            UsersList = _userManager.Users.Select(u => new UserListViewModel
            {
                UserName = u.UserName,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email,
            }).ToList();

            return UsersList;
        }
        public async Task<AuthResponse> Remove(string ID)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(ID).ConfigureAwait(false);
            AuthResponse authResponse = new AuthResponse();
            if (user == null)
            {
                authResponse.StatusCode = "404";
                authResponse.Response = "User Couldn't be found";
                authResponse.Success = false;
                return authResponse;
            }
            else
            {
                //delete registered user
                IdentityResult result = await _userManager.DeleteAsync(user).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    authResponse.StatusCode = "200";
                    authResponse.Response = "OK";
                    authResponse.Success = true;
                    return authResponse;
                }
                else
                {
                    authResponse.StatusCode = "400";
                    authResponse.Response = result.Errors.ToString();
                    authResponse.Success = false;
                    return authResponse;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> UpdateAsync(ApplicationUser applicationUser)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                IdentityResult result = await _userManager.UpdateAsync(applicationUser).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    authResponse.StatusCode = Ok().StatusCode.ToString(_cultures);
                    authResponse.Response = "Ok";
                    authResponse.Success = true;
                    return Ok(authResponse);
                }
                else
                {
                    authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                    authResponse.Response = "Failed to Update User";
                    authResponse.Success = false;
                    return BadRequest(authResponse);
                }
            }
            catch (System.Exception Ex)
            {

                _logger.LogError("RegisterRepository.UpdateAsync \r\n", Ex);
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        public async Task<IActionResult> ListAsync2()
        {
            //var applicationUsers = (from users in _userManager.Users
            //                        select users);
            IList<ApplicationUser> applicationUsersList= await Task.Run(() => 
            (from users in _userManager.Users
                                  select users)
                                  .ToList()).ConfigureAwait(false);
            return Ok(applicationUsersList);
        }
    }
}
