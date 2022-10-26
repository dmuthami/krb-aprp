using APRP.Web.Domain.Models.Abstract;

namespace APRP.Web.Domain.Models.Audit
{
    public class RoadConditionCodeUnitAudit : RoadConditionCodeUnitAbstract
    {
        public int ID { get; set; }
        public int RoadConditionCodeUnitId { get; set; }
        public long SurfaceTypeId { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> deleted { get; set; }
        public string deleted_by { get; set; }
    }
}
