using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class RoadClassCodeUnit
    {
        public int ID { get; set; }

        [Display(Name = "Road Class")]
        public string RoadClass { get; set; }
    }
}
