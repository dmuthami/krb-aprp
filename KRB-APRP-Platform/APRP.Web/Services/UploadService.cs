using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class UploadService : IUploadService
    {
        private readonly IUploadRepository _uploadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public UploadService(IUploadRepository uploadRepository, IUnitOfWork unitOfWork, ILogger<UploadService> logger)
        {
            _uploadRepository = uploadRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<UploadResponse> AddAsync(Upload upload)
        {
            try
            {
                await _uploadRepository.AddAsync(upload).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new UploadResponse(upload); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UploadService.AddAsync Error: {Environment.NewLine}");
                return new UploadResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<UploadResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingARICSUpload = await _uploadRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingARICSUpload == null)
                {
                    return new UploadResponse("Record Not Found");
                }
                else
                {
                    return new UploadResponse(existingARICSUpload);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UploadService.FindByIdAsync Error: {Environment.NewLine}");
                return new UploadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<UploadListResponse> ListAsync()
        {
            try
            {
                var existingARICSUploadList = await _uploadRepository.ListAsync().ConfigureAwait(false);
                if (existingARICSUploadList == null)
                {
                    return new UploadListResponse("Records Not Found");
                }
                else
                {
                    return new UploadListResponse(existingARICSUploadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UploadService.ListAsync Error: {Environment.NewLine}");
                return new UploadListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<UploadListResponse> ListAsync(string Type, long ForeignId)
        {
            try
            {
                var existingARICSUploadList = await _uploadRepository.ListAsync(Type, ForeignId).ConfigureAwait(false);
                if (existingARICSUploadList == null)
                {
                    return new UploadListResponse("Records Not Found");
                }
                else
                {
                    return new UploadListResponse(existingARICSUploadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UploadService.ListAsync Error: {Environment.NewLine}");
                return new UploadListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<UploadResponse> RemoveAsync(long ID)
        {
            var existingARICSUpload = await _uploadRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingARICSUpload == null)
            {
                return new UploadResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _uploadRepository.Remove(existingARICSUpload);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new UploadResponse(existingARICSUpload);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"UploadService.RemoveAsync Error: {Environment.NewLine}");
                    return new UploadResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<UploadResponse> Update(Upload upload)
        {
            var existingARICS = await _uploadRepository.FindByIdAsync(upload.ID).ConfigureAwait(false);
            if (existingARICS == null)
            {
                return new UploadResponse("Record Not Found");
            }
            else
            {
                //existingRoad.RoadNumber = road.RoadNumber;
                //existingRoad.RoadName = road.RoadName;
                try
                {
                    _uploadRepository.Update(upload);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new UploadResponse(upload);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"UploadService.Updated Error: {Environment.NewLine}");
                    return new UploadResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }
    }
}
