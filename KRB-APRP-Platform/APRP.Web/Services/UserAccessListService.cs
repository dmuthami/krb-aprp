using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class UserAccessListService : IUserAccessListService
    {
        private readonly IUserAccessListRepository _userAccessListRepository;
        private readonly IShoulderSurfaceTypePavedService _shoulderSurfaceTypePavedService;
        private readonly IShoulderInterventionPavedService _shoulderInterventionPavedService;
        private readonly ISurfaceTypeUnPavedService _surfaceTypeUnPavedService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public UserAccessListService(IUserAccessListRepository userAccessListRepository, IUnitOfWork unitOfWork
            , ILogger<UserAccessListService> logger,
            IShoulderSurfaceTypePavedService shoulderSurfaceTypePavedService,
            IShoulderInterventionPavedService shoulderInterventionPavedService,
            ISurfaceTypeUnPavedService surfaceTypeUnPavedService)
        {
            _userAccessListRepository = userAccessListRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _shoulderSurfaceTypePavedService = shoulderSurfaceTypePavedService;
            _shoulderInterventionPavedService = shoulderInterventionPavedService;
            _surfaceTypeUnPavedService = surfaceTypeUnPavedService;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<UserAccessListResponse> AddAsync(UserAccessList userAccessList)
        {
            try
            {
                await _userAccessListRepository.AddAsync(userAccessList).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new UserAccessListResponse(userAccessList); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UserAccessListService.AddAsync Error: {Environment.NewLine}");
                return new UserAccessListResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<UserAccessListResponse> FindByEmailAsync(string Email)
        {
            try
            {
                var existingUserAccessList = await _userAccessListRepository.FindByEmailAsync(Email).ConfigureAwait(false);
                if (existingUserAccessList == null)
                {
                    return new UserAccessListResponse("Record Not Found");
                }
                else
                {
                    return new UserAccessListResponse(existingUserAccessList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UserAccessListService.FindByEmailAsync Error {Environment.NewLine}");
                return new UserAccessListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<UserAccessListResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingUserAccessList = await _userAccessListRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingUserAccessList == null)
                {
                    return new UserAccessListResponse("Record Not Found");
                }
                else
                {
                    return new UserAccessListResponse(existingUserAccessList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UserAccessListService.FindByIdAsync Error {Environment.NewLine}");
                return new UserAccessListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<UserAccessListDataResponse> ListAsync()
        {
            try
            {
                var existingUserAccessList = await _userAccessListRepository.ListAsync().ConfigureAwait(false);
                if (existingUserAccessList == null)
                {
                    return new UserAccessListDataResponse("Records Not Found");
                }
                else
                {
                    return new UserAccessListDataResponse(existingUserAccessList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UserAccessListService.ListAsync Error {Environment.NewLine}");
                return new UserAccessListDataResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<UserAccessListResponse> RemoveAsync(long ID)
        {
            var existingUserAccessList = await _userAccessListRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingUserAccessList == null)
            {
                return new UserAccessListResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _userAccessListRepository.Remove(existingUserAccessList);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new UserAccessListResponse(existingUserAccessList);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"UserAccessListService.RemoveAsync Error {Environment.NewLine}");
                    return new UserAccessListResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<UserAccessListResponse> Update(long ID, UserAccessList userAccessList)
        {
            try
            {
                _userAccessListRepository.Update(ID, userAccessList);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new UserAccessListResponse(userAccessList);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UserAccessListService.Update Error {Environment.NewLine}");
                return new UserAccessListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
