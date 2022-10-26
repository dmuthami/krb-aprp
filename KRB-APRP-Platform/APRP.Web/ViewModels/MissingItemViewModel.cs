using System.ComponentModel;

namespace APRP.Web.ViewModels
{
    public class MissingItemViewModel
    {
        [DisplayName("Item Code")]
        public string Code { get; set; }

        [DisplayName("Item Description")]
        public string ItemName { get; set; }
    }
}
