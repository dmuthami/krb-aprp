using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class RoadClassificationViewModel
    {
        public RoadClassification RoadClassification { get; set; }
        public string Referer { get; set; }

        public IEnumerable<Comment> Comments { get; set; }
    }
}
