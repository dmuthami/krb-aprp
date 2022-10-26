using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class Municipality
    {
        [Key]
        public long ID { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        [Display(Name = "County ID")]
        public long CountyId { get; set; }
        public County County { get; set; }

        public virtual ICollection<KuraRoad> KuraRoads { get; set; }//Allow lazy loading

        public virtual ICollection<RoadSection> RoadSections { get; set; }//Allow lazy loading
    }
}
