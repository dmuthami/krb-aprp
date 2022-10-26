using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Extensions
{
    public class FileUpload
    {
        //[Required]
        //[Display(Name = "Title")]
        //[StringLength(60, MinimumLength = 3)]
        //public string Title { get; set; }

        //[Required]
        [Display(Name = "ARICS Form")]
        public IFormFile UploadPublicSchedule { get; set; }
    }
}
