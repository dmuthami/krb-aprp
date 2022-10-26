using APRP.Web.Domain.Models;
namespace APRP.Web.ViewModels
{
    public class KwsRoadViewModel
    {
        public long id { get; set; }
        public string rdnum { get; set; }
        public string rdname { get; set; }
        public string sectionid { get; set; }
        public string secname { get; set; }
        public string surfaceclass { get; set; }
        public string surfacetype { get; set; }
        public string rdclass { get; set; }
        public decimal? length { get; set; }
        public KwsRoad KwsRoad { get; set; }
        public string Referer { get; set; }
    }
}
