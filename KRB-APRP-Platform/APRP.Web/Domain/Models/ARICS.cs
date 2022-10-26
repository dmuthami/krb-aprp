using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class ARICS
    {
        public long ID { get; set; }

        public int Chainage { get; set;}

        [Display(Name = "Rate of Deterioration")]
        public int RateOfDeterioration { get; set; }

        [Display(Name = "New Line Required(N)")]
        public int CulvertN { get; set; }

        [Display(Name = "Repair of Ring(RR)")]
        public int CulvertRR { get; set; }

        [Display(Name = "Headwall Repair(HR)")]
        public int CulvertHR { get; set; }

        [Display(Name = "New Headwall(NH)")]
        public int CulvertNH { get; set; }

        [Display(Name = "Good(G)")]
        public int CulvertG { get; set; }

        [Display(Name = "Blocked(B)")]
        public int CulvertB { get; set; }

        [Display(Name = "Remarks (Other Structures)")]
        public string OtherStructureRemarks { get; set; }

        [Display(Name = "Remarks (Spot Improvement)")]
        public string SpotImprovementRemarks { get; set; }

        //Navigation Properties
        public long RoadSheetId { get; set; }
        public RoadSheet RoadSheet { get; set; }

        //Navigation Properties
        [Display(Name = "Shoulder Surface Type(Paved)")]
        public long ShoulderSurfaceTypePavedId { get; set; }
        public ShoulderSurfaceTypePaved ShoulderSurfaceTypePaved { get; set; }

        //Navigation Properties
        [Display(Name = "Shoulder Intervention(Paved)")]
        public long ShoulderInterventionPavedId { get; set; }
        public ShoulderInterventionPaved ShoulderInterventionPaved { get; set; }

        //Navigation Properties
        [Display(Name = "Surface Type(UnPaved)")]
        public long SurfaceTypeUnPavedId { get; set; }
        public SurfaceTypeUnPaved SurfaceTypeUnPaved { get; set; }

        //Navigation Properties
        [Display(Name = "Gravel Required")]
        public long GravelRequiredId { get; set; } = 1;
        public GravelRequired GravelRequired { get; set; }
    }
}
