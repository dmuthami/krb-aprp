using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.Domain.Models;
using APRP.Web.ViewModels;

namespace APRP.Web.Services
{
    public class KwSRoadService : IKwSRoadService
    {

        private readonly IKwSRoadRepository _kwSRoadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public KwSRoadService(IKwSRoadRepository kwSRoadRepository, IUnitOfWork unitOfWork
            , ILogger<KenHARoadService> logger)
        {
            _kwSRoadRepository = kwSRoadRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KwSRoadResponse> AddAsync(KwsRoad kwsRoad)
        {
            try
            {
                await _kwSRoadRepository.AddAsync(kwsRoad).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new KwSRoadResponse(kwsRoad); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KwsRoadService.AddAsync Error: {Environment.NewLine}");
                return new KwSRoadResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KwSRoadResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingKeRRARoad = await _kwSRoadRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingKeRRARoad == null)
                {
                    return new KwSRoadResponse("Record Not Found");
                }
                else
                {
                    return new KwSRoadResponse(existingKeRRARoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KwSRoadService.FindByAsync Error: {Environment.NewLine}");
                return new KwSRoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KwSRoadResponse> FindByRoadNumberAsync(string RoadNumber)
        {
            try
            {
                var existingKeRRARoad = await _kwSRoadRepository.FindByRoadNumberAsync(RoadNumber).ConfigureAwait(false);
                if (existingKeRRARoad == null)
                {
                    return new KwSRoadResponse("Record Not Found");
                }
                else
                {
                    return new KwSRoadResponse(existingKeRRARoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KwSRoadService.FindByAsync Error: {Environment.NewLine}");
                return new KwSRoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KwSRoadResponse> FindBySectionIdAsync(string SectionID)
        {
            try
            {
                var existingKenHARoad = await _kwSRoadRepository.FindBySectionIdAsync(SectionID).ConfigureAwait(false);
                if (existingKenHARoad == null)
                {
                    return new KwSRoadResponse("Record Not Found");
                }
                else
                {
                    return new KwSRoadResponse(existingKenHARoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KwSRoadService.FindBySectionIdAsync Error: {Environment.NewLine}");
                return new KwSRoadResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KwSRoadListResponse> ListAsync()
        {
            try
            {
                var kwSRoadList = await _kwSRoadRepository.ListAsync().ConfigureAwait(false);

                if (kwSRoadList == null)
                {
                    return new KwSRoadListResponse("Record Not Found");
                }
                else
                {
                    return new KwSRoadListResponse(kwSRoadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KwSRoadService.ListAsync Error: {Environment.NewLine}");
                return new KwSRoadListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KwSRoadListResponse> ListAsync(string RoadNumber)
        {
            try
            {
                var kwSRoadList = await _kwSRoadRepository.ListAsync(RoadNumber).ConfigureAwait(false);

                if (kwSRoadList == null)
                {
                    return new KwSRoadListResponse("Record Not Found");
                }
                else
                {
                    return new KwSRoadListResponse(kwSRoadList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KwSRoadService.ListAsync Error: {Environment.NewLine}");
                return new KwSRoadListResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KwsRoadViewModelResponse> ListViewAsync()
        {
            try
            {
                var roadList = await _kwSRoadRepository.ListViewAsync().ConfigureAwait(false);
                IQueryable<KwsRoadViewModel> kerraRoadsData;
                kerraRoadsData =
                 from roads in roadList
                 select new KwsRoadViewModel()
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
                    return new KwsRoadViewModelResponse("Record Not Found");
                }
                else
                {
                    return new KwsRoadViewModelResponse(kerraRoadsData);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KWSService.ListAsync Error: {Environment.NewLine}");
                return new KwsRoadViewModelResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KwSRoadResponse> RemoveAsync(long ID)
        {
            var existingKwsRoad = await _kwSRoadRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingKwsRoad == null)
            {
                return new KwSRoadResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _kwSRoadRepository.Remove(existingKwsRoad);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new KwSRoadResponse(existingKwsRoad);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"KWSService.RemoveAsync Error: {Environment.NewLine}");
                    return new KwSRoadResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<KwSRoadResponse> UpdateAsync(long ID, KwsRoad kwsRoad)
        {
            try
            {
                _kwSRoadRepository.Update(ID, kwsRoad);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new KwSRoadResponse(kwsRoad);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"KWSRoadService.Update Error: {Environment.NewLine}");
                return new KwSRoadResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
