using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.Domain.Models;
using APRP.Web.ViewModels;

namespace APRP.Web.Services
{
    public class KeRRARoadService : IKeRRARoadService
    {

        private readonly IKeRRARoadRepository _keRRARoadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public KeRRARoadService(IKeRRARoadRepository keRRARoadRepository, IUnitOfWork unitOfWork
            , ILogger<KenHARoadService> logger)
        {
            _keRRARoadRepository = keRRARoadRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KeRRARoadResponse> AddAsync(KerraRoad kerraRoad)
        {
            try
            {
                await _keRRARoadRepository.AddAsync(kerraRoad).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new KeRRARoadResponse(kerraRoad); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KeRRARoadService.AddAsync Error: {Environment.NewLine}");
                return new KeRRARoadResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KeRRARoadResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingKeRRARoad = await _keRRARoadRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingKeRRARoad == null)
                {
                    return new KeRRARoadResponse("Record Not Found");
                }
                else
                {
                    return new KeRRARoadResponse(existingKeRRARoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KeRRARoadService.FindByAsync Error: {Environment.NewLine}");
                return new KeRRARoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KeRRARoadResponse> FindByRoadNumberAsync(string RoadNumber)
        {
            try
            {
                var existingKeRRARoad = await _keRRARoadRepository.FindByRoadNumberAsync(RoadNumber).ConfigureAwait(false);
                if (existingKeRRARoad == null)
                {
                    return new KeRRARoadResponse("Record Not Found");
                }
                else
                {
                    return new KeRRARoadResponse(existingKeRRARoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KeRRARoadService.FindByAsync Error: {Environment.NewLine}");
                return new KeRRARoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KeRRARoadResponse> FindBySectionIdAsync(string SectionID)
        {
            try
            {
                var existingKenHARoad = await _keRRARoadRepository.FindBySectionIdAsync(SectionID).ConfigureAwait(false);
                if (existingKenHARoad == null)
                {
                    return new KeRRARoadResponse("Record Not Found");
                }
                else
                {
                    return new KeRRARoadResponse(existingKenHARoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KeRRARoadService.FindBySectionIdAsync Error: {Environment.NewLine}");
                return new KeRRARoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KeRRARoadListResponse> ListAsync()
        {
            try
            {
                var kenHARoadList = await _keRRARoadRepository.ListAsync().ConfigureAwait(false);

                if (kenHARoadList == null)
                {
                    return new KeRRARoadListResponse("Record Not Found");
                }
                else
                {
                    return new KeRRARoadListResponse(kenHARoadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KeRRARoadService.ListAsync Error: {Environment.NewLine}");
                return new KeRRARoadListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KeRRARoadListResponse> ListAsync(string RoadNumber)
        {
            try
            {
                var kenHARoadList = await _keRRARoadRepository.ListAsync(RoadNumber).ConfigureAwait(false);

                if (kenHARoadList == null)
                {
                    return new KeRRARoadListResponse("Record Not Found");
                }
                else
                {
                    return new KeRRARoadListResponse(kenHARoadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KeRRARoadService.ListAsync Error: {Environment.NewLine}");
                return new KeRRARoadListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KenHARoadDictResponse> ListByRoadClassAsync()
        {
            try
            {
                var resp = await ListAsync().ConfigureAwait(false);
                var kerraRoads = resp.KerraRoads;
                if (kerraRoads == null)
                {
                    return new KenHARoadDictResponse("Record Not Found");
                }
                decimal pavedinKm = kerraRoads.Where(s =>s.SurfaceType != "Paved").Sum(x => x.Length ?? 0m);
                decimal unPavedSinKm = kerraRoads.Where(s =>s.SurfaceType == "UnPaved" ).Sum(x => x.Length ?? 0m);
                decimal totalinKm = kerraRoads.Sum(x => x.Length ?? 0m);


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

        public async Task<KerraRoadViewModelResponse> ListViewAsync()
        {
            try
            {
                var roadList = await _keRRARoadRepository.ListViewAsync().ConfigureAwait(false);
                IQueryable<KerraRoadViewModel> kerraRoadsData;
                kerraRoadsData =
                 from roads in roadList
                 select new KerraRoadViewModel()
                 {
                     id = roads.ID,
                     rdnum = roads.RdNum,
                     rdname = roads.RdName,
                     sectionid = roads.Section_ID,
                     secname = roads.Sec_Name,
                     surfaceclass = roads.CW_Surf_Co,
                     surfacetype = roads.SurfaceType,
                     rdclass = roads.RdClass,
                     length = Math.Round(roads.Length??0, 2, MidpointRounding.AwayFromZero) 
                 };

                if (roadList == null)
                {
                    return new KerraRoadViewModelResponse("Record Not Found");
                }
                else
                {
                    return new KerraRoadViewModelResponse(kerraRoadsData);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KERRAService.ListAsync Error: {Environment.NewLine}");
                return new KerraRoadViewModelResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        public async Task<KeRRARoadResponse> RemoveAsync(long ID)
        {
            var existingKerraRoad = await _keRRARoadRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingKerraRoad == null)
            {
                return new KeRRARoadResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _keRRARoadRepository.Remove(existingKerraRoad);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new KeRRARoadResponse(existingKerraRoad);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"KERRAService.RemoveAsync Error: {Environment.NewLine}");
                    return new KeRRARoadResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KeRRARoadResponse> UpdateAsync(long ID, KerraRoad kerraRoad)
        {
            try
            {
                _keRRARoadRepository.Update(ID, kerraRoad);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new KeRRARoadResponse(kerraRoad);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KeRRARoadService.Update Error: {Environment.NewLine}");
                return new KeRRARoadResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
