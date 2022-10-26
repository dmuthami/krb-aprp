using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ContractorResponse : BaseResponse
    {
        public Contractor Contractor { get; set; }
        private ContractorResponse(bool success, string message, Contractor contractor) : base(success, message)
        {
            Contractor = contractor;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="contractor">Saved county.</param>
        /// <returns>Response.</returns>
        public ContractorResponse(Contractor contractor) : this(true, string.Empty, contractor)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ContractorResponse(string message) : this(false, message, null)
        { }


    }
}
