using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Services
{
    public class DisbursementReleaseService : IDisbursementReleaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IDisbursementReleaseRepository _disbursementReleaseRepository;
        public DisbursementReleaseService(IUnitOfWork unitOfWork, ILogger<DisbursementReleaseService> logger,
            IDisbursementReleaseRepository disbursementReleaseRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _disbursementReleaseRepository = disbursementReleaseRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> AddAsync(Domain.Models.DisbursementRelease disbursementRelease)
        {
            try
            {
                var iActionResult = await _disbursementReleaseRepository.AddAsync(disbursementRelease).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"DisbursementReleaseService.AddAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the disbursementRelease record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> FindbisbursementReleaseAsync(long ReleaseId, long DisbursementId)
        {
            try
            {
                var iActionResult = await _disbursementReleaseRepository.FindbisbursementReleaseAsync(
                    ReleaseId, DisbursementId).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementReleaseService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the disbursementRelease record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> ListAsync()
        {
            try
            {
                var iActionResult = await _disbursementReleaseRepository.ListAsync().ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementReleaseService.ListAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the disbursementRelease record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> Remove(DisbursementRelease disbursementRelease)
        {
            try
            {
                var iActionResult = await _disbursementReleaseRepository.Remove(disbursementRelease).ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementReleaseService.Remove Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the disbursementRelease record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> RemoveAsync(long ReleaseId, long DisbursementId)
        {
            try
            {
                var iActionResult = await _disbursementReleaseRepository.FindbisbursementReleaseAsync(ReleaseId, DisbursementId).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        var disbursementRelease = (DisbursementRelease)result.Value;
                        var iActionResult2 = await _disbursementReleaseRepository.Remove(disbursementRelease).ConfigureAwait(false);
                       
                        //Complete Async Works
                        var objectResult2 = (ObjectResult)iActionResult2;
                        if (objectResult2 != null)
                        {
                            if (objectResult2.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                            }
                        }

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
                _logger.LogError(Ex, $"DisbursementReleaseService.RemoveAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the disbursementRelease record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> Update(Domain.Models.DisbursementRelease disbursementRelease)
        {
            throw new System.NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> Update(int ID, DisbursementRelease disbursementRelease)
        {
            try
            {
                var iActionResult = await _disbursementReleaseRepository.Update(ID, disbursementRelease).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"DisbursementReleaseService.Update Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the disbursementRelease record : {Ex.Message}");
            }
        }
    }
}