using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using APRP.Web.ViewModels.UserViewModels;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class UserManagementController : Controller
    {
        private readonly ILogger _logger;
        private readonly IApplicationRolesService _applicationRolesService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        public UserManagementController(IApplicationRolesService applicationRolesService,
            IApplicationUsersService applicationUsersService, ILogger<UserManagementController> logger,
            IAuthorityService authorityService)
        {
            _applicationRolesService = applicationRolesService;
            _applicationUsersService = applicationUsersService;
            _logger = logger;
            _authorityService = authorityService;
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> UpdateUserRoles(string[] myArray, string id)
        {
            try
            {
                //Get Application User
                ApplicationUser ApplicationUser = null;
                var applicationUsersResponse = await _applicationUsersService.FindByIdAsync(id).ConfigureAwait(false);
                var objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    //var result = (OkObjectResult)objectResult.Value;
                    ApplicationUser = (ApplicationUser)objectResult.Value;
                }
                if (ApplicationUser == null)
                {
                    return Json(false);
                }

                //Get All Roles Names
                var applicationRolesListResponse = await _applicationRolesService.ListAsync().ConfigureAwait(false);
                var ApplicationRole = (IList<ApplicationRole>)applicationRolesListResponse.ApplicationRole;

                //Get User Roles Names
                IList<string> myRoles = null;
                applicationUsersResponse = await _applicationUsersService.GetRolesAsync(ApplicationUser).ConfigureAwait(false);
                objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    //var result = (OkObjectResult)objectResult.Value;
                    myRoles = (IList<string>)objectResult.Value;
                }

                if (myRoles != null)
                {
                    //check if my role is in my array,if not remove
                    bool isExisting = false;
                    foreach (string role in myRoles)
                    {
                        //Get role id
                        var applicationRolesResponse = await _applicationRolesService.FindByNameAsync(role).ConfigureAwait(false);
                        if (applicationRolesResponse.ApplicationRole != null)
                        {
                            //Check if existing role id matches new assigned role ids
                            isExisting = Array.Exists(myArray, element => element == applicationRolesResponse.ApplicationRole.Id);
                            if (!isExisting)
                            {
                                //Remove user from role
                                applicationUsersResponse = await _applicationUsersService.RemoveFromRoleAsync(ApplicationUser, role).ConfigureAwait(false);
                            }
                        }
                    }
                }

                //Return User Role Names Array for my role IDS
                IList<ApplicationRole> MyRoleNamesFromArray = new List<ApplicationRole>();

                foreach (string roleID in myArray)
                {
                    var applicationRolesResponse = await _applicationRolesService.FindByIdAsync(roleID).ConfigureAwait(false);
                    try
                    {
                        MyRoleNamesFromArray.Add(applicationRolesResponse.ApplicationRole);
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"UserManagementController.UpdateUserRoles API Error {Environment.NewLine}");
                    }
                }

                //Get user existing roles in Array and loop through array                
                bool b = false;
                if (myArray != null)
                {
                    foreach (ApplicationRole possibleNewRole in MyRoleNamesFromArray)
                    {
                        //Check my array for new roles and add
                        string result = myRoles.FirstOrDefault(item => item == possibleNewRole.Name);
                        if (result == null)
                        {
                            //Remove user from role
                            applicationUsersResponse = await _applicationUsersService.AddToRoleAsync(ApplicationUser, possibleNewRole).ConfigureAwait(false);
                        }
                    }
                }

                string referer = Request.Headers["Referer"].ToString();

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Referer = referer,
                    Href = "Users/UserRoles?" + ApplicationUser.Id
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetRoads API Error {Ex.Message}");
                return Json(false);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetMyRoles(string id)
        {
            try
            {
                //Get Application User
                ApplicationUser ApplicationUser = null;
                var applicationUsersResponse = await _applicationUsersService.FindByIdAsync(id).ConfigureAwait(false);
                var objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    //var result = (OkObjectResult)objectResult.Value;
                    ApplicationUser = (ApplicationUser)objectResult.Value;
                }
                if (ApplicationUser == null)
                {
                    return Json(false);
                }

                //Get All Roles
                var applicationRolesListResponse = await _applicationRolesService.ListAsync().ConfigureAwait(false);
                var ApplicationRole = (IList<ApplicationRole>)applicationRolesListResponse.ApplicationRole;

                //Get User Roles names
                IList<string> myRoles = null;
                applicationUsersResponse = await _applicationUsersService.GetRolesAsync(ApplicationUser).ConfigureAwait(false);
                objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    //var result = (OkObjectResult)objectResult.Value;
                    myRoles = (IList<string>)objectResult.Value;
                }

                //Return User Role IDS for my roles
                IList<string> MyRoleIDs = new List<string>();

                foreach (string roleName in myRoles)
                {
                    var applicationRolesResponse = await _applicationRolesService.FindByNameAsync(roleName).ConfigureAwait(false);

                    try
                    {
                        MyRoleIDs.Add(applicationRolesResponse.ApplicationRole.Id);
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"ARICSController.GetRoads API Error {Environment.NewLine}");
                    }
                }
                return Json(MyRoleIDs);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController.GetRoads API Error {Environment.NewLine}");
                return Json(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetShortName(string id)
        {
            try
            {
                DisplayUserViewModel DisplayUser = new DisplayUserViewModel();
                var userResp = await _applicationUsersService.FindByNameAsync(User.Identity.Name).ConfigureAwait(false);
                ApplicationUser user = null;
                if (userResp.Success)
                {
                    //user is found
                    var objectResult = (ObjectResult)userResp.IActionResult;
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        user = (ApplicationUser)result2.Value;

                        if (user != null && user.Authority == null)
                        {
                            var authorityResp = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                            user.Authority = authorityResp.Authority;
                        }
                        //string username
                        string username = user.UserName;
                        DisplayUser.SN = username.Substring(0, 2).ToUpper();
                        DisplayUser.FullName = username;
                        DisplayUser.AuthorityCode = user.Authority.Code;
                        DisplayUser.AuthorityName = user.Authority.Name;

                    }
                }
                return Json(DisplayUser);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UserManagementController.GetRoads API Error {Environment.NewLine}");
                return Json(null);
            }
        }
    }
}