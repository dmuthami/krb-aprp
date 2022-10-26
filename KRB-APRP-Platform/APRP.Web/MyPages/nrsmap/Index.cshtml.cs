using System.Globalization;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APRP.Web.MyPages.nrsmap
{
    [TypeFilter(typeof(APRP.Web.Extensions.Filters.SessionTimeoutRazorFilter))]
    public class IndexModel : PageModel
    {
        private readonly ILogger _logger;
        public IConfiguration Configuration { get; }
        private CultureInfo _cultures = new CultureInfo("en-US");
        public IndexModel(IConfiguration configuration
             , ILogger<IndexModel> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public Mapping Mapping { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public IActionResult OnGet()
        {
            try
            {
                string mapURL = Configuration["MapURL"];
                Mapping = new Mapping();
                Mapping.MapURL = mapURL;
                return Page();
            }
            catch (Exception Ex)
            {
                _logger.LogError("Map.Mapping Razor Page Error", Ex);
                ModelState.AddModelError(string.Empty, Ex.Message.ToString(_cultures));
                return Page();
            }
        }
    }
}