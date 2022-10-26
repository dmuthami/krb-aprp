using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models.Abstract
{
    public abstract class RoadConditionCodeUnitAbstract
    {
        [Display(Name = "Condition")]
        public string RoadCondition { get; set; }

        public int Rate { get; set; }

        [Display(Name = "Activities Required")]
        public string ActivitiesRequired { get; set; }
    }
}
