using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CommentService(ICommentRepository commentRepository, IUnitOfWork unitOfWork, ILogger<CommentService> logger)
        {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CommentResponse> AddAsync(Comment comment)
        {
            try
            {
                await _commentRepository.AddAsync(comment).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new CommentResponse(comment); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CommentService.AddAsync Error: {Environment.NewLine}");
                return new CommentResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CommentResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingComment = await _commentRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingComment == null)
                {
                    return new CommentResponse("Record Not Found");
                }
                else
                {
                    return new CommentResponse(existingComment);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CommentService.FindByIdAsync Error: {Environment.NewLine}");
                return new CommentResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CommentListResponse> ListAsync()
        {
            try
            {
                var existingCommentList = await _commentRepository.ListAsync().ConfigureAwait(false);
                if (existingCommentList == null)
                {
                    return new CommentListResponse("Records Not Found");
                }
                else
                {
                    return new CommentListResponse(existingCommentList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CommentService.ListAsync Error: {Environment.NewLine}");
                return new CommentListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<CommentListResponse> ListAsync(string Type, long ForeignId)
        {
            try
            {
                var existingCommentList = await _commentRepository.ListAsync(Type, ForeignId).ConfigureAwait(false);
                if (existingCommentList == null)
                {
                    return new CommentListResponse("Records Not Found");
                }
                else
                {
                    return new CommentListResponse(existingCommentList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CommentService.ListAsync Error: {Environment.NewLine}");
                return new CommentListResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CommentResponse> RemoveAsync(long ID)
        {
            var existingComment = await _commentRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingComment == null)
            {
                return new CommentResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _commentRepository.Remove(existingComment);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new CommentResponse(existingComment);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"CommentService.RemoveAsync Error: {Environment.NewLine}");
                    return new CommentResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<CommentResponse> Update(Comment comment)
        {
            var existingARICS = await _commentRepository.FindByIdAsync(comment.ID).ConfigureAwait(false);
            if (existingARICS == null)
            {
                return new CommentResponse("Record Not Found");
            }
            else
            {
                //existingRoad.RoadNumber = road.RoadNumber;
                //existingRoad.RoadName = road.RoadName;
                try
                {
                    _commentRepository.Update(comment);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new CommentResponse(comment);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"CommentService.Updated Error: {Environment.NewLine}");
                    return new CommentResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }
    }
}
