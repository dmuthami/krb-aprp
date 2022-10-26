using APRP.Web.Domain.Models.Abstract;

namespace APRP.Web.Domain.Models.History
{
    public class RoadConditionCodeUnith : RoadConditionCodeUnitAbstract
    {
        public int ID { get; set; }

        public int RoadConditionCodeUnitId { get; set; }

        public long SurfaceTypeId { get; set; }

        public DateTime BeginLifeSpan { get; set; }

        public Nullable<DateTime> EndLifeSpan { get; set; }
    }
}
