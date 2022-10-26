using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class Upload
    {
        public long ID { get; set; }
        public string type { get; set; }

        [Display(Name = "File Name")]
        public string filename { get; set; }
        public long ForeignId { get; set; }
    }
}
