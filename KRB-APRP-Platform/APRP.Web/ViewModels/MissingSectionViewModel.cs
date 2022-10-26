using System.ComponentModel;

namespace APRP.Web.ViewModels
{
    public class MissingSectionViewModel
    {
        [DisplayName("Road ID")]
        public string RoadId { get; set; }

        [DisplayName("Road Section ID")]
        public string RoadSectionId { get; set; }
        public string RoadSectionName { get; set; }
    }
}
