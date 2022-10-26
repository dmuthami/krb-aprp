using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels.UserViewModels;

namespace APRP.Web.Services
{
    public class RoadSectionService : IRoadSectionService
    {

        private readonly IRoadSectionRepository _roadSectionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RoadSectionService(IRoadSectionRepository roadSectionRepository, IUnitOfWork unitOfWork,
            ILogger<RoadSectionService> logger)
        {
            _roadSectionRepository = roadSectionRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general Exception types", Justification = "<Pending>")]
        public async Task<RoadSectionResponse> AddAsync(RoadSection roadSection)
        {
            try
            {
                await _roadSectionRepository.AddAsync(roadSection).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadSectionResponse(roadSection);

            }
            catch (Exception Ex)
            {
                //log error
                _logger.LogError(Ex,$"RoadSectionService.AddAsync Error{Environment.NewLine}");
                return new RoadSectionResponse($"Error occured while adding the record : {Ex.Message}");
            }
        }

        public async Task<RoadSectionResponse> DetachFirstEntryAsync(RoadSection roadSection)
        {
            try
            {
                await _roadSectionRepository.DetachFirstEntryAsync(roadSection).ConfigureAwait(false);

                return new RoadSectionResponse(roadSection); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSectionService.DetachFirstEntryAsync Error: {Environment.NewLine}");
                return new RoadSectionResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSectionResponse> FindByDisbursementEntryAsync(RoadSection roadSection)
        {
            try
            {
                var existingRoadSecton = await _roadSectionRepository.FindByDisbursementEntryAsync(roadSection).ConfigureAwait(false);
                if (existingRoadSecton == null)
                {
                    return new RoadSectionResponse("Records Not Found");
                }
                else
                {
                    return new RoadSectionResponse(existingRoadSecton);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSectionService.ListAsync Error: {Environment.NewLine}");
                return new RoadSectionResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general Exception types", Justification = "<Pending>")]
        public async Task<RoadSectionResponse> FindByIdAsync(long ID)
        {
            try
            {
                var ExistingRoadSection = await _roadSectionRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (ExistingRoadSection == null)
                {
                    return new RoadSectionResponse("Record Not Found");
                }
                else
                {
                    return new RoadSectionResponse(ExistingRoadSection);
                }

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSectionService.FindByIdAsync Error: {Environment.NewLine}");
                return new RoadSectionResponse($"Error occured while finding the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSectionResponse> FindBySectionIdAsync(string SectionID)
        {
            try
            {
                var ExistingRoadSection = await _roadSectionRepository.FindBySectionIdAsync(SectionID).ConfigureAwait(false);
                if (ExistingRoadSection == null)
                {
                    return new RoadSectionResponse("Record Not Found");
                }
                else
                {
                    return new RoadSectionResponse(ExistingRoadSection);
                }

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSectionService.FindByIdAsync Error: {Environment.NewLine}");
                return new RoadSectionResponse($"Error occured while finding the record : {Ex.Message}");
            }
        }

        public async Task<RoadSectionResponse> FindBySectionIdAsync(string SectionID, long AuthorityId)
        {
            try
            {
                var ExistingRoadSection = await _roadSectionRepository.FindBySectionIdAsync(SectionID, AuthorityId).ConfigureAwait(false);
                if (ExistingRoadSection == null)
                {
                    return new RoadSectionResponse("Record Not Found");
                }
                else
                {
                    return new RoadSectionResponse(ExistingRoadSection);
                }

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSectionService.FindByIdAsync Error: {Environment.NewLine}");
                return new RoadSectionResponse($"Error occured while finding the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSectionListResponse> GetRoadSectionsForAgencyAsync(Authority authority, string SurfaceType)
        {
            try
            {
                var roadList = await _roadSectionRepository.GetRoadSectionsForAgencyAsync(authority,  SurfaceType).ConfigureAwait(false);


                if (roadList == null)
                {
                    return new RoadSectionListResponse("Record Not Found");
                }
                else
                {
                    return new RoadSectionListResponse(roadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.ListAsync Error: {Environment.NewLine}");
                return new RoadSectionListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general Exception types", Justification = "<Pending>")]
        public async Task<RoadSectionListResponse> ListAsync()
        {            
            try
            {
                var roadList = await _roadSectionRepository.ListAsync().ConfigureAwait(false);
                if (roadList == null)
                {
                    return new RoadSectionListResponse("Records Not Found");
                }
                else
                {

                    return new RoadSectionListResponse(roadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSectionService.RemoveAsync Error: {Environment.NewLine}");
                return new RoadSectionListResponse($"Error occurred will removing the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general Exception types", Justification = "<Pending>")]
        public async Task<RoadSectionListResponse> ListByRoadIdAsync(long roadID)
        {
            try
            {
                var roadList = await _roadSectionRepository.ListByRoadIdAsync(roadID).ConfigureAwait(false);
                if (roadList == null)
                {
                    return new RoadSectionListResponse("Records Not Found");
                }
                else
                {

                    return new RoadSectionListResponse(roadList);
                }
            }
            catch (Exception Ex)
            {
                //log error
                _logger.LogError(Ex,$"RoadSectionService.ListByRoadIdAsync Error{Environment.NewLine}");
                return new RoadSectionListResponse($"Error occurred will removing the record : {Ex.Message}");
            }
          
        }

       public async Task<IEnumerable<RoadSection>> ListUnPlannedSectionsByRoadIdAsync (long roadId, long financialYearId)
        {
            return await _roadSectionRepository.ListUnPlannedSectionsByRoadIdAsync(roadId,financialYearId).ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSectionViewModelResponse> ListViewAsync()
        {
            try
            {
                var roadList = await _roadSectionRepository.ListViewAsync().ConfigureAwait(false);
                IQueryable<RoadSectionViewModel> RoadSectionsData;
                RoadSectionsData =
                 from roadsections
                 in roadList
                 select new RoadSectionViewModel()
                 {
                     id = roadsections.ID,
                     road_number = roadsections.Road.RoadNumber,
                     section_name = roadsections.SectionName,
                     length = Math.Round(roadsections.Length,3, MidpointRounding.AwayFromZero)
                 };

                if (roadList == null)
                {
                    return new RoadSectionViewModelResponse("Record Not Found");
                }
                else
                {
                    return new RoadSectionViewModelResponse(RoadSectionsData);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.ListAsync Error: {Environment.NewLine}");
                return new RoadSectionViewModelResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSectionViewModelResponse> ListViewAsync(Authority authority)
        {
            try
            {
                var roadList = await _roadSectionRepository.ListViewAsync(authority).ConfigureAwait(false);
                IQueryable<RoadSectionViewModel> RoadSectionsData;
                RoadSectionsData =
                 from roadsections
                 in roadList
                 select new RoadSectionViewModel()
                 {
                     id = roadsections.ID,
                     road_number = roadsections.Road.RoadNumber,
                     section_name = roadsections.SectionName,
                     length = Math.Round(roadsections.Length, 3, MidpointRounding.AwayFromZero),
                     roadid = roadsections.RoadId
                 };

                if (roadList == null)
                {
                    return new RoadSectionViewModelResponse("Record Not Found");
                }
                else
                {
                    return new RoadSectionViewModelResponse(RoadSectionsData);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.ListAsync Error: {Environment.NewLine}");
                return new RoadSectionViewModelResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general Exception types", Justification = "<Pending>")]
        public async Task<RoadSectionResponse> RemoveAsync(long ID)
        {
            try
            {
                var ExistingRoadSection = await _roadSectionRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if(ExistingRoadSection == null)
                {
                    return new RoadSectionResponse("Record Not Found");
                }
                else
                {
                    _roadSectionRepository.Remove(ExistingRoadSection);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new RoadSectionResponse(ExistingRoadSection);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSectionService.RemoveAsync Error: {Environment.NewLine}");
                return new RoadSectionResponse($"Error occurred while removing the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSectionResponse> UpdateAsync(long ID, RoadSection roadSection)
        {
            try
            {
                _roadSectionRepository.Update(ID, roadSection);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadSectionResponse(roadSection);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSectionService.Update Error: {Environment.NewLine}");
                return new RoadSectionResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general Exception types", Justification = "<Pending>")]
        public async Task<RoadSectionResponse> UpdateAsync(RoadSection roadSection)
        {
            try
            {
                var ExistingRoadSection = await _roadSectionRepository.FindByIdAsync(roadSection.ID).ConfigureAwait(false);
                if(ExistingRoadSection == null)
                {
                    return new RoadSectionResponse("Record Not Found");
                }
                else
                {
                    ExistingRoadSection.Length = roadSection.Length;
                    ExistingRoadSection.SectionName = roadSection.SectionName;
                    ExistingRoadSection.Length = roadSection.Length;
                    
                    _roadSectionRepository.Update(ExistingRoadSection);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new RoadSectionResponse(ExistingRoadSection);
                }
            }
            catch(Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSectionService.UpdateAsync Error: {Environment.NewLine}");
                return new RoadSectionResponse($"Error occured while updating the record : {Ex.Message}");
            }
        }
    }
}
