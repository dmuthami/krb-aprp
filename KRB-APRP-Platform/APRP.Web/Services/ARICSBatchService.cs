using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Services
{
    public class ARICSBatchService : ControllerBase, IARICSBatchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IARICSBatchRepository _aRICSBatchRepository;
        public ARICSBatchService(IUnitOfWork unitOfWork, ILogger<ARICSBatchService> logger,
            IARICSBatchRepository aRICSBatchRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _aRICSBatchRepository = aRICSBatchRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> AddAsync(Domain.Models.ARICSBatch aRICSBatch)
        {
            try
            {
                var iActionResult = await _aRICSBatchRepository.AddAsync(aRICSBatch).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"ARICSBatchService.AddAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSBatch record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> DetachFirstEntryAsync(ARICSBatch aRICSBatch)
        {
            try
            {
                var iActionResult = await _aRICSBatchRepository
                    .DetachFirstEntryAsync(aRICSBatch).
                    ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSBatchService.DetachFirstEntryAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSBatch record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> FindByIdAsync(long ID)
        {
            try
            {
                var iActionResult = await _aRICSBatchRepository.FindByIdAsync(ID).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSBatchService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSBatch record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> ListByAuthorityAndARICSYearAsync(long AuthorityId, int ARICSYearId)
        {
            try
            {
                var resp = await _aRICSBatchRepository.ListByAuthorityAndARICSYearAsync(AuthorityId, ARICSYearId).ConfigureAwait(false);

                IQueryable<ARICSBatch> aRICSBatches = null;

                var objectResult = (ObjectResult)resp;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        aRICSBatches = (IQueryable<ARICSBatch>)result2.Value;
                    }
                }


                IQueryable<ARICSBatchViewModel> ARICSBatchApprovalData;
                ARICSBatchApprovalData =
                 from aRICSBatch
                 in aRICSBatches
                 select new ARICSBatchViewModel()
                 {
                     id = aRICSBatch.ID,
                     batchno = aRICSBatch.ARICSMasterApproval.BatchNo,
                     description = aRICSBatch.ARICSMasterApproval.Description,
                     sectionid= aRICSBatch.RoadSection.SectionID,
                     sectionname = aRICSBatch.RoadSection.SectionName,
                     roadnumber = aRICSBatch.RoadSection.Road.RoadNumber
                 };

                return new GenericResponse(Ok(ARICSBatchApprovalData));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSBatchService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSBatch record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> ListAsync()
        {
            try
            {
                var iActionResult = await _aRICSBatchRepository.ListAsync().ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSBatchService.ListAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSBatch record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> RemoveAsync(long ID)
        {
            try
            {
                var iActionResult = await _aRICSBatchRepository.FindByIdAsync(ID).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        var aRICSBatch = (ARICSBatch)result.Value;
                        var iActionResult2 = await _aRICSBatchRepository.Remove(aRICSBatch).ConfigureAwait(false);

                        //var objectResult2 = (ObjectResult)iActionResult2;
                        //if (objectResult2 != null)
                        //{
                        //    if (objectResult2.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        //    {

                        //    }
                        //}
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);
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
                _logger.LogError(Ex, $"ARICSBatchService.RemoveAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSBatch record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> Update(Domain.Models.ARICSBatch aRICSBatch)
        {
            throw new System.NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> Update(long ID, ARICSBatch aRICSBatch)
        {
            try
            {
                var iActionResult = await _aRICSBatchRepository.Update(ID, aRICSBatch).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"ARICSBatchService.Update Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSBatch record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> FindByRoadSectionIdAndARICSMasterApprovalIdAsync(long ARICSMasterApprovalID, long RoadSectionId)
        {
            try
            {
                var iActionResult = await _aRICSBatchRepository.FindByRoadSectionIdAndARICSMasterApprovalIdAsync
                    (ARICSMasterApprovalID,
                    RoadSectionId).ConfigureAwait(false);

                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSBatchService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSBatch record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> ListByARICSMasterApprovalIdAndARICSYearIdAsync(long ARICSMasterApprovalId, int ARICSYearId)
        {
            try
            {
                var resp = await _aRICSBatchRepository.ListByARICSMasterApprovalIdAndARICSYearIdAsync(ARICSMasterApprovalId, ARICSYearId).ConfigureAwait(false);

                IQueryable<ARICSBatch> aRICSBatchList = null;

                var objectResult = (ObjectResult)resp;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        aRICSBatchList = (IQueryable<ARICSBatch>)result2.Value;
                    }
                }


                IQueryable<ARICSBatchViewModel> ARICSBatchData;
                ARICSBatchData =
                 from aRICSBatch
                 in aRICSBatchList
                 select new ARICSBatchViewModel()
                 {
                     id = aRICSBatch.ID,
                     aricsmasterapprovalid = aRICSBatch.ARICSMasterApprovalId,
                     roadsectionid = aRICSBatch.RoadSectionId,
                     sectionid= aRICSBatch.RoadSection.SectionID,
                     sectionname= aRICSBatch.RoadSection.SectionName
                 };

                return new GenericResponse(Ok(ARICSBatchData));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSBatchService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSBatch record : {Ex.Message}");
            }
        }
    }
}