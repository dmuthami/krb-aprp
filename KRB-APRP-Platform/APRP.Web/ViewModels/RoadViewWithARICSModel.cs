using APRP.Web.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APRP.Web.ViewModels
{
    public class RoadViewWithARICSModel
    {
        [Column("id")]
        public long id { get; set; }

        [Column("road_number")]
        [Display(Name = "Road Number")]
        public string road_number { get; set; }

        [Column("road_name")]
        [Display(Name = "Road Name")]
        public string road_name { get; set; }

        [Column("authority_id")]
        public long authority_id { get; set; }


        [Column("authority_name")]
        [Display(Name = "Authority")]
        public string authority_name { get; set; }


        [Display(Name = "Rate of Deteroriation")]
        public double ard { get; set; }

        public IEnumerable<RoadCondition> RoadConditions { get; set; }
    }
}
