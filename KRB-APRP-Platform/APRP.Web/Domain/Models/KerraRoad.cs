using System.ComponentModel.DataAnnotations;
using APRP.Web.Domain.Models.Abstract;

namespace APRP.Web.Domain.Models
{
    public partial class KerraRoad : GISAbstract
    {
        [Key]
        public long ID { get; set; }

        public long ConstituencyId { get; set; }//To be used by GIS tool

        /*----Navigation Properties--------------------------*/
        public long RoadId { get; set; }
        public Road Road { get; set; }
        /*----Navigation Properties--------------------------*/

    }
}
