using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class SurfaceTypeUnPavedData
    {
        public static void SeedSurfaceTypeUnPaved(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SurfaceTypeUnPaved>().HasData(
            new SurfaceTypeUnPaved
            {
                ID = 1L,
                Code = "NA",
                Name = "NA",
                Description = "NA"
            },
             new SurfaceTypeUnPaved
             {
                 ID = 2L,
                 Code = "E",
                 Name = "Earth",
                 Description = "Earth"
             },
             new SurfaceTypeUnPaved
             {
                 ID = 3L,
                 Code = "G",
                 Name = "Gravel",
                 Description = "Gravel"
             }
            );
        }
    }
}
