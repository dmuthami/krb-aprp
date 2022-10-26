
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class AdminOperationalActivityService : IAdminOperationalActivityService
    {

        private readonly IAdminOperationalActivityRepository _adminOperationalActivityRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdminOperationalActivityService(IAdminOperationalActivityRepository adminOperationalActivityRepository, IUnitOfWork unitOfWork)
        {
            _adminOperationalActivityRepository = adminOperationalActivityRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<AdminOperationalActivityResponse> AddAsync(AdminOperationalActivity adminOperationalActivity)
        {
            try
            {

                await _adminOperationalActivityRepository.AddAsync(adminOperationalActivity).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new AdminOperationalActivityResponse(adminOperationalActivity);
            }
            catch (Exception ex)
            {
                return new AdminOperationalActivityResponse($"Error occured while saving the record {ex.Message}");
            }
        }


        public async Task<AdminOperationalActivityResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord = await _adminOperationalActivityRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new AdminOperationalActivityResponse("Record Not Found");
                }
                else
                {
                    return new AdminOperationalActivityResponse(existingRecord);
                }
            }
            catch (Exception ex)
            {
                //logging
                return new AdminOperationalActivityResponse($"Error occured while getting the record {ex.Message}");
            }
        }

        public async Task<IEnumerable<AdminOperationalActivity>> ListByAuthorityAsync(long authorityId, long financialYearId)
        {
            try
            {
               return await _adminOperationalActivityRepository.ListByAuthorityAsync(authorityId,financialYearId).ConfigureAwait(false);
                
            }
            catch (Exception ex)
            {
                //logging
                return Enumerable.Empty<AdminOperationalActivity>();
            }
        }

        public async Task<AdminOperationalActivityResponse> Remove(long ID)
        {
            try
            {
                var existingRecord = await _adminOperationalActivityRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new AdminOperationalActivityResponse("Record Not Found");
                }
                else
                {
                    _adminOperationalActivityRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new AdminOperationalActivityResponse(existingRecord);

                }
            }
            catch (Exception ex)
            {
                //log error
                return new AdminOperationalActivityResponse($"Error occured while removing the record : {ex.Message}");
            }
        }

        public async Task<AdminOperationalActivityResponse> Update(AdminOperationalActivity adminOperationalActivity)
        {
            try
            {
                var existingRecord = await _adminOperationalActivityRepository.FindByIdAsync(adminOperationalActivity.ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new AdminOperationalActivityResponse("Record Not Found");

                }
                else
                {
                    existingRecord.UpdateBy = adminOperationalActivity.UpdateBy;
                    existingRecord.UpdateDate = DateTime.UtcNow;
                    existingRecord.ActivityGroup = adminOperationalActivity.ActivityGroup;
                    existingRecord.ActivityWorks= adminOperationalActivity.ActivityWorks;
                    existingRecord.Amount= adminOperationalActivity.Amount;
                    existingRecord.KM= adminOperationalActivity.KM;
                    existingRecord.ST= adminOperationalActivity.ST;
                    
                    _adminOperationalActivityRepository.Update(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new AdminOperationalActivityResponse(existingRecord);
                }

            }
            catch (Exception ex)
            {
                //log
                return new AdminOperationalActivityResponse($"Error occured while updating the record : {ex.Message}");
            }
        }
    }
}
