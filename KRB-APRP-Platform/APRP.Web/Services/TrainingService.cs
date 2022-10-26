using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly ITrainingRepository _trainingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public TrainingService(ITrainingRepository trainingRepository, IUnitOfWork unitOfWork
            , ILogger<TrainingService> logger)
        {
            _trainingRepository = trainingRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingResponse> AddAsync(Training training)
        {
            try
            {
                await _trainingRepository.AddAsync(training).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new TrainingResponse(training); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingService.AddAsync Error: {Environment.NewLine}");
                return new TrainingResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingTraining = await _trainingRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingTraining == null)
                {
                    return new TrainingResponse("Records Not Found");
                }
                else
                {
                    return new TrainingResponse(existingTraining);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingService.ListAsync Error: {Environment.NewLine}");
                return new TrainingResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<TrainingResponse> FindByQuarterCodeUnitIdAndAuthorityIdAsync(long QuarterCodeUnitId, long AuthorityId)
        {
            try
            {
                var existingTraining = await _trainingRepository.FindByQuarterCodeUnitIdAndAuthorityIdAsync(QuarterCodeUnitId, AuthorityId).ConfigureAwait(false);
                if (existingTraining == null)
                {
                    return new TrainingResponse("Records Not Found");
                }
                else
                {
                    return new TrainingResponse(existingTraining);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingService.ListAsync Error: {Environment.NewLine}");
                return new TrainingResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingResponse> FindByQuarterCodeUnitIdAndFinancialIdAsync(long QuarterCodeUnitId, long FinancialYearId)
        {
            try
            {
                var existingTraining = await _trainingRepository.FindByQuarterCodeUnitIdAndFinancialIdAsync(QuarterCodeUnitId, FinancialYearId).ConfigureAwait(false);
                if (existingTraining == null)
                {
                    return new TrainingResponse("Records Not Found");
                }
                else
                {
                    return new TrainingResponse(existingTraining);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingService.ListAsync Error: {Environment.NewLine}");
                return new TrainingResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingListResponse> GetCourceTrainingsThisFinancialYearAsync(int TrainingCourseID, long FinancialYearId)
        {
            try
            {
                var existingTrainings = await _trainingRepository.GetCourceTrainingsThisFinancialYearAsync(TrainingCourseID, FinancialYearId).ConfigureAwait(false);
                if (existingTrainings == null)
                {
                    return new TrainingListResponse("Records Not Found");
                }
                else
                {
                    return new TrainingListResponse(existingTrainings);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingService.ListAsync Error: {Environment.NewLine}");
                return new TrainingListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingListResponse> GetCourceTrainingsThisQuarterAsync(int TrainingCourseID, int quarter)
        {
            try
            {
                var existingTrainings = await _trainingRepository.GetCourceTrainingsThisQuarterAsync(TrainingCourseID, quarter).ConfigureAwait(false);
                if (existingTrainings == null)
                {
                    return new TrainingListResponse("Records Not Found");
                }
                else
                {
                    return new TrainingListResponse(existingTrainings);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingService.ListAsync Error: {Environment.NewLine}");
                return new TrainingListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<TrainingCourseListResponse> GetDistinctCoursesByFinancialYearAndAuthorityAsync(long AuthorityId, long FinancialYearId)
        {
            try
            {
                var existingTrainings = await _trainingRepository
                    .GetDistinctCoursesByFinancialYearAndAuthorityAsync(AuthorityId,FinancialYearId).ConfigureAwait(false);
                if (existingTrainings == null)
                {
                    return new TrainingCourseListResponse("Records Not Found");
                }
                else
                {
                    return new TrainingCourseListResponse(existingTrainings);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingService.GetDistinctCoursesByFinancialYearAndAuthorityAsync Error: {Environment.NewLine}");
                return new TrainingCourseListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingListResponse> ListAsync()
        {
            try
            {
                var existingTrainings = await _trainingRepository.ListAsync().ConfigureAwait(false);
                if (existingTrainings == null)
                {
                    return new TrainingListResponse("Records Not Found");
                }
                else
                {
                    return new TrainingListResponse(existingTrainings);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingService.ListAsync Error: {Environment.NewLine}");
                return new TrainingListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingListResponse> ListAsync(long AuthorityId)
        {
            try
            {
                var existingTrainings = await _trainingRepository.ListAsync(AuthorityId).ConfigureAwait(false);
                if (existingTrainings == null)
                {
                    return new TrainingListResponse("Records Not Found");
                }
                else
                {
                    return new TrainingListResponse(existingTrainings);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingService.ListAsync Error: {Environment.NewLine}");
                return new TrainingListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<TrainingListResponse> ListAsync(long AuthorityId, long FinancialYearId)
        {
            try
            {
                var existingTrainings = await _trainingRepository.ListAsync(AuthorityId,FinancialYearId).ConfigureAwait(false);
                if (existingTrainings == null)
                {
                    return new TrainingListResponse("Records Not Found");
                }
                else
                {
                    return new TrainingListResponse(existingTrainings);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingService.ListAsync Error: {Environment.NewLine}");
                return new TrainingListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingListResponse> ListByFinancialYearAsync(long FinancialYearId)
        {
            try
            {
                var existingTrainings = await _trainingRepository.ListByFinancialYearAsync(FinancialYearId).ConfigureAwait(false);
                if (existingTrainings == null)
                {
                    return new TrainingListResponse("Records Not Found");
                }
                else
                {
                    return new TrainingListResponse(existingTrainings);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingService.ListAsync Error: {Environment.NewLine}");
                return new TrainingListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<TrainingListResponse> ListByTrainingCourseInFinancialYearAsync(long TrainingCourseId, long FinancialYearId)
        {
            try
            {
                var existingTrainings = await _trainingRepository.ListByTrainingCourseInFinancialYearAsync(TrainingCourseId,FinancialYearId).ConfigureAwait(false);
                if (existingTrainings == null)
                {
                    return new TrainingListResponse("Records Not Found");
                }
                else
                {
                    return new TrainingListResponse(existingTrainings);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingService.ListAsync Error: {Environment.NewLine}");
                return new TrainingListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<TrainingListResponse> ListDistinctCoursesByFinancialYearAsync(long FinancialYearId)
        {
            try
            {
                var existingTrainings = await _trainingRepository.ListDistinctCoursesByFinancialYearAsync(FinancialYearId).ConfigureAwait(false);
                if (existingTrainings == null)
                {
                    return new TrainingListResponse("Records Not Found");
                }
                else
                {
                    return new TrainingListResponse(existingTrainings);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingService.ListAsync Error: {Environment.NewLine}");
                return new TrainingListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingResponse> RemoveAsync(long ID)
        {
            var existingTraining = await _trainingRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingTraining == null)
            {
                return new TrainingResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _trainingRepository.Remove(existingTraining);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new TrainingResponse(existingTraining);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"TrainingService.RemoveAsync Error: {Environment.NewLine}");
                    return new TrainingResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingResponse> Update(Training training)
        {
            var existingTraining = await _trainingRepository.FindByIdAsync(training.ID).ConfigureAwait(false);
            if (existingTraining == null)
            {
                return new TrainingResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _trainingRepository.Update(training);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new TrainingResponse(training);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"TrainingService.Update Error: {Environment.NewLine}");
                    return new TrainingResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingResponse> Update(long ID, Training training)
        {
            try
            {
                _trainingRepository.Update(ID, training);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new TrainingResponse(training);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingService.Update Error: {Environment.NewLine}");
                return new TrainingResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
