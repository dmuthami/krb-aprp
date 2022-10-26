namespace APRP.Web.Domain.Services.Communication
{
    public class KenHARoadDictResponse : BaseResponse
    {
        public Dictionary<string, string> Dictionary { get; set; }

       
        private KenHARoadDictResponse(bool success, string message, Dictionary<string, string> dictionary) : base(success, message)
        {
            Dictionary = dictionary;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public KenHARoadDictResponse(Dictionary<string, string> dictionary) : this(true, string.Empty, dictionary)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public KenHARoadDictResponse(string message) : this(false, message, null)
        { }

    }
}
