using System.ComponentModel.DataAnnotations;
using APRP.Web.Domain.Models.Abstract;

namespace APRP.Web.Domain.Models
{
    public partial class KuraRoad : GISAbstract
    {
        [Key]
        public long ID { get; set; }

        public long ConstituencyId { get; set; }//To be used by GIS tool

        [Display(Name = "Municipality ID")]
        public long? MunicipalityId { get; set; }
        public virtual Municipality Municipality { get; set; }//allow lazy loading


        /*----Navigation Properties--------------------------*/

        public long RoadId { get; set; }
        public Road Road { get; set; }

        /*----Navigation Properties--------------------------*/


    }
}
