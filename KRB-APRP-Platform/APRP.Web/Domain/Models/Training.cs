using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class Training
    {
        public long ID { get; set; }

        //[Display(Name = "Course Title")]
        //public string CourseTitle { get; set; }

        [Display(Name = "Cadre of Trainees")]
        public string CadreOfTrainees { get; set; }

        [Display(Name = "No Trained")]
        public int NoTrained { get; set; }

        [Display(Name = "Training Expenditure")]
        public double TrainingExpenditure { get; set; }

        [Display(Name = "Quarter Code Unit")]
        public long QuarterCodeUnitId { get; set; }
        public QuarterCodeUnit QuarterCodeUnit { get; set; }

        [Display(Name = "Authority")]
        public long AuthorityId { get; set; }

        [Display(Name = "Authority")]
        public Authority Authority { get; set; }

        [Display(Name = "Training Course")]
        public int TrainingCourseId { get; set; }

        [Display(Name = "Training Course")]
        public TrainingCourse TrainingCourse { get; set; }
    }
}
