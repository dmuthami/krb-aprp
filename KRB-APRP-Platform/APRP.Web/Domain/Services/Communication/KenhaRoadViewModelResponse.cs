using APRP.Web.ViewModels;

namespace APRP.Web.Domain.Services.Communication
{
    public class KenhaRoadViewModelResponse : BaseResponse
    {
        public IQueryable<KenhaRoadViewModel> Roads { get; set; }
        private KenhaRoadViewModelResponse(bool success, string message, IQueryable<KenhaRoadViewModel> roads) : base(success, message)
        {
            Roads = roads;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="category">Saved category.</param>
        /// <returns>Response.</returns>
        public KenhaRoadViewModelResponse(IQueryable<KenhaRoadViewModel> roads) : this(true, string.Empty, roads)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summaryKenhaRoadViewModel
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public KenhaRoadViewModelResponse(string message) : this(false, message, null)
        { }
    }
}
