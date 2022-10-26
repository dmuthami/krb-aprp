using APRP.Web.ViewModels.Account;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels.ResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Services
{
    public class IMService : ControllerBase, IIMService
    {
        private readonly IIMRepository _iMRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IApplicationUsersService _applicationUsersService;


        public IMService(IIMRepository iMRepository, IUnitOfWork unitOfWork
            ,ILogger<IMService> logger,
            IApplicationUsersService applicationUsersService)
        {
            _iMRepository = iMRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _applicationUsersService = applicationUsersService;
        }



        public async Task<IMResponse> ForgotPassword2(InputModel inputModel)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                ApplicationUser user = null;
                var iActionResult = await _iMRepository.ForgotPassword2(inputModel).ConfigureAwait(false);

                var objectResult = (ObjectResult)iActionResult;
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    authResponse = (AuthResponse)result.Value;
                    user = authResponse.ApplicationUser;
                } else
                {
                    var result = (NotFoundObjectResult)objectResult;
                    authResponse = (AuthResponse)result.Value;
                    return new IMResponse(BadRequest(authResponse));
                }
                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713

                var applicationUsersResponse = await _applicationUsersService.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
                objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    authResponse = (AuthResponse)result2.Value;
                    authResponse.ApplicationUser = user;
                    string msg = "IMRepository.ForgotPassword : \r\n" +
                    $" Password token has been successfully generated";
                    _logger.LogInformation(msg);
                    return new IMResponse (Ok(authResponse));
                }  else
                {
                    var result2 = (BadRequestObjectResult)objectResult;
                    authResponse = (AuthResponse)result2.Value;
                    string msg = "IMRepository.ForgotPassword : \r\n" +
                                 $" Password token has failed";
                    _logger.LogInformation(msg);
                    return new IMResponse(BadRequest(authResponse));
                }

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"IMService.ForgotPassword2 Error: {Environment.NewLine}");
                return new IMResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }



        public async Task<IMResponse> ResetPassword2(ResetModel resetModel)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                ApplicationUser user = null;
                var applicationUsersResponse = await _applicationUsersService.FindByIdAsync(resetModel.Id).ConfigureAwait(false);
                var objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("404"))
                {
                    var result2 = (NotFoundObjectResult)objectResult;
                    user = null;

                }
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    user = (ApplicationUser)result2.Value;

                }
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    authResponse.StatusCode = Ok().StatusCode.ToString();
                    authResponse.Response = "User cannot be found";
                    string msg = "IMRepository.ResetPassword : \r\n" +
                    $" {authResponse.Response}";
                    _logger.LogError($"IMService.ForgotPassword2 Error: {msg} {Environment.NewLine}");
                    return new IMResponse( BadRequest(authResponse));
                }

                //Reset Password
                applicationUsersResponse = await _applicationUsersService.ResetPasswordAsync(user, resetModel.Code, resetModel.Password).ConfigureAwait(false);
                objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    authResponse = (AuthResponse)result2.Value;
                    authResponse.Response = "Password reset was successful";
                    string msg = "IMRepository.ResetPassword : \r\n" +
                       $" Password reset was successful";
                    _logger.LogInformation(msg);
                    return new IMResponse(Ok(authResponse));
                } else
                {
                    var result2 = (BadRequestObjectResult)objectResult;
                    authResponse = (AuthResponse)result2.Value;                  
                    authResponse.Response = authResponse.result.Errors.ToString();
                    string msg = "IMRepository.ResetPassword : \r\n" +
                        $" {authResponse.Response}";
                    _logger.LogError(msg);
                    return new IMResponse( BadRequest(authResponse));
                }

            }
            catch (Exception Ex)
            {
                authResponse.StatusCode = BadRequest().StatusCode.ToString();
                authResponse.Response = Ex.Message;
                string msg = "IMRepository.ResetPassword : \r\n" +
                    $" {Ex.Message}";
                _logger.LogError(msg);
                return new IMResponse(BadRequest(authResponse));
            }
        }



    }
}
