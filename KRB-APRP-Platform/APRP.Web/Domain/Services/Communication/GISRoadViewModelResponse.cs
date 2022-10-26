using APRP.Web.ViewModels;

namespace APRP.Web.Domain.Services.Communication
{
    public class GISRoadViewModelResponse : BaseResponse
    {
        public GISRoadViewModel GISRoadViewModel { get; set; }

       
        private GISRoadViewModelResponse(bool success, string message, GISRoadViewModel gISRoadViewModel) : base(success, message)
        {
            GISRoadViewModel = gISRoadViewModel;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public GISRoadViewModelResponse(GISRoadViewModel gISRoadViewModel) : this(true, string.Empty, gISRoadViewModel)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public GISRoadViewModelResponse(string message) : this(false, message, null)
        { }

    }
}
