using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class ShoulderInterventionPavedService : IShoulderInterventionPavedService
    {
        private readonly IShoulderInterventionPavedRepository _shoulderInterventionPavedRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public ShoulderInterventionPavedService(IShoulderInterventionPavedRepository shoulderInterventionPavedRepository, IUnitOfWork unitOfWork
            ,ILogger<ShoulderInterventionPavedService> logger)
        {
            _shoulderInterventionPavedRepository = shoulderInterventionPavedRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ShoulderInterventionPavedResponse> AddAsync(ShoulderInterventionPaved shoulderInterventionPaved)
        {
            try
            {
                await _shoulderInterventionPavedRepository.AddAsync(shoulderInterventionPaved).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ShoulderInterventionPavedResponse(shoulderInterventionPaved); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ShoulderInterventionPavedService.AddAsync Error: {Environment.NewLine}");
                return new ShoulderInterventionPavedResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ShoulderInterventionPavedResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingShoulderInterventionPaved = await _shoulderInterventionPavedRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingShoulderInterventionPaved == null)
                {
                    return new ShoulderInterventionPavedResponse("Record Not Found");
                }
                else
                {
                    return new ShoulderInterventionPavedResponse(existingShoulderInterventionPaved);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ShoulderInterventionPavedService.FindByIdAsync Error: {Environment.NewLine}");
                return new ShoulderInterventionPavedResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ShoulderInterventionPavedListResponse> ListAsync()
        {
            try
            {
                var existingShoulderInterventionPaved = await _shoulderInterventionPavedRepository.ListAsync().ConfigureAwait(false);
                if (existingShoulderInterventionPaved == null)
                {
                    return new ShoulderInterventionPavedListResponse("Records Not Found");
                }
                else
                {
                    return new ShoulderInterventionPavedListResponse(existingShoulderInterventionPaved);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ShoulderInterventionPavedService.ListAsync Error: {Environment.NewLine}");
                return new ShoulderInterventionPavedListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ShoulderInterventionPavedResponse> RemoveAsync(long ID)
        {
            var existingShoulderInterventionPaved = await _shoulderInterventionPavedRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingShoulderInterventionPaved == null)
            {
                return new ShoulderInterventionPavedResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _shoulderInterventionPavedRepository.Remove(existingShoulderInterventionPaved);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new ShoulderInterventionPavedResponse(existingShoulderInterventionPaved);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ShoulderInterventionPavedService.RemoveAsync Error: {Environment.NewLine}");
                    return new ShoulderInterventionPavedResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ShoulderInterventionPavedResponse> Update(ShoulderInterventionPaved shoulderInterventionPaved)
        {
            var existingSurvey = await _shoulderInterventionPavedRepository.FindByIdAsync(shoulderInterventionPaved.ID).ConfigureAwait(false);
            if (existingSurvey == null)
            {
                return new ShoulderInterventionPavedResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _shoulderInterventionPavedRepository.Update(shoulderInterventionPaved);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new ShoulderInterventionPavedResponse(shoulderInterventionPaved);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ShoulderInterventionPavedService.FindByIdAsync Error: {Environment.NewLine}");
                    return new ShoulderInterventionPavedResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }
    }
}
