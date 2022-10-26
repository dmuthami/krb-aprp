using APRP.Web.Domain.Models.Abstract;

namespace APRP.Web.Domain.Models.History
{
    public class RoadClassificationh : RoadClassificationAbstract
    {
        public long ID { get; set; }

        public long RoadClassificationId { get; set; }

        public int RoadClassCodeUnitId { get; set; }

        public RoadClassCodeUnit RoadClassCodeUnit { get; set; }

        public long SurfaceTypeId { get; set; }

        public int RoadConditionCodeUnitId { get; set; }

        public DateTime BeginLifeSpan { get; set; }

        public Nullable<DateTime> EndLifeSpan { get; set; }
    }
}
