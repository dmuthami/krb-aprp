using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.Domain.Models;
using APRP.Web.ViewModels;

namespace APRP.Web.Services
{
    public class CountiesRoadService : ICountiesRoadService
    {

        private readonly ICountiesRoadRepository _countiesRoadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CountiesRoadService(ICountiesRoadRepository countiesRoadRepository, IUnitOfWork unitOfWork
            , ILogger<KenHARoadService> logger)
        {
            _countiesRoadRepository = countiesRoadRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CountiesRoadResponse> AddAsync(CountiesRoad countiesRoad)
        {
            try
            {
                await _countiesRoadRepository.AddAsync(countiesRoad).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new CountiesRoadResponse(countiesRoad); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CountiesRoadService.AddAsync Error: {Environment.NewLine}");
                return new CountiesRoadResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CountiesRoadResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingCountiesRoad = await _countiesRoadRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingCountiesRoad == null)
                {
                    return new CountiesRoadResponse("Record Not Found");
                }
                else
                {
                    return new CountiesRoadResponse(existingCountiesRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CountiesRoadService.FindByAsync Error: {Environment.NewLine}");
                return new CountiesRoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CountiesRoadResponse> FindByRoadNumberAsync(string RoadNumber)
        {
            try
            {
                var existingKenHARoad = await _countiesRoadRepository.FindByRoadNumberAsync(RoadNumber).ConfigureAwait(false);
                if (existingKenHARoad == null)
                {
                    return new CountiesRoadResponse("Record Not Found");
                }
                else
                {
                    return new CountiesRoadResponse(existingKenHARoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CountiesRoadService.FindBySectionIdAsync Error: {Environment.NewLine}");
                return new CountiesRoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CountiesRoadListResponse> ListAsync()
        {
            try
            {
                var countiesRoadList = await _countiesRoadRepository.ListAsync().ConfigureAwait(false);

                if (countiesRoadList == null)
                {
                    return new CountiesRoadListResponse("Record Not Found");
                }
                else
                {
                    return new CountiesRoadListResponse(countiesRoadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CountiesRoadService.ListAsync Error: {Environment.NewLine}");
                return new CountiesRoadListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CountiesRoadListResponse> ListAsync(string RoadNumber)
        {
            try
            {
                var countiesRoadList = await _countiesRoadRepository.ListAsync(RoadNumber).ConfigureAwait(false);

                if (countiesRoadList == null)
                {
                    return new CountiesRoadListResponse("Record Not Found");
                }
                else
                {
                    return new CountiesRoadListResponse(countiesRoadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CountiesRoadService.ListAsync(RoadID) Error: {Environment.NewLine}");
                return new CountiesRoadListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KenHARoadDictResponse> ListByRoadClassAsync()
        {
            try
            {
                var resp = await ListAsync().ConfigureAwait(false);
                var countiesRoads = resp.CountiesRoads;
                if (countiesRoads == null)
                {
                    return new KenHARoadDictResponse("Record Not Found");
                }
                decimal pavedinKm = countiesRoads.Where(s => s.SurfaceType != "Paved").Sum(x => x.Length ?? 0m);
                decimal unPavedSinKm = countiesRoads.Where(s => s.SurfaceType == "UnPaved").Sum(x => x.Length ?? 0m);
                decimal totalinKm = countiesRoads.Sum(x => x.Length ?? 0m);


                var dictionary = new Dictionary<string, string>();
                dictionary.Add("Paved", String.Format("{0:0,0.00}", pavedinKm));
                dictionary.Add("UnPaved", String.Format("{0:0,0.00}", unPavedSinKm));
                dictionary.Add("Total", String.Format("{0:0,0.00}", totalinKm));

                return new KenHARoadDictResponse(dictionary);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CountiesRoadService.ListByRoadClassAsync Error: {Environment.NewLine}");
                return new KenHARoadDictResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        public async Task<CountiesRoadViewModelResponse> ListViewAsync(long AuthorityId)
        {
            try
            {
                var roadList = await _countiesRoadRepository.ListViewAsync(AuthorityId).ConfigureAwait(false);
                IQueryable<CountiesRoadViewModel> countiesRoadsData;
                countiesRoadsData =
                 from roads in roadList
                 select new CountiesRoadViewModel()
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
                    return new CountiesRoadViewModelResponse("Record Not Found");
                }
                else
                {
                    return new CountiesRoadViewModelResponse(countiesRoadsData);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CountiesService.ListAsync Error: {Environment.NewLine}");
                return new CountiesRoadViewModelResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CountiesRoadResponse> RemoveAsync(long ID)
        {
            var existingCountiesRoad = await _countiesRoadRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingCountiesRoad == null)
            {
                return new CountiesRoadResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _countiesRoadRepository.Remove(existingCountiesRoad);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new CountiesRoadResponse(existingCountiesRoad);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"CountiesService.RemoveAsync Error: {Environment.NewLine}");
                    return new CountiesRoadResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CountiesRoadResponse> UpdateAsync(long ID, CountiesRoad countiesRoad)
        {
            try
            {
                _countiesRoadRepository.Update(ID, countiesRoad);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new CountiesRoadResponse(countiesRoad);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CountiesRoadService.Update Error: {Environment.NewLine}");
                return new CountiesRoadResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}

