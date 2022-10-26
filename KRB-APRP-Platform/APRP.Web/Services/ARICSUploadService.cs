using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class ARICSUploadService : IARICSUploadService
    {
        private readonly IARICSUploadRepository _aRICSUploadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public ARICSUploadService(IARICSUploadRepository aRICSUploadRepository, IUnitOfWork unitOfWork, ILogger<ARICSUploadService> logger)
        {
            _aRICSUploadRepository = aRICSUploadRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSUploadResponse> AddAsync(ARICSUpload aRICSUpload)
        {
            try
            {
                await _aRICSUploadRepository.AddAsync(aRICSUpload).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ARICSUploadResponse(aRICSUpload); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSUploadService.AddAsync Error: {Environment.NewLine}");
                return new ARICSUploadResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSUploadResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingARICSUpload = await _aRICSUploadRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingARICSUpload == null)
                {
                    return new ARICSUploadResponse("Record Not Found");
                }
                else
                {
                    return new ARICSUploadResponse(existingARICSUpload);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSUploadService.FindByIdAsync Error: {Environment.NewLine}");
                return new ARICSUploadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSUploadListResponse> ListAsync()
        {
            try
            {
                var existingARICSUploadList = await _aRICSUploadRepository.ListAsync().ConfigureAwait(false);
                if (existingARICSUploadList == null)
                {
                    return new ARICSUploadListResponse("Records Not Found");
                }
                else
                {
                    return new ARICSUploadListResponse(existingARICSUploadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSUploadService.ListAsync Error: {Environment.NewLine}");
                return new ARICSUploadListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSUploadResponse> RemoveAsync(long ID)
        {
            var existingARICSUpload = await _aRICSUploadRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingARICSUpload == null)
            {
                return new ARICSUploadResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _aRICSUploadRepository.Remove(existingARICSUpload);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new ARICSUploadResponse(existingARICSUpload);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ARICSUploadService.RemoveAsync Error: {Environment.NewLine}");
                    return new ARICSUploadResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSUploadResponse> Update(ARICSUpload aRICSUpload)
        {
            var existingARICS = await _aRICSUploadRepository.FindByIdAsync(aRICSUpload.ID).ConfigureAwait(false);
            if (existingARICS == null)
            {
                return new ARICSUploadResponse("Record Not Found");
            }
            else
            {
                //existingRoad.RoadNumber = road.RoadNumber;
                //existingRoad.RoadName = road.RoadName;
                try
                {
                    _aRICSUploadRepository.Update(aRICSUpload);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new ARICSUploadResponse(aRICSUpload);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ARICSUploadService.Updated Error: {Environment.NewLine}");
                    return new ARICSUploadResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }
    }
}
