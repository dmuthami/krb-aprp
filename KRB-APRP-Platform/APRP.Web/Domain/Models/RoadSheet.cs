using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class RoadSheet
    {
        public long ID { get; set; }

        [Required]
        public double StartChainage { get; set; }

        [Required]
        public double EndChainage { get; set; }

        //[Required]
        public string StartLocation { get; set; }

        //[Required]
        public string EndLocation { get; set; }

        [Required]
        public double CarriageWidth { get; set; }

        public string CompiledBy { get; set; }

        public DateTime Date { get; set; }

        public string SpotImprovementPriority { get; set; }
        public int Year { get; set; }

        [Display(Name = "Sheet No")]
        public int SheetNo { get; set; }

        public string StructurePriority { get; set; }

        //Navigation Properties
        [Display(Name = "Road Section Id")]
        public long RoadSectionId { get; set; }
        public RoadSection RoadSection { get; set; }

        /// <summary>
        /// Road sheet includes many arics entity for the specified road sheet
        /// </summary>
        public ICollection<ARICS> ARICSS { get; set; }

        [Display(Name = "Terrain Type")]
        public long TerrainTypeId { get; set; }
        public TerrainType TerrainType { get; set; }
        //-------Navigation----
    }
}