using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels;
using APRP.Web.ViewModels.ResponseTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace APRP.Web.Services
{
    public class RegisterService : ControllerBase, IRegisterService
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IApplicationUsersService _applicationUsersService;

        private CultureInfo _cultures = new CultureInfo("en-US");

        public RegisterService(IRegisterRepository registerRepository, IUnitOfWork unitOfWork
            ,ILogger<RegisterService> logger,
             IApplicationUsersService applicationUsersService)
        {
            _registerRepository = registerRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _applicationUsersService = applicationUsersService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RegisterResponse> AddAsync(Register register)
        {
            try
            {
                AuthResponse authResponse = await _registerRepository.AddAsync(register).ConfigureAwait(false);
                //await _unitOfWork.CompleteAsync();

                return new RegisterResponse(authResponse); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RegisterService.AddAsync Error: {Environment.NewLine}");
                return new RegisterResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RegisterResponse> AddAsync2(Register register)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {

                //check if the user exists
                ApplicationUser applicationUser = null;
                var applicationUsersResponse = await _applicationUsersService.FindByNameAsync(register.Username).ConfigureAwait(false);
                var objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("404", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (NotFoundObjectResult)objectResult;
                    applicationUser = null;

                }
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    applicationUser = (ApplicationUser)result2.Value;

                }
                if (applicationUser != null)
                {
                    authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                    authResponse.Response = "The supplied UserName exists in the system." +
                        " Please contact the system administrator";
                    authResponse.Success = false;
                    return new RegisterResponse(BadRequest(authResponse));
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
                    PhoneNumber = register.PhoneNumberFormatted,
                    AuthorityId = register.AuthorityID
                };

                //var result = await _userManager.CreateAsync(user, register.Password);
                applicationUsersResponse = await _applicationUsersService.CreateAsync(user, register.Password).ConfigureAwait(false);
                objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    //enable two factor authentication
                    //await _userManager.SetTwoFactorEnabledAsync(user, true);//enable twofactor authentication

                    applicationUsersResponse = await _applicationUsersService.SetTwoFactorEnabledAsync(user, true).ConfigureAwait(false);
                    objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        authResponse = (AuthResponse)result2.Value;
                        authResponse.Success = true;
                        authResponse.ApplicationUser = user;
                        return new RegisterResponse(Ok(authResponse));
                    }
                    else
                    {
                        var result2 = (BadRequestObjectResult)objectResult;
                        IdentityResult result = (IdentityResult)result2.Value;
                        authResponse.result = result;
                        authResponse.Success = false;
                        authResponse.ApplicationUser = user;
                        return new RegisterResponse(BadRequest(authResponse));
                    }
                }
                else
                {
                    var result2 = (BadRequestObjectResult)objectResult;
                    IdentityResult result = (IdentityResult)result2.Value;
                    authResponse.result = result;
                    authResponse.Success = false;
                    authResponse.ApplicationUser = user;
                    return new RegisterResponse(BadRequest(authResponse));
                }
            }
            catch (Exception Ex)
            {

                authResponse.Success = false;
                _logger.LogError(Ex, $"RegisterService.AddAsync2 Error: {Environment.NewLine}");
                return new RegisterResponse(BadRequest(authResponse));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RegisterListResponse> ListAsync()
        {
            try
            {
                var existingRegister = await _registerRepository.ListAsync().ConfigureAwait(false);
                if (existingRegister == null)
                {
                    return new RegisterListResponse("Records Not Found");
                }
                else
                {
                    return new RegisterListResponse(existingRegister);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RegisterService.ListAsync Error: {Environment.NewLine}");
                return new RegisterListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RegisterResponse> ListAsync2()
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                var iActionResult = await _registerRepository.ListAsync2().ConfigureAwait(false);
                return new RegisterResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RegisterService.ListAsync2 Error: {Environment.NewLine}");
                return new RegisterResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RegisterResponse> RemoveAsync(string ID)
        {
            try
            {
                var authResponse = await _registerRepository.Remove(ID).ConfigureAwait(false);
                if (authResponse.Success == false)
                {
                    return new RegisterResponse("Record Not Found");
                }
                else
                {
                    return new RegisterResponse(authResponse);
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"RegisterService.RemoveAsync Error: {Environment.NewLine}");
                return new RegisterResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RegisterResponse> UpdateAsync(ApplicationUser applicationUser)
        {            
            try
            {
                var iActionResult = await _registerRepository.UpdateAsync(applicationUser).ConfigureAwait(false);
                return new RegisterResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RegisterService.RemoveAsync Error: {Environment.NewLine}");
                return new RegisterResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
