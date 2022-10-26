using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class FinancialYearService : IFinancialYearService
    {
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public FinancialYearService(IFinancialYearRepository financialYearRepository, IUnitOfWork unitOfWork, ILogger<FinancialYearService> logger)
        {
            _financialYearRepository = financialYearRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<FinancialYearResponse> AddAsync(FinancialYear financialYear)
        {
            try
            {
                await _financialYearRepository.AddAsync(financialYear).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                //Ensure only one record has iscurrent==1
                await _financialYearRepository.SetAllToNotCurrent(financialYear).ConfigureAwait(false);

                return new FinancialYearResponse(financialYear); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"FinancialYearService.AddAsync Error: {Environment.NewLine}");
                string errMessage = Ex.InnerException.InnerException.Message ?? "No inner exception";
                return new FinancialYearResponse($"Error occured while saving the record : {Ex.Message}:" +
                    $"Inner Message:{errMessage}");
            }
        }

        public async Task<FinancialYearResponse> FindPreviousYearFromCurrentYear(FinancialYear financialYear)
        {
            try
            {
                //get code for last year
                var codeFromCurrentYear = financialYear.Code.Substring(0, 4);
                var codeLastYear = int.Parse(codeFromCurrentYear) - 1;

                var existingRecord = await _financialYearRepository.FindPreviousYearFromCurrentYear("" + codeLastYear + "").ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new FinancialYearResponse("Record Not Found");
                }
                else
                {
                    return new FinancialYearResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"FinancialYearService.FindByIdAsync Error: {Environment.NewLine}");
                return new FinancialYearResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<FinancialYearResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord = await _financialYearRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new FinancialYearResponse("Record Not Found");
                }
                else
                {
                    return new FinancialYearResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"FinancialYearService.FindByIdAsync Error: {Environment.NewLine}");
                return new FinancialYearResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<FinancialYearResponse> FindCurrentYear()
        {
            try
            {
                var existingRecord = await _financialYearRepository.FindCurrentYear().ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new FinancialYearResponse("Record Not Found");
                }
                else
                {
                    return new FinancialYearResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"FinancialYearService.AddAsync Error: {Environment.NewLine}");
                return new FinancialYearResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }


        public async Task<IEnumerable<FinancialYear>> ListAsync()
        {
            return await _financialYearRepository
                .ListAsync().ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<FinancialYearResponse> RemoveAsync(long ID)
        {
            var existingRecord = await _financialYearRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new FinancialYearResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _financialYearRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new FinancialYearResponse(existingRecord);
                }
                catch (Exception Ex)
                {
                
                    String msg = null;
                    if (Ex.InnerException!=null)
                    {
                        msg = $"Error occured while retrieving the record : {Ex.Message}" +
                            $"More Details : {Ex.InnerException.ToString()}";
                    }
                    else
                    {
                        msg = $"Error occured while retrieving the record : {Ex.Message}";
                    }
                    _logger.LogError(Ex, $"FinancialYearService.RemoveAsync Error:  {Environment.NewLine} {msg}");
                    return new FinancialYearResponse($"Error occured while retrieving the record : {Ex.Message}" +
                        $" {Environment.NewLine} See logs for more details");
                }
            }
        }

        public async Task<FinancialYearResponse> Update(FinancialYear financialYear)
        {
            if (financialYear != null)
            {

                var existingRecord = await _financialYearRepository.FindByIdAsync(financialYear.ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new FinancialYearResponse("Record Not Found");
                }
                else
                {
                    existingRecord.Code = financialYear.Code;
                    existingRecord.Summary = financialYear.Summary;

                    try
                    {
                        _financialYearRepository.Update(existingRecord);
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                        return new FinancialYearResponse(existingRecord);
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"FinancialYearService.Update Error: {Environment.NewLine}");
                        return new FinancialYearResponse($"Error occured while retrieving the record : {Ex.Message}");
                    }
                }

            }
            return new FinancialYearResponse("Invalid record");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<FinancialYearResponse> Update(long ID, FinancialYear financialYear)
        {
            try
            {
                _financialYearRepository.Update(ID, financialYear);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                //Ensure only one record has iscurrent==1
                await _financialYearRepository.SetAllToNotCurrent(financialYear).ConfigureAwait(false);
                return new FinancialYearResponse(financialYear);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationService.Update Error: {Environment.NewLine}");
                return new FinancialYearResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        public async Task<GenericResponse> ListAsync(string Code)
        {
            try
            {
                var iActionResult = await _financialYearRepository
                    .ListAsync(Code).
                    ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"BudgetCeilingComputationService.ListAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }
    }
}
