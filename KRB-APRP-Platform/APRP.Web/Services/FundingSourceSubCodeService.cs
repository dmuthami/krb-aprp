using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Services
{
    public class FundingSourceSubCodeService : IFundingSourceSubCodeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IFundingSourceSubCodeRepository _fundingSourceSubCodeRepository;
        public FundingSourceSubCodeService(IUnitOfWork unitOfWork, ILogger<FundingSourceSubCodeService> logger,
            IFundingSourceSubCodeRepository fundingSourceSubCodeRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _fundingSourceSubCodeRepository = fundingSourceSubCodeRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> AddAsync(Domain.Models.FundingSourceSubCode fundingSourceSubCode)
        {
            try
            {
                var iActionResult = await _fundingSourceSubCodeRepository.AddAsync(fundingSourceSubCode).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"FundingSourceSubCodeService.AddAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> FindByIdAsync(long ID)
        {
            try
            {
                var iActionResult = await _fundingSourceSubCodeRepository.FindByIdAsync(ID).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"FundingSourceSubCodeService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> ListAsync()
        {
            try
            {
                var iActionResult = await _fundingSourceSubCodeRepository
                    .ListAsync().
                    ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"FundingSourceSubCodeService.ListAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> RemoveAsync(long ID)
        {
            try
            {
                var iActionResult = await _fundingSourceSubCodeRepository.FindByIdAsync(ID).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        var fundingSourceSubCode = (FundingSourceSubCode)result.Value;
                        var iActionResult2 = await _fundingSourceSubCodeRepository.Remove(fundingSourceSubCode).ConfigureAwait(false);

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
                _logger.LogError(Ex, $"FundingSourceSubCodeService.RemoveAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> Update(Domain.Models.FundingSourceSubCode fundingSourceSubCode)
        {
            throw new System.NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> Update(long ID, FundingSourceSubCode fundingSourceSubCode)
        {
            try
            {
                var iActionResult = await _fundingSourceSubCodeRepository.Update(ID, fundingSourceSubCode).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"FundingSourceSubCodeService.Update Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }
    }
}