using APRP.Web.Domain.Models.Abstract;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace APRP.Web.Domain.Models
{
    public class County : CountyAbstract
    {
        public long ID { get; set; }

        [Column(TypeName = "geometry")]
        public virtual Geometry Geom { get; set; }

        /*---Start of Navigation Properties*/

        /// <summary>
        /// County entity includes many constituencies 
        /// </summary>
        public ICollection<Constituency> Constituencys { get; set; }

        /*---Start of Navigation Properties*/

        public ICollection<RegionToCounty> RegionToCountys { get; set; }

        public ICollection<Municipality> Municipalitys { get; set; }
    }
}
