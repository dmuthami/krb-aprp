using APRP.Web.Domain.Models.Abstract;
using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class RoadSection : RoadSectionAbstract
    {
        public long ID { get; set; }

        //[Required]
        [Display(Name = "Section Name")]
        public string SectionName { get; set; }

        //[Required]
        //[StringLength(70)]
        [Display(Name = "Section ID")]
        public string SectionID { get; set; }

        [Required]
        public double StartChainage { get; set; }

        [Required]
        public double EndChainage { get; set; }

        [Required]
        [Display(Name = "Length (Km)")]
        public double Length { get; set; }

        //Say 200m
        [Required]
        public int Interval { get; set; }

        [Required]
        public long SurfaceTypeId { get; set; }
        public SurfaceType SurfaceType { get; set; }

        [Display(Name = "Surface Type 2")]
        public string SurfaceType2 { get; set; }

        [Display(Name = "Surface Condition")]
        public string CW_Surf_Co { get; set; }

        public long? ConstituencyId { get; set; }
        public Constituency Constituency { get; set; }

        [Display(Name = "Road ID")]
        public long RoadId { get; set; }
        public Road Road { get; set; }

        public RoadWorkSectionPlan RoadWorkSectionPlan { get; set; }

        public IList<RoadSheet> RoadSheets { get; set; } = new List<RoadSheet>();

        /// <summary>
        /// Road Section includes many uploaded aricsApproval for the same road section
        /// </summary>
        public ICollection<ARICSUpload> ARICSUploads { get; set; }

        [Display(Name = "KWSPark ID")]
        public long? KWSParkId { get; set; }//To be used by GIS tool
        public virtual KWSPark KWSPark { get; set; }


        [Display(Name = "Municipality ID")]
        public long? MunicipalityId { get; set; }//To be used by GIS tool
        public virtual Municipality Municipality { get; set; }

        public ICollection<ARICSBatch> ARICSBatchs { get; set; }

    }
}