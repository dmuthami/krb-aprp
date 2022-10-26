using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APRP.Web.Domain.Models.Abstract
{
    public class GISAbstract
    {
        [Column(TypeName = "geometry")]
        public MultiLineString Shape { get; set; }

        [Display(Name = "Road Number")]
        public string RdNum { get; set; }

        [Display(Name = "Road Name")]
        public string RdName { get; set; }

        [Display(Name = "Road Class")]
        public string RdClass { get; set; }

        [Display(Name = "Section ID")]
        public string Section_ID { get; set; }

        [Display(Name = "Section Name")]
        public string Sec_Name { get; set; }

        [Display(Name = "Surface Condition")]
        public string CW_Surf_Co { get; set; }

        [Display(Name = "Surface Type 1")]
        public string SurfaceType{ get; set; }

        [Display(Name = "Surface Type 2")]
        public string SurfaceType2 { get; set; }

        [Display(Name = "Length(Km)")]
        public decimal? Length { get; set; }

        [Display(Name = "Start Chainage")]
        public double StartChainage { get; set; }

        [Display(Name = "End Chainage")]
        public double EndChainage { get; set; }

        [Display(Name = "Update By")]
        public string UpdateBy { get; set; }

        [Display(Name = "Update Date")]
        public DateTime UpdateDate { get; set; }

        [Display(Name = "Creator")]
        public string Creator { get; set; }

        [Display(Name = "Creator Date")]
        public DateTime CreatorDate { get; set; }
    }
}
