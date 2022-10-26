namespace APRP.Web.Domain.Models
{
    public class Comment
    {
        public long ID { get; set; }
        public string Type { get; set; }
        public string MyComment { get; set; }
        public long ForeignId { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
