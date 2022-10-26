using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Services
{
    public class ItemActivityUnitCostRateService : IItemActivityUnitCostRateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IItemActivityUnitCostRateRepository _iItemActivityUnitCostRateRepository;
        public ItemActivityUnitCostRateService(IUnitOfWork unitOfWork, ILogger<ARICSYearService> logger,
            IItemActivityUnitCostRateRepository iItemActivityUnitCostRateRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _iItemActivityUnitCostRateRepository = iItemActivityUnitCostRateRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> AddAsync(Domain.Models.ItemActivityUnitCostRate itemActivityUnitCostRate)
        {
            try
            {
                var iActionResult = await _iItemActivityUnitCostRateRepository.AddAsync(
                    itemActivityUnitCostRate).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"ItemActivityUnitCostRateService.AddAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> FindByFinancialYearAuthorityAndItemUnitCostAsync(long FinancialYearId, long AuthorityId, long ItemActivityUnitCostId)
        {
            try
            {
                var iActionResult = await _iItemActivityUnitCostRateRepository.FindByFinancialYearAuthorityAndItemUnitCostAsync(
                    FinancialYearId, AuthorityId, ItemActivityUnitCostId).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ItemActivityUnitCostRateService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> FindByIdAsync(long ID)
        {
            try
            {
                var iActionResult = await _iItemActivityUnitCostRateRepository.FindByIdAsync(ID).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ItemActivityUnitCostRateService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> ListAsync()
        {
            try
            {
                var iActionResult = await _iItemActivityUnitCostRateRepository.ListAsync().ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ItemActivityUnitCostRateService.ListAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> RemoveAsync(long ID)
        {
            throw new System.NotImplementedException();
        }

        public async Task<GenericResponse> Update(Domain.Models.ItemActivityUnitCostRate itemActivityUnitCostRate)
        {
            throw new System.NotImplementedException();
        }
    }
}