using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels;

namespace APRP.Web.Services
{
    public class GISRoadService : IGISRoadService
    {

        private readonly IGISRoadRepository _roadGISRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IRoadService _roadService;
        private readonly IRoadSectionService _roadSectionService;
        private readonly IKuRARoadService _kURARoadService;
        private readonly ICountiesRoadService _countiesRoadService;
        private readonly IKenHARoadService _kenHARoadService;
        private readonly IKeRRARoadService _keRRARoadService;
        private readonly IKwSRoadService _kwSRoadService;
        private readonly IAuthorityService _authorityService;
        private readonly ISurfaceTypeService _surfaceTypeService;

        public GISRoadService(IGISRoadRepository roadGISRepository, IUnitOfWork unitOfWork
            , ILogger<GISRoadService> logger, IRoadService roadService, IKuRARoadService kURARoadService,
            IRoadSectionService roadSectionService, ICountiesRoadService countiesRoadService, IKenHARoadService kenHARoadService,
            IKeRRARoadService keRRARoadService, IKwSRoadService kwSRoadService, IAuthorityService authorityService,
            ISurfaceTypeService surfaceTypeService)
        {
            _roadGISRepository = roadGISRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _roadService = roadService;
            _kURARoadService = kURARoadService;
            _roadSectionService = roadSectionService;
            _countiesRoadService = countiesRoadService;
            _kenHARoadService = kenHARoadService;
            _keRRARoadService = keRRARoadService;
            _kwSRoadService = kwSRoadService;
            _authorityService = authorityService;
            _surfaceTypeService = surfaceTypeService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GISRoadResponse> AddAsync(GISRoad gISRoad)
        {
            try
            {
                await _roadGISRepository.AddAsync(gISRoad).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new GISRoadResponse(gISRoad); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISRoadService.AddAsync Error: {Environment.NewLine}");
                return new GISRoadResponse($"Error occured while saving the GIS road record : {Ex.Message}");
            }
        }

        public async Task<GISRoadResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingGISRoad = await _roadGISRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingGISRoad == null)
                {
                    return new GISRoadResponse("Record Not Found");
                }
                else
                {
                    return new GISRoadResponse(existingGISRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISRoadService.FindByAsync Error: {Environment.NewLine}");
                return new GISRoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GISRoadResponse> FindByNameAsyc(string GISRoadName)
        {
            try
            {
                var gISRoadList = await ListAsync().ConfigureAwait(false);
                var existingGISRoad = gISRoadList.Where(r => r.RoadName == GISRoadName).FirstOrDefault();

                if (existingGISRoad == null)
                {
                    return new GISRoadResponse("Record Not Found");
                }
                else
                {
                    return new GISRoadResponse(existingGISRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISRoadService.FindByNameAsync Error: {Environment.NewLine}");
                return new GISRoadResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        public async Task<GISRoadResponse> GetRoadLengthAsync(string RoadNumber)
        {
            try
            {
                var gISRoads = await _roadGISRepository.ListByRoadNumberAsync(RoadNumber).ConfigureAwait(false);

                if (gISRoads == null)
                {
                    return new GISRoadResponse($"GIS Records not found for Road:{RoadNumber}");
                }
                GISRoad GISRoad = new GISRoad();
                GISRoad.RoadNumber = RoadNumber;
                GISRoad.Length = await Task.Run(() => RoadLength(gISRoads)).ConfigureAwait(false);
                return new GISRoadResponse(GISRoad);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISRoadsController.GetRoadIDLength Error : " +
                   $"{Environment.NewLine}");
                //return NotFound()
                return new GISRoadResponse("Failed to compute Length");
            }
        }

        private decimal RoadLength(IEnumerable<GISRoad> gISRoads)
        {
            return gISRoads.Sum(x => x.Length);
        }

        public async Task<IEnumerable<GISRoad>> ListAsync()
        {
            return await _roadGISRepository.ListAsync().ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IEnumerable<GISRoad>> ListByNameAsync(string RoadName)
        {
            try
            {
                var roadList = await ListAsync().ConfigureAwait(false);
                return roadList.Where(r => r.RoadName.Contains(RoadName));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISRoadService.ListByNameAsync Error: {Environment.NewLine}");
                return Enumerable.Empty<GISRoad>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GISRoadResponse> RemoveAsync(long GISRoadID)
        {
            var existingGISRoad = await _roadGISRepository.FindByIdAsync(GISRoadID).ConfigureAwait(false);
            if (existingGISRoad == null)
            {
                return new GISRoadResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _roadGISRepository.Remove(existingGISRoad);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new GISRoadResponse(existingGISRoad);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"GISRoadService.RemoveAsync Error: {Environment.NewLine}");
                    return new GISRoadResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }


        public async Task<GISRoadResponse> UpdateAsync(GISRoad gISRoad)
        {
            var existingGISRoad = await _roadGISRepository.FindByIdAsync(gISRoad.ID).ConfigureAwait(false);
            if (existingGISRoad == null)
            {
                return new GISRoadResponse("Record Not Found");
            }
            else
            {
                existingGISRoad.RoadNumber = gISRoad.RoadNumber;
                existingGISRoad.RoadName = gISRoad.RoadName;
                try
                {
                    _roadGISRepository.Update(existingGISRoad);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new GISRoadResponse(existingGISRoad);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"GISRoadService.UpdateAsync Error: {Environment.NewLine}");
                    return new GISRoadResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GISRoadResponse> FindByRoadNumberAsync(string RoadNumber)
        {
            try
            {
                var existingGISRoad = await _roadGISRepository.FindByRoadNumberAsync(RoadNumber).ConfigureAwait(false);
                if (existingGISRoad == null)
                {
                    return new GISRoadResponse("Record Not Found");
                }
                else
                {
                    return new GISRoadResponse(existingGISRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISRoadService.FindByRoadNumberAsyncAsync Error: {Environment.NewLine}");
                return new GISRoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadResponse> PullRoadSectionFromGISAsync(long RoadID)
        {
            try
            {
                //Get road
                var roadRsp = await _roadService.FindByIdAsync(RoadID).ConfigureAwait(false);
                Road road = roadRsp.Road;
                if (road != null)
                {
                    //Get Authority
                    Authority authority = roadRsp.Road.Authority;

                    //Get all the road sections for the road for the respective authority
                    if (authority.ID == 3)//KURA
                    {
                        //Todo: Hardcoded id for KURA needs to be made dynamic
                        await PopulateKuRARoadSections(road).ConfigureAwait(false);
                    }
                    else if (authority.ID == 1)//KenHA
                    {
                        await PopulateKenHARoadSections(road).ConfigureAwait(false);
                    }
                    else if (authority.ID == 2)//KeRRA
                    {
                        await PopulateKeRRARoadSections(road).ConfigureAwait(false);
                    }
                    else if (authority.ID == 4)//KWS
                    {
                        await PopulateKwSRoadSections(road).ConfigureAwait(false);
                    }
                    else
                    {
                        //County road
                        await PopulateCountyRoadSections(road).ConfigureAwait(false);
                    }

                    return new RoadResponse(road);
                }
                else
                {
                    return new RoadResponse("Record Not Found");
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISRoadService.PullRoadSectionFromGISAsync Error: {Environment.NewLine}");
                return new RoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task PopulateKuRARoadSections(Road road)
        {
            //for each road section then add it to roadsections
            var kURAListResp = await _kURARoadService.ListAsync(road.RoadNumber).ConfigureAwait(false);
            IList<KuraRoad> kURARoadList = (IList<KuraRoad>)kURAListResp.KuraRoads;

            //For each Road Section add to Road Section Table
            Double len = 0;
            foreach (var kuraRd in kURARoadList)
            {
                //Road Section instatntiation
                RoadSection roadSection = new RoadSection();

                //Length
                bool result = Double.TryParse(kuraRd.Length.ToString(), out len);
                if (result == true)
                {
                    //in KM
                    roadSection.Length = len;
                }

                //Interval
                roadSection.Interval = 200;

                //Start and End Chainage
                roadSection.StartChainage = 0.0;
                roadSection.EndChainage = 0.0;

                //Road ID
                roadSection.RoadId = road.ID;

                //Surface Type
                roadSection.SurfaceTypeId = 5;//UnPaved
                var surfaceTypeResp = await _surfaceTypeService.FindByNameAsync(kuraRd.SurfaceType).ConfigureAwait(false);
                if (surfaceTypeResp.Success)
                {
                    roadSection.SurfaceTypeId = surfaceTypeResp.SurfaceType.ID;
                }
                roadSection.SurfaceType2 = kuraRd.SurfaceType2;
                roadSection.StartChainage = kuraRd.StartChainage;
                roadSection.EndChainage = kuraRd.EndChainage;
                roadSection.CW_Surf_Co = kuraRd.CW_Surf_Co;
                //Constituency intersected by the road
                roadSection.ConstituencyId = kuraRd.ConstituencyId;
                //Todo : Constituency ID hardcoded

                //Section Name
                roadSection.SectionName = kuraRd.Sec_Name;

                //Kura
                try
                {
                    roadSection.MunicipalityId = kuraRd.MunicipalityId;
                }
                catch (Exception Ex)
                {

                    _logger.LogError(Ex, $"GISRoadService.PopulateKuRARoadSections Error: {Environment.NewLine}");
                }

                //SectionID==RoadNumber in the case of KURA
                roadSection.SectionID = kuraRd.RdNum;

                if (roadSection.SectionID.Trim().Length > 0)
                {
                    //Try updating/Adding
                    try
                    {
                        // if section exists is true then update else Add
                        var roadSectFind = await _roadSectionService.FindBySectionIdAsync(roadSection.SectionID).ConfigureAwait(false);
                        RoadSection roadSectionfind = roadSectFind.RoadSection;

                        if (roadSectionfind == null)
                        {
                            //Add
                            var roadSectionResponse = await _roadSectionService.AddAsync(roadSection).ConfigureAwait(false);
                            roadSection = roadSectionResponse.RoadSection;
                        }
                        else
                        {
                            //Update
                            roadSectionfind.Length = roadSection.Length;
                            roadSectionfind.Interval = roadSection.Interval;
                            roadSectionfind.StartChainage = roadSection.StartChainage;
                            roadSectionfind.EndChainage = roadSection.EndChainage;
                            //Road ID
                            roadSectionfind.RoadId = roadSection.RoadId;
                            //Surface Type
                            roadSectionfind.SurfaceTypeId = roadSection.SurfaceTypeId;//Gravel
                                                                                      //Todo : surface Type hard coded in PopulateKuRARoadSections
                                                                                      //Constituency intersected by the road
                            roadSectionfind.ConstituencyId = roadSection.ConstituencyId;
                            //Todo : Constituency ID hardcoded
                            //Section Name
                            roadSectionfind.SectionName = roadSection.SectionName;
                            //SectionID==RoadNumber in the case of KURA
                            roadSectionfind.SectionID = roadSection.SectionID;
                            roadSectionfind.MunicipalityId = roadSection.MunicipalityId;
                            roadSectionfind.SurfaceType2 = roadSection.SurfaceType2;
                            roadSectionfind.StartChainage = roadSection.StartChainage;
                            roadSectionfind.EndChainage = roadSection.EndChainage;
                            roadSectionfind.CW_Surf_Co = roadSection.CW_Surf_Co;
                            var roadSectionResponse = await _roadSectionService.UpdateAsync(roadSectionfind.ID, roadSectionfind).ConfigureAwait(false);
                            roadSection = roadSectionResponse.RoadSection;
                        }


                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"GISRoadService.PopulateKuRARoadSections Error: {Environment.NewLine}");
                    }
                }
            }
            //Set pulledfromgis  to true 
            road.PulledSectionsFromGIS = true;
            var kwsResp2 = await _roadService.UpdateAsync(road.ID, road).ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task PopulateCountyRoadSections(Road road)
        {
            //for each road section then add it to roadsections
            var countiesListResp = await _countiesRoadService.ListAsync(road.RoadNumber).ConfigureAwait(false);
            IList<CountiesRoad> countiesRoadList = (IList<CountiesRoad>)countiesListResp.CountiesRoads;

            //For each Road Section add to Road Section Table
            Double len = 0;
            foreach (var countyRd in countiesRoadList)
            {
                //Road Section instatntiation
                RoadSection roadSection = new RoadSection();

                //Length
                bool result = Double.TryParse(countyRd.Length.ToString(), out len);
                if (result == true)
                {
                    roadSection.Length = len;
                }

                //Interval
                roadSection.Interval = 200;

                //Start and End Chainage
                roadSection.StartChainage = 0.0;
                roadSection.EndChainage = 0.0;

                //Road ID
                roadSection.RoadId = road.ID;

                //Surface Type
                roadSection.SurfaceTypeId = 5;//UnPaved
                var surfaceTypeResp = await _surfaceTypeService.FindByNameAsync(countyRd.SurfaceType).ConfigureAwait(false);
                if (surfaceTypeResp.Success)
                {
                    roadSection.SurfaceTypeId = surfaceTypeResp.SurfaceType.ID;
                }
                roadSection.SurfaceType2 = countyRd.SurfaceType2;
                roadSection.StartChainage = countyRd.StartChainage;
                roadSection.EndChainage = countyRd.EndChainage;
                roadSection.CW_Surf_Co = countyRd.CW_Surf_Co;

                //Constituency intersected by the road
                roadSection.ConstituencyId = countyRd.ConstituencyId;
                //Todo : Constituency ID hardcoded

                //Section Name
                roadSection.SectionName = countyRd.Sec_Name;

                //SectionID==RoadNumber in the case of County
                roadSection.SectionID = countyRd.RdNum;
                if (roadSection.SectionID.Trim().Length > 0)
                {
                    try
                    {
                        // if section exists is true then update else Add
                        var roadSectFind = await _roadSectionService.FindBySectionIdAsync(roadSection.SectionID).ConfigureAwait(false);
                        RoadSection roadSectionfind = roadSectFind.RoadSection;

                        if (roadSectionfind == null)
                        {
                            //Add
                            var roadSectionResponse = await _roadSectionService.AddAsync(roadSection).ConfigureAwait(false);
                            roadSection = roadSectionResponse.RoadSection;
                        }
                        else
                        {
                            //Update
                            roadSectionfind.Length = roadSection.Length;
                            roadSectionfind.Interval = roadSection.Interval;
                            roadSectionfind.StartChainage = roadSection.StartChainage;
                            roadSectionfind.EndChainage = roadSection.EndChainage;
                            //Road ID
                            roadSectionfind.RoadId = roadSection.RoadId;
                            //Surface Type
                            roadSectionfind.SurfaceTypeId = roadSection.SurfaceTypeId;//Gravel
                                                                                      //Todo : surface Type hard coded in PopulateCountyRoadSections
                                                                                      //Constituency intersected by the road
                            roadSectionfind.ConstituencyId = roadSection.ConstituencyId;
                            //Todo : Constituency ID hardcoded
                            //Section Name
                            roadSectionfind.SectionName = roadSection.SectionName;
                            //SectionID==RoadNumber in the case of CountyRoad
                            roadSectionfind.SectionID = roadSection.SectionID;
                            roadSectionfind.SurfaceType2 = roadSection.SurfaceType2;
                            roadSectionfind.StartChainage = roadSection.StartChainage;
                            roadSectionfind.EndChainage = roadSection.EndChainage;
                            roadSectionfind.CW_Surf_Co = roadSection.CW_Surf_Co;
                            var roadSectionResponse = await _roadSectionService.UpdateAsync(roadSectionfind.ID, roadSectionfind).ConfigureAwait(false);
                            roadSection = roadSectionResponse.RoadSection;
                        }


                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"GISRoadService.PopulateCountyRoadSections Error: {Environment.NewLine}");
                    }
                }

            }
            //Set pulledfromgis  to true 
            road.PulledSectionsFromGIS = true;
            var kwsResp2 = await _roadService.UpdateAsync(road.ID, road).ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task PopulateKenHARoadSections(Road road)
        {
            //for each road section then add it to roadsections
            var kenHAListResp = await _kenHARoadService.ListAsync(road.RoadNumber).ConfigureAwait(false);
            IList<KenhaRoad> kenHARoadList = (IList<KenhaRoad>)kenHAListResp.KenhaRoads;

            //For each Road Section add to Road Section Table
            Double len = 0;
            foreach (var kenHARd in kenHARoadList)
            {
                //Road Section instatntiation
                RoadSection roadSection = new RoadSection();

                //Length
                bool result = Double.TryParse(kenHARd.Length.ToString(), out len);
                if (result == true)
                {
                    roadSection.Length = len;
                }

                //Interval
                roadSection.Interval = 200;

                //Start and End Chainage
                roadSection.StartChainage = 0.0;
                roadSection.EndChainage = 0.0;

                //Road ID
                roadSection.RoadId = road.ID;

                //Surface Type
                roadSection.SurfaceTypeId = 5;//UnPaved
                var surfaceTypeResp = await _surfaceTypeService.FindByNameAsync(kenHARd.SurfaceType).ConfigureAwait(false);
                if (surfaceTypeResp.Success)
                {
                    roadSection.SurfaceTypeId = surfaceTypeResp.SurfaceType.ID;
                }
                roadSection.SurfaceType2 = kenHARd.SurfaceType2;
                roadSection.StartChainage = kenHARd.StartChainage;
                roadSection.EndChainage = kenHARd.EndChainage;
                roadSection.CW_Surf_Co = kenHARd.CW_Surf_Co;
                //Constituency intersected by the road
                roadSection.ConstituencyId = kenHARd.ConstituencyId;
                //Todo : Constituency ID hardcoded

                //Section Name
                roadSection.SectionName = kenHARd.Sec_Name;

                //SectionID==RoadNumber in the case of County
                roadSection.SectionID = kenHARd.Section_ID;

                if (roadSection.SectionID.Trim().Length > 0)
                {
                    try
                    {
                        // if section exists is true then update else Add
                        var roadSectFind = await _roadSectionService.FindBySectionIdAsync(roadSection.SectionID, road.AuthorityId).ConfigureAwait(false);
                        RoadSection roadSectionfind = roadSectFind.RoadSection;

                        if (roadSectionfind == null)
                        {
                            //Add
                            var roadSectionResponse = await _roadSectionService.AddAsync(roadSection).ConfigureAwait(false);
                            roadSection = roadSectionResponse.RoadSection;
                        }
                        else
                        {
                            //Update
                            roadSectionfind.Length = roadSection.Length;
                            roadSectionfind.Interval = roadSection.Interval;
                            roadSectionfind.StartChainage = roadSection.StartChainage;
                            roadSectionfind.EndChainage = roadSection.EndChainage;
                            //Road ID
                            roadSectionfind.RoadId = roadSection.RoadId;
                            //Surface Type
                            roadSectionfind.SurfaceTypeId = roadSection.SurfaceTypeId;//Gravel
                                                                                      //Todo : surface Type hard coded in PopulateCountyRoadSections
                                                                                      //Constituency intersected by the road
                            roadSectionfind.ConstituencyId = roadSection.ConstituencyId;
                            //Todo : Constituency ID hardcoded
                            //Section Name
                            roadSectionfind.SectionName = roadSection.SectionName;
                            //SectionID==RoadNumber in the case of CountyRoad
                            roadSectionfind.SectionID = roadSection.SectionID;
                            roadSectionfind.SurfaceType2 = roadSection.SurfaceType2;
                            roadSectionfind.StartChainage = roadSection.StartChainage;
                            roadSectionfind.EndChainage = roadSection.EndChainage;
                            roadSectionfind.CW_Surf_Co = roadSection.CW_Surf_Co;
                            var roadSectionResponse = await _roadSectionService.UpdateAsync(roadSectionfind.ID, roadSectionfind).ConfigureAwait(false);
                            roadSection = roadSectionResponse.RoadSection;
                        }


                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"GISRoadService.PopulateCountyRoadSections Error: {Environment.NewLine}");
                    }
                }

            }
            //Set pulledfromgis  to true 
            road.PulledSectionsFromGIS = true;
            var kwsResp2 = await _roadService.UpdateAsync(road.ID, road).ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task PopulateKeRRARoadSections(Road road)
        {
            //for each road section then add it to roadsections
            var keRRAListResp = await _keRRARoadService.ListAsync(road.RoadNumber).ConfigureAwait(false);
            IList<KerraRoad> keRRARoadList = (IList<KerraRoad>)keRRAListResp.KerraRoads;

            //For each Road Section add to Road Section Table
            Double len = 0;
            foreach (var keRRARd in keRRARoadList)
            {
                //Road Section instatntiation
                RoadSection roadSection = new RoadSection();

                //Length
                bool result = Double.TryParse(keRRARd.Length.ToString(), out len);
                if (result == true)
                {
                    roadSection.Length = len;
                }

                //Interval
                roadSection.Interval = 200;

                //Start and End Chainage
                roadSection.StartChainage = 0.0;
                roadSection.EndChainage = 0.0;

                //Road ID
                roadSection.RoadId = road.ID;

                //Surface Type
                roadSection.SurfaceTypeId = 5;//UnPaved
                var surfaceTypeResp = await _surfaceTypeService.FindByNameAsync(keRRARd.SurfaceType).ConfigureAwait(false);
                if (surfaceTypeResp.Success)
                {
                    roadSection.SurfaceTypeId = surfaceTypeResp.SurfaceType.ID;
                }
                roadSection.SurfaceType2 = keRRARd.SurfaceType2;
                roadSection.StartChainage = keRRARd.StartChainage;
                roadSection.EndChainage = keRRARd.EndChainage;
                roadSection.CW_Surf_Co = keRRARd.CW_Surf_Co;
                //Constituency intersected by the road
                roadSection.ConstituencyId = keRRARd.ConstituencyId;
                //Todo : Constituency ID hardcoded

                //Section Name
                roadSection.SectionName = keRRARd.Sec_Name;

                //SectionID==RoadNumber in the case of County
                roadSection.SectionID = keRRARd.Section_ID;
                if (roadSection.SectionID.Trim().Length > 0)
                {
                    try
                    {
                        // if section exists is true then update else Add
                        var roadSectFind = await _roadSectionService.FindBySectionIdAsync(roadSection.SectionID, road.AuthorityId).ConfigureAwait(false);
                        RoadSection roadSectionfind = roadSectFind.RoadSection;

                        if (roadSectionfind == null)
                        {
                            //Add
                            var roadSectionResponse = await _roadSectionService.AddAsync(roadSection).ConfigureAwait(false);
                            roadSection = roadSectionResponse.RoadSection;
                        }
                        else
                        {
                            //Update
                            roadSectionfind.Length = roadSection.Length;
                            roadSectionfind.Interval = roadSection.Interval;
                            roadSectionfind.StartChainage = roadSection.StartChainage;
                            roadSectionfind.EndChainage = roadSection.EndChainage;
                            //Road ID
                            roadSectionfind.RoadId = roadSection.RoadId;
                            //Surface Type
                            roadSectionfind.SurfaceTypeId = roadSection.SurfaceTypeId;//Gravel
                                                                                      //Todo : surface Type hard coded in PopulateCountyRoadSections
                                                                                      //Constituency intersected by the road
                            roadSectionfind.ConstituencyId = roadSection.ConstituencyId;
                            //Todo : Constituency ID hardcoded
                            //Section Name
                            roadSectionfind.SectionName = roadSection.SectionName;
                            //SectionID==RoadNumber in the case of CountyRoad
                            roadSectionfind.SectionID = roadSection.SectionID;
                            roadSectionfind.SurfaceType2 = roadSection.SurfaceType2;
                            roadSectionfind.StartChainage = roadSection.StartChainage;
                            roadSectionfind.EndChainage = roadSection.EndChainage;
                            roadSectionfind.CW_Surf_Co = roadSection.CW_Surf_Co;
                            var roadSectionResponse = await _roadSectionService.UpdateAsync(roadSectionfind.ID, roadSectionfind).ConfigureAwait(false);
                            roadSection = roadSectionResponse.RoadSection;
                        }


                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"GISRoadService.PopulateCountyRoadSections Error: {Environment.NewLine}");
                    }
                }

            }

            //Set pulledfromgis  to true 
            road.PulledSectionsFromGIS = true;
            var kwsResp2 = await _roadService.UpdateAsync(road.ID, road).ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task PopulateKwSRoadSections(Road road)
        {
            //for each road section then add its roadsections
            var kwSListResp = await _kwSRoadService.ListAsync(road.RoadNumber).ConfigureAwait(false);
            IList<KwsRoad> kwSRoadList = ((IList<KwsRoad>)kwSListResp.KwsRoads);

            //For each Road Section add to Road Section Table
            Double len = 0;
            foreach (var kwS in kwSRoadList)
            {
                //Road Section instatntiation
                RoadSection roadSection = new RoadSection();

                //Length
                bool result = Double.TryParse(kwS.Length.ToString(), out len);
                if (result == true)
                {
                    roadSection.Length = len;
                }

                //Interval
                roadSection.Interval = 200;

                //Start and End Chainage
                roadSection.StartChainage = 0.0;
                roadSection.EndChainage = 0.0;

                //Road ID
                roadSection.RoadId = road.ID;

                //Surface Type
                roadSection.SurfaceTypeId = 5;//UnPaved
                var surfaceTypeResp = await _surfaceTypeService.FindByNameAsync(kwS.SurfaceType).ConfigureAwait(false);
                if (surfaceTypeResp.Success)
                {
                    roadSection.SurfaceTypeId = surfaceTypeResp.SurfaceType.ID;
                }
                roadSection.SurfaceType2 = kwS.SurfaceType2;
                roadSection.StartChainage = kwS.StartChainage;
                roadSection.EndChainage = kwS.EndChainage;
                roadSection.CW_Surf_Co = kwS.CW_Surf_Co;
                //Constituency intersected by the road
                roadSection.ConstituencyId = kwS.ConstituencyId;
                //Todo : Constituency ID hardcoded

                //Section Name
                roadSection.SectionName = kwS.Sec_Name;

                //Kwspark
                try
                {
                    roadSection.KWSParkId = kwS.KWSParkId;
                }
                catch (Exception Ex)
                {

                    _logger.LogError(Ex, $"GISRoadService.PopulateKwSRoadSections Error: {Environment.NewLine}");
                }

                //SectionID==RoadNumber in the case of County
                roadSection.SectionID = kwS.Section_ID;
                if (roadSection.SectionID.Trim().Length > 0)
                {
                    try
                    {
                        // if section exists is true then update else Add
                        var roadSectFind = await _roadSectionService.FindBySectionIdAsync(roadSection.SectionID, road.AuthorityId).ConfigureAwait(false);
                        RoadSection roadSectionfind = roadSectFind.RoadSection;

                        if (roadSectionfind == null)
                        {
                            //Add
                            var roadSectionResponse = await _roadSectionService.AddAsync(roadSection).ConfigureAwait(false);
                            roadSection = roadSectionResponse.RoadSection;

                        }
                        else
                        {
                            //Update
                            roadSectionfind.Length = roadSection.Length;
                            roadSectionfind.Interval = roadSection.Interval;
                            roadSectionfind.StartChainage = roadSection.StartChainage;
                            roadSectionfind.EndChainage = roadSection.EndChainage;
                            //Road ID
                            roadSectionfind.RoadId = roadSection.RoadId;
                            //Surface Type
                            roadSectionfind.SurfaceTypeId = roadSection.SurfaceTypeId;//Gravel
                                                                                      //Todo : surface Type hard coded in PopulateCountyRoadSections
                                                                                      //Constituency intersected by the road
                            roadSectionfind.ConstituencyId = roadSection.ConstituencyId;
                            //Todo : Constituency ID hardcoded
                            //Section Name
                            roadSectionfind.SectionName = roadSection.SectionName;
                            //SectionID==RoadNumber in the case of CountyRoad
                            roadSectionfind.SectionID = roadSection.SectionID;
                            roadSectionfind.KWSParkId = roadSection.KWSParkId;
                            roadSectionfind.SurfaceType2 = roadSection.SurfaceType2;
                            roadSectionfind.StartChainage = roadSection.StartChainage;
                            roadSectionfind.EndChainage = roadSection.EndChainage;
                            roadSectionfind.CW_Surf_Co = roadSection.CW_Surf_Co;

                            var roadSectionResponse = await _roadSectionService.UpdateAsync(roadSectionfind.ID, roadSectionfind).ConfigureAwait(false);
                            roadSection = roadSectionResponse.RoadSection;

                        }

                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"GISRoadService.PopulateCountyRoadSections Error: {Environment.NewLine}");
                    }
                }

            }

            //Set pulledfromgis  to true 
            road.PulledSectionsFromGIS = true;
            var kwsResp2 = await _roadService.UpdateAsync(road.ID, road).ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GISRoadViewModelResponse> GetSurfaceType(RoadSection roadSection)
        {
            try
            {
                GISRoadViewModel gISRoadViewModel = new GISRoadViewModel();
                if (roadSection != null)
                {
                    //Get authority
                    var authResp = await _authorityService.FindByIdAsync(roadSection.Road.AuthorityId).ConfigureAwait(false);
                    Authority authority = authResp.Authority;
                    if (authority != null && authority.ID == 2)//KeRRA
                    {
                        //Get road surface type and set global variable
                        var kerraRdResp = await _keRRARoadService.FindBySectionIdAsync(roadSection.SectionID).ConfigureAwait(false);
                        KerraRoad kerraRoad = kerraRdResp.KerraRoad;
                        if (kerraRoad != null)
                        {
                            if (kerraRoad.SurfaceType == "Paved")
                            {
                                gISRoadViewModel.SurfaceType = "Paved";
                            }
                            else
                            {
                                gISRoadViewModel.SurfaceType = "UnPaved";
                            }
                        }
                        else
                        {
                            if (roadSection.SurfaceType.Name == "Paved")
                            {
                                gISRoadViewModel.SurfaceType = "Paved";
                            }
                            else
                            {
                                gISRoadViewModel.SurfaceType = "UnPaved";
                            }
                        }

                    }

                    if (authority != null && authority.ID == 1)//KenHA
                    {
                        //Get road surface type and set global variable
                        var kenHARdResp = await _kenHARoadService.FindBySectionIdAsync(roadSection.SectionID).ConfigureAwait(false);
                        KenhaRoad kenhaRoad = kenHARdResp.KenhaRoad;
                        if (kenhaRoad != null)
                        {
                            if (kenhaRoad.SurfaceType == "UnPaved")
                            {
                                gISRoadViewModel.SurfaceType = "UnPaved";
                            }
                            else
                            {
                                gISRoadViewModel.SurfaceType = "Paved";
                            }
                        }
                        else //added manually
                        {
                            if (roadSection.SurfaceType.Name == "Paved")
                            {
                                gISRoadViewModel.SurfaceType = "Paved";
                            }
                            else
                            {
                                gISRoadViewModel.SurfaceType = "UnPaved";
                            }
                        }

                    }

                    if (authority != null && authority.ID == 3)//KURA
                    {
                        //Get road surface type and set global variable
                        var kuRARdResp = await _kURARoadService.FindByRoadNumberAsync(roadSection.Road.RoadNumber).ConfigureAwait(false);
                        KuraRoad kuraRoad = kuRARdResp.KuraRoad;
                        if (kuraRoad != null)
                        {
                            if (kuraRoad.SurfaceType == "Paved")
                            {
                                gISRoadViewModel.SurfaceType = "Paved";
                            }
                            else
                            {
                                gISRoadViewModel.SurfaceType = "UnPaved";
                            }
                        }
                        else //roads added manually
                        {
                            if (roadSection.SurfaceType.Name == "Paved")
                            {
                                gISRoadViewModel.SurfaceType = "Paved";
                            }
                            else
                            {
                                gISRoadViewModel.SurfaceType = "UnPaved";
                            }
                        }

                    }

                    if (authority != null && authority.ID == 4)//KWS
                    {
                        //Get road surface type and set global variable
                        var kwSRdResp = await _kwSRoadService.FindBySectionIdAsync(roadSection.SectionID).ConfigureAwait(false);
                        KwsRoad kwsRoad = kwSRdResp.KwsRoad;
                        if (kwsRoad != null)
                        {
                            if (kwsRoad.SurfaceType == "Paved")
                            {
                                gISRoadViewModel.SurfaceType = "Paved";
                            }
                            else
                            {
                                gISRoadViewModel.SurfaceType = "UnPaved";
                            }
                        }
                        else //roads added manually
                        {
                            if (roadSection.SurfaceType.Name == "Paved")
                            {
                                gISRoadViewModel.SurfaceType = "Paved";
                            }
                            else
                            {
                                gISRoadViewModel.SurfaceType = "UnPaved";
                            }
                        }

                    }

                    if (authority != null && authority.ID > 4)//Counties
                    {
                        //Get road surface type and set global variable
                        var countiesRdResp = await _countiesRoadService.FindByRoadNumberAsync(roadSection.Road.RoadNumber).ConfigureAwait(false);
                        CountiesRoad countiesRoad = countiesRdResp.CountiesRoad;
                        if (countiesRoad != null)
                        {
                            if (countiesRoad.SurfaceType == "Paved")
                            {
                                gISRoadViewModel.SurfaceType = "Paved";
                            }
                            else
                            {
                                gISRoadViewModel.SurfaceType = "UnPaved";
                            }
                        }
                        else //Added manually
                        {
                            if (roadSection.SurfaceType.Name == "Paved")
                            {
                                gISRoadViewModel.SurfaceType = "Paved";
                            }
                            else
                            {
                                gISRoadViewModel.SurfaceType = "UnPaved";
                            }
                        }

                    }
                }
                return new GISRoadViewModelResponse(gISRoadViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISRoadService.GetSurfaceType Error: {Environment.NewLine}");
                return new GISRoadViewModelResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
