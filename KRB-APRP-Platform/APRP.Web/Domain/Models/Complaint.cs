namespace APRP.Web.Domain.Models
{
    public class Complaint
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public long ComplaintTypeId { get; set; }
        public ComplaintType ComplaintType { get; set; }

        public string RaisedBy { get; set; }
        public string ResolvedBy { get; set; }

        public DateTime DateRaised { get; set; }

        public DateTime DateResolved { get; set; }

        public int Status { get; set; }
        public String ResolutionComment { get; set; }

        public long AuthorityId { get; set; }
        public Authority Authority { get; set; }

        public int Severity { get; set; }


    }
}
