using System.ComponentModel.DataAnnotations;
using APRP.Web.Domain.Models.Abstract;

namespace APRP.Web.Domain.Models
{
    public partial class KwsRoad : GISAbstract
    {
        [Key]
        public long ID { get; set; }
 
        public long ConstituencyId { get; set; }//To be used by GIS tool

        [Display(Name = "KWSPark ID")]
        public long? KWSParkId { get; set; }//To be used by GIS tool
        public virtual KWSPark KWSPark { get; set; }

        /*----Navigation Properties--------------------------*/
        public long RoadId { get; set; }
        public Road Road { get; set; }
        /*----Navigation Properties--------------------------*/
    }
}
