using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class ARICSUpload
    {
        public long ID { get; set; }

        public int Year { get; set; }

        [Display(Name = "File Name")]
        public string filename { get; set; }

        public long RoadSectionId { get; set; }
        public RoadSection RoadSection { get; set; }
    }
}
