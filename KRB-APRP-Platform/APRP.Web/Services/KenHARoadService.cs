using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels;

namespace APRP.Web.Services
{
    public class KenHARoadService : IKenHARoadService
    {

        private readonly IKenHARoadRepository _kenHARoadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public KenHARoadService(IKenHARoadRepository kenHARoadRepository, IUnitOfWork unitOfWork
            , ILogger<KenHARoadService> logger)
        {
            _kenHARoadRepository = kenHARoadRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KenHARoadResponse> AddAsync(KenhaRoad kenhaRoad)
        {
            try
            {
                await _kenHARoadRepository.AddAsync(kenhaRoad).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new KenHARoadResponse(kenhaRoad); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KenHARoadService.AddAsync Error: {Environment.NewLine}");
                return new KenHARoadResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KenHARoadResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingKenHARoad = await _kenHARoadRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingKenHARoad == null)
                {
                    return new KenHARoadResponse("Record Not Found");
                }
                else
                {
                    return new KenHARoadResponse(existingKenHARoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KenHARoadService.FindByAsync Error: {Environment.NewLine}");
                return new KenHARoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KenHARoadResponse> FindByRoadNumberAsync(string RoadNumber)
        {
            try
            {
                var existingKenHARoad = await _kenHARoadRepository.FindByRoadNumberAsync(RoadNumber).ConfigureAwait(false);
                if (existingKenHARoad == null)
                {
                    return new KenHARoadResponse("Record Not Found");
                }
                else
                {
                    return new KenHARoadResponse(existingKenHARoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KenHARoadService.FindByAsync Error: {Environment.NewLine}");
                return new KenHARoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KenHARoadResponse> FindBySectionIdAsync(string SectionID)
        {
            try
            {
                var existingKenHARoad = await _kenHARoadRepository.FindBySectionIdAsync(SectionID).ConfigureAwait(false);
                if (existingKenHARoad == null)
                {
                    return new KenHARoadResponse("Record Not Found");
                }
                else
                {
                    return new KenHARoadResponse(existingKenHARoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KenHARoadService.FindBySectionIdAsync Error: {Environment.NewLine}");
                return new KenHARoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KenHARoadListResponse> ListAsync()
        {
            try
            {
                var kenHARoadList = await _kenHARoadRepository.ListAsync().ConfigureAwait(false);

                if (kenHARoadList == null)
                {
                    return new KenHARoadListResponse("Record Not Found");
                }
                else
                {
                    return new KenHARoadListResponse(kenHARoadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KenHARoadService.ListAsync Error: {Environment.NewLine}");
                return new KenHARoadListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KenHARoadListResponse> ListAsync(string RoadNumber)
        {
            try
            {
                var kenHARoadList = await _kenHARoadRepository.ListAsync(RoadNumber).ConfigureAwait(false);

                if (kenHARoadList == null)
                {
                    return new KenHARoadListResponse("Record Not Found");
                }
                else
                {
                    return new KenHARoadListResponse(kenHARoadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KenHARoadService.ListAsync Error: {Environment.NewLine}");
                return new KenHARoadListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KenHARoadDictResponse> ListByRoadClassAsync()
        {
            try
            {
                var resp = await ListAsync().ConfigureAwait(false);
                var kenhaRoads = resp.KenhaRoads;
                if (kenhaRoads == null)
                {
                    return new KenHARoadDictResponse("Record Not Found");
                }
                decimal pavedAinKm = kenhaRoads.Where(s => s.RdClass == "A" && (s.SurfaceType != "Paved")).Sum(x => x.Length ?? 0m);
                decimal pavedSinKm = kenhaRoads.Where(s => s.RdClass == "S" && (s.SurfaceType != "Paved")).Sum(x => x.Length ?? 0m);
                decimal pavedBinKm = kenhaRoads.Where(s => s.RdClass == "B" && (s.SurfaceType != "Paved")).Sum(x => x.Length ?? 0m);
                decimal unPavedAinKm = kenhaRoads.Where(s => s.RdClass == "A" && (s.SurfaceType == "UnPaved")).Sum(x => x.Length ?? 0m);
                decimal unPavedSinKm = kenhaRoads.Where(s => s.RdClass == "S" && (s.SurfaceType == "UnPaved")).Sum(x => x.Length ?? 0m);
                decimal unPavedBinKm = kenhaRoads.Where(s => s.RdClass == "B" && (s.SurfaceType == "UnPaved")).Sum(x => x.Length ?? 0m);

                decimal totalAinKm = kenhaRoads.Where(s => s.RdClass == "A").Sum(x => x.Length ?? 0m);
                decimal totalSinKm = kenhaRoads.Where(s => s.RdClass == "S").Sum(x => x.Length ?? 0m);
                decimal totalBinKm = kenhaRoads.Where(s => s.RdClass == "B").Sum(x => x.Length ?? 0m);


                var dictionary = new Dictionary<string, string>();
                dictionary.Add("Paved_A_S", String.Format("{0:0,0.00}", pavedAinKm + pavedSinKm));
                dictionary.Add("Paved_B", String.Format("{0:0,0.00}", pavedBinKm));
                dictionary.Add("UnPaved_A_S", String.Format("{0:0,0.00}", unPavedAinKm + unPavedSinKm));
                dictionary.Add("UnPaved_B", String.Format("{0:0,0.00}", unPavedBinKm));
                dictionary.Add("Total_A_S", String.Format("{0:0,0.00}", totalAinKm + totalSinKm));
                dictionary.Add("Total_B", String.Format("{0:0,0.00}", totalBinKm));

                return new KenHARoadDictResponse(dictionary);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KenHARoadService.ListByRoadClassAsync Error: {Environment.NewLine}");
                return new KenHARoadDictResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KenhaRoadViewModelResponse> ListViewAsync()
        {
            try
            {
                var roadList = await _kenHARoadRepository.ListViewAsync().ConfigureAwait(false);
                IQueryable<KenhaRoadViewModel> kenhaRoadsData;
                kenhaRoadsData =
                 from roads in roadList
                 select new KenhaRoadViewModel()
                 {
                     id = roads.ID,
                     rdnum = roads.RdNum,
                     rdname = roads.RdName,
                     sectionid = roads.Section_ID,
                     secname = roads.Sec_Name,
                     surfaceclass = roads.CW_Surf_Co,
                     surfacetype = roads.SurfaceType,
                     rdclass = roads.RdClass,
                     length = Math.Round(roads.Length ?? 0, 2, MidpointRounding.AwayFromZero)
                 };

                if (roadList == null)
                {
                    return new KenhaRoadViewModelResponse("Record Not Found");
                }
                else
                {
                    return new KenhaRoadViewModelResponse(kenhaRoadsData);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KENHAService.ListAsync Error: {Environment.NewLine}");
                return new KenhaRoadViewModelResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KenHARoadResponse> RemoveAsync(long ID)
        {
            var existingKenhaRoad = await _kenHARoadRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingKenhaRoad == null)
            {
                return new KenHARoadResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _kenHARoadRepository.Remove(existingKenhaRoad);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new KenHARoadResponse(existingKenhaRoad);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"KeNHARoadService.RemoveAsync Error: {Environment.NewLine}");
                    return new KenHARoadResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KenHARoadResponse> UpdateAsync(long ID, KenhaRoad kenhaRoad)
        {
            try
            {
                _kenHARoadRepository.Update(ID, kenhaRoad);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new KenHARoadResponse(kenhaRoad);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KeNHARoadService.Update Error: {Environment.NewLine}");
                return new KenHARoadResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
