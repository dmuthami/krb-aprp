using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels.UserViewModels
{
    public class RoadSectionViewModel
    {
        public long id { get; set; }
        public string SectionName { get; set; }

        public string SectionID { get; set; }

        public string road_number { get; set; }

        public string section_name { get; set; }

        public string section_surface_type { get; set; }

        public double length { get; set; }

        public Road Road { get; set; }
        public IEnumerable<RoadSection> RoadSections { get; set; }

        public IEnumerable<Road> Roads { get; set; }

        public long RoadCount { get; set; }

        public long roadid { get; set; }

    }
}
