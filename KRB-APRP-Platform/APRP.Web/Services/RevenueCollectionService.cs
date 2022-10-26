using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class RevenueCollectionService : IRevenueCollectionService
    {
        private readonly IRevenueCollectionRepository _revenueCollectionRepository;
        private readonly IShoulderSurfaceTypePavedService _shoulderSurfaceTypePavedService;
        private readonly IShoulderInterventionPavedService _shoulderInterventionPavedService;
        private readonly ISurfaceTypeUnPavedService _surfaceTypeUnPavedService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RevenueCollectionService(IRevenueCollectionRepository aRICSRepository, IUnitOfWork unitOfWork
            , ILogger<RevenueCollectionService> logger,
            IShoulderSurfaceTypePavedService shoulderSurfaceTypePavedService,
            IShoulderInterventionPavedService shoulderInterventionPavedService,
            ISurfaceTypeUnPavedService surfaceTypeUnPavedService)
        {
            _revenueCollectionRepository = aRICSRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _shoulderSurfaceTypePavedService = shoulderSurfaceTypePavedService;
            _shoulderInterventionPavedService = shoulderInterventionPavedService;
            _surfaceTypeUnPavedService = surfaceTypeUnPavedService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionResponse> AddAsync(RevenueCollection revenueCollection)
        {
            try
            {
                await _revenueCollectionRepository.AddAsync(revenueCollection).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RevenueCollectionResponse(revenueCollection); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionService.AddAsync Error: {Environment.NewLine}");
                return new RevenueCollectionResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingARICS = await _revenueCollectionRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new RevenueCollectionResponse("Record Not Found");
                }
                else
                {
                    return new RevenueCollectionResponse(existingARICS);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionService.FinfByIdAsync Error: {Environment.NewLine}");
                return new RevenueCollectionResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionResponse> FindByRevenueCollectionCodeUnitIdAsync(long RevenueCollectionCodeUnitId)
        {
            try
            {
                var existingARICS = await _revenueCollectionRepository.FindByRevenueCollectionCodeUnitIdAsync(RevenueCollectionCodeUnitId).ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new RevenueCollectionResponse("Record Not Found");
                }
                else
                {
                    return new RevenueCollectionResponse(existingARICS);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionService.FinfByIdAsync Error: {Environment.NewLine}");
                return new RevenueCollectionResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionResponse> FindByRevenueStreamAndFinancialYearAsync(long FinancialYearID, RevenueStream RevenueStream)
        {
            try
            {
                var revenueStream = await _revenueCollectionRepository.FindByRevenueStreamAndFinancialYearAsync(FinancialYearID, RevenueStream).ConfigureAwait(false);
                if (revenueStream == null)
                {
                    return new RevenueCollectionResponse("Record Not Found");
                }
                else
                {
                    return new RevenueCollectionResponse(revenueStream);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionService.FinfByIdAsync Error: {Environment.NewLine}");
                return new RevenueCollectionResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionListResponse> ListAsync()
        {
            try
            {
                var existingARICS = await _revenueCollectionRepository.ListAsync().ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new RevenueCollectionListResponse("Records Not Found");
                }
                else
                {
                    return new RevenueCollectionListResponse(existingARICS);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionService.ListAsync Error: {Environment.NewLine}");
                return new RevenueCollectionListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionListResponse> ListAsync(long FinancialYearId, string Type)
        {
            try
            {
                var existingRevenueCollectionItem = await _revenueCollectionRepository.ListAsync(FinancialYearId, Type).ConfigureAwait(false);
                if (existingRevenueCollectionItem == null)
                {
                    return new RevenueCollectionListResponse("Records Not Found");
                }
                else
                {
                    return new RevenueCollectionListResponse(existingRevenueCollectionItem);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionService.ListAsync Error: {Environment.NewLine}");
                return new RevenueCollectionListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionResponse> RemoveAsync(long ID)
        {
            try
            {
                var existingARICS = await _revenueCollectionRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new RevenueCollectionResponse("Record Not Found");
                }
                else
                {
                    _revenueCollectionRepository.Remove(existingARICS);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new RevenueCollectionResponse(existingARICS);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionService.RemoveAsync Error: {Environment.NewLine}");
                return new RevenueCollectionResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<double> RevenueCollectionSum(IList<RevenueCollection> revenueCollectionList)
        {
            try
            {
                if (revenueCollectionList == null)
                {
                    return 0d;
                }
                else
                {
                    //Do a computation
                    return await Task.Run(() =>
                    {
                        double sum = 0.0;
                        sum = (revenueCollectionList.Sum(item => item.Amount));
                        return sum;
                    }).ConfigureAwait(false);
                    
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionService.RevenueCollectionSum Error: {Environment.NewLine}");
                return 0d;
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionResponse> Update(RevenueCollection revenueCollection)
        {
            try
            {
                var existingARICS = await _revenueCollectionRepository.FindByIdAsync(revenueCollection.ID).ConfigureAwait(false);
                if (existingARICS == null)
                {
                    return new RevenueCollectionResponse("Record Not Found");
                }
                else
                {
                    _revenueCollectionRepository.Update(revenueCollection);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new RevenueCollectionResponse(revenueCollection);
                }

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionService.Update Error: {Environment.NewLine}");
                return new RevenueCollectionResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RevenueCollectionResponse> Update(long ID, RevenueCollection revenueCollection)
        {
            try
            {
                _revenueCollectionRepository.Update(ID, revenueCollection);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RevenueCollectionResponse(revenueCollection);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionService.Update Error: {Environment.NewLine}");
                return new RevenueCollectionResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
