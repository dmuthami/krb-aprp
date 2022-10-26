using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;

namespace APRP.Web.Persistence.Repositories
{
    public class AuthenticateRepository : BaseRepository, IAuthenticateRepository
    {
        private IConfiguration Configuration { get; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        public AuthenticateRepository(AppDbContext context, UserManager<ApplicationUser> userManager,
             IConfiguration configuration, ILogger<AuthenticateRepository> logger) : base(context)
        {
            _userManager = userManager;
            Configuration = configuration;
            _logger = logger;
        }

    }
}
