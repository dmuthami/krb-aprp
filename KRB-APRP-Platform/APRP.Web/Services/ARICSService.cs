using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Services
{
    public class ARICSService : ControllerBase, IARICSService
    {
        private readonly IARICSRepository _aRICSRepository;
        private readonly IShoulderSurfaceTypePavedService _shoulderSurfaceTypePavedService;
        private readonly IShoulderInterventionPavedService _shoulderInterventionPavedService;
        private readonly ISurfaceTypeUnPavedService _surfaceTypeUnPavedService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IRoadConditionService _roadConditionService;

        public ARICSService(IARICSRepository aRICSRepository, IUnitOfWork unitOfWork
            , ILogger<ARICSService> logger,
            IShoulderSurfaceTypePavedService shoulderSurfaceTypePavedService,
            IShoulderInterventionPavedService shoulderInterventionPavedService,
            ISurfaceTypeUnPavedService surfaceTypeUnPavedService,
            IRoadConditionService roadConditionService)
        {
            _aRICSRepository = aRICSRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _shoulderSurfaceTypePavedService = shoulderSurfaceTypePavedService;
            _shoulderInterventionPavedService = shoulderInterventionPavedService;
            _surfaceTypeUnPavedService = surfaceTypeUnPavedService;
            _roadConditionService = roadConditionService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSResponse> AddAsync(ARICS aRICS)
        {
            try
            {
                await _aRICSRepository.AddAsync(aRICS).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ARICSResponse(aRICS); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.AddAsync Error: {Environment.NewLine}");
                return new ARICSResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSResponse> CheckARICSForSheet(ARICSVM _ARICSVM)
        {
            try
            {
                var existingARICS = await _aRICSRepository.CheckARICSForSheet(_ARICSVM.RoadSheetID).ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new ARICSResponse("Record Not Found");
                }
                else
                {
                    return new ARICSResponse(existingARICS);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.CheckAricsforSheet Error: {Environment.NewLine}");
                return new ARICSResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSListResponse> CreateARICSForSheet(ARICSVM _ARICSVM)
        {
            try
            {
                //Get default/NA Shoulder Surface Type Paved
                var shoulderSurfaceTypePavedResponse = await _shoulderSurfaceTypePavedService.ListAsync().ConfigureAwait(false);
                IList<ShoulderSurfaceTypePaved> shoulderSurfaceTypePaved = (IList<ShoulderSurfaceTypePaved>)shoulderSurfaceTypePavedResponse.ShoulderSurfaceTypePaved;
                long shoulderSurfaceTypePavedId = shoulderSurfaceTypePaved.Where(s => s.Code == "NA").FirstOrDefault().ID;

                //Get default/NA Shoulder Intervention Paved
                var shoulderInterventionPavedResponse = await _shoulderInterventionPavedService.ListAsync().ConfigureAwait(false);
                IList<ShoulderInterventionPaved> shoulderInterventionPaved = (IList<ShoulderInterventionPaved>)shoulderInterventionPavedResponse.ShoulderInterventionPaved;
                long shoulderInterventionPavedId = shoulderInterventionPaved.Where(s => s.Code == "NA").FirstOrDefault().ID;

                //Get default/NA Shoulder Intervention Paved
                var surfaceTypeUnPavedResponse = await _surfaceTypeUnPavedService.ListAsync().ConfigureAwait(false);
                IList<SurfaceTypeUnPaved> surfaceTypeUnPaved = (IList<SurfaceTypeUnPaved>)surfaceTypeUnPavedResponse.SurfaceTypeUnPaved;
                long surfaceTypeUnPavedId = surfaceTypeUnPaved.Where(s => s.Code == "NA").FirstOrDefault().ID;



                int myQuotient = (int)Math.Ceiling(_ARICSVM.SectionLengthKM * 1000);

                int remainder, quotient = Math.DivRem(myQuotient, _ARICSVM.Interval, out remainder);
                if (remainder > 0)
                {
                    quotient++;
                }
                int incrementalChainage = 0;
                //for each arics post record
                for (int i = 1; i <= quotient; i++)
                {
                    //Incremental chainage
                    incrementalChainage += _ARICSVM.Interval;

                    ARICS _ARICS = new ARICS();
                    _ARICS.Chainage = _ARICSVM.incrementalChainage * 1000 + incrementalChainage;
                    //_ARICS.SurveyId = 1;
                    _ARICS.ShoulderSurfaceTypePavedId = shoulderSurfaceTypePavedId;
                    _ARICS.ShoulderInterventionPavedId = shoulderInterventionPavedId;
                    _ARICS.SurfaceTypeUnPavedId = surfaceTypeUnPavedId;
                    _ARICS.RoadSheetId = _ARICSVM.RoadSheetID;
                    await _aRICSRepository.AddAsync(_ARICS).ConfigureAwait(false);

                }

                await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                var aRICSList = await _aRICSRepository.GetARICSForSheetNo(_ARICSVM.RoadSheetID).ConfigureAwait(false);
                return new ARICSListResponse(aRICSList);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController.CreateARICSForSheet Error : " +
                    $"{Environment.NewLine}");
                return null; ;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingARICS = await _aRICSRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new ARICSResponse("Record Not Found");
                }
                else
                {
                    return new ARICSResponse(existingARICS);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.FinfByIdAsync Error: {Environment.NewLine}");
                return new ARICSResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSResponse> FindByRoadSheetAndChainageAsync(long RoadSheetID, int Chainage)
        {
            try
            {
                var existingARICS = await _aRICSRepository.FindByRoadSheetAndChainageAsync(RoadSheetID, Chainage).ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new ARICSResponse("Record Not Found");
                }
                else
                {
                    return new ARICSResponse(existingARICS);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.FindByRoadSheetAndChainageAsync Error: {Environment.NewLine}");
                return new ARICSResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadListResponse> GetARICEDRoads(int? Year)
        {
            try
            {
                var existingARICS = await _aRICSRepository.GetARICS(Year).ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new RoadListResponse("Record Not Found");
                }
                else
                {
                    //Perform some operation to get the unique roads for the ARICS 
                    IEnumerable<Road> roads = await Task.Run(() => GetARICEDRoads((IList<ARICS>)existingARICS)).ConfigureAwait(false);
                    return new RoadListResponse(roads);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.GetARICEDRoads Error: {Environment.NewLine}");
                return new RoadListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<IList<Road>> GetARICEDRoads(IList<ARICS> _ARICS)
        {
            IList<Road> roads = new List<Road>();
            var RoadsDictionary = new Dictionary<long, Road>();
            foreach (var arics in _ARICS)
            {
                //Add Road To Roads Dictionary
                try
                {
                    if (!RoadsDictionary.ContainsKey(arics.RoadSheet.RoadSection.Road.ID))
                    {
                        RoadsDictionary.Add(arics.RoadSheet.RoadSection.Road.ID, arics.RoadSheet.RoadSection.Road);
                        roads.Add(arics.RoadSheet.RoadSection.Road);
                    }
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ARICSService.GetARICEDRoads :{Environment.NewLine}");
                }
            }
            return roads;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSListResponse> GetARICSBySheetNo(ARICSVM _ARICSVM)
        {
            try
            {
                var existingARICS = await _aRICSRepository.GetARICSBySheetNo(_ARICSVM.RoadSheetID).ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new ARICSListResponse("Record Not Found");
                }
                else
                {
                    return new ARICSListResponse(existingARICS);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.GetARICSBySheetNo Error: {Environment.NewLine}");
                return new ARICSListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        /// <summary>
        /// Similar to FindByIdAsync but explicitly loads Survey 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSResponse> GetARICSDetails(long ID)
        {
            try
            {
                var existingARICS = await _aRICSRepository.GetARICSDetails(ID).ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new ARICSResponse("Record Not Found");
                }
                else
                {
                    return new ARICSResponse(existingARICS);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.GetARICSDetails Error: {Environment.NewLine}");
                return new ARICSResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSListResponse> GetARICSForRoad(Road road, int? Year)
        {
            try
            {
                var existingARICS = await _aRICSRepository.GetARICSForRoad(road, Year).ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new ARICSListResponse("Record Not Found");
                }
                else
                {
                    return new ARICSListResponse(existingARICS);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.GetARICSForRoad Error: {Environment.NewLine}");
                return new ARICSListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSListResponse> GetARICSForRoad(Road road, double StartChainage, double EndChainage, int? Year)
        {
            try
            {
                var existingARICS = await _aRICSRepository.GetARICSForRoad(road, StartChainage, EndChainage, Year).ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new ARICSListResponse("Record Not Found");
                }
                else
                {
                    return new ARICSListResponse(existingARICS);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.GetARICSForRoad Error: {Environment.NewLine}");
                return new ARICSListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSListResponse> GetARICSForSheet(ARICSVM _ARICSVM)
        {
            try
            {
                var existingARICS = await _aRICSRepository.GetARICSForSheetNo(_ARICSVM.RoadSheetID).ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new ARICSListResponse("Record Not Found");
                }
                else
                {
                    return new ARICSListResponse(existingARICS);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.GetARICSForSheet Error: {Environment.NewLine}");
                return new ARICSListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSListResponse> ListAsync()
        {
            try
            {
                var existingARICS = await _aRICSRepository.ListAsync().ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new ARICSListResponse("Records Not Found");
                }
                else
                {
                    return new ARICSListResponse(existingARICS);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.ListAsync Error: {Environment.NewLine}");
                return new ARICSListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSResponse> RemoveAsync(long ID)
        {
            var existingARICS = await _aRICSRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingARICS == null)
            {
                return new ARICSResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _aRICSRepository.Remove(existingARICS);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new ARICSResponse(existingARICS);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ARICSService.RemoveAsync Error: {Environment.NewLine}");
                    return new ARICSResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSResponse> Update(ARICS aRICS)
        {
            var existingARICS = await _aRICSRepository.FindByIdAsync(aRICS.ID).ConfigureAwait(false);
            if (existingARICS == null)
            {
                return new ARICSResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _aRICSRepository.Update(aRICS);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new ARICSResponse(aRICS);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ARICSService.Update Error: {Environment.NewLine}");
                    return new ARICSResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSResponse> Update(long ID, ARICS aRICS)
        {
            try
            {
                _aRICSRepository.Update(ID, aRICS);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ARICSResponse(aRICS);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.Update Error: {Environment.NewLine}");
                return new ARICSResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSDataListResponse> GetARICEDRoadsAndConditions(int? Year)
        {
            IList<ARICSData> aRICSDatas = new List<ARICSData>();
            try
            {
                IList<Road> aRICEDRoadsList = (IList<Road>)((await GetARICEDRoads(Year).ConfigureAwait(false)).Roads);

                IList<ARICS> roadARICS; double iri = 0.0;
                ARICSData aRICSData;// = new ARICSData();
                if (aRICEDRoadsList != null)
                {
                    foreach (var road in aRICEDRoadsList)
                    {
                        aRICSData = new ARICSData();
                        //Get list of arics for the road
                        var aricslistresp = await GetARICSForRoad(road, Year).ConfigureAwait(false);
                        roadARICS = (IList<ARICS>)aricslistresp.ARICS;
                        //Call function to average and return Ratr of Deteroriation/IRI
                        var _aricsDataResp = await GetIRI(roadARICS).ConfigureAwait(false);
                        aRICSData.Road = road;
                        aRICSData.RateOfDeterioration = _aricsDataResp.ARICSData.RateOfDeterioration;
                        aRICSDatas.Add(aRICSData);
                    }
                    //Try to order by IRI in descending order.
                    aRICSDatas = aRICSDatas.OrderBy(s => s.RateOfDeterioration).ToList();
                }

                if (aRICSDatas == null)
                {
                    return new ARICSDataListResponse("Record Not Found");
                }
                else
                {
                    //Perform some operation to get the unique roads for the ARICS       
                    return new ARICSDataListResponse(aRICSDatas);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.GetARICEDRoadsAndConditions Error: {Environment.NewLine}");
                return new ARICSDataListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSDataResponse> GetIRI(IList<ARICS> roadARICS)
        {
            ARICSData aRICSData = new ARICSData();
            IList<ARICS> _RoadARICS = roadARICS.Where(s => s.RateOfDeterioration != 0).ToList();//Remove entries where IRI=0
            try
            {
                aRICSData.RateOfDeterioration = await Task.Run(() =>
                    {
                        double iri = 0.0;
                        double iri_1 = 0.0;
                        double iri_2 = 0.0;
                        double iri_3 = 0.0;
                        double iri_4 = 0.0;
                        double iri_5 = 0.0;
                        iri_1 = (_RoadARICS.Where(s => s.RateOfDeterioration == 1).Sum(item => item.RateOfDeterioration));
                        iri_2 = (_RoadARICS.Where(s => s.RateOfDeterioration == 2).Sum(item => item.RateOfDeterioration));
                        iri_3 = (_RoadARICS.Where(s => s.RateOfDeterioration == 3).Sum(item => item.RateOfDeterioration));
                        iri_4 = (_RoadARICS.Where(s => s.RateOfDeterioration == 4).Sum(item => item.RateOfDeterioration));
                        iri_5 = (_RoadARICS.Where(s => s.RateOfDeterioration == 5).Sum(item => item.RateOfDeterioration));
                        iri = (iri_1 + iri_2 + iri_3 + iri_4 + iri_5) / _RoadARICS.Count;
                        return iri;
                    }).ConfigureAwait(false);

                return new ARICSDataResponse(aRICSData);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.GetIRI Error: {Environment.NewLine}");
                return new ARICSDataResponse($"Error occured while retrieving the record : {Ex.Message}");
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSDataResponse> GetIRIForRoad(Road road, int? Year)
        {
            ARICSData aRICSData = new ARICSData();

            var arics = await GetARICSForRoad(road, Year).ConfigureAwait(false);
            aRICSData.Road = road;

            if (arics.Success)
            {
                if (arics.ARICS.Count() > 0)
                {
                    var iri = await GetIRI((IList<ARICS>)arics.ARICS).ConfigureAwait(false);
                    aRICSData.RateOfDeterioration = iri.ARICSData.RateOfDeterioration;
                }
                else
                {
                    aRICSData.RateOfDeterioration = 0.0;
                }
            }


            try
            {
                return new ARICSDataResponse(aRICSData);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.GetIRIForRoad Error: {Environment.NewLine}");
                return new ARICSDataResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSDataResponse> GetCulvertsSummaryForSheet(IList<ARICS> roadARICS)
        {
            ARICSData aRICSData = new ARICSData();

            try
            {
                await Task.Run(() =>
                {
                    aRICSData.CulvertN = (roadARICS.Sum(item => item.CulvertN));
                    aRICSData.CulvertRR = (roadARICS.Sum(item => item.CulvertRR));
                    aRICSData.CulvertHR = (roadARICS.Sum(item => item.CulvertHR));
                    aRICSData.CulvertNH = (roadARICS.Sum(item => item.CulvertNH));
                    aRICSData.CulvertG = (roadARICS.Sum(item => item.CulvertG));
                    aRICSData.CulvertB = (roadARICS.Sum(item => item.CulvertB));
                }).ConfigureAwait(false);

                return new ARICSDataResponse(aRICSData);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.GetIRI Error: {Environment.NewLine}");
                return new ARICSDataResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSDataListResponse> GetRoadsAndConditions(int? Year, Authority authority)
        {
            IList<ARICSData> aRICSDatas = new List<ARICSData>();
            try
            {
                //Get road conditions
                IList<RoadCondition> roadConditions = null;
                if (authority == null)
                {
                    var roadcondResp = await _roadConditionService.ListAsync(Year).ConfigureAwait(false);
                    roadConditions = (((IList<RoadCondition>)roadcondResp.RoadCondtion)
                    .OrderBy(o => o.Road.Authority.Name)
                    .ThenBy(t => t.PriorityRate)
                    .ThenBy(t => t.IRI)
                    .ThenBy(a => a.ARD)).ToList();

                }
                else
                {
                    var roadcondResp = await _roadConditionService.ListAsync(authority, Year).ConfigureAwait(false);
                    roadConditions = (((IList<RoadCondition>)roadcondResp.RoadCondtion)
                    .OrderBy(o => o.PriorityRate)
                    .ThenBy(t => t.IRI)
                    .ThenBy(a => a.ARD))
                    .ToList();

                }

                IList<ARICS> roadARICS; double iri = 0.0;
                ARICSData aRICSData;// = new ARICSData();

                if (roadConditions != null)
                {
                    foreach (var roadCondition in roadConditions)
                    {
                        aRICSData = new ARICSData();
                        aRICSData.Road = roadCondition.Road;
                        aRICSData.IRI = roadCondition.IRI;
                        aRICSData.RateOfDeterioration = roadCondition.ARD;
                        aRICSData.PriorityRate = roadCondition.PriorityRate;
                        aRICSData.Comment = roadCondition.Comment;
                        aRICSDatas.Add(aRICSData);
                    }
                }

                return new ARICSDataListResponse(aRICSDatas);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.GetARICEDRoadsAndConditions Error: {Environment.NewLine}");
                return new ARICSDataListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ARICSListResponse> GetARICSByRoadSection(long RoadSectionId)
        {
            try
            {
                var existingARICS = await _aRICSRepository.GetARICSByRoadSection(RoadSectionId).ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new ARICSListResponse("Record Not Found");
                }
                else
                {
                    return new ARICSListResponse(existingARICS);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.GetARICSForSheet Error: {Environment.NewLine}");
                return new ARICSListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> GetARICEDRoadSectionByAuthorityAndYear(long AuthorityId, int ARICSYear)
        {
            try
            {
                var iActionResult = await _aRICSRepository.GetARICEDRoadSectionByAuthorityAndYear(AuthorityId, ARICSYear).ConfigureAwait(false);
                IList<RoadSheet> roadSheets = null;
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        roadSheets = (IList<RoadSheet>)result2.Value;
                    }
                }

                //Get unique road sections
                IList<RoadSection> roadSections = new List<RoadSection>();
                var RoadSectionDictionary = new Dictionary<long, RoadSection>();
                foreach (var roadSheet in roadSheets)
                {
                    //Add Road To Roads Dictionary
                    try
                    {
                        if (!RoadSectionDictionary.ContainsKey(roadSheet.RoadSection.ID))
                        {
                            RoadSectionDictionary.Add(roadSheet.RoadSection.ID, roadSheet.RoadSection);
                            roadSections.Add(roadSheet.RoadSection);
                        }
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"ARICSService.GetARICEDRoads :{Environment.NewLine}");
                    }
                }

                return new GenericResponse(Ok(roadSections));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSBatchService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the ARICSBatch record : {Ex.Message}");
            }
        }

        #region ARICED Road Sections
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSectionListResponse> GetARICEDRoadSections(int? Year)
        {
            try
            {
                var existingARICS = await _aRICSRepository.GetARICS(Year).ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new RoadSectionListResponse("Record Not Found");
                }
                else
                {
                    //Perform some operation to get the unique road sections for the ARICS 
                    IEnumerable<RoadSection> roadSections = await Task.Run(() => GetARICEDRoadSections((IList<ARICS>)existingARICS)).ConfigureAwait(false);
                    return new RoadSectionListResponse(roadSections);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSService.GetARICEDRoadSections (int? Year) Error: {Environment.NewLine}");
                return new RoadSectionListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<IList<RoadSection>> GetARICEDRoadSections(IList<ARICS> _ARICS)
        {
            IList<RoadSection> roadSections = new List<RoadSection>();
            var RoadSectionDictionary = new Dictionary<long, RoadSection>();
            foreach (var arics in _ARICS)
            {
                //Add RoadSection To RoadSection Dictionary
                try
                {
                    if (!RoadSectionDictionary.ContainsKey(arics.RoadSheet.RoadSection.ID))
                    {
                        RoadSectionDictionary.Add(arics.RoadSheet.RoadSection.ID, arics.RoadSheet.RoadSection);
                        roadSections.Add(arics.RoadSheet.RoadSection);
                    }
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ARICSService.GetARICEDRoadSections :{Environment.NewLine}");
                }
            }
            return roadSections;
        }

        /// <summary>
        /// GetARICEDRoadSection for authority in certain year
        /// </summary>
        /// <param name="authority"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public async Task<RoadSectionViewModelResponse> GetARICEDRoadSection(Authority authority, int? Year)
        {
            try
            {
                var aRICSList = await _aRICSRepository.GetARICS2(authority, Year).ConfigureAwait(false);

                IQueryable<RoadSectionViewModel> RoadSectionsData = Enumerable.Empty<RoadSectionViewModel>().AsQueryable(); 

                if (aRICSList.Any())
                {
                    //Return Ienumerable ARICS
                    var aRICSListEnumerable = aRICSList.AsEnumerable<ARICS>().ToList();

                    //Filter and return uniques
                    //Perform some operation to get the unique road sections for the ARICS 
                    IEnumerable<RoadSection> roadSections = await
                        Task.Run(() => GetARICEDRoadSections((IList<ARICS>)aRICSListEnumerable)).ConfigureAwait(false);

                    //Return iQueryable
                    var roadSectionsQueryable = roadSections.AsQueryable<RoadSection>();


                    RoadSectionsData =
                     from roadsections
                     in roadSectionsQueryable
                     select new RoadSectionViewModel()
                     {
                         id = roadsections.ID,
                         road_number = roadsections.Road.RoadNumber,
                         section_name = roadsections.SectionName,
                         length = Math.Round(roadsections.Length, 3, MidpointRounding.AwayFromZero)
                     };
                }


                if (aRICSList.Any())
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
                _logger.LogError(Ex, $"ARICSService.GetARICEDRoadSection Error: {Environment.NewLine}");
                return new RoadSectionViewModelResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        #endregion

    }
}
