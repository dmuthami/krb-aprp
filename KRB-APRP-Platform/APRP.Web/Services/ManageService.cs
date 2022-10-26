using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels.DTO;
using APRP.Web.ViewModels.ResponseTypes;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace APRP.Web.Services
{
    public class ManageService : ControllerBase, IManageService
    {
        private readonly IManageRepository _manageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly ICommunicationService _communicationService;

        private CultureInfo _cultures = new CultureInfo("en-US");

        public ManageService(IManageRepository manageRepository, IUnitOfWork unitOfWork
            ,ILogger<RegisterService> logger,
            IApplicationUsersService applicationUsersService,
            ICommunicationService communicationService)
        {
            _manageRepository = manageRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _applicationUsersService = applicationUsersService;
            _communicationService = communicationService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ManageResponse> AddPhone(UserDTO userDTO)
        {
            try
            {
                var iActionResult = await _manageRepository.AddPhone(userDTO).ConfigureAwait(false);                
                //await _unitOfWork.CompleteAsync();

                return new ManageResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ManageService.AddPhone Error: {Environment.NewLine}");
                return new ManageResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ManageResponse> AddPhone2(UserDTO userDTO)
        {
            try
            {
                AuthResponse authResponse = null;

                //get the user id of the user added into the system
                ApplicationUser applicationUser = null;
                var applicationUsersResponse = await _applicationUsersService.FindByIdAsync(userDTO.Id).ConfigureAwait(false);
                var objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    applicationUser = (ApplicationUser)result.Value;
                    
                }
                if (applicationUser == null)
                {
                    authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                    authResponse.Response = "Your User credentials couldn't be retrieved" +
                        " from the system.";
                    authResponse.Success = false;
                    return new ManageResponse(BadRequest(authResponse));
                }

                //get the phone number
                string _PhoneNumber = null;
                applicationUsersResponse = await _applicationUsersService.GetPhoneNumberAsync(applicationUser).ConfigureAwait(false);
                objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    _PhoneNumber = (string)result.Value;                    
                }
                if (_PhoneNumber == null)
                {
                    authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                    authResponse.Response = "User telephone number couldn't be retrieved" +
                        " from the system.";
                    authResponse.Success = false;
                    return new ManageResponse(BadRequest(authResponse));
                }


                //Generate phone number token
                string code = null;
                applicationUsersResponse = await _applicationUsersService.GenerateChangePhoneNumberTokenAsync(applicationUser, applicationUser.PhoneNumber).ConfigureAwait(false);
                objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    code = (string)result.Value;
                }
                if (code == null)
                {
                    authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                    authResponse.Response = "Unable to generate phone number token for the user" +
                        " from the system.";
                    authResponse.Success = false;
                    return new ManageResponse(BadRequest(authResponse));
                }

                //var iActionResult = await _manageRepository.AddPhone2(applicationUser, code);

                //Send SMS
                var communicationResponse = await _communicationService.SendSMS(applicationUser, "Your security code is: " + code).ConfigureAwait(false);
                objectResult = (ObjectResult)communicationResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    authResponse = (AuthResponse)result.Value;
                    authResponse.Success = true;
                    return new ManageResponse(Ok(authResponse)); //successful

                } else
                {
                    var result = (BadRequestObjectResult)objectResult;
                    authResponse = (AuthResponse)result.Value;
                    authResponse.Success = false;
                    return new ManageResponse(BadRequest(authResponse)); //successful

                }
                //await _unitOfWork.CompleteAsync();

                
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ManageService.AddPhone2 Error: {Environment.NewLine}");
                return new ManageResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        public async Task<ManageResponse> SendCode3(UserDTO userDTO)
        {
            AuthResponse authResponse = null;

            try
            {
                ApplicationUser applicationUser = null;
                var applicationUsersResponse = await _applicationUsersService.FindByIdAsync(userDTO.Id).ConfigureAwait(false);
                var objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    applicationUser = (ApplicationUser)result.Value;

                }
                if (applicationUser == null)
                {
                    authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                    authResponse.Response = "Your User credentials couldn't be retrieved" +
                        " from the system.";
                    authResponse.Success = false;
                    return new ManageResponse(BadRequest(authResponse));
                }

                //Send SMS
                var communicationResponse = await _communicationService.SendSMS(applicationUser, "Your security code is: " + userDTO.Code).ConfigureAwait(false);
                objectResult = (ObjectResult)communicationResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    authResponse = (AuthResponse)result.Value;
                    authResponse.Success = true;
                    return new ManageResponse(Ok(authResponse)); //successful
                }
                else
                {
                    var result = (BadRequestObjectResult)objectResult;
                    authResponse = (AuthResponse)result.Value;
                    authResponse.Success = false;
                    return new ManageResponse(BadRequest(authResponse)); //successful

                }
            }
            catch (Exception Ex)
            {

                _logger.LogError($"ManageRepository.SendCode3 Error{Ex.Message}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return new ManageResponse(BadRequest(authResponse));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ManageResponse> VerifyPhone(UserDTO userDTO)
        {
            try
            {
                var iActionResult = await _manageRepository.VerifyPhone(userDTO).ConfigureAwait(false);
                //await _unitOfWork.CompleteAsync();

                return new ManageResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ManageService.VerifyPhone Error: {Environment.NewLine}");
                return new ManageResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ManageResponse> VerifyPhone2(UserDTO userDTO)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                //get the user id of the user added into the system
                ApplicationUser applicationUser = null;
                var applicationUsersResponse = await _applicationUsersService.FindByIdAsync(userDTO.Id).ConfigureAwait(false);
                var objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    applicationUser = (ApplicationUser)result.Value;

                }
                if (applicationUser == null)
                {
                    authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                    authResponse.Response = "Your User credentials couldn't be retrieved" +
                        " from the system.";
                    authResponse.Success = false;
                    return new ManageResponse(BadRequest(authResponse));
                }

                //get the phone number
                string _PhoneNumber = null;
                applicationUsersResponse = await _applicationUsersService.GetPhoneNumberAsync(applicationUser).ConfigureAwait(false);
                objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    _PhoneNumber = (string)result.Value;
                }
                if (_PhoneNumber == null)
                {
                    authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                    authResponse.Response = "User telephone number couldn't be retrieved" +
                        " from the system.";
                    authResponse.Success = false;
                    return new ManageResponse(BadRequest(authResponse));
                }

                //verify code
                applicationUsersResponse = await _applicationUsersService.ChangePhoneNumberAsync(applicationUser, _PhoneNumber,userDTO.Code).ConfigureAwait(false);
                objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    authResponse = (AuthResponse)result.Value;
                    authResponse.Response = applicationUser.Id;
                    return new ManageResponse(Ok(authResponse));
                }
                else
                {
                    var result = (BadRequestObjectResult)objectResult;
                    authResponse = (AuthResponse)result.Value;
                    authResponse.Response = "Invalid verification code." +
                   "Reenter code received or try signing in again";
                    return new ManageResponse(BadRequest(authResponse));
                }


            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex,$"ManageRepository.VerifyPhone Error{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return new ManageResponse( BadRequest(authResponse));
            }
        }
    }
}
