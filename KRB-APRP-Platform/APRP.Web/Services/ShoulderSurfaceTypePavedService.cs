using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class ShoulderSurfaceTypePavedService : IShoulderSurfaceTypePavedService
    {
        private readonly IShoulderSurfaceTypePavedRepository _shoulderSurfaceTypePavedRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public ShoulderSurfaceTypePavedService(IShoulderSurfaceTypePavedRepository shoulderSurfaceTypePavedRepository, IUnitOfWork unitOfWork
            ,ILogger<ShoulderSurfaceTypePavedService> logger)
        {
            _shoulderSurfaceTypePavedRepository = shoulderSurfaceTypePavedRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ShoulderSurfaceTypePavedResponse> AddAsync(ShoulderSurfaceTypePaved shoulderSurfaceTypePaved)
        {
            try
            {
                await _shoulderSurfaceTypePavedRepository.AddAsync(shoulderSurfaceTypePaved).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ShoulderSurfaceTypePavedResponse(shoulderSurfaceTypePaved); //Successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ShoulderSurfaceTypePavedService.AddAsync Error: {Environment.NewLine}");
                return new ShoulderSurfaceTypePavedResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ShoulderSurfaceTypePavedResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingShoulderSurfaceTypePaved = await _shoulderSurfaceTypePavedRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingShoulderSurfaceTypePaved == null)
                {
                    return new ShoulderSurfaceTypePavedResponse("Record Not Found");
                }
                else
                {
                    return new ShoulderSurfaceTypePavedResponse(existingShoulderSurfaceTypePaved);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ShoulderSurfaceTypePavedService.FindByIdAsync Error: {Environment.NewLine}");
                return new ShoulderSurfaceTypePavedResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<ShoulderSurfaceTypePavedListResponse> ListAsync()
        {
            try
            {
                var existingShoulderSurfaceTypePaved = await _shoulderSurfaceTypePavedRepository.ListAsync().ConfigureAwait(false);
                if (existingShoulderSurfaceTypePaved == null)
                {
                    return new ShoulderSurfaceTypePavedListResponse("Records Not Found");
                }
                else
                {
                    return new ShoulderSurfaceTypePavedListResponse(existingShoulderSurfaceTypePaved);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ShoulderSurfaceTypePavedService.ListAsync Error: {Environment.NewLine}");
                return new ShoulderSurfaceTypePavedListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ShoulderSurfaceTypePavedResponse> RemoveAsync(long ID)
        {
            var existingShoulderSurfaceTypePaved = await _shoulderSurfaceTypePavedRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingShoulderSurfaceTypePaved == null)
            {
                return new ShoulderSurfaceTypePavedResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _shoulderSurfaceTypePavedRepository.Remove(existingShoulderSurfaceTypePaved);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new ShoulderSurfaceTypePavedResponse(existingShoulderSurfaceTypePaved);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ShoulderSurfaceTypePavedService.RemoveAsync Error: {Environment.NewLine}");
                    return new ShoulderSurfaceTypePavedResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ShoulderSurfaceTypePavedResponse> Update(ShoulderSurfaceTypePaved shoulderSurfaceTypePaved)
        {
            var existingSurvey = await _shoulderSurfaceTypePavedRepository.FindByIdAsync(shoulderSurfaceTypePaved.ID).ConfigureAwait(false);
            if (existingSurvey == null)
            {
                return new ShoulderSurfaceTypePavedResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _shoulderSurfaceTypePavedRepository.Update(shoulderSurfaceTypePaved);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new ShoulderSurfaceTypePavedResponse(shoulderSurfaceTypePaved);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ShoulderSurfaceTypePavedService.FindByIdAsync Error: {Environment.NewLine}");
                    return new ShoulderSurfaceTypePavedResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }
    }
}
