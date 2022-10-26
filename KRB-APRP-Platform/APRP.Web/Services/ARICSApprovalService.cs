using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Services
{
    public class ARICSApprovalService : IARICSApprovalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IARICSApprovalRepository _aRICSApprovalRepository;
        public ARICSApprovalService(IUnitOfWork unitOfWork, ILogger<ARICSApprovalService> logger,
            IARICSApprovalRepository aRICSYearRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _aRICSApprovalRepository = aRICSYearRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> AddAsync(Domain.Models.ARICSApproval aRICSApproval)
        {
            try
            {
                var iActionResult = await _aRICSApprovalRepository.AddAsync(aRICSApproval).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"ARICSApprovalService.AddAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApproval record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> DetachFirstEntryAsync(ARICSApproval aRICSApproval)
        {
            try
            {
                var iActionResult = await _aRICSApprovalRepository
                    .DetachFirstEntryAsync(aRICSApproval).
                    ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSApprovalService.DetachFirstEntryAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApproval record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> FindByIdAsync(long ID)
        {
            try
            {
                var iActionResult = await _aRICSApprovalRepository.FindByIdAsync(ID).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSApprovalService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApproval record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> FindByARICSMasterApprovalIdAsync(long ARICSMasterApprovalIdId)
        {
            try
            {
                var iActionResult = await _aRICSApprovalRepository.FindByARICSMasterApprovalIdAsync(ARICSMasterApprovalIdId).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSApprovalService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApproval record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> ListAsync()
        {
            try
            {
                var iActionResult = await _aRICSApprovalRepository.ListAsync().ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSApprovalService.ListAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApproval record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> ListHistoryAsync(long ARICSApprovalId)
        {
            try
            {
                var iActionResult = await _aRICSApprovalRepository.ListHistoryAsync(ARICSApprovalId).ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSApprovalService.ListAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApproval record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> RemoveAsync(long ID)
        {
            try
            {
                var iActionResult = await _aRICSApprovalRepository.FindByIdAsync(ID).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        var aRICSApproval = (ARICSApproval)result.Value;
                        var iActionResult2 = await _aRICSApprovalRepository.Remove(aRICSApproval).ConfigureAwait(false);

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
                _logger.LogError(Ex, $"ARICSApprovalService.RemoveAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApproval record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> Update(Domain.Models.ARICSApproval aRICSApproval)
        {
            throw new System.NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> Update(long ID, ARICSApproval aRICSApproval)
        {
            try
            {
                var iActionResult = await _aRICSApprovalRepository.Update(ID, aRICSApproval).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"ARICSApprovalService.Update Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApproval record : {Ex.Message}");
            }
        }
    }
}