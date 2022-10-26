using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class FundingSourceService : IFundingSourceService
    {
        private readonly IFundingSourceRepository _fundingSourceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public FundingSourceService(IFundingSourceRepository fundingSourceRepository, IUnitOfWork unitOfWork, ILogger<FundingSourceService> logger)
        {
            _fundingSourceRepository = fundingSourceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<FundingSourceResponse> AddAsync(FundingSource fundingSource)
        {
            try
            {
                await _fundingSourceRepository.AddAsync(fundingSource).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new FundingSourceResponse(fundingSource); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"FundingSourceService.AddAsync Error: {Environment.NewLine}");
                return new FundingSourceResponse($"Error occured while saving the fund source record : {Ex.Message}");
            }
        }

        public async Task<FundingSourceResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingFundingSource = await _fundingSourceRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingFundingSource == null)
                {
                    return new FundingSourceResponse("Record Not Found");
                }
                else
                {
                    return new FundingSourceResponse(existingFundingSource);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"FundingSourceService.FindByIdAsync Error: {Environment.NewLine}");
                return new FundingSourceResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<FundingSourceResponse> FindByNameAsync(string Name)
        {
            try
            {
                var existingFundingSource = await _fundingSourceRepository.FindByNameAsync(Name).ConfigureAwait(false);
                if (existingFundingSource == null)
                {
                    return new FundingSourceResponse("Record Not Found");
                }
                else
                {
                    return new FundingSourceResponse(existingFundingSource);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"FundingSourceService.FindByIdAsync Error: {Environment.NewLine}");
                return new FundingSourceResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<IEnumerable<FundingSource>> ListAsync()
        {
            return await _fundingSourceRepository.ListAsync().ConfigureAwait(false);
        }

        public async Task<FundingSourceResponse> RemoveAsync(long ID)
        {
            var existingFundingSource = await _fundingSourceRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingFundingSource == null)
            {
                return new FundingSourceResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _fundingSourceRepository.Remove(existingFundingSource);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new FundingSourceResponse(existingFundingSource);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"FundingSourceService.RemoveAsync Error: {Environment.NewLine}");
                    return new FundingSourceResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        public async Task<FundingSourceResponse> Update(FundingSource fundingSource)
        {
            var existingFundingSource = await _fundingSourceRepository.FindByIdAsync(fundingSource.ID).ConfigureAwait(false);
            if (existingFundingSource == null)
            {
                return new FundingSourceResponse("Record Not Found");
            }
            else
            {
                existingFundingSource.Code = fundingSource.Code;
                existingFundingSource.Name = fundingSource.Name;
                existingFundingSource.ShortName = fundingSource.ShortName;
                
                try
                {
                    _fundingSourceRepository.Update(existingFundingSource);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new FundingSourceResponse(existingFundingSource);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"FundingSourceService.Update Error: {Environment.NewLine}");
                    return new FundingSourceResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<FundingSourceResponse> Update(long ID, FundingSource fundingSource)
        {
            try
            {
                _fundingSourceRepository.Update(ID, fundingSource);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new FundingSourceResponse(fundingSource);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"FundTypeService.Update Error: {Environment.NewLine}");
                return new FundingSourceResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
