using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class TerrainTypeService : ITerrainTypeService
    {
        private readonly ITerrainTypeRepository _terrainTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public TerrainTypeService(ITerrainTypeRepository terrainTypeRepository, IUnitOfWork unitOfWork
            ,ILogger<TerrainTypeService> logger)
        {
            _terrainTypeRepository = terrainTypeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TerrainTypeResponse> AddAsync(TerrainType terrainType)
        {
            try
            {
                await _terrainTypeRepository.AddAsync(terrainType).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new TerrainTypeResponse(terrainType); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TerrainTypeService.AddAsync Error: {Environment.NewLine}");
                return new TerrainTypeResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TerrainTypeResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingTerrainType = await _terrainTypeRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingTerrainType == null)
                {
                    return new TerrainTypeResponse("Record Not Found");
                }
                else
                {
                    return new TerrainTypeResponse(existingTerrainType);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TerrainTypeService.FindByIdAsync Error: {Environment.NewLine}");
                return new TerrainTypeResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TerrainTypeListResponse> ListAsync()
        {
            try
            {
                var existingTerrainType = await _terrainTypeRepository.ListAsync().ConfigureAwait(false);
                if (existingTerrainType == null)
                {
                    return new TerrainTypeListResponse("Records Not Found");
                }
                else
                {
                    return new TerrainTypeListResponse(existingTerrainType);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TerrainTypeService.ListAsync Error: {Environment.NewLine}");
                return new TerrainTypeListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TerrainTypeResponse> RemoveAsync(long ID)
        {
            var existingTerrainType = await _terrainTypeRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingTerrainType == null)
            {
                return new TerrainTypeResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _terrainTypeRepository.Remove(existingTerrainType);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new TerrainTypeResponse(existingTerrainType);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"TerrainTypeService.RemoveAsync Error: {Environment.NewLine}");
                    return new TerrainTypeResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TerrainTypeResponse> Update(TerrainType terrainType)
        {
            if (terrainType==null)
            {
                throw new NullReferenceException("Terrain Type object is null");
            }
            var existingTerrainType = await _terrainTypeRepository.FindByIdAsync(terrainType.ID).ConfigureAwait(false);
            if (existingTerrainType == null)
            {
                return new TerrainTypeResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _terrainTypeRepository.Update(terrainType);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new TerrainTypeResponse(terrainType);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"TerrainTypeService.Update Error: {Environment.NewLine}");
                    return new TerrainTypeResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }
    }
}
