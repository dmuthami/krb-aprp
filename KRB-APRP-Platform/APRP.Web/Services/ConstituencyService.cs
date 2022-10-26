using APRP.Web.ViewModels.CountyVM;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class ConstituencyService : IConstituencyService
    {

        private readonly IConstituencyRepository _constituencyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public ConstituencyService(IConstituencyRepository constituencyRepository, IUnitOfWork unitOfWork, ILogger<ConstituencyService> logger)
        {
            _constituencyRepository = constituencyRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ConstituencyResponse> AddAsync(Constituency constituency)
        {

            try
            {
                await _constituencyRepository.AddAsync(constituency).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ConstituencyResponse(constituency);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CommunicationService.AddAsync Error: {Environment.NewLine}");
                return new ConstituencyResponse($"Error occured while saving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ConstituencyResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingConstituency = await _constituencyRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingConstituency == null)
                {
                    return new ConstituencyResponse("No Record Found");
                }
                else
                {
                    return new ConstituencyResponse(existingConstituency);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CommunicationService.FindByAsync Error: {Environment.NewLine}");
                return new ConstituencyResponse($"Error occured while saving the record : {Ex.Message}");
            }
        }

        public async Task<IList<Constituency>> GetConstituenciesForCounty(CountyViewModel CVM)
        {
            var countyConstituencies =(IList<Constituency>) await _constituencyRepository.GetConstituenciesForCounty(CVM).ConfigureAwait(false);
            return countyConstituencies;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ConstituencyResponse> GetConstituencyAndCounty(long ID)
        {
            try
            {
                var existingConstituency = await _constituencyRepository.GetConstituencyAndCounty(ID).ConfigureAwait(false);
                if (existingConstituency == null)
                {
                    return new ConstituencyResponse("No Record Found");
                }
                else
                {
                    return new ConstituencyResponse(existingConstituency);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CommunicationService.GetConstituencyAndCounty Error: {Environment.NewLine}");
                return new ConstituencyResponse($"Error occured while saving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ConstituencyListResponse> ListAsync()
        {
            try
            {
                var existingConstituencyList = await _constituencyRepository.ListAsync().ConfigureAwait(false); 
                if (existingConstituencyList == null)
                {
                    return new ConstituencyListResponse("Record NotFound");
                }
                else
                {
                    return new ConstituencyListResponse(existingConstituencyList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CommunicationService.ListAsync Error: {Environment.NewLine}");
                return new ConstituencyListResponse($"Error occurred while removing the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ConstituencyListResponse> ListByNameAsync(string ConstituencyName)
        {
            try
            {
                var existingConstituencyList =  await _constituencyRepository.ListByNameAsync(ConstituencyName).ConfigureAwait(false);
                if (existingConstituencyList == null)
                {
                    return new ConstituencyListResponse("Record NotFound");
                }
                else
                {
                    return new ConstituencyListResponse(existingConstituencyList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CommunicationService.ListByNameAsync Error: {Environment.NewLine}");
                return new ConstituencyListResponse($"Error occurred while removing the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ConstituencyResponse> RemoveAsync(long constituencyID)
        {
            try
            {
                var existingConstituency = await _constituencyRepository.FindByIdAsync(constituencyID).ConfigureAwait(false);
                if (existingConstituency == null)
                {
                    return new ConstituencyResponse("Record NotFound");
                }
                else
                {
                    _constituencyRepository.Remove(existingConstituency);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new ConstituencyResponse(existingConstituency);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CommunicationService.RemoveAsync Error: {Environment.NewLine}");
                return new ConstituencyResponse($"Error occurred while removing the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ConstituencyResponse> UpdateAsync(Constituency constituency)
        {
            try
            {
                var existingConstituency = await _constituencyRepository.FindByIdAsync(constituency.ID).ConfigureAwait(false);
                if (existingConstituency == null)
                {
                    return new ConstituencyResponse("Record Not Found");
                }
                else
                {
                    existingConstituency.Name = constituency.Name;
                    existingConstituency.Code = constituency.Code;

                    _constituencyRepository.Update(existingConstituency);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new ConstituencyResponse(constituency);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CommunicationService.Update Error: {Environment.NewLine}");
                return new ConstituencyResponse($"Error occured while updating the record : {Ex.Message}");
            }
        }

        Task<ConstituencyListResponse> IConstituencyService.GetConstituenciesForCounty(CountyViewModel CVM)
        {
            throw new NotImplementedException();
        }
    }
}
