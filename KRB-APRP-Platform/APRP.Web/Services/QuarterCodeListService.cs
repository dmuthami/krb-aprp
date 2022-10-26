using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class QuarterCodeListService : IQuarterCodeListService
    {
        private readonly IQuarterCodeListRepository _quarterCodeListRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public QuarterCodeListService(IQuarterCodeListRepository quarterCodeListRepository, IUnitOfWork unitOfWork
            , ILogger<QuarterCodeListService> logger)
        {
            _quarterCodeListRepository = quarterCodeListRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<QuarterCodeListResponse> AddAsync(QuarterCodeList quarterCodeList)
        {
            try
            {
                await _quarterCodeListRepository.AddAsync(quarterCodeList).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new QuarterCodeListResponse(quarterCodeList); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"QuarterCodeListService.AddAsync Error: {Environment.NewLine}");
                return new QuarterCodeListResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<QuarterCodeListResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingQuarterCodeList = await _quarterCodeListRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingQuarterCodeList == null)
                {
                    return new QuarterCodeListResponse("Records Not Found");
                }
                else
                {
                    return new QuarterCodeListResponse(existingQuarterCodeList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"QuarterCodeListService.ListAsync Error: {Environment.NewLine}");
                return new QuarterCodeListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<QuarterCodeListResponse> FindByNameAsync(string Item)
        {
            try
            {
                var existingQuarterCodeList = await _quarterCodeListRepository.FindByNameAsync(Item).ConfigureAwait(false);
                if (existingQuarterCodeList == null)
                {
                    return new QuarterCodeListResponse("Records Not Found");
                }
                else
                {
                    return new QuarterCodeListResponse(existingQuarterCodeList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"QuarterCodeListService.ListAsync Error: {Environment.NewLine}");
                return new QuarterCodeListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<QuarterCodeListListResponse> ListAsync()
        {
            try
            {
                var existingQuarterCodeLists = await _quarterCodeListRepository.ListAsync().ConfigureAwait(false);
                if (existingQuarterCodeLists == null)
                {
                    return new QuarterCodeListListResponse("Records Not Found");
                }
                else
                {
                    return new QuarterCodeListListResponse(existingQuarterCodeLists);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"QuarterCodeListService.ListAsync Error: {Environment.NewLine}");
                return new QuarterCodeListListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<QuarterCodeListResponse> RemoveAsync(long ID)
        {
            var existingQuarterCodeList = await _quarterCodeListRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingQuarterCodeList == null)
            {
                return new QuarterCodeListResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _quarterCodeListRepository.Remove(existingQuarterCodeList);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new QuarterCodeListResponse(existingQuarterCodeList);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"QuarterCodeListService.RemoveAsync Error: {Environment.NewLine}");
                    return new QuarterCodeListResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<QuarterCodeListResponse> Update(QuarterCodeList quarterCodeList)
        {
            var existingQuarterCodeList = await _quarterCodeListRepository.FindByIdAsync(quarterCodeList.ID).ConfigureAwait(false);
            if (existingQuarterCodeList == null)
            {
                return new QuarterCodeListResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _quarterCodeListRepository.Update(quarterCodeList);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new QuarterCodeListResponse(quarterCodeList);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"QuarterCodeListService.Update Error: {Environment.NewLine}");
                    return new QuarterCodeListResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<QuarterCodeListResponse> Update(long ID, QuarterCodeList quarterCodeList)
        {
            try
            {
                _quarterCodeListRepository.Update(ID, quarterCodeList);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new QuarterCodeListResponse(quarterCodeList);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"QuarterCodeListService.Update Error: {Environment.NewLine}");
                return new QuarterCodeListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
