using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class GravelRequiredData
    {
        public static void SeedGravelRequired(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GravelRequired>().HasData(
            new GravelRequired
            {
                ID = 1,
                Code = "NA",
                Description = "Not Applicable"
            },
            new GravelRequired
            {
                ID = 2,
                Code = "G",
                Description = "Gravelling of entire section (stacks included)"
            },           
            new GravelRequired
            {
                ID = 3,
                Code = "S",
                Description = "Provision of gravel stacks for Routine Maintenance"
            }
            );
        }
    }
}
