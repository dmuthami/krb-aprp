using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class AuthorityService : IAuthorityService
    {
        private readonly IAuthorityRepository _authorityRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public AuthorityService(IAuthorityRepository authorityRepository,
            IUnitOfWork unitOfWork,
            ILogger<AuthorityService> logger)
        {
            _authorityRepository = authorityRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AuthorityResponse> AddAsync(Authority authority)
        {
            try
            {
                await _authorityRepository.AddAsync(authority).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new AuthorityResponse(authority); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationService.AddAsync Error: {Environment.NewLine}");
                return new AuthorityResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        public async Task<AuthorityResponse> FindByCodeAsync(string code)
        {
            try
            {
                var existingRecord = await _authorityRepository.FindByCodeAsync(code).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new AuthorityResponse("Record Not Found");
                }
                else
                {
                    return new AuthorityResponse(existingRecord);
                }
            }
            catch (Exception ex)
            {
                //logging
                return new AuthorityResponse($"Error occured while getting the record {ex.Message}");
            }
        }

        public async Task<AuthorityResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord =  await _authorityRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new AuthorityResponse("Record Not Found");
                }
                else
                {
                    return new AuthorityResponse(existingRecord);
                }
            }
            catch (Exception ex)
            {
                //logging
                return new AuthorityResponse($"Error occured while getting the record {ex.Message}");
            }
        }

        public async Task<IEnumerable<Authority>> ListAsync()
        {
            return await _authorityRepository.ListAsync().ConfigureAwait(false);
        }

        public async Task<GenericResponse> ListAsync(int PageNumber, int PageSize)
        {
            try
            {
                var iActionResult = await _authorityRepository.ListAsync(PageNumber, PageSize).ConfigureAwait(false);
                return new GenericResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AuthorityService.ListAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        public async Task<IEnumerable<Authority>> ListAsync(string AuthorityType)
        {
            return await _authorityRepository.ListAsync(AuthorityType).ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AuthorityResponse> RemoveAsync(long ID)
        {
            var existingAuthority = await _authorityRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingAuthority == null)
            {
                return new AuthorityResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _authorityRepository.Remove(existingAuthority);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new AuthorityResponse(existingAuthority);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"AllocationService.RemoveAsync Error: {Environment.NewLine}");
                    return new AuthorityResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<AuthorityResponse> Update(long ID, Authority authority)
        {
            try
            {
                _authorityRepository.Update(ID, authority);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new AuthorityResponse(authority);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AuthorityService.Update Error: {Environment.NewLine}");
                return new AuthorityResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }
    }
}
