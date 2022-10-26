using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels.UserViewModels;

namespace APRP.Web.Services
{
    public class RoadSheetService : IRoadSheetService
    {

        private readonly IRoadSheetRepository _roadSheetRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly ITerrainTypeService _terrainTypeService;

        public RoadSheetService(IRoadSheetRepository roadSheetRepository, IUnitOfWork unitOfWork
            , ILogger<RoadSheetService> logger,
            ITerrainTypeService terrainTypeService)
        {
            _roadSheetRepository = roadSheetRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _terrainTypeService = terrainTypeService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetResponse> AddAsync(RoadSheet roadSheet)
        {
            try
            {
                await _roadSheetRepository.AddAsync(roadSheet).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadSheetResponse(roadSheet);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetService.AddAsync Error: {Environment.NewLine}");
                return new RoadSheetResponse($"Error occured while adding the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetResponse> CheckRoadSheetsForYear(RoadSheetVM _RoadSheetVM)
        {
            try
            {
                var roadSheet = await _roadSheetRepository.CheckRoadSheetsForYear(_RoadSheetVM).ConfigureAwait(false);

                return new RoadSheetResponse(roadSheet);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetService.CheckRoadSheetsForYear Error: {Environment.NewLine}");
                return new RoadSheetResponse($"Error occured while adding the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetListResponse> CreateRoadSheets(double roadLengthKM, double sectionLengthKM, long RoadSectionID, int Year)
        {
            try
            {
                //Get default terrain type
                var terrrainTypeResponse = await _terrainTypeService.ListAsync().ConfigureAwait(false);
                IList<TerrainType> terrainType = (IList<TerrainType>)terrrainTypeResponse.TerrainType;
                long terrainTypeId = terrainType.Where(s => s.Code == "NA").FirstOrDefault().ID;

                int myQuotient = (int)Math.Ceiling(roadLengthKM);
                int myDivisor = (int)Math.Ceiling(sectionLengthKM);
                int remainder, quotient = Math.DivRem(myQuotient, myDivisor, out remainder);
                if (remainder > 0)
                {
                    quotient++;
                }
                RoadSheet roadSheet = null;

                //for each sheet, post record in DB
                double startChain = 0 - sectionLengthKM; double endChain = 0;
                for (int i = 1; i <= quotient; i++)
                {
                    roadSheet = new RoadSheet();

                    //Set year
                    roadSheet.Year = Year;

                    //set road section id
                    roadSheet.RoadSectionId = RoadSectionID;

                    //set sheet number
                    roadSheet.SheetNo = i;

                    //Set Terrain Type
                    roadSheet.TerrainTypeId = terrainTypeId;

                    //Set start chainage and end chainage
                    startChain += sectionLengthKM;
                    endChain += sectionLengthKM;
                    if (i == quotient)
                    {
                        endChain -= sectionLengthKM;
                        endChain += Math.Round(roadLengthKM - (i - 1) * sectionLengthKM, 2);
                    }
                    roadSheet.StartChainage = startChain;
                    roadSheet.EndChainage = endChain;

                    //Add Road sheet
                    await _roadSheetRepository.AddAsync(roadSheet).ConfigureAwait(false);                    
                }

                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                //return same list
                var roadSheets = await _roadSheetRepository.DisplayRoadsheetsAsync(RoadSectionID,Year).ConfigureAwait(false);
                return new RoadSheetListResponse(roadSheets);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetService.CreateRoadSheets Error: {Environment.NewLine}");
                return null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetListResponse> DisplayRoadsheetsAsync(long RoadSectionID, int Year)
        {
            try
            {
                var roadSheets = await _roadSheetRepository.DisplayRoadsheetsAsync(RoadSectionID, Year).ConfigureAwait(false);
                return new RoadSheetListResponse(roadSheets);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetService.DisplayRoadsheetsAsync Error: {Environment.NewLine}");
                return null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRoadSheet = await _roadSheetRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRoadSheet == null)
                {
                    return new RoadSheetResponse("Record Not Found");
                }
                else
                {
                    return new RoadSheetResponse(existingRoadSheet);
                }

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetService.FindByIdAsync Error: {Environment.NewLine}");
                return new RoadSheetResponse($"Error occured while finding the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetListResponse> ListAsync()
        {
            try
            {
                var existingRoadSheets = await _roadSheetRepository.ListAsync().ConfigureAwait(false);

                if (existingRoadSheets == null)
                {
                    return new RoadSheetListResponse("Record Not Found");
                }
                else
                {
                    return new RoadSheetListResponse(existingRoadSheets);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetService.ListAsync Error: {Environment.NewLine}");
                return new RoadSheetListResponse($"Error occurred will removing the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetListResponse> ListByRoadSectionIdAsync(long RoadSectionID, int? Year)
        {            
            try
            {
                var existingRoadSheets = await _roadSheetRepository.ListByRoadSectionIdAsync(RoadSectionID,Year).ConfigureAwait(false);
                
                if (existingRoadSheets == null)
                {
                    return new RoadSheetListResponse("Record Not Found");
                }
                else
                {
                    return new RoadSheetListResponse(existingRoadSheets);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetService.ListByRoadSectionIdAsync Error: {Environment.NewLine}");
                return new RoadSheetListResponse($"Error occurred will removing the record : {Ex.Message}");
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetResponse> RemoveAsync(long ID)
        {
            try
            {
                var existingRoadSheet = await _roadSheetRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if(existingRoadSheet == null)
                {
                    return new RoadSheetResponse("Record Not Found");
                }
                else
                {
                    _roadSheetRepository.Remove(existingRoadSheet);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new RoadSheetResponse(existingRoadSheet);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetService.RemoveAsync Error: {Environment.NewLine}");
                return new RoadSheetResponse($"Error occurred will removing the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetResponse> Update(RoadSheet roadSheet)
        {
            try
            {
                var existingRoadSheet = await _roadSheetRepository.FindByIdAsync(roadSheet.ID).ConfigureAwait(false);
                if(existingRoadSheet == null)
                {
                    return new RoadSheetResponse("Record Not Found");
                }
                else
                {
                    existingRoadSheet.CarriageWidth = roadSheet.CarriageWidth;
                    existingRoadSheet.EndChainage = roadSheet.EndChainage;
                    existingRoadSheet.StartChainage = roadSheet.StartChainage;
                    existingRoadSheet.StructurePriority = roadSheet.StructurePriority;
                    existingRoadSheet.TerrainType = roadSheet.TerrainType;
                    existingRoadSheet.SpotImprovementPriority = roadSheet.SpotImprovementPriority;
                    
                    _roadSheetRepository.Update(existingRoadSheet);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new RoadSheetResponse(existingRoadSheet);
                }
            }
            catch(Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetService.RemoveAsync Error: {Environment.NewLine}");
                return new RoadSheetResponse($"Error occured while updating the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadSheetResponse> Update(long ID, RoadSheet roadSheet)
        {
            try
            {
                _roadSheetRepository.Update(ID, roadSheet);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadSheetResponse(roadSheet);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSheetService.Update Error: {Environment.NewLine}");
                return new RoadSheetResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

    }
}
