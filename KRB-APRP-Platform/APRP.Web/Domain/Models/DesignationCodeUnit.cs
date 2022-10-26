namespace APRP.Web.Domain.Models
{
    public class DesignationCodeUnit
    {
        public long ID { get; set; }

        public string UserId { get; set; }

        public string Designation { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
