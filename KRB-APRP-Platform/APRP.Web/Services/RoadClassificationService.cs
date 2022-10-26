using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class RoadClassificationService : IRoadClassificationService
    {
        private readonly IRoadClassificationRepository _roadClassificationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RoadClassificationService(IRoadClassificationRepository allocationCodeUnitRepository, IUnitOfWork unitOfWork
            , ILogger<RoadClassificationService> logger)
        {
            _roadClassificationRepository = allocationCodeUnitRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadClassificationResponse> AddAsync(RoadClassification roadClassification)
        {
            try
            {
                await _roadClassificationRepository.AddAsync(roadClassification).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadClassificationResponse(roadClassification); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationService.AddAsync Error: {Environment.NewLine}");
                return new RoadClassificationResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadClassificationResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRoadClassification = await _roadClassificationRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRoadClassification == null)
                {
                    return new RoadClassificationResponse("Records Not Found");
                }
                else
                {
                    return new RoadClassificationResponse(existingRoadClassification);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationService.ListAsync Error: {Environment.NewLine}");
                return new RoadClassificationResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<RoadClassificationResponse> FindByNameAsync(string RoadName)
        {
            try
            {
                var existingRoadClassification = await _roadClassificationRepository.FindByNameAsync(RoadName).ConfigureAwait(false);
                if (existingRoadClassification == null)
                {
                    return new RoadClassificationResponse("Records Not Found");
                }
                else
                {
                    return new RoadClassificationResponse(existingRoadClassification);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationService.ListAsync Error: {Environment.NewLine}");
                return new RoadClassificationResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadClassificationResponse> FindByRoadIdAsync(string RoadId)
        {
            try
            {
                var existingRoadClassification = await _roadClassificationRepository.FindByRoadIdAsync(RoadId).ConfigureAwait(false);
                if (existingRoadClassification == null)
                {
                    return new RoadClassificationResponse("Records Not Found");
                }
                else
                {
                    return new RoadClassificationResponse(existingRoadClassification);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationService.ListAsync Error: {Environment.NewLine}");
                return new RoadClassificationResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadClassificationListResponse> ListAsync()
        {
            try
            {
                var existingRoadClassifications = await _roadClassificationRepository.ListAsync().ConfigureAwait(false);
                if (existingRoadClassifications == null)
                {
                    return new RoadClassificationListResponse("Records Not Found");
                }
                else
                {
                    return new RoadClassificationListResponse(existingRoadClassifications);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationService.ListAsync Error: {Environment.NewLine}");
                return new RoadClassificationListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<RoadClassificationListResponse> ListAsync(long AuthorityId)
        {
            try
            {
                var existingRoadClassifications = await _roadClassificationRepository.ListAsync(AuthorityId).ConfigureAwait(false);
                if (existingRoadClassifications == null)
                {
                    return new RoadClassificationListResponse("Records Not Found");
                }
                else
                {
                    return new RoadClassificationListResponse(existingRoadClassifications);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationService.ListAsync Error: {Environment.NewLine}");
                return new RoadClassificationListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<KenHARoadDictResponse> ListByStatusAsync()
        {
            try
            {
                var resp = await ListAsync().ConfigureAwait(false);
                var roadClassifications = resp.RoadClassification;
                if (roadClassifications == null)
                {
                    return new KenHARoadDictResponse("Record Not Found");
                }
                int New = roadClassifications.Where(s => s.Status ==0 ).Count();
                int Submitted = roadClassifications.Where(s => s.Status == 1).Count();
                int ApprovedAtAgency = roadClassifications.Where(s => s.Status == 2).Count();
                int RejectedAtAgency = roadClassifications.Where(s => s.Status == 3).Count();
                int ApprovedAtKRB = roadClassifications.Where(s => s.Status == 4).Count();
                int RejectedAtKRB = roadClassifications.Where(s => s.Status == 5).Count();
                int AddedToGIS = roadClassifications.Where(s => s.Status == 6).Count();


                var dictionary = new Dictionary<string, string>();
                dictionary.Add("New", New.ToString());
                dictionary.Add("Submitted", Submitted.ToString());
                dictionary.Add("ApprovedAtAgency", ApprovedAtAgency.ToString());
                dictionary.Add("RejectedAtAgency", RejectedAtAgency.ToString());
                dictionary.Add("ApprovedAtKRB", ApprovedAtKRB.ToString());
                dictionary.Add("RejectedAtKRB", RejectedAtKRB.ToString());
                dictionary.Add("AddedToGIS", AddedToGIS.ToString());

                return new KenHARoadDictResponse(dictionary);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationsService.ListByStatusAsync Error: {Environment.NewLine}");
                return new KenHARoadDictResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadClassificationResponse> RemoveAsync(long ID)
        {
            var existingRoadClassification = await _roadClassificationRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRoadClassification == null)
            {
                return new RoadClassificationResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _roadClassificationRepository.Remove(existingRoadClassification);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new RoadClassificationResponse(existingRoadClassification);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RoadClassificationService.RemoveAsync Error: {Environment.NewLine}");
                    return new RoadClassificationResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadClassificationResponse> Update(RoadClassification roadClassification)
        {
            var existingRoadClassification = await _roadClassificationRepository.FindByIdAsync(roadClassification.ID).ConfigureAwait(false);
            if (existingRoadClassification == null)
            {
                return new RoadClassificationResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _roadClassificationRepository.Update(roadClassification);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new RoadClassificationResponse(roadClassification);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"RoadClassificationService.Update Error: {Environment.NewLine}");
                    return new RoadClassificationResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadClassificationResponse> Update(long ID, RoadClassification roadClassification)
        {
            try
            {
                _roadClassificationRepository.Update(ID, roadClassification);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadClassificationResponse(roadClassification);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationService.Update Error: {Environment.NewLine}");
                return new RoadClassificationResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
