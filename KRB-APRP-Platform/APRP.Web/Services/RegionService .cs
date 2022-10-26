using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegionService(IRegionRepository regionRepository, IUnitOfWork unitOfWork)
        {
            _regionRepository = regionRepository;
            _unitOfWork = unitOfWork;

        }
        public async Task<IEnumerable<Region>> ListAsync()
        {
            return await _regionRepository.ListAsync().ConfigureAwait(false);
        }

        public async Task<RegionResponse> GetRegionAsync(long regionId)
        {
            try
            {
                var existingRecord = await _regionRepository.GetRegionAsync(regionId).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new RegionResponse("Record Not Found");
                }
                else
                {
                    return new RegionResponse(existingRecord);
                }
            }
            catch (Exception ex)
            {
                //logging
                return new RegionResponse($"Error occured while getting the record {ex.Message}");
            }
        }

        public async Task<IEnumerable<Region>> ListRegionsByAuthority(long authorityID)
        {
            return await _regionRepository.ListRegionsByAuthority(authorityID).ConfigureAwait(false);
        }
    }
}
