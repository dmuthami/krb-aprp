using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class DirectorService : IDirectorService
    {

        private readonly IDirectorRepository _directorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DirectorService(IDirectorRepository directorRepository, IUnitOfWork unitOfWork)
        {
            _directorRepository= directorRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<DirectorResponse> AddAsync(Director director)
        {
            try
            {

                await _directorRepository.AddAsync(director).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new DirectorResponse(director);
            }
            catch (Exception ex)
            {
                return new DirectorResponse($"Error occured while saving the record {ex.Message}");
            }
        }

        public async Task<DirectorResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord = await _directorRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new DirectorResponse("Record Not Found");
                }
                else
                {
                    return new DirectorResponse(existingRecord);
                }
            }
            catch (Exception ex)
            {
                //logging
                return new DirectorResponse($"Error occured while getting the record {ex.Message}");
            }
        }

        public async Task<IEnumerable<Director>> ListAsync()
        {
            try
            {
                return await _directorRepository.ListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //log error
                return Enumerable.Empty<Director>();
            }
        }

        public async Task<DirectorResponse> RemoveAsync(long ID)
        {
            try
            {
                var existingRecord = await _directorRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new DirectorResponse("Record Not Found");
                }
                else
                {
                    _directorRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new DirectorResponse(existingRecord);

                }
            }
            catch (Exception ex)
            {
                //log error
                return new DirectorResponse($"Error occured while removing the record : {ex.Message}");
            }
        }

        public async Task<DirectorResponse> UpdateAsync(Director director)
        {
            try
            {
                var existingRecord = await _directorRepository.FindByIdAsync(director.ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new DirectorResponse("Record Not Found");

                }
                else
                {
                    existingRecord.FirstName = director.FirstName;
                    existingRecord.LastName = director.LastName;
                    existingRecord.MiddleName = director.MiddleName;
                    existingRecord.Gender = director.Gender;
                    existingRecord.UpdateBy = director.UpdateBy;
                    existingRecord.UpdateDate = director.UpdateDate;

                    _directorRepository.Update(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new DirectorResponse(existingRecord);
                }

            }
            catch (Exception ex)
            {
                //log
                return new DirectorResponse($"Error occured while updating the record : {ex.Message}");
            }
        }
    }
}
