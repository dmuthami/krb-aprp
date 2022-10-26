using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class TrainingViewModel
    {
        public Authority Authority { get; set; }

        public FinancialYear FinancialYear { get; set; }

        public Training Training { get; set; }

        public IEnumerable<Training> Trainings { get; set; }

        public IEnumerable<TrainingCourse> TrainingCourses { get; set; }
    }
}
