using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Services
{
    public class ARICSApprovalLevelService : IARICSApprovalLevelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IARICSApprovalLevelRepository _aRICSApprovalLevelRepository;
        public ARICSApprovalLevelService(IUnitOfWork unitOfWork, ILogger<ARICSApprovalLevelService> logger,
            IARICSApprovalLevelRepository aRICSApprovalLevelRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _aRICSApprovalLevelRepository = aRICSApprovalLevelRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> AddAsync(Domain.Models.ARICSApprovalLevel aRICSApprovalLevel)
        {
            try
            {
                var iActionResult = await _aRICSApprovalLevelRepository.AddAsync(aRICSApprovalLevel).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"ARICSApprovalLevelService.AddAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApprovalLevel record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> DetachFirstEntryAsync(ARICSApprovalLevel aRICSApprovalLevel)
        {
            try
            {
                var iActionResult = await _aRICSApprovalLevelRepository
                    .DetachFirstEntryAsync(aRICSApprovalLevel).
                    ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSApprovalLevelService.DetachFirstEntryAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApprovalLevel record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> FindByIdAsync(long ID)
        {
            try
            {
                var iActionResult = await _aRICSApprovalLevelRepository.FindByIdAsync(ID).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSApprovalLevelService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApprovalLevel record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> FindByAuthorityTypeAndStatusAsync(long AuthorityType, int Status)
        {
            try
            {
                var iActionResult = await _aRICSApprovalLevelRepository.FindByAuthorityTypeAndStatusAsync(AuthorityType, Status).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSApprovalLevelService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApprovalLevel record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> ListAsync()
        {
            try
            {
                var iActionResult = await _aRICSApprovalLevelRepository.ListAsync().ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSApprovalLevelService.ListAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApprovalLevel record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> RemoveAsync(long ID)
        {
            try
            {
                var iActionResult = await _aRICSApprovalLevelRepository.FindByIdAsync(ID).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        var aRICSApprovalLevel = (ARICSApprovalLevel)result.Value;
                        var iActionResult2 = await _aRICSApprovalLevelRepository.Remove(aRICSApprovalLevel).ConfigureAwait(false);

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
                _logger.LogError(Ex, $"ARICSApprovalLevelService.RemoveAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApprovalLevel record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> Update(Domain.Models.ARICSApprovalLevel aRICSApprovalLevel)
        {
            throw new System.NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> Update(long ID, ARICSApprovalLevel aRICSApprovalLevel)
        {
            try
            {
                var iActionResult = await _aRICSApprovalLevelRepository.Update(ID, aRICSApprovalLevel).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"ARICSApprovalLevelService.Update Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApprovalLevel record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> FindByStatusAsync(int Status)
        {
            try
            {
                var iActionResult = await _aRICSApprovalLevelRepository.FindByStatusAsync(Status).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSApprovalLevelService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSApprovalLevel record : {Ex.Message}");
            }
        }
    }
}