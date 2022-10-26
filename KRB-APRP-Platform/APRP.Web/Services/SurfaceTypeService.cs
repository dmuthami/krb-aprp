
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class SurfaceTypeService : ISurfaceTypeService
    {
        private readonly ISurfaceTypeRepository _surfaceTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public SurfaceTypeService(ISurfaceTypeRepository surfaceTypeRepository, IUnitOfWork unitOfWork, ILogger<SurfaceTypeService> logger)
        {
            _surfaceTypeRepository = surfaceTypeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<SurfaceTypeResponse> AddAsync(SurfaceType surfaceType)
        {
            try
            {
                await _surfaceTypeRepository.AddAsync(surfaceType).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new SurfaceTypeResponse(surfaceType); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"SurfaceTypeService.AddAsync Error: {Environment.NewLine}");
                return new SurfaceTypeResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<SurfaceTypeResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingSurfaceType = await _surfaceTypeRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingSurfaceType == null)
                {
                    return new SurfaceTypeResponse("Record Not Found");
                }
                else
                {
                    return new SurfaceTypeResponse(existingSurfaceType);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"SurfaceTypeService.FindByIdAsync Error: {Environment.NewLine}");
                return new SurfaceTypeResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<SurfaceTypeResponse> FindByNameAsync(string SurfaceType)
        {
            try
            {
                var existingSurfaceType = await _surfaceTypeRepository.FindByNameAsync(SurfaceType).ConfigureAwait(false);
                if (existingSurfaceType == null)
                {
                    return new SurfaceTypeResponse("Record Not Found");
                }
                else
                {
                    return new SurfaceTypeResponse(existingSurfaceType);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"SurfaceTypeService.FindByNameAsync Error: {Environment.NewLine}");
                return new SurfaceTypeResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<SurfaceTypeListResponse> ListAsync()
        {
            try
            {
                var existingSurfaceType = await _surfaceTypeRepository.ListAsync().ConfigureAwait(false);
                if (existingSurfaceType == null)
                {
                    return new SurfaceTypeListResponse("Records Not Found");
                }
                else
                {
                    return new SurfaceTypeListResponse(existingSurfaceType);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"SurfaceTypeService.ListAsync Error: {Environment.NewLine}");
                return new SurfaceTypeListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<SurfaceTypeResponse> RemoveAsync(long ID)
        {
            try
            {
                var existingSurfaceType = await _surfaceTypeRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingSurfaceType == null)
                {
                    return new SurfaceTypeResponse("Record Not Found");
                }
                else
                {
                    try
                    {
                        _surfaceTypeRepository.Remove(existingSurfaceType);
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                        return new SurfaceTypeResponse(existingSurfaceType);
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"SurfaceTypeService.RemoveAsync Error: {Environment.NewLine}");
                        return new SurfaceTypeResponse($"Error occured while retrieving the record : {Ex.Message}");
                    }
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"SurfaceTypeService.RemoveAsync Error: {Environment.NewLine}");
                return new SurfaceTypeResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<SurfaceTypeResponse> Update(SurfaceType surfaceType)
        {
            try
            {
                if (surfaceType==null)
                {
                    throw new NullReferenceException("SurfaceType object is null");
                }
                var existingSurfaceType = await _surfaceTypeRepository.FindByIdAsync(surfaceType.ID).ConfigureAwait(false);
                if (existingSurfaceType == null)
                {
                    return new SurfaceTypeResponse("Record Not Found");
                }
                else
                {
                    try
                    {
                        _surfaceTypeRepository.Update(surfaceType);
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                        return new SurfaceTypeResponse(surfaceType);
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"SurfaceTypeService.Update Error: {Environment.NewLine}");
                        return new SurfaceTypeResponse($"Error occured while retrieving the record : {Ex.Message}");
                    }
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"SurfaceTypeService.Update Error: {Environment.NewLine}");
                return new SurfaceTypeResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
