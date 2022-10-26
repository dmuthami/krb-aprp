using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class ComplaintService : IComplaintService
    {

        private readonly IComplaintRepository _complaintRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ComplaintService(IComplaintRepository complaintRepository, IUnitOfWork unitOfWork)
        {
            _complaintRepository = complaintRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ComplaintResponse> AddAsync(Complaint complaint)
        {
            try
            {

                await _complaintRepository.AddAsync(complaint).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ComplaintResponse(complaint);
            }
            catch (Exception ex)
            {
                return new ComplaintResponse($"Error occured while saving the record {ex.Message}");
            }
        }

        public async Task<ComplaintResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord = await _complaintRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ComplaintResponse("Record Not Found");
                }
                else
                {
                    return new ComplaintResponse(existingRecord);
                }
            }
            catch (Exception ex)
            {
                //logging
                return new ComplaintResponse($"Error occured while getting the record {ex.Message}");
            }
        }

        public async Task<IEnumerable<Complaint>> ListAsync()
        {
            try
            {
                return await _complaintRepository.ListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //log error
                return Enumerable.Empty<Complaint>();
            }
        }

        public async Task<IEnumerable<Complaint>> ListUnresolvedAsyc()
        {
            try
            {
                return await _complaintRepository.ListUnresolvedAsyc().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //log error
                return Enumerable.Empty<Complaint>();
            }
        }

        public async Task<ComplaintResponse> RemoveAsync(long ID)
        {
            try
            {
                var existingRecord = await _complaintRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ComplaintResponse("Record Not Found");
                }
                else
                {
                    _complaintRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new ComplaintResponse(existingRecord);

                }
            }
            catch (Exception ex)
            {
                //log error
                return new ComplaintResponse($"Error occured while removing the record : {ex.Message}");
            }
        }

        public async Task<ComplaintResponse> UpdateAsync(Complaint complaint)
        {
            try
            {
                var existingRecord = await _complaintRepository.FindByIdAsync(complaint.ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ComplaintResponse("Record Not Found");

                }
                else
                {
                    existingRecord.Authority = complaint.Authority;
                    existingRecord.Code = complaint.Code;
                    existingRecord.ComplaintType = complaint.ComplaintType;
                    existingRecord.DateRaised = complaint.DateRaised;
                    existingRecord.DateResolved = complaint.DateResolved;
                    existingRecord.Description = complaint.Description;
                    existingRecord.RaisedBy = complaint.RaisedBy;
                    existingRecord.ResolutionComment = complaint.ResolutionComment;
                    existingRecord.ResolvedBy = complaint.ResolvedBy;
                    existingRecord.Severity = complaint.Severity;
                    existingRecord.Status = complaint.Status;

                    _complaintRepository.Update(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new ComplaintResponse(existingRecord);
                }

            }
            catch (Exception ex)
            {
                //log
                return new ComplaintResponse($"Error occured while updating the record : {ex.Message}");
            }
        }
    }
}
