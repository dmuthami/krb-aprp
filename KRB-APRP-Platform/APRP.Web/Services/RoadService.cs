using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Services
{
    public class RoadService : IRoadService
    {

        private readonly IRoadRepository _roadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        private readonly IRoadSectionService _roadSectionService;

        public RoadService(IRoadRepository roadRepository, IUnitOfWork unitOfWork, ILogger<RoadService> logger,
            IRoadSectionService roadSectionService)
        {
            _roadRepository = roadRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _roadSectionService = roadSectionService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadResponse> AddAsync(Road road)
        {
            try
            {
                await _roadRepository.AddAsync(road).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadResponse(road); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.AddAsync Error: {Environment.NewLine}");
                return new RoadResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRoad = await _roadRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRoad == null)
                {
                    return new RoadResponse("Record Not Found");
                }
                else
                {
                    return new RoadResponse(existingRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.FindByIdAsync Error: {Environment.NewLine}");
                return new RoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadResponse> FindByNameAsyc(string RoadName)
        {
            try
            {
                var roadList = (await ListAsync().ConfigureAwait(false)).Roads;
                var existingRoad = roadList.Where(r => r.RoadName == RoadName).FirstOrDefault();

                if (existingRoad == null)
                {
                    return new RoadResponse("Record Not Found");
                }
                else
                {
                    return new RoadResponse(existingRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.FindByNameAsyc Error: {Environment.NewLine}");
                return new RoadResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadListResponse> RoadNumberAjaxListAsync(string RoadNumber)
        {
            try
            {
                var existingRoad = await _roadRepository.RoadNumberAjaxListAsync(RoadNumber).ConfigureAwait(true);
                if (existingRoad == null)
                {
                    return new RoadListResponse("Record Not Found");
                }
                else
                {
                    return new RoadListResponse(existingRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.RoadNumberAjaxListAsync Error: {Environment.NewLine}");
                return new RoadListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadListResponse> ListAsync()
        {
            try
            {
                var roadList = await _roadRepository.ListAsync().ConfigureAwait(false);

                if (roadList == null)
                {
                    return new RoadListResponse("Record Not Found");
                }
                else
                {
                    return new RoadListResponse(roadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.ListAsync Error: {Environment.NewLine}");
                return new RoadListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadListResponse> ListByNameAsync(string RoadName)
        {
            try
            {
                var roadList = (await ListAsync().ConfigureAwait(false)).Roads;
                var roads = roadList.Where(r => r.RoadName.Contains(RoadName));
                if (roads == null)
                {
                    return new RoadListResponse("Record Not Found");
                }
                else
                {
                    return new RoadListResponse(roads);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.ListByNameAsync Error: {Environment.NewLine}");
                return new RoadListResponse(Enumerable.Empty<Road>());
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadResponse> RemoveAsync(long RoadID)
        {
            var existingRoad = await _roadRepository.FindByIdAsync(RoadID).ConfigureAwait(false);
            if (existingRoad == null)
            {
                return new RoadResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _roadRepository.Remove(existingRoad);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new RoadResponse(existingRoad);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RoadService.RemoveAsync Error: {Environment.NewLine}");
                    return new RoadResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadResponse> UpdateAsync(Road road)
        {
            var existingRoad = await _roadRepository.FindByIdAsync(road.ID).ConfigureAwait(false);
            if (existingRoad == null)
            {
                return new RoadResponse("Record Not Found");
            }
            else
            {
                existingRoad.RoadNumber = road.RoadNumber;
                existingRoad.RoadName = road.RoadName;
                try
                {
                    _roadRepository.Update(existingRoad);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new RoadResponse(existingRoad);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RoadService.UpdateAsync Error: {Environment.NewLine}");
                    return new RoadResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadResponse> FindByRoadNumberAsync(string RoadNumber)
        {
            try
            {
                var existingRoad = await _roadRepository.FindByRoadNumberAsync(RoadNumber).ConfigureAwait(false);
                if (existingRoad == null)
                {
                    return new RoadResponse("Record Not Found");
                }
                else
                {
                    return new RoadResponse(existingRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.FindByRoadNumberAsync Error: {Environment.NewLine}");
                return new RoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadListResponse> ListAsync(Authority authority)
        {
            try
            {
                IEnumerable<Road> roadList = null;
                var resp = await _roadRepository.ListAsync(authority).ConfigureAwait(false);
                var objectResult = (ObjectResult)resp;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        roadList = (IEnumerable<Road>)result2.Value;
                    }
                }

                if (roadList == null)
                {
                    return new RoadListResponse("Record Not Found");
                }
                else
                {
                    return new RoadListResponse(roadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.ListAsync Error: {Environment.NewLine}");
                return new RoadListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadListResponse> GetRoadWithSectionsAsync(Authority authority, string SurfaceType)
        {
            try
            {

                var roadList = await _roadRepository.GetRoadWithSectionsAsync(authority).ConfigureAwait(false);

                //Get roadsections for agency
                var roadSectionResp = await _roadSectionService.GetRoadSectionsForAgencyAsync(authority, SurfaceType).ConfigureAwait(false);
                var roadSection = roadSectionResp.RoadSectionList;

                //Required Road IDS
                var requiredRoadIDs = roadSection.Select(x => x.RoadId).ToArray();

                //Filter out and return roads with road sections only
                var roadsWithLoadedSectionList = roadList.Where(r => requiredRoadIDs.Contains(r.ID)).ToList();

                if (roadsWithLoadedSectionList == null)
                {
                    return new RoadListResponse("Record Not Found");
                }
                else
                {
                    return new RoadListResponse(roadsWithLoadedSectionList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.ListAsync Error: {Environment.NewLine}");
                return new RoadListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadViewResponse> ListViewAsync()
        {
            try
            {
                var roadList = await _roadRepository.ListViewAsync().ConfigureAwait(false);
                IQueryable<RoadViewModel> RoadsData;
                RoadsData =
                 from roads
                 in roadList
                 select new RoadViewModel()
                 {
                     id = roads.ID,
                     road_number = roads.RoadNumber,
                     road_name = roads.RoadName,
                     authority_id = roads.AuthorityId,
                     authority_name = roads.Authority.Name,
                     pulled_from_gis = roads.PulledSectionsFromGIS
                 };

                if (roadList == null)
                {
                    return new RoadViewResponse("Record Not Found");
                }
                else
                {
                    return new RoadViewResponse(RoadsData);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.ListAsync Error: {Environment.NewLine}");
                return new RoadViewResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadViewResponse> ListViewAsync(Authority authority)
        {
            try
            {
                var roadList = await _roadRepository.ListViewAsync(authority).ConfigureAwait(false);
                IQueryable<RoadViewModel> RoadsData;
                RoadsData =
                 from roads in roadList
                 select new RoadViewModel()
                 {
                     id = roads.ID,
                     road_number = roads.RoadNumber,
                     road_name = roads.RoadName,
                     authority_id = roads.AuthorityId,
                     authority_name = roads.Authority.Name,
                     pulled_from_gis = roads.PulledSectionsFromGIS
                 };

                if (roadList == null)
                {
                    return new RoadViewResponse("Record Not Found");
                }
                else
                {
                    return new RoadViewResponse(RoadsData);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.ListAsync Error: {Environment.NewLine}");
                return new RoadViewResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadViewWithARICSResponse> ListViewWithAricsAsync(Authority authority, int? Year)
        {
            try
            {
                var roadList = await _roadRepository.ListViewWithAricsAsync(authority, Year).ConfigureAwait(false);
                IQueryable<RoadViewWithARICSModel> RoadsData;
                RoadsData =
                 from roads in roadList
                 select new RoadViewWithARICSModel()
                 {
                     id = roads.ID,
                     road_number = roads.RoadNumber,
                     road_name = roads.RoadName,
                     authority_id = roads.AuthorityId,
                     authority_name = roads.Authority.Name,
                     RoadConditions = roads.RoadConditions
                     //ard = roads.RoadConditions.FirstOrDefault().ARD
                 };

                if (roadList == null)
                {
                    return new RoadViewWithARICSResponse("Record Not Found");
                }
                else
                {
                    return new RoadViewWithARICSResponse(RoadsData);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.ListAsync Error: {Environment.NewLine}");
                return new RoadViewWithARICSResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadViewWithARICSResponse> ListViewWithAricsAsync(int? Year)
        {
            try
            {
                var roadList = await _roadRepository.ListViewWithAricsAsync(Year).ConfigureAwait(false);
                IQueryable<RoadViewWithARICSModel> RoadsData;
                RoadsData =
                 from roads in roadList
                 select new RoadViewWithARICSModel()
                 {
                     id = roads.ID,
                     road_number = roads.RoadNumber,
                     road_name = roads.RoadName,
                     authority_id = roads.AuthorityId,
                     authority_name = roads.Authority.Name,
                     ard = roads.RoadConditions.FirstOrDefault().ARD
                 };

                if (roadList == null)
                {
                    return new RoadViewWithARICSResponse("Record Not Found");
                }
                else
                {
                    return new RoadViewWithARICSResponse(RoadsData);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.ListAsync Error: {Environment.NewLine}");
                return new RoadViewWithARICSResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadResponse> FindByRoadNumberAsync(long AuthorityId, string RoadNumber)
        {
            try
            {
                var existingRoad = await _roadRepository.FindByRoadNumberAsync(AuthorityId, RoadNumber).ConfigureAwait(false);
                if (existingRoad == null)
                {
                    return new RoadResponse("Record Not Found");
                }
                else
                {
                    return new RoadResponse(existingRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.FindByRoadNumberAsync Error: {Environment.NewLine}");
                return new RoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadResponse> UpdateAsync(long ID, Road road)
        {
            try
            {
                _roadRepository.Update(ID, road);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadResponse(road);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSectionService.Update Error: {Environment.NewLine}");
                return new RoadResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadResponse> FindByIdAsync(long ID, int ARICSYear)
        {
            try
            {
                var existingRoad = await _roadRepository.FindByIdAsync(ID, ARICSYear).ConfigureAwait(false);
                if (existingRoad == null)
                {
                    return new RoadResponse("Record Not Found");
                }
                else
                {
                    return new RoadResponse(existingRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.FindByIdAsync Error: {Environment.NewLine}");
                return new RoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadResponse> DetachFirstEntryAsync(Road road)
        {
            try
            {
                await _roadRepository.DetachFirstEntryAsync(road).ConfigureAwait(false);

                return new RoadResponse(road); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.DetachFirstEntryAsync Error: {Environment.NewLine}");
                return new RoadResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        public async Task<RoadResponse> FindByDisbursementEntryAsync(Road road)
        {
            try
            {
                var existingRoad = await _roadRepository.FindByDisbursementEntryAsync(road).ConfigureAwait(false);
                if (existingRoad == null)
                {
                    return new RoadResponse("Records Not Found");
                }
                else
                {
                    return new RoadResponse(existingRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.ListAsync Error: {Environment.NewLine}");
                return new RoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }
    }
}
