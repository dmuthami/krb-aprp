using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class KenHARoadListResponse : BaseResponse
    {
        public IEnumerable<KenhaRoad> KenhaRoads { get; set; }
        private KenHARoadListResponse(bool success, string message, IEnumerable<KenhaRoad> kenhaRoads) : base(success, message)
        {
            KenhaRoads = kenhaRoads;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="category">Saved category.</param>
        /// <returns>Response.</returns>
        public KenHARoadListResponse(IEnumerable<KenhaRoad> kenhaRoads) : this(true, string.Empty, kenhaRoads)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public KenHARoadListResponse(string message) : this(false, message, null)
        { }
    }
}
