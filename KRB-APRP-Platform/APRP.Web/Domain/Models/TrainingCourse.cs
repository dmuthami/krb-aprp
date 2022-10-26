using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class TrainingCourse
    {
        public int ID { get; set; }

        [Required]
        public string Course { get; set; }

        public string Description { get; set; }

        [Display(Name = "Update By")]
        public string UpdateBy { get; set; }

        [Display(Name = "Update Date")]
        public Nullable<DateTime> UpdateDate { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }

        public ICollection<Training> Trainings { get; set; }
    }
}
