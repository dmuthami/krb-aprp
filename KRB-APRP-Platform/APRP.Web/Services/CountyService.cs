using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class CountyService : ICountyService
    {

        private readonly ICountyRepository _countyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CountyService(ICountyRepository countyRepository, IUnitOfWork unitOfWork)
        {
            _countyRepository = countyRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<CountyResponse> AddAsync(County county)
        {
            try
            {

                await _countyRepository.AddAsync(county).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new CountyResponse(county);
            }
            catch (Exception ex)
            {
                return new CountyResponse($"Error occured while saving the record {ex.Message}");
            }
        }

        public async Task<CountyResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingCounty = await _countyRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingCounty == null)
                {
                    return new CountyResponse("Record Not Found");
                }
                else
                {
                    return new CountyResponse(existingCounty);
                }
            }
            catch (Exception ex)
            {
                //logging
                return new CountyResponse($"Error occured while getting the record {ex.Message}");
            }
        }

        public async Task<IEnumerable<County>> ListAsync()
        {
            try
            {
                return await _countyRepository.ListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //log error
                return Enumerable.Empty<County>();
            }
        }

        public async Task<IEnumerable<County>> ListByNameAsync(string CountyName)
        {
            try
            {
                return await _countyRepository.ListByNameAsync(CountyName).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //log error
                return Enumerable.Empty<County>();
            }
        }

        public async Task<CountyResponse> RemoveAsync(long countyID)
        {
            try
            {
                var existingCounty = await _countyRepository.FindByIdAsync(countyID).ConfigureAwait(false);
                if (existingCounty == null)
                {
                    return new CountyResponse("Record Not Found");
                }
                else
                {
                    _countyRepository.Remove(existingCounty);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new CountyResponse(existingCounty);

                }
            }
            catch (Exception ex)
            {
                //log error
                return new CountyResponse($"Error occured while removing the record : {ex.Message}");
            }
        }

        public async Task<CountyResponse> UpdateAsync(County county)
        {
            try
            {
                var existingCounty = await _countyRepository.FindByIdAsync(county.ID).ConfigureAwait(false);
                if (existingCounty == null)
                {
                    return new CountyResponse("Record Not Found");

                }
                else
                {
                    existingCounty.Code = county.Code;
                    existingCounty.Name = county.Name;

                    _countyRepository.Update(existingCounty);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new CountyResponse(existingCounty);
                }

            }
            catch (Exception ex)
            {
                //log
                return new CountyResponse($"Error occured while updating the record : {ex.Message}");
            }
        }
    }
}
