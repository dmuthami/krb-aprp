using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class ShoulderSurfaceTypePavedData
    {
        public static void SeedShoulderSurfaceTypePaved(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShoulderSurfaceTypePaved>().HasData(
            new ShoulderSurfaceTypePaved
            {
                ID = 1L,
                Code = "NA",
                Name = "NA",
                Description = "Not Applicable"
            },
             new ShoulderSurfaceTypePaved
             {
                 ID = 2L,
                 Code = "E",
                 Name = "Earth",
                 Description = "Earth"
             },
             new ShoulderSurfaceTypePaved
             {
                 ID = 3L,
                 Code = "G",
                 Name = "Gravel",
                 Description = "Gravel"
             },
            new ShoulderSurfaceTypePaved
            {
                ID = 4L,
                Code = "P",
                Name = "Paved",
                Description = "Paved"
            }
            );
        }
    }
}
