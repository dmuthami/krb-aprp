using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Services
{
    public class ARICSMasterApprovalService : ControllerBase, IARICSMasterApprovalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IARICSMasterApprovalRepository _aRICSMasterApprovalRepository;
        public ARICSMasterApprovalService(IUnitOfWork unitOfWork, ILogger<ARICSMasterApprovalService> logger,
            IARICSMasterApprovalRepository aRICSMasterApprovalRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _aRICSMasterApprovalRepository = aRICSMasterApprovalRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> AddAsync(Domain.Models.ARICSMasterApproval aRICSMasterApproval)
        {
            try
            {
                var iActionResult = await _aRICSMasterApprovalRepository.AddAsync(aRICSMasterApproval).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"ARICSMasterApprovalService.AddAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSMasterApproval record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> DetachFirstEntryAsync(ARICSMasterApproval aRICSMasterApproval)
        {
            try
            {
                var iActionResult = await _aRICSMasterApprovalRepository
                    .DetachFirstEntryAsync(aRICSMasterApproval).
                    ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSMasterApprovalService.DetachFirstEntryAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSMasterApproval record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> FindByIdAsync(long ID)
        {
            try
            {
                var iActionResult = await _aRICSMasterApprovalRepository.FindByIdAsync(ID).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSMasterApprovalService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSMasterApproval record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> ListByAuthorityAndARICSYearAsync(long AuthorityId, int ARICSYearId)
        {
            try
            {
                var resp = await _aRICSMasterApprovalRepository.ListByAuthorityAndARICSYearAsync(AuthorityId, ARICSYearId).ConfigureAwait(false);

                IQueryable<ARICSMasterApproval> aRICSMasterApprovalList = null;

                var objectResult = (ObjectResult)resp;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        aRICSMasterApprovalList = (IQueryable<ARICSMasterApproval>)result2.Value;
                    }
                }


                IQueryable<ARICSMasterApprovalViewModel> ARICSMasterApprovalData;
                ARICSMasterApprovalData =
                 from aRICSMasterApproval
                 in aRICSMasterApprovalList
                 select new ARICSMasterApprovalViewModel()
                 {
                     id = aRICSMasterApproval.ID,
                     batchno = aRICSMasterApproval.BatchNo,
                     description = aRICSMasterApproval.Description
                 };

                return new GenericResponse(Ok(ARICSMasterApprovalData));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSMasterApprovalService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSMasterApproval record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> ListAsync()
        {
            try
            {
                var iActionResult = await _aRICSMasterApprovalRepository.ListAsync().ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSMasterApprovalService.ListAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSMasterApproval record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> RemoveAsync(long ID)
        {
            try
            {
                var iActionResult = await _aRICSMasterApprovalRepository.FindByIdAsync(ID).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        var aRICSMasterApproval = (ARICSMasterApproval)result.Value;
                        var iActionResult2 = await _aRICSMasterApprovalRepository.Remove(aRICSMasterApproval).ConfigureAwait(false);

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
                _logger.LogError(Ex, $"ARICSMasterApprovalService.RemoveAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSMasterApproval record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> Update(Domain.Models.ARICSMasterApproval aRICSMasterApproval)
        {
            throw new System.NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> Update(long ID, ARICSMasterApproval aRICSMasterApproval)
        {
            try
            {
                var iActionResult = await _aRICSMasterApprovalRepository.Update(ID, aRICSMasterApproval).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"ARICSMasterApprovalService.Update Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSMasterApproval record : {Ex.Message}");
            }
        }
    }
}