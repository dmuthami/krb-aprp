using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Services
{
    public class DisbursementTrancheService : IDisbursementTrancheService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IDisbursementTrancheRepository _disbursementTrancheRepository;
        public DisbursementTrancheService(IUnitOfWork unitOfWork, ILogger<DisbursementTrancheService> logger,
            IDisbursementTrancheRepository disbursementTrancheRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _disbursementTrancheRepository = disbursementTrancheRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> AddAsync(Domain.Models.DisbursementTranche disbursementTranche)
        {
            try
            {
                var iActionResult = await _disbursementTrancheRepository.AddAsync(disbursementTranche).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"DisbursementTrancheService.AddAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> FindByIdAsync(long ID)
        {
            try
            {
                var iActionResult = await _disbursementTrancheRepository.FindByIdAsync(ID).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementTrancheService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> ListAsync()
        {
            try
            {
                var iActionResult = await _disbursementTrancheRepository
                    .ListAsync().
                    ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementTrancheService.ListAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> RemoveAsync(long ID)
        {
            try
            {
                var iActionResult = await _disbursementTrancheRepository.FindByIdAsync(ID).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        var disbursementTranche = (DisbursementTranche)result.Value;
                        var iActionResult2 = await _disbursementTrancheRepository.Remove(disbursementTranche).ConfigureAwait(false);

                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
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
                _logger.LogError(Ex, $"DisbursementTrancheService.RemoveAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> Update(Domain.Models.DisbursementTranche disbursementTranche)
        {
            throw new System.NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> Update(long ID, DisbursementTranche disbursementTranche)
        {
            try
            {
                var iActionResult = await _disbursementTrancheRepository.Update(ID, disbursementTranche).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"DisbursementTrancheService.Update Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }
    }
}