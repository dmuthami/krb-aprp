using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class MessageOutService : IMessageOutService
    {
        private readonly IMessageOutRepository _messageOutRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public MessageOutService(IMessageOutRepository messageOutRepository, 
             IUnitOfWork unitOfWork
            , ILogger<MessageOutService> logger)
        {
            _messageOutRepository = messageOutRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<MessageOutResponse> AddAsync(MessageOut messageOut)
        {
            try
            {
                await _messageOutRepository.AddAsync(messageOut).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new MessageOutResponse(messageOut); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"MessageOutService.AddAsync Error: {Environment.NewLine}");
                return new MessageOutResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }
    }
}
