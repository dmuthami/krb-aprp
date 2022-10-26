using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class Road
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(254)]        
        [Display(Name="Road Number")]
        public string RoadNumber { get; set; }

        [Display(Name = "Road Name")]
        public string RoadName { get; set; }

        public bool PulledSectionsFromGIS { get; set; } = false;

        /*----Navigation Properties--------------------------*/

        /// <summary>
        /// Road entity includes many road sections for the same rode code
        /// </summary>
        public ICollection<RoadSection> RoadSections { get; set; }

        /// <summary>
        /// Road entity includes many road sections for the same rode code
        /// </summary>
        public ICollection<GISRoad> GISRoads { get; set; }

        public ICollection<RoadWorkSectionPlan> RoadWorkSectionPlans { get; set; }

        public virtual ICollection<RoadCondition> RoadConditions { get; set; }

        public long AuthorityId { get; set; }
        public Authority Authority { get; set; }

        /*----Navigation Properties----------------------------*/


    }
}
