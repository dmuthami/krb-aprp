using APRP.Web.Domain.Models;
namespace APRP.Web.Domain.Services.Communication
{
    public class KuRRARoadListResponse : BaseResponse
    {
        public IEnumerable<KuraRoad> KuraRoads { get; set; }
        private KuRRARoadListResponse(bool success, string message, IEnumerable<KuraRoad> kuraRoads) : base(success, message)
        {
            KuraRoads = kuraRoads;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="category">Saved category.</param>
        /// <returns>Response.</returns>
        public KuRRARoadListResponse(IEnumerable<KuraRoad> kuraRoads) : this(true, string.Empty, kuraRoads)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public KuRRARoadListResponse(string message) : this(false, message, null)
        { }
    }
}
