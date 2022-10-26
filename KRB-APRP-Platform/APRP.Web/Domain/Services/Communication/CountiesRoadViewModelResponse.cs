using APRP.Web.ViewModels;

namespace APRP.Web.Domain.Services.Communication
{
    public class CountiesRoadViewModelResponse : BaseResponse
    {
        public IQueryable<CountiesRoadViewModel> Roads { get; set; }
        private CountiesRoadViewModelResponse(bool success, string message, IQueryable<CountiesRoadViewModel> roads) : base(success, message)
        {
            Roads = roads;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="category">Saved category.</param>
        /// <returns>Response.</returns>
        public CountiesRoadViewModelResponse(IQueryable<CountiesRoadViewModel> roads) : this(true, string.Empty, roads)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summaryKuraViewModel
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public CountiesRoadViewModelResponse(string message) : this(false, message, null)
        { }
    }
}
