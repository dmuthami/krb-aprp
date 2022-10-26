using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class KWSPark
    {
        public long ID { get; set; }

        public string Code { get; set; }

        public string NationalPark { get; set; }

        public virtual ICollection<KwsRoad> KwsRoads { get; set; }//Allow lazy loading

        public virtual ICollection<RoadSection> RoadSections { get; set; }//Allow lazy loading

        [Display(Name = "Region ID")]
        public long? RegionId { get; set; }//To be used by GIS tool
        public virtual Region Region { get; set; }
    }
}
