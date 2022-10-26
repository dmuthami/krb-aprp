using System.Security.Claims;
using APRP.Web.Claims;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Controllers
{
    public class PermissionsController : Controller
    {

        private readonly IApplicationRolesService _applicationRolesService;
        private readonly ILogger _logger;
        private readonly IApplicationUsersService _applicationUsersService;
        public PermissionsController(IApplicationRolesService applicationRolesService,
             ILogger<PermissionsController> logger, IApplicationUsersService applicationUsersService)
        {
            _applicationRolesService = applicationRolesService;
            _logger = logger;
            _applicationUsersService = applicationUsersService;
        }

        #region Role/Group Claims
        [Authorize(Claims.Permission.Administrator.Role_View)]
        public async Task<IActionResult> ListRoleClaims()
        {
            var rolesResponse = await _applicationRolesService.ListRoleClaimsAsync().ConfigureAwait(false);
            return View(rolesResponse.ApplicationRole);
        }

        [Authorize(Claims.Permission.Administrator.Role_View)]
        public async Task<IActionResult> ListGroupUsers(string id)
        {
            ApplicationUserViewModel applicationUserViewModel = new ApplicationUserViewModel();
            applicationUserViewModel.applicationUsers = (Enumerable.Empty<ApplicationUser>()).ToList();
            //Get role id

            var role = await _applicationRolesService.FindByIdAsync(id).ConfigureAwait(false);
            applicationUserViewModel.ApplicationRole = role.ApplicationRole;
            if (role != null)
            {
                var rolesResponse = await _applicationUsersService.GetUsersInRoleAsync(applicationUserViewModel.ApplicationRole.Name).ConfigureAwait(false);
                var objectResult = (ObjectResult)rolesResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    applicationUserViewModel.applicationUsers = (IList<ApplicationUser>)result.Value;
                }
            }

            return View(applicationUserViewModel);
        }

        [Authorize(Claims.Permission.Administrator.Role_Add), Authorize(Claims.Permission.Administrator.Role_Change)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditRoleClaim(string id)
        {
            try
            {
                if (id == null)
                {
                    ApplicationRole applicationRole = new ApplicationRole();
                    return View(applicationRole);
                }
                else
                {
                    var resp = await _applicationRolesService.FindByIdAsync(id).ConfigureAwait(false);
                    var applicationRole = resp.ApplicationRole;
                    if (applicationRole == null)
                    {
                        return NotFound();
                    }
                    return View(applicationRole);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PermissionsController.AddRoadtoGIS Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "PermissionsController.AddEditRoleClaims Page has reloaded");
                return View();
            }
        }

        // POST: Permissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.Administrator.Role_Add), Authorize(Claims.Permission.Administrator.Role_Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditRoleClaim(string id, [Bind("Id,Description" +
            ",Name")] ApplicationRole applicationRole)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != applicationRole.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var resp = await _applicationRolesService.FindByIdAsync(id).ConfigureAwait(false);
                    if (resp.Success)
                    {
                        //Update
                        if (resp.ApplicationRole.Name.ToLower() == "Administrators".ToLower())//ADMINISTRATORS
                        {
                            success = false;
                            msg = "Administrators Group cannot be edited";
                        }
                        else
                        {
                            var resp2 = await _applicationRolesService.FindByName2Async(applicationRole.Name).ConfigureAwait(false);
                            if (resp2.Success)
                            {
                                IEnumerable<ApplicationRole> applicationRoles = resp2.ApplicationRole.Where(s => s.Id != applicationRole.Id);
                                if (applicationRoles.Count() < 1)
                                {
                                    //Edit or update
                                    resp.ApplicationRole.Name = applicationRole.Name;
                                    resp.ApplicationRole.Description = applicationRole.Description;
                                    var applicationRoleResp = await _applicationRolesService.
                                        Update(resp.ApplicationRole.Id, resp.ApplicationRole).ConfigureAwait(false);
                                    if (applicationRoleResp.Success)
                                    {
                                        success = true;
                                        msg = "Role/Group Successfully Updated";
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //Add
                        if (!await _applicationRolesService.RoleExistsAsync(applicationRole.Name).ConfigureAwait(false))
                        {
                            applicationRole.RoleType = 1;
                            var trainingResp = await _applicationRolesService.AddAsync(applicationRole).ConfigureAwait(false);
                            if (trainingResp.Success)
                            {
                                success = true;
                                msg = "Role/Group Successfully Added";
                            }
                        }
                    }
                }
                else
                { //Model Error                    
                    success = false;
                    msg = $"Model Error: {ModelState.IsValid}";
                }

                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("ListRoleClaims", "Permissions")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PermissionsController Add/Edit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "PermissionsController Add/Edit Page has reloaded");
                return View(applicationRole);
            }
        }


        // GET: Permissions/Details/5
        [Authorize(Claims.Permission.Administrator.Role_View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> RoleClaimDetails(string id)
        {
            try
            {

                if (id == null)
                {
                    return NotFound();
                }

                var resp = await _applicationRolesService.FindByIdAsync(id).ConfigureAwait(false);
                var applicationRole = resp.ApplicationRole;
                if (applicationRole == null)
                {
                    return NotFound();
                }

                ApplicationRoleViewModel applicationRoleViewModel = new ApplicationRoleViewModel();
                applicationRoleViewModel.ApplicationRole = applicationRole;

                //Get all claims for the particular role
                var authenticateResponse = await _applicationRolesService.GetClaimsAsync(applicationRoleViewModel.ApplicationRole).ConfigureAwait(false);
                var objectResult = (ObjectResult)authenticateResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    applicationRoleViewModel.RoleClaims = (IList<System.Security.Claims.Claim>)result.Value;
                }

                //Get All Permissions
                applicationRoleViewModel.AllPermissions = (IList<string>)AllPermissions.GlobalPermissions.Values.ToList();

                //Return
                return View(applicationRoleViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PermissionsController.Details Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "PermissionsController.Details Page has reloaded");
                return View();
            }
        }

        // GET: Permissions/Delete/5
        [Authorize(Claims.Permission.Administrator.Role_View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> RoleClaimDelete(string id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var resp = await _applicationRolesService.FindByIdAsync(id).ConfigureAwait(false);
                var applicationRole = resp.ApplicationRole;
                if (applicationRole == null)
                {
                    return NotFound();
                }
                return View(applicationRole);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PermissionsController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "PermissionsController.Delete Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.Administrator.Role_Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> RoleClaimDelete(string Id, [Bind("Id")] ApplicationRole applicationRole)
        {
            try
            {
                bool success = false;
                string msg = "Delete Failed";
                if (Id != applicationRole.Id)
                {
                    return NotFound();
                }
                var resp = await _applicationRolesService.FindByIdAsync(Id).ConfigureAwait(false);
                if (resp.Success)
                {
                    if (resp.ApplicationRole.Name.ToLower() == "Administrators".ToLower())//ADMINISTRATORS
                    {
                        success = false;
                        msg = "Administrators Group cannot be edited";
                    }
                }
                else
                {
                    resp = await _applicationRolesService.RemoveAsync(Id).ConfigureAwait(false);
                    if (resp.Success)
                    {
                        success = true;
                        msg = "Role/Group Deleted";
                    }

                }
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("ListRoleClaims", "Permissions")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PermissionsController Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "PermissionsController Delete Page has reloaded");
                return View(applicationRole);
            }
        }

        [Authorize(Claims.Permission.Administrator.Role_Add)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddPermissionFromRole(string Claim, string RoleId)
        {
            try
            {
                if (Claim == null)
                {
                    return NotFound();
                }

                PermissionClaim permissionClaim = new PermissionClaim();
                permissionClaim.Claim = Claim;
                permissionClaim.RoleId = RoleId;

                return View(permissionClaim);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PermissionsController.AddRoadtoGIS Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "PermissionsController.AddEditRoleClaims Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.Administrator.Role_Add)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddPermissionFromRole(string RoleId, [Bind("Claim,RoleId" +
            "")] PermissionClaim permissionClaim)
        {
            try
            {
                bool success = false;
                string msg = null;


                if (ModelState.IsValid)
                {

                    var resp = await _applicationRolesService.FindByIdAsync(RoleId).ConfigureAwait(false);
                    if (resp.Success)
                    {
                        ApplicationRole applicationRole = resp.ApplicationRole;

                        var resp2 = await _applicationRolesService.GetClaimsAsync(applicationRole).ConfigureAwait(false);
                        var objectResult = (ObjectResult)resp2.IActionResult;

                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var claimsList = (IList<System.Security.Claims.Claim>)result.Value;

                            var claim = claimsList.Where(s => s.Value == permissionClaim.Claim).FirstOrDefault();
                            if (claim == null)
                            {
                                if (applicationRole.Name != "Administrators")
                                {
                                    var respAddRole = await _applicationRolesService
                                        .AddClaimAsync(applicationRole, new Claim(CustomClaimTypes.Permission, permissionClaim.Claim)).ConfigureAwait(false);
                                    success = true;
                                    msg = "Role/Group Successfully Updated";
                                }
                                else
                                {
                                    success = false;
                                    msg = "Administrators role cannot be modified";
                                }

                            }
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("RoleClaimDetails", "Permissions", new { id = RoleId })
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("RoleClaimDetails", "Permissions", new { id = RoleId })
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PermissionsController Add/Edit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "PermissionsController Add/Edit Page has reloaded");
                return View(permissionClaim);
            }
        }


        [Authorize(Claims.Permission.Administrator.Role_Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> RemovePermissionFromRole(string Claim, string RoleId)
        {
            try
            {
                if (Claim == null)
                {
                    return NotFound();
                }

                PermissionClaim permissionClaim = new PermissionClaim();
                permissionClaim.Claim = Claim;
                permissionClaim.RoleId = RoleId;

                return View(permissionClaim);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PermissionsController.AddRoadtoGIS Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "PermissionsController.AddEditRoleClaims Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.Administrator.Role_Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> RemovePermissionFromRole(string RoleId, [Bind("Claim,RoleId" +
            "")] PermissionClaim permissionClaim)
        {
            try
            {
                bool success = false;
                string msg = null;


                if (ModelState.IsValid)
                {

                    var resp = await _applicationRolesService.FindByIdAsync(RoleId).ConfigureAwait(false);
                    if (resp.Success)
                    {
                        ApplicationRole applicationRole = resp.ApplicationRole;
                        if (applicationRole.Name.ToLower() != "Administrators".ToLower())
                        {
                            var resp2 = await _applicationRolesService.
                            RemoveClaimAsync(applicationRole, new Claim(CustomClaimTypes.Permission, permissionClaim.Claim)).ConfigureAwait(false);
                            var objectResult = (ObjectResult)resp2.IActionResult;

                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result = (OkObjectResult)objectResult;
                                var identityResult = (IdentityResult)result.Value;
                                if (identityResult.Succeeded)
                                {
                                    success = true;
                                    msg = "Role/Group Successfully Removed";
                                }
                            }
                        }
                        else
                        {
                            success = false;
                            msg = "Administrators role cannot be modified";
                        }

                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("RoleClaimDetails", "Permissions", new { id = RoleId })
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("RoleClaimDetails", "Permissions", new { id = RoleId })
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PermissionsController Remove Permission Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "PermissionsController Remove Permission Page has reloaded");
                return View(permissionClaim);
            }
        }

        #endregion

        #region Add/Remove User from Group
        [Authorize(Claims.Permission.Administrator.Role_View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListUserGroups(string userid)
        {
            if (userid == null)
            {
                return NotFound();
            }
            ApplicationUserViewModel applicationUserViewModel = new ApplicationUserViewModel();

            //All Groups
            var rolesResponse = await _applicationRolesService.ListRoleClaimsAsync().ConfigureAwait(false);
            applicationUserViewModel.ApplicationRoles = rolesResponse.ApplicationRole;

            //Get My Groups
            var resp = await _applicationUsersService.FindByIdAsync(userid).ConfigureAwait(false);
            if (resp.Success)
            {
                //
                var objectResult = (ObjectResult)resp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    applicationUserViewModel.ApplicationUser = (ApplicationUser)result.Value;

                    //Get my roles
                    var resp2 = await _applicationUsersService.GetRolesAsync(applicationUserViewModel.ApplicationUser).ConfigureAwait(false);
                    var objectResult2 = (ObjectResult)resp2.IActionResult;
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult2;
                        IList<string> roleNames = null; IList<ApplicationRole> applicationRoles = new List<ApplicationRole>();
                        roleNames = (IList<string>)result2.Value;

                        //loop through the role names and return an array of application roles
                        foreach (string roleName in roleNames)
                        {
                            var respRole = await _applicationRolesService.FindByNameAsync(roleName).ConfigureAwait(false);
                            try
                            {
                                if (respRole.ApplicationRole.RoleType == 1)
                                {//Add Roles Created dynamicall. The rest are treated as permissions
                                    applicationRoles.Add(respRole.ApplicationRole);
                                }

                            }
                            catch (Exception Ex)
                            {

                                _logger.LogError(Ex, $"PermissionsController.ListUserGroups Page Error: {Ex.Message} " +
                                $"{Environment.NewLine}");
                            }
                        }
                        applicationUserViewModel.MyGroups = applicationRoles;
                    }
                }
            }


            return View(applicationUserViewModel);
        }

        [Authorize(Claims.Permission.Administrator.Role_Add), Authorize(Claims.Permission.Administrator.Role_Change)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddRemoveUserToGroup(string RoleId, string UserId, bool Add)
        {
            try
            {
                ApplicationUserViewModel applicationUserViewModel = new ApplicationUserViewModel();
                if (RoleId == null || UserId == null)
                {
                    return NotFound();
                }


                //Get role
                var respRole = await _applicationRolesService.FindByIdAsync(RoleId).ConfigureAwait(false);
                applicationUserViewModel.ApplicationRole = respRole.ApplicationRole;

                //Get User
                var respUser = await _applicationUsersService.FindByIdAsync(UserId).ConfigureAwait(false);
                var objectResult = (ObjectResult)respUser.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    applicationUserViewModel.ApplicationUser = (ApplicationUser)result.Value;
                }

                applicationUserViewModel.Add = Add;
                return View(applicationUserViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PermissionsController.AddRemoveUserToGroup Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "PermissionsController.AddRemoveUserToGroup Page has reloaded");
                return View();
            }
        }

        // POST: Permissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.Administrator.Role_Add), Authorize(Claims.Permission.Administrator.Role_Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddRemoveUserToGroup([Bind("UserId,RoleId" +
            ",Add")] ApplicationUserViewModel applicationUserViewModel)
        {
            try
            {
                bool success = false;
                string msg = null;

                //Get role
                var respRole = await _applicationRolesService.FindByIdAsync(applicationUserViewModel.RoleId).ConfigureAwait(false);
                applicationUserViewModel.ApplicationRole = respRole.ApplicationRole;

                //Get User
                var respUser = await _applicationUsersService.FindByIdAsync(applicationUserViewModel.UserId).ConfigureAwait(false);
                var objectResult = (ObjectResult)respUser.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    applicationUserViewModel.ApplicationUser = (ApplicationUser)result.Value;
                }

                if (applicationUserViewModel.Add)
                {
                    if (applicationUserViewModel.ApplicationUser.AuthorityId != 8
                        && applicationUserViewModel.ApplicationRole.Name.ToLower() == "Administrators".ToLower())//KRB=Authority ID=8
                    {
                        success = false;
                        msg = "Only members within KRB can be added to the Administrator Group";
                    }
                    else
                    {
                        var resp = await _applicationUsersService.
                            AddToRoleAsync(applicationUserViewModel.ApplicationUser, applicationUserViewModel.ApplicationRole).ConfigureAwait(false);
                        success = true;
                        msg = $"{applicationUserViewModel.ApplicationUser.UserName} added to the group named {applicationUserViewModel.ApplicationRole.Name} successfuly";
                    }

                }
                else//Remove
                {
                    var resp = await _applicationUsersService.
                        RemoveFromRoleAsync(applicationUserViewModel.ApplicationUser, applicationUserViewModel.ApplicationRole.Name).ConfigureAwait(false);
                    success = true;
                    msg = $"{applicationUserViewModel.ApplicationUser.UserName} removed from the group named {applicationUserViewModel.ApplicationRole.Name} successfuly";
                }

                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("ListUserGroups", "Permissions", new { userid = applicationUserViewModel.UserId })
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PermissionsController AddRemoveUserToGroup Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "PermissionsController AddRemoveUserToGroup Page has reloaded");
                return View(applicationUserViewModel);
            }
        }
    }
}
#endregion

