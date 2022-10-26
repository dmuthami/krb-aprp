using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class TechnologyData
    {
        public static void SeedTechnology(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Technology>().HasData(
                new Technology
                {
                    ID = 1L,
                    Code = "LB",
                    Description = "Labour Based"
                },
                new Technology
                {
                    ID = 2L,
                    Code = "MB",
                    Description = "Machine Based"
                }, new Technology
                {
                    ID = 3L,
                    Code = "LB & MB",
                    Description = "Labour and Machine Based"
                }
            );
        }
    }
}
