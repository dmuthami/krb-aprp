using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class FundTypeService : IFundTypeService
    {
        private readonly IFundTypeRepository _fundTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public FundTypeService(IFundTypeRepository fundTypeRepository, IUnitOfWork unitOfWork, ILogger<FundTypeService> logger)
        {
            _fundTypeRepository = fundTypeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<FundTypeResponse> AddAsync(FundType fundType)
        {
            try
            {
                await _fundTypeRepository.AddAsync(fundType).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new FundTypeResponse(fundType); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"FundTypeService.AddAsync Error: {Environment.NewLine}");
                return new FundTypeResponse($"Error occured while saving the fund type record : {Ex.Message}");
            }
        }

        public async Task<FundTypeResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingFundType = await _fundTypeRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingFundType == null)
                {
                    return new FundTypeResponse("Record Not Found");
                }
                else
                {
                    return new FundTypeResponse(existingFundType);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"FundTypeService.FindByIdAsync Error: {Environment.NewLine}");
                return new FundTypeResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<IEnumerable<FundType>> ListAsync()
        {
            return await _fundTypeRepository.ListAsync().ConfigureAwait(false);
        }

        public async Task<FundTypeResponse> RemoveAsync(long ID)
        {
            var existingFundType = await _fundTypeRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingFundType == null)
            {
                return new FundTypeResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _fundTypeRepository.Remove(existingFundType);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new FundTypeResponse(existingFundType);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"FundTypeService.RemoveAsync Error: {Environment.NewLine}");
                    return new FundTypeResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<FundTypeResponse> Update(FundType fundType)
        {
            var existingFundType = await _fundTypeRepository.FindByIdAsync(fundType.ID).ConfigureAwait(false);
            if (existingFundType == null)
            {
                return new FundTypeResponse("Record Not Found");
            }
            else
            {
                existingFundType.Code = fundType.Code;
                existingFundType.Name = fundType.Name;
                
                try
                {
                    _fundTypeRepository.Update(existingFundType);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new FundTypeResponse(existingFundType);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"FundTypeService.Update Error: {Environment.NewLine}");
                    return new FundTypeResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<FundTypeResponse> Update(long ID, FundType fundType)
        {
            try
            {
                _fundTypeRepository.Update(ID, fundType);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new FundTypeResponse(fundType);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"FundTypeService.Update Error: {Environment.NewLine}");
                return new FundTypeResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
