using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class SurfaceTypeUnPavedService : ISurfaceTypeUnPavedService
    {
        private readonly ISurfaceTypeUnPavedRepository _surfaceTypeUnPavedRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public SurfaceTypeUnPavedService(ISurfaceTypeUnPavedRepository surfaceTypeUnPavedRepository, IUnitOfWork unitOfWork
            ,ILogger<SurfaceTypeUnPavedService> logger)
        {
            _surfaceTypeUnPavedRepository = surfaceTypeUnPavedRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<SurfaceTypeUnPavedResponse> AddAsync(SurfaceTypeUnPaved surfaceTypeUnPaved)
        {
            try
            {
                await _surfaceTypeUnPavedRepository.AddAsync(surfaceTypeUnPaved).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new SurfaceTypeUnPavedResponse(surfaceTypeUnPaved); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"SurfaceTypeUnPavedService.AddAsync Error: {Environment.NewLine}");
                return new SurfaceTypeUnPavedResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<SurfaceTypeUnPavedResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingSurfaceTypeUnPaved = await _surfaceTypeUnPavedRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingSurfaceTypeUnPaved == null)
                {
                    return new SurfaceTypeUnPavedResponse("Record Not Found");
                }
                else
                {
                    return new SurfaceTypeUnPavedResponse(existingSurfaceTypeUnPaved);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"SurfaceTypeUnPavedService.FindByIdAsync Error: {Environment.NewLine}");
                return new SurfaceTypeUnPavedResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<SurfaceTypeUnPavedListResponse> ListAsync()
        {
            try
            {
                var existingSurfaceTypeUnPaved = await _surfaceTypeUnPavedRepository.ListAsync().ConfigureAwait(false);
                if (existingSurfaceTypeUnPaved == null)
                {
                    return new SurfaceTypeUnPavedListResponse("Records Not Found");
                }
                else
                {
                    return new SurfaceTypeUnPavedListResponse(existingSurfaceTypeUnPaved);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"SurfaceTypeUnPavedService.ListAsync Error: {Environment.NewLine}");
                return new SurfaceTypeUnPavedListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<SurfaceTypeUnPavedResponse> RemoveAsync(long ID)
        {
            var existingSurfaceTypeUnPaved = await _surfaceTypeUnPavedRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingSurfaceTypeUnPaved == null)
            {
                return new SurfaceTypeUnPavedResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _surfaceTypeUnPavedRepository.Remove(existingSurfaceTypeUnPaved);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new SurfaceTypeUnPavedResponse(existingSurfaceTypeUnPaved);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"SurfaceTypeUnPavedService.RemoveAsync Error: {Environment.NewLine}");
                    return new SurfaceTypeUnPavedResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<SurfaceTypeUnPavedResponse> Update(SurfaceTypeUnPaved surfaceTypeUnPaved)
        {
            var existingSurfaceTypeUnPaved = await _surfaceTypeUnPavedRepository.FindByIdAsync(surfaceTypeUnPaved.ID).ConfigureAwait(false);
            if (existingSurfaceTypeUnPaved == null)
            {
                return new SurfaceTypeUnPavedResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _surfaceTypeUnPavedRepository.Update(surfaceTypeUnPaved);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new SurfaceTypeUnPavedResponse(surfaceTypeUnPaved);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"SurfaceTypeUnPavedService.Update Error: {Environment.NewLine}");
                    return new SurfaceTypeUnPavedResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }
    }
}
