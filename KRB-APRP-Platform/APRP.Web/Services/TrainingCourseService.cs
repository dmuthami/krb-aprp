using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class TrainingCourseService : ITrainingCourseService
    {
        private readonly ITrainingCourseRepository _trainingCourseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public TrainingCourseService(ITrainingCourseRepository allocationCodeUnitRepository, IUnitOfWork unitOfWork
            , ILogger<TrainingCourseService> logger)
        {
            _trainingCourseRepository = allocationCodeUnitRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingCourseResponse> AddAsync(TrainingCourse trainingCourse)
        {
            try
            {
                await _trainingCourseRepository.AddAsync(trainingCourse).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new TrainingCourseResponse(trainingCourse); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingCourseService.AddAsync Error: {Environment.NewLine}");
                return new TrainingCourseResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingCourseResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingTrainingCourse = await _trainingCourseRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingTrainingCourse == null)
                {
                    return new TrainingCourseResponse("Records Not Found");
                }
                else
                {
                    return new TrainingCourseResponse(existingTrainingCourse);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingCourseService.ListAsync Error: {Environment.NewLine}");
                return new TrainingCourseResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<TrainingCourseResponse> FindByCourseAsync(string Course)
        {
            try
            {
                var existingTrainingCourse = await _trainingCourseRepository.FindByCourseAsync(Course).ConfigureAwait(false);
                if (existingTrainingCourse == null)
                {
                    return new TrainingCourseResponse("Records Not Found");
                }
                else
                {
                    return new TrainingCourseResponse(existingTrainingCourse);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingCourseService.ListAsync Error: {Environment.NewLine}");
                return new TrainingCourseResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingCourseListResponse> ListAsync()
        {
            try
            {
                var existingTrainingCourses = await _trainingCourseRepository.ListAsync().ConfigureAwait(false);
                if (existingTrainingCourses == null)
                {
                    return new TrainingCourseListResponse("Records Not Found");
                }
                else
                {
                    return new TrainingCourseListResponse(existingTrainingCourses);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingCourseService.ListAsync Error: {Environment.NewLine}");
                return new TrainingCourseListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingCourseResponse> RemoveAsync(long ID)
        {
            var existingTrainingCourse = await _trainingCourseRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingTrainingCourse == null)
            {
                return new TrainingCourseResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _trainingCourseRepository.Remove(existingTrainingCourse);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new TrainingCourseResponse(existingTrainingCourse);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"TrainingCourseService.RemoveAsync Error: {Environment.NewLine}");
                    return new TrainingCourseResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingCourseResponse> Update(TrainingCourse trainingCourse)
        {
            var existingTrainingCourse = await _trainingCourseRepository.FindByIdAsync(trainingCourse.ID).ConfigureAwait(false);
            if (existingTrainingCourse == null)
            {
                return new TrainingCourseResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _trainingCourseRepository.Update(trainingCourse);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new TrainingCourseResponse(trainingCourse);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"TrainingCourseService.Update Error: {Environment.NewLine}");
                    return new TrainingCourseResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<TrainingCourseResponse> Update(long ID, TrainingCourse trainingCourse)
        {
            try
            {
                _trainingCourseRepository.Update(ID, trainingCourse);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new TrainingCourseResponse(trainingCourse);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingCourseService.Update Error: {Environment.NewLine}");
                return new TrainingCourseResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
