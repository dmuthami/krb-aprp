using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels.UserViewModels
{
    public class ARICSData
    {
        public Road Road { get; set; }

        public double RateOfDeterioration { get; set; }

        public long PriorityRate { get; set; }

        public string Comment { get; set; }

        public double IRI { get; set; }

        public int CulvertN { get; set; }

        public int CulvertRR { get; set; }

        public int CulvertHR { get; set; }

        public int CulvertNH { get; set; }

        public int CulvertG { get; set; }

        public int CulvertB { get; set; }


    }
}
