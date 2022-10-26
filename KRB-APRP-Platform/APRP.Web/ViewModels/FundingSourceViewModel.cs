using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class FundingSourceViewModel
    {
        public FundingSource FundingSource { get; set; }
        public IEnumerable<FundingSource> FundingSourceList { get; set; }

        public IEnumerable<FundingSourceSubCode> FundingSourceSubCodeList { get; set; }

        public FundingSourceSubCode FundingSourceSubCode { get; set; }

        public long FundingSourceId { get; set; }

        public string Referer { get; set; }

        public string FundingSourceSubCodeView { get; set; }
    }
}
