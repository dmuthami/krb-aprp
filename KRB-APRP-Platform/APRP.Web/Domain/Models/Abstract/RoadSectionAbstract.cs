using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models.Abstract
{
    public abstract class RoadSectionAbstract
    {

        [Display(Name = "ARICS Region")]
        public string ARICSRegion { get; set; }

        [Display(Name = "ARICS Category Type")]
        public string ARICSCategoryType { get; set; }

        [Display(Name = "ARICS Road Name")]
        public string ARICSRoadName { get; set; }

        [Display(Name = "ARICS Road Length")]
        public Nullable<double> ARICSRoadLength { get; set; }

        [Display(Name = "ARICS Section ID")]
        public string ARICSSectionID { get; set; }

        [Display(Name = "ARICS Section Name")]
        public string ARICSSectionName { get; set; }

        [Display(Name = "ARICS Section Length")]
        public Nullable<double> ARICSSectionLength { get; set; }

        [Display(Name = "ARICS Sheet No")]
        public string ARICSSheetNo { get; set; }

        [Display(Name = "ARICS Sheet Length")]
        public Nullable<double> ARICSSheetLength { get; set; }

        [Display(Name = "ARICS Sheet Interval")]
        public Nullable<int> ARICSSheetInterval { get; set; }

    }
}
