using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class ContractorService : IContractorService
    {

        private readonly IContractorRepository _contractorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContractorService(IContractorRepository contractorRepository, IUnitOfWork unitOfWork)
        {
            _contractorRepository = contractorRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ContractorResponse> AddAsync(Contractor contractor)
        {
            try
            {

                await _contractorRepository.AddAsync(contractor).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ContractorResponse(contractor);
            }
            catch (Exception ex)
            {
                return new ContractorResponse($"Error occured while saving the record {ex.Message}");
            }
        }

        public async Task<ContractorResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord = await _contractorRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ContractorResponse("Record Not Found");
                }
                else
                {
                    return new ContractorResponse(existingRecord);
                }
            }
            catch (Exception ex)
            {
                //logging
                return new ContractorResponse($"Error occured while getting the record {ex.Message}");
            }
        }

        public async Task<ContractorResponse> FindByKraPinAsync(string kraPin)
        {
            try
            {
                var existingRecord = await _contractorRepository.FindByKraPinAsync(kraPin).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ContractorResponse("Record Not Found");
                }
                else
                {
                    return new ContractorResponse(existingRecord);
                }
            }
            catch (Exception ex)
            {
                //logging
                return new ContractorResponse($"Error occured while getting the record {ex.Message}");
            }
        }

        public async Task<IEnumerable<Contractor>> ListAsync()
        {
            try
            {
                return await _contractorRepository.ListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //log error
                return Enumerable.Empty<Contractor>();
            }
        }

        public async Task<ContractorResponse> RemoveAsync(long ID)
        {
            try
            {
                var existingRecord = await _contractorRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ContractorResponse("Record Not Found");
                }
                else
                {
                    _contractorRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new ContractorResponse(existingRecord);

                }
            }
            catch (Exception ex)
            {
                //log error
                return new ContractorResponse($"Error occured while removing the record : {ex.Message}");
            }
        }

        public async Task<ContractorResponse> UpdateAsync(Contractor contractor)
        {
            try
            {
                var existingRecord = await _contractorRepository.FindByIdAsync(contractor.ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ContractorResponse("Record Not Found");

                }
                else
                {
                    existingRecord.Name = contractor.Name;
                    existingRecord.POBox = contractor.POBox;
                    existingRecord.Telephone = contractor.Telephone;
                    existingRecord.Town = contractor.Town;
                    existingRecord.UpdateBy = contractor.UpdateBy;
                    existingRecord.UpdateDate = contractor.UpdateDate;
                    existingRecord.IncorporationCertificateLink = contractor.IncorporationCertificateLink;
                    existingRecord.Email = contractor.Email;
                    existingRecord.BankName = contractor.BankName;
                    existingRecord.BankBranchName = contractor.BankBranchName;
                    existingRecord.BankBranchCode = contractor.BankBranchCode;
                    existingRecord.BankAccountNumber = contractor.BankAccountNumber;

                    _contractorRepository.Update(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new ContractorResponse(existingRecord);
                }

            }
            catch (Exception ex)
            {
                //log
                return new ContractorResponse($"Error occured while updating the record : {ex.Message}");
            }
        }
    }
}
