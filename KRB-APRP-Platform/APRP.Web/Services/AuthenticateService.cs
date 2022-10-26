using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels;
using APRP.Web.ViewModels.ResponseTypes;
using APRP.Web.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace APRP.Web.Services
{
    public class AuthenticateService : ControllerBase, IAuthenticateService
    {
        private readonly IAuthenticateRepository _authenticateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IApplicationUsersService _applicationUsersService;
        private IConfiguration Configuration { get; }
        private CultureInfo _cultures = new CultureInfo("en-US");

        public AuthenticateService(IAuthenticateRepository authenticateRepository, IUnitOfWork unitOfWork
            , ILogger<RegisterService> logger,
             IApplicationUsersService applicationUsersService,
              IConfiguration configuration)
        {
            _authenticateRepository = authenticateRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _applicationUsersService = applicationUsersService;
            Configuration = configuration;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AuthenticateResponse> LoginAsync2(LoginModel loginModel)
        {
            AuthResponse _authResponse = new AuthResponse();
            try
            {
                //Check if user exists
                ApplicationUser user = null;
                var applicationUsersResponse = await _applicationUsersService.FindByNameAsync(loginModel.Username).ConfigureAwait(false);
                var objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("404", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (NotFoundObjectResult)objectResult;
                    user = null;

                }
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    user = (ApplicationUser)result2.Value;

                }

                //check if password is valid
                AuthResponse authResponse = new AuthResponse();
                applicationUsersResponse = await _applicationUsersService.CheckPasswordAsync(user, loginModel.Password).ConfigureAwait(false);
                objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("400", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (BadRequestObjectResult)objectResult;
                    authResponse = (AuthResponse)result2.Value;

                }
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    authResponse = (AuthResponse)result2.Value;


                }

                if (user != null && authResponse.Success)
                {
                    //create token
                    var claims = new[]
                        {
                            new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                            new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                        };

                    var signingkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));

                    var token = new JwtSecurityToken(
                        issuer: Configuration["Jwt:Issuer"],
                        audience: Configuration["Jwt:Audience"],
                        expires: DateTime.UtcNow.AddHours(2),
                        claims: claims,
                        signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(signingkey, SecurityAlgorithms.HmacSha256)
                        );

                    //return token
                    LoginToken loginToken = new LoginToken();
                    loginToken.token = new JwtSecurityTokenHandler().WriteToken(token);
                    loginToken.expiration = token.ValidTo.ToString(_cultures);
                    loginToken.userid = user.Id;
                    loginToken.name = user.Surname + " " + user.OtherNames;
                    loginToken.UpdatedBio = user.UpdatedBio.ToString(_cultures);

                    _authResponse.Response = "Success";
                    _authResponse.StatusCode = Ok().StatusCode.ToString(_cultures);
                    _authResponse.LoginToken = loginToken;
                    return new AuthenticateResponse(Ok(_authResponse));
                }
                else
                {

                    _authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                    _authResponse.Response = "Invalid Username or Password";
                    return new AuthenticateResponse(BadRequest(_authResponse));
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AuthenticateService.LoginAsync2 Error: {Environment.NewLine}");
                _authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                _authResponse.Response = Ex.Message;
                _authResponse.InnerException = Ex.InnerException.ToString();
                _authResponse.StackTrace = Ex.StackTrace.ToString(_cultures);
                return new AuthenticateResponse(Unauthorized(_authResponse));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AuthenticateResponse> CheckToken(CheckToken checkToken)
        {
            IActionResult _iActionResult = null;
            try
            {
                await Task.Run(() =>
                {
                    _iActionResult = CheckResult(checkToken);
                }).ConfigureAwait(false);

                return new AuthenticateResponse(_iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AuthenticateService.Checktoken Error: {Environment.NewLine}");
                return new AuthenticateResponse(_iActionResult);
            }
        }

        private IActionResult CheckResult(CheckToken checkToken)
        {
            string token = checkToken.Token;
            //check if null
            if (token ==null|| token.Length<1)
            {
                return BadRequest(checkToken);
            }
            var tk = new JwtSecurityTokenHandler().ReadToken(token);
            DateTime _now = DateTime.UtcNow;
            if (tk.ValidTo < _now)
            {
                checkToken.IsValid = false;
                return BadRequest(checkToken);
            }
            else
            {
                checkToken.IsValid = true;
                return Ok(checkToken);
            }

        }
    }
}
