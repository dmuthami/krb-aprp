using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class ContractService : IContractService
    {

        private readonly IContractRepository _contractRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContractService(IContractRepository contractRepository, IUnitOfWork unitOfWork)
        {
            _contractRepository = contractRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ContractResponse> AddAsync(Contract contract)
        {
            try
            {

                await _contractRepository.AddAsync(contract).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new ContractResponse(contract);
            }
            catch (Exception ex)
            {
                return new ContractResponse($"Error occured while saving the record {ex.Message}");
            }
        }

        public async Task<ContractResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRecord = await _contractRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ContractResponse("Record Not Found");
                }
                else
                {
                    return new ContractResponse(existingRecord);
                }
            }
            catch (Exception ex)
            {
                //logging
                return new ContractResponse($"Error occured while getting the record {ex.Message}");
            }
        }


        public async Task<ContractResponse> FindContractByPackageIdAsync(long workpackageId)
        {
            try
            {
                var existingRecord = await _contractRepository.FindContractByPackageIdAsync(workpackageId).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ContractResponse("Record Not Found");
                }
                else
                {
                    return new ContractResponse(existingRecord);
                }
            }
            catch(Exception ex)
            {
                //logging
                return new ContractResponse($"Error occurred while getting the record {ex.Message}");
            }
        }
        public async Task<IEnumerable<Contract>> ListAsync()
        {
            try
            {
                return await _contractRepository.ListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //log error
                return Enumerable.Empty<Contract>();
            }
        }

        public async Task<IEnumerable<Contract>> ListContractsByAgencyByFinancialYear(long authorityId, long financialYearId)
        {
            try
            {
                return await _contractRepository.ListContractsByAgencyByFinancialYear(authorityId, financialYearId).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                return Enumerable.Empty<Contract>();
            }
        }

        public async Task<ContractResponse> RemoveAsync(long ID)
        {
            try
            {
                var existingRecord = await _contractRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ContractResponse("Record Not Found");
                }
                else
                {
                    _contractRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new ContractResponse(existingRecord);

                }
            }
            catch (Exception ex)
            {
                //log error
                return new ContractResponse($"Error occured while removing the record : {ex.Message}");
            }
        }

        public async Task<ContractResponse> UpdateAsync(Contract contract)
        {
            try
            {
                var existingRecord = await _contractRepository.FindByIdAsync(contract.ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new ContractResponse("Record Not Found");

                }
                else
                {
                    existingRecord.AdvanceClearance = contract.AdvanceClearance;
                    existingRecord.AdvancePayment = contract.AdvancePayment;
                    existingRecord.ContractEndDate = contract.ContractEndDate;
                    existingRecord.ContractStartDate = contract.ContractStartDate;
                    existingRecord.ContractIsSigned = contract.ContractIsSigned;
                    existingRecord.ContractorId = contract.ContractorId;
                    existingRecord.ContractSumPackage = contract.ContractSumPackage;
                    existingRecord.ContractSumWorkplan = contract.ContractSumWorkplan;
                    existingRecord.ContractTaxable = contract.ContractTaxable;
                    existingRecord.CreatedBy = contract.CreatedBy;
                    existingRecord.CreationDate = contract.CreationDate;
                    existingRecord.inPaymentCertificate = contract.inPaymentCertificate;
                    existingRecord.PercentageRetention = contract.PercentageRetention;
                    existingRecord.PerformanceBond = contract.PerformanceBond;
                    existingRecord.SpecialGroup = contract.SpecialGroup;
                    existingRecord.UpdateBy = contract.UpdateBy;
                    existingRecord.UpdateDate = contract.UpdateDate;
                  
                    _contractRepository.Update(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new ContractResponse(existingRecord);
                }

            }
            catch (Exception ex)
            {
                //log
                return new ContractResponse($"Error occured while updating the record : {ex.Message}");
            }
        }
    }
}
