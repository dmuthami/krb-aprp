using APRP.Web.Domain.Models.Abstract;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace APRP.Web.Domain.Models
{
    public class Constituency : ConstituencyAbstract
    {
        public long ID { get; set; }


        [Column(TypeName = "geometry")]
        public Geometry Geom { get; set; }

        /*---Start of Navigation Properties*/

        //public virtual Region Region { get; set; }

        public ICollection<RoadSection> RoadSections { get; set; }

        public long CountyId { get; set; }
        public County County { get; set; }

        /*---End of Navigation Properties*/
    }
}
