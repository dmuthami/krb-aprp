using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Services
{
    public class CostesRegionService : ICostesRegionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly ICostesRegionRepository _costesRegionRepository;
        public CostesRegionService(IUnitOfWork unitOfWork, ILogger<CostesRegionService> logger,
            ICostesRegionRepository costesRegionRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _costesRegionRepository = costesRegionRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> AddAsync(Domain.Models.CostesRegion costesRegion)
        {
            try
            {
                var iActionResult = await _costesRegionRepository.AddAsync(costesRegion).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    }
                }
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CostesRegionService.AddAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> FindByIdAsync(int ID)
        {
            try
            {
                var iActionResult = await _costesRegionRepository.FindByIdAsync(ID).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CostesRegionService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> ListAsync()
        {
            try
            {
                var iActionResult = await _costesRegionRepository.ListAsync().ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CostesRegionService.ListAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> RemoveAsync(int ID)
        {
            try
            {
                var iActionResult = await _costesRegionRepository.FindByIdAsync(ID).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        var costesRegion = (CostesRegion)result.Value;
                        var iActionResult2 = await _costesRegionRepository.Remove(costesRegion).ConfigureAwait(false);
                        return new GenericResponse(iActionResult2); //successful
                    }
                    else
                    {
                        return new GenericResponse(iActionResult); //failure
                    }
                }
                else
                {
                    return new GenericResponse(iActionResult); //failure
                }

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CostesRegionService.RemoveAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> Update(Domain.Models.CostesRegion costesRegion)
        {
            throw new System.NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> Update(int ID, CostesRegion costesRegion)
        {
            try
            {
                var iActionResult = await _costesRegionRepository.Update(ID, costesRegion).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    }
                }
                return new GenericResponse(iActionResult); //failure or success
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CostesRegionService.Update Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }
    }
}