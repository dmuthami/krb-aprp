using APRP.Web.Domain.Models.Abstract;
using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class RoadConditionCodeUnit : RoadConditionCodeUnitAbstract
    {
        public int ID { get; set; }

        [Display(Name = "Surface Type")]
        public long SurfaceTypeId { get; set; }

        [Display(Name = "Surface Type")]
        public SurfaceType SurfaceType { get; set; }
    }
}
