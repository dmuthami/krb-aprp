namespace APRP.Services.AuthorityAPI.Models.Dto
{
    public class AuthorityResponse : BaseResponse
    {
        public Authority Authority { get; set; }

        private AuthorityResponse(bool success, string message, Authority authority) : base(success, message)
        {
            Authority = authority;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="Authority">Saved Authority.</param>
        /// <returns>Response.</returns>
        public AuthorityResponse(Authority authority) : this(true, string.Empty, authority)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public AuthorityResponse(string message) : this(false, message, null)
        { }
    }
}
