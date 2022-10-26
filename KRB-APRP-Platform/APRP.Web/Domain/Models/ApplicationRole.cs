using Microsoft.AspNetCore.Identity;

namespace APRP.Web.Domain.Models
{
    public class ApplicationRole : IdentityRole
    {
        /// <summary>
        /// 0=Role Based authorization
        /// 1=Role Claim authorization coupled with permissions
        /// </summary>
        public int RoleType { get; set; } =0;

        public string Description { get; set; }

    }
}
