using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class PaymentCertificateActivitiesService : IPaymentCertificateActivitiesService
    {
        private readonly IPaymentCertificateActivitiesRepository _paymentCertificateActivitiesRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public PaymentCertificateActivitiesService(IPaymentCertificateActivitiesRepository paymentCertificateActivitiesRepository, 
            IUnitOfWork unitOfWork, 
             ILogger<PlanActivityService> logger)
        {
            _paymentCertificateActivitiesRepository = paymentCertificateActivitiesRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<PaymentCertificateActivityResponse> AddAsync(PaymentCertificateActivity paymentCertificateActivity)
        {
            try
            {
                await _paymentCertificateActivitiesRepository.AddAsync(paymentCertificateActivity).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new PaymentCertificateActivityResponse(paymentCertificateActivity); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PlanActivityService.AddAsync Error: {Environment.NewLine}");
                return new PaymentCertificateActivityResponse($"Error occured while saving the record : {Ex.Message}");
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<PaymentCertificateActivityResponse> FindByIdAsync(long activityId)
        {
            try
            {
                var existingRecord = await _paymentCertificateActivitiesRepository.FindByIdAsync(activityId).ConfigureAwait(false);
                if (existingRecord == null)
                {
                    return new PaymentCertificateActivityResponse("Record Not Found");
                }
                else
                {
                    return new PaymentCertificateActivityResponse(existingRecord);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"PlanActivityService.FindByIdAsync Error: {Environment.NewLine}");
                return new PaymentCertificateActivityResponse($"Error occured while retrieving the record : {Ex.Message}");

            }
        }

        public async Task<PaymentCertificateActivityResponse> RemoveAsync(long ID)
        {
            var existingRecord = await _paymentCertificateActivitiesRepository.FindByIdAsync(ID).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new PaymentCertificateActivityResponse("Record Not Found");
            }
            else
            {
                try
                {
                    _paymentCertificateActivitiesRepository.Remove(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new PaymentCertificateActivityResponse(existingRecord);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"FundTypeService.RemoveAsync Error: {Environment.NewLine}");
                    return new PaymentCertificateActivityResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }

        public async Task<PaymentCertificateActivityResponse> UpdateAsync(PaymentCertificateActivity paymentCertificateActivity)
        {
            var existingRecord = await _paymentCertificateActivitiesRepository.FindByIdAsync(paymentCertificateActivity.ID).ConfigureAwait(false);
            if (existingRecord == null)
            {
                return new PaymentCertificateActivityResponse("Record Not Found");
            }
            else
            {
                existingRecord.Quantity = paymentCertificateActivity.Quantity;
                existingRecord.UpdateDate = DateTime.UtcNow;
                existingRecord.UpdateBy = paymentCertificateActivity.UpdateBy;

                try
                {
                    _paymentCertificateActivitiesRepository.Update(existingRecord);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new PaymentCertificateActivityResponse(existingRecord);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"FundTypeService.Update Error: {Environment.NewLine}");
                    return new PaymentCertificateActivityResponse($"Error occured while retrieving the record : {Ex.Message}");
                }
            }
        }
    }
}
