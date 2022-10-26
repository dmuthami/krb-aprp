using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class DisbursementListResponse : BaseResponse
    {
        public IEnumerable<Disbursement> Disbursement { get; set; }


        private DisbursementListResponse(bool success, string message, IEnumerable<Disbursement> disbursement) : base(success, message)
        {
            Disbursement =disbursement;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public DisbursementListResponse(IEnumerable<Disbursement> disbursement) : this(true, string.Empty, disbursement)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public DisbursementListResponse(string message) : this(false, message, null)
        { }
    }
}
