using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using System.Security.Claims;

namespace APRP.Web.Services
{
    public class ApplicationRolesService : IApplicationRolesService
    {
        private readonly IApplicationRolesRepository _applicationRolesRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public ApplicationRolesService(IApplicationRolesRepository applicationRolesRepository,
            IUnitOfWork unitOfWork,ILogger<ApplicationRolesService> logger)
        {
            _applicationRolesRepository = applicationRolesRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationRolesResponse> AddAsync(ApplicationRole applicationRole)
        {
            try
            {
                await _applicationRolesRepository.AddAsync(applicationRole).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ApplicationRolesResponse(applicationRole); //success
            }
            catch (Exception Ex)
            {
                //exception logging
                _logger.LogError(Ex,$"ApplicationRolesService.AddAsync : {Environment.NewLine}");
                return new ApplicationRolesResponse($"Error occured while saving the application role record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> AddClaimAsync(ApplicationRole applicationRole, Claim claim)
        {
            try
            {
                var iActionResult = await _applicationRolesRepository.AddClaimAsync(applicationRole,claim).ConfigureAwait(false);
                //await _unitOfWork.CompleteAsync();

                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsers.AddToRoleAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationRolesResponse> FindByIdAsync(string ID)
        {
            try
            {
                var existingApplicationrole = await _applicationRolesRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingApplicationrole == null)
                {
                    return new ApplicationRolesResponse("Record Not Found");
                }
                else
                {
                    return new ApplicationRolesResponse(existingApplicationrole);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationRoleService.FindByIdAsync Error: {Environment.NewLine}");
                return new ApplicationRolesResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        public async Task<ApplicationRolesListResponse> FindByName2Async(string roleName)
        {
            try
            {
                var ApplicationroleList = await _applicationRolesRepository.FindByName2Async(roleName).ConfigureAwait(false);
                if (ApplicationroleList == null)
                {
                    return new ApplicationRolesListResponse("Record Not Found");
                }
                else
                {
                    return new ApplicationRolesListResponse(ApplicationroleList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationRoleService.FindByNameAsync Error: {Environment.NewLine}");
                return new ApplicationRolesListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationRolesResponse> FindByNameAsync(string roleName)
        {
            try
            {
                var existingApplicationrole = await _applicationRolesRepository.FindByNameAsync(roleName).ConfigureAwait(false);
                if (existingApplicationrole == null)
                {
                    return new ApplicationRolesResponse("Record Not Found");
                }
                else
                {
                    return new ApplicationRolesResponse(existingApplicationrole);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationRoleService.FindByNameAsync Error: {Environment.NewLine}");
                return new ApplicationRolesResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> GetClaimsAsync(ApplicationRole applicationRole)
        {
            try
            {
                var iActionResult = await _applicationRolesRepository.GetClaimsAsync(applicationRole).ConfigureAwait(false);
                //await _unitOfWork.CompleteAsync();

                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsers.AddToRoleAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationRolesListResponse> ListAsync()
        {
            try
            {
                var existingRoles= await _applicationRolesRepository.ListAsync().ConfigureAwait(false);
                if (existingRoles == null)
                {
                    return new ApplicationRolesListResponse("Records Not Found");
                }
                else
                {
                    return new ApplicationRolesListResponse(existingRoles);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationRoleService.ListByIdAsync Error: {Environment.NewLine}");
                return new ApplicationRolesListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<ApplicationRolesListResponse> ListDefaultRolesAsync()
        {
            try
            {
                var existingRoles = await _applicationRolesRepository.ListDefaultRolesAsync().ConfigureAwait(false);
                if (existingRoles == null)
                {
                    return new ApplicationRolesListResponse("Records Not Found");
                }
                else
                {
                    return new ApplicationRolesListResponse(existingRoles);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationRoleService.ListByIdAsync Error: {Environment.NewLine}");
                return new ApplicationRolesListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationRolesListResponse> ListRoleClaimsAsync()
        {
            try
            {
                var existingRoles = await _applicationRolesRepository.ListRoleClaimsAsync().ConfigureAwait(false);
                if (existingRoles == null)
                {
                    return new ApplicationRolesListResponse("Records Not Found");
                }
                else
                {
                    return new ApplicationRolesListResponse(existingRoles);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationRoleService.ListByIdAsync Error: {Environment.NewLine}");
                return new ApplicationRolesListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationRolesResponse> RemoveAsync(string ID)
        {
            var existingApplicationRole = await _applicationRolesRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingApplicationRole == null)
            {
                return new ApplicationRolesResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _applicationRolesRepository.Remove(existingApplicationRole);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new ApplicationRolesResponse(existingApplicationRole);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ApplicationRoleService.RemoveAsync Error: {Environment.NewLine}");
                    return new ApplicationRolesResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> RemoveClaimAsync(ApplicationRole applicationRole, Claim claim)
        {
            try
            {
                var iActionResult = await _applicationRolesRepository.RemoveClaimAsync(applicationRole, claim).ConfigureAwait(false);

                return new GenericResponse(iActionResult); //OK/or BadRequesk plus object
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationRoles.RemoveClaimAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<bool> RoleExistsAsync(string roleName)
        {
            try
            {
                return await _applicationRolesRepository.RoleExistsAsync(roleName).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationRoleService.FindByNameAsync Error: {Environment.NewLine}");
                return false;

            }
            throw new NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationRolesResponse> Update(string ID,ApplicationRole applicationRole)
        {
            var existingApplicationRole = await _applicationRolesRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingApplicationRole == null)
            {
                return new ApplicationRolesResponse("Record Not Found");
            }
            else
            {
                existingApplicationRole.Name = applicationRole.Name;
                try
                {
                    _applicationRolesRepository.Update(existingApplicationRole);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new ApplicationRolesResponse(existingApplicationRole);
                }
                catch (Exception Ex)
                {
                    //exception logging
                    return new ApplicationRolesResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

    }
}
