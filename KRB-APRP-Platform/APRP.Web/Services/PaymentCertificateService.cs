using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class PaymentCertificateService : IPaymentCertificateService
    {
        private readonly IPaymentCertificateRepository _paymentCertificateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public PaymentCertificateService(IPaymentCertificateRepository paymentCertificateRepository, 
            IUnitOfWork unitOfWork, 
             ILogger<PlanActivityService> logger)
        {
            _paymentCertificateRepository =paymentCertificateRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<PaymentCertificateResponse> AddAsync(PaymentCertificate paymentCertificate)
        {
            try
            {
                await _paymentCertificateRepository.AddAsync(paymentCertificate).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new PaymentCertificateResponse(paymentCertificate); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PlanActivityService.AddAsync Error: {Environment.NewLine}");
                return new PaymentCertificateResponse($"Error occured while saving the record : {Ex.Message}");
            }
        }

        public async Task<PaymentCertificateResponse> UpdateAsync(PaymentCertificate paymentCertificate)
        {
            try
            {
                var existingRecord = await _paymentCertificateRepository.FindByIdAsync(paymentCertificate.ID).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new PaymentCertificateResponse("Record Not Found");

                }
                else
                {
                    existingRecord.CertificateStatus = paymentCertificate.CertificateStatus;
                    existingRecord.UpdateBy = paymentCertificate.UpdateBy;
                    existingRecord.UpdateDate = paymentCertificate.UpdateDate;

                    _paymentCertificateRepository.Update(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new PaymentCertificateResponse(existingRecord);
                }

            }
            catch (Exception ex)
            {
                //log
                return new PaymentCertificateResponse($"Error occured while updating the record : {ex.Message}");
            }
        }

        public async Task<PaymentCertificateResponse> FindByContractIdAndCertificateNo(long contractId, int currentCertificateNo)
        {
            try
            {
                var existingRecord = await _paymentCertificateRepository.FindByContractIdAndCertificateNo(contractId, currentCertificateNo).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new PaymentCertificateResponse("Record Not Found");
                }
                else
                {
                    return new PaymentCertificateResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PlanActivityService.FindByIdAsync Error: {Environment.NewLine}");
                return new PaymentCertificateResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<PaymentCertificateResponse> FindByIdAsync(long certificateId)
        {
            try
            {
                var existingRecord = await _paymentCertificateRepository.FindByIdAsync(certificateId).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new PaymentCertificateResponse("Record Not Found");
                }
                else
                {
                    return new PaymentCertificateResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PlanActivityService.FindByIdAsync Error: {Environment.NewLine}");
                return new PaymentCertificateResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<IEnumerable<PaymentCertificate>> ListAsync(long contractId)
        {
            return await _paymentCertificateRepository.ListAsync(contractId).ConfigureAwait(false);
        }
    }
}
