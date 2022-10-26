namespace APRP.Web.Domain.Models.Abstract
{
    public abstract class CountiesRoadAbstract
    {
        public int ObjectID_1 { get; set; }
        public int? Objectid { get; set; }
        public string RdNum { get; set; }
        public string First_RdNa { get; set; }
        public string First_RdCl { get; set; }
        public decimal? Shape_Leng { get; set; }
        public decimal? Shape_Le_1 { get; set; }
        public long ConstituencyId { get; set; }


    }
}
