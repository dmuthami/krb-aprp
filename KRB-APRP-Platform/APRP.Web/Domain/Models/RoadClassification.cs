using APRP.Web.Domain.Models.Abstract;
using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class RoadClassification : RoadClassificationAbstract
    {
        public long ID { get; set; }

        /*
         * Proposed Class
         *
         */
        [Display(Name = "Road Class")]
        public int RoadClassCodeUnitId { get; set; }
        [Display(Name = "Road Class")]
        public RoadClassCodeUnit RoadClassCodeUnit { get; set; }

        [Display(Name = "Surface Type")]
        public long SurfaceTypeId { get; set; }
        [Display(Name = "Surface Type")]
        public SurfaceType SurfaceType { get; set; }

        [Display(Name = "Road Condition")]
        public int RoadConditionCodeUnitId { get; set; }

        [Display(Name = "Road Condition")]
        public RoadConditionCodeUnit RoadConditionCodeUnit { get; set; }

        /// <summary>
        /// 0=New
        /// 1=Submitted//RA/County officer
        /// 2=Approved//Road Agency’s Director General (DG) or County Executive Committee Member (CECM) in charge of Roads
        /// 3-Rejected//Road Agency’s Director General (DG) or County Executive Committee Member (CECM)
        /// 4-Approved//CS Roads Representative in KRB
        /// 5-Rejected//CS Roads Representative in KRB
        /// 6-Added to Road Register
        /// </summary>
    }
}
