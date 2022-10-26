using APRP.Services.AuthorityAPI.Models;
using APRP.Services.AuthorityAPI.Models.Dto;
using APRP.Services.AuthorityAPI.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace APRP.Services.AuthorityAPI.Services
{
    public class AuthorityService : IAuthorityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorityRepository _authorityRepository;
        private readonly ILogger _logger;

        public AuthorityService(IAuthorityRepository authorityRepository, ILogger<AuthorityService> logger,
            IUnitOfWork unitOfWork)
        {
            _authorityRepository = authorityRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<GenericResponse> AddAsync(Authority authority)
        {
            try
            {
                var iActionResult = await _authorityRepository.AddAsync(authority).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.HasValue)
                    {
                        if (objectResult.StatusCode == (int?)HttpStatusCode.OK)
                        {
                            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                        }
                    }
                }
                return new GenericResponse(iActionResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AuthorityService.AddAsync Error: {Environment.NewLine}");
                return new GenericResponse("Error occured while saving the road record");
            }
        }

        public async Task<GenericResponse> FindByCodeAsync(string code)
        {
            try
            {
                var iActionResult = await _authorityRepository.FindByCodeAsync(code).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AuthorityService.FindByCodeAsync Error: {Environment.NewLine}");
                return new GenericResponse("Error occured while retrieving the record");
            }
        }

        public async Task<GenericResponse> FindByIdAsync(long ID)
        {
            try
            {
                var iActionResult = await _authorityRepository.FindByIdAsync(ID).ConfigureAwait(false);
                return new GenericResponse(iActionResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AuthorityService.FindByIdAsync Error: {Environment.NewLine}");
                return new GenericResponse("Error occured while retrieving the record");
            }
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


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> RemoveAsync(long ID)
        {
            try
            {
                var iActionResult = await _authorityRepository.FindByIdAsync(ID).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.HasValue)
                    {
                        if (objectResult.StatusCode == (int?)HttpStatusCode.OK)
                        {
                            var result = (OkObjectResult)objectResult;
                            if (result.Value == null) { return new GenericResponse(iActionResult); } //successful
                            var authority = (Authority)result.Value;
                            var iActionResult2 = await _authorityRepository.Remove(authority).ConfigureAwait(false);
                            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                            return new GenericResponse(iActionResult2); //successful
                        }
                        else
                        {
                            return new GenericResponse(iActionResult); //failure
                        }
                    }
                    else
                    {
                        return new GenericResponse(iActionResult); //failure
                    }

                }
                else
                {
                    return new GenericResponse(iActionResult); //failure
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"AuthorityService.RemoveAsync Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<GenericResponse> Update(long ID, Authority authority)
        {
            try
            {
                var iActionResult = await _authorityRepository.Update(ID, authority).ConfigureAwait(false);
                var objectResult = (ObjectResult)iActionResult;
                if (objectResult == null) { return new GenericResponse(iActionResult); }
                if (objectResult.StatusCode.HasValue)
                {
                    if (objectResult.StatusCode == (int?)HttpStatusCode.OK)
                    {
                        await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    }
                }
                return new GenericResponse(iActionResult); //failure or success
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AuthorityService.Update Error: {Environment.NewLine}");
                return new GenericResponse($"Error occured while updating the record");
            }
        }
    }
}
