using System.IdentityModel.Tokens.Jwt;
using System.Text;
using APRP.Web.Domain.Models;
using APRP.Web.ViewModels;
using APRP.Web.ViewModels.ResponseTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace UserManagementV1._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private IConfiguration Configuration { get; }

        public AuthController(UserManager<ApplicationUser> userManager
            , IConfiguration configuration
            , ILogger<AuthController> logger)
        {
            _userManager = userManager;
            Configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
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
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                        userid = user.Id,
                        name = user.Surname+" "+user.OtherNames,                      
                        Updated_Bio = user.UpdatedBio.ToString()
                    });
                }
                else
                {
                    AuthResponse _authResponse = new AuthResponse();
                    _authResponse.StatusCode = BadRequest().StatusCode.ToString();
                    _authResponse.Response = "Invalid Username or Password";
                    return BadRequest(_authResponse);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError("Auth Error", Ex);
                AuthResponse _authResponse = new AuthResponse();
                _authResponse.StatusCode = BadRequest().StatusCode.ToString();
                _authResponse.Response = Ex.Message;
                _authResponse.InnerException = Ex.InnerException.ToString();
                _authResponse.StackTrace = Ex.StackTrace.ToString();
                return Unauthorized(_authResponse);
            }

        }

    }
}