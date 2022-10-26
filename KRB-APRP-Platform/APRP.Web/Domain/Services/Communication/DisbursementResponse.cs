using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class DisbursementResponse : BaseResponse
    {
        public Disbursement Disbursement { get; set; }


        private DisbursementResponse(bool success, string message, Disbursement disbursement) : base(success, message)
        {
            Disbursement = disbursement;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="Disbursement">Saved Disbursement.</param>
        /// <returns>Response.</returns>
        public DisbursementResponse(Disbursement disbursement) : this(true, string.Empty, disbursement)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public DisbursementResponse(string message) : this(false, message, null)
        { }
    }
}
