using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.Domain.Models;
using APRP.Web.ViewModels;

namespace APRP.Web.Services
{
    public class KuRARoadService : IKuRARoadService
    {

        private readonly IKuRARoadRepository _kuRARoadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public KuRARoadService(IKuRARoadRepository kuRARoadRepository, IUnitOfWork unitOfWork
            , ILogger<KenHARoadService> logger)
        {
            _kuRARoadRepository = kuRARoadRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KuRRARoadResponse> AddAsync(KuraRoad kuraRoad)
        {
            try
            {
                await _kuRARoadRepository.AddAsync(kuraRoad).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new KuRRARoadResponse(kuraRoad); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KURARoadService.AddAsync Error: {Environment.NewLine}");
                return new KuRRARoadResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KuRRARoadResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingKeRRARoad = await _kuRARoadRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingKeRRARoad == null)
                {
                    return new KuRRARoadResponse("Record Not Found");
                }
                else
                {
                    return new KuRRARoadResponse(existingKeRRARoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KuRARoadService.FindByAsync Error: {Environment.NewLine}");
                return new KuRRARoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KuRRARoadResponse> FindByRoadNumberAsync(string RoadNumber)
        {
            try
            {
                var existingKenHARoad = await _kuRARoadRepository.FindByRoadNumberAsync(RoadNumber).ConfigureAwait(false);
                if (existingKenHARoad == null)
                {
                    return new KuRRARoadResponse("Record Not Found");
                }
                else
                {
                    return new KuRRARoadResponse(existingKenHARoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KuRARoadService.FindBySectionIdAsync Error: {Environment.NewLine}");
                return new KuRRARoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KuRRARoadListResponse> ListAsync()
        {
            try
            {
                var kURARoadList = await _kuRARoadRepository.ListAsync().ConfigureAwait(false);

                if (kURARoadList == null)
                {
                    return new KuRRARoadListResponse("Record Not Found");
                }
                else
                {
                    return new KuRRARoadListResponse(kURARoadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.ListAsync Error: {Environment.NewLine}");
                return new KuRRARoadListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KuRRARoadListResponse> ListAsync(string RoadNumber)
        {
            try
            {
                var kURARoadList = await _kuRARoadRepository.ListAsync(RoadNumber).ConfigureAwait(false);

                if (kURARoadList == null)
                {
                    return new KuRRARoadListResponse("Record Not Found");
                }
                else
                {
                    return new KuRRARoadListResponse(kURARoadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadService.ListAsync(RoadID) Error: {Environment.NewLine}");
                return new KuRRARoadListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KenHARoadDictResponse> ListByRoadClassAsync()
        {
            try
            {
                var resp = await ListAsync().ConfigureAwait(false);
                var kuRARoads = resp.KuraRoads;
                if (kuRARoads == null)
                {
                    return new KenHARoadDictResponse("Record Not Found");
                }
                decimal pavedinKm = kuRARoads.Where(s => s.RdClass != "Paved").Sum(x => x.Length ?? 0m);
                decimal unPavedSinKm = kuRARoads.Where(s => s.RdClass == "Unpaved").Sum(x => x.Length ?? 0m);
                decimal totalinKm = kuRARoads.Sum(x => x.Length ?? 0m);


                var dictionary = new Dictionary<string, string>();
                dictionary.Add("Paved", String.Format("{0:0,0.00}", pavedinKm));
                dictionary.Add("UnPaved", String.Format("{0:0,0.00}", unPavedSinKm));
                dictionary.Add("Total", String.Format("{0:0,0.00}", totalinKm));

                return new KenHARoadDictResponse(dictionary);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KenHARoadService.ListByRoadClassAsync Error: {Environment.NewLine}");
                return new KenHARoadDictResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        public async Task<KuraRoadViewModelResponse> ListViewAsync()
        {
            try
            {
                var roadList = await _kuRARoadRepository.ListViewAsync().ConfigureAwait(false);
                IQueryable<KuraRoadViewModel> kuraRoadsData;
                kuraRoadsData =
                 from roads in roadList
                 select new KuraRoadViewModel()
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
                    return new KuraRoadViewModelResponse("Record Not Found");
                }
                else
                {
                    return new KuraRoadViewModelResponse(kuraRoadsData);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KURAService.ListAsync Error: {Environment.NewLine}");
                return new KuraRoadViewModelResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KuRRARoadResponse> RemoveAsync(long ID)
        {
            var existingKuraRoad = await _kuRARoadRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingKuraRoad == null)
            {
                return new KuRRARoadResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _kuRARoadRepository.Remove(existingKuraRoad);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new KuRRARoadResponse(existingKuraRoad);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"KURAService.RemoveAsync Error: {Environment.NewLine}");
                    return new KuRRARoadResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KuRRARoadResponse> UpdateAsync(long ID, KuraRoad kuraRoad)
        {
            try
            {
                _kuRARoadRepository.Update(ID, kuraRoad);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new KuRRARoadResponse(kuraRoad);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KURARoadService.Update Error: {Environment.NewLine}");
                return new KuRRARoadResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
