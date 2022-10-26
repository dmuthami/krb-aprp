using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ContractResponse : BaseResponse
    {
        public Contract Contract { get; set; }
        private ContractResponse(bool success, string message, Contract contract) : base(success, message)
        {
            Contract = contract;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="contract">Saved county.</param>
        /// <returns>Response.</returns>
        public ContractResponse(Contract contract) : this(true, string.Empty, contract)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ContractResponse(string message) : this(false, message, null)
        { }


    }
}
