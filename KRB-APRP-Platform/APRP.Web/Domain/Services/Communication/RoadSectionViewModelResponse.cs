using APRP.Web.ViewModels.UserViewModels;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadSectionViewModelResponse : BaseResponse
    {
        public IQueryable<RoadSectionViewModel> RoadSections { get; set; }
        private RoadSectionViewModelResponse(bool success, string message, IQueryable<RoadSectionViewModel> roadSections) : base(success, message)
        {
            RoadSections = roadSections;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="category">Saved category.</param>
        /// <returns>Response.</returns>
        public RoadSectionViewModelResponse(IQueryable<RoadSectionViewModel> roadSections) : this(true, string.Empty, roadSections)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadSectionViewModelResponse(string message) : this(false, message, null)
        { }
    }
}
