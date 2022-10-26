namespace APRP.Web.ViewModels
{
    public class R2000ProjectViewModel
    {
        public string Intervention { get; set; }

        //physical achievement properties
        public double AchievedKM { get; set; }
        public double TargetKMAchievedToDate { get; set; }
        public double ExpenditureOnAchieved { get; set; } //expenditure consumed on the quarter
        public double ExpenditureOnAchievedToDate { get; set; }
        //Employment target properties
        public int ManCount { get; set; }
        public int WomanCount { get; set; }
        public int YouthCount { get; set; }
        public int PersonWithDisabilityCount { get; set; }
        public int TotalContractPersonDays { get; set; }
        public string SurfaceType { get; set; }

    }
}
