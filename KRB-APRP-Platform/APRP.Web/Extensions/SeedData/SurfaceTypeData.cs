using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class SurfaceTypeData
    {
        public static void SeedSurfaceType(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SurfaceType>().HasData(
            new SurfaceType
            {
                ID = 1L,
                Code = "G",
                Name = "Gravel",
                Description = "Gravel"
            },
             new SurfaceType
             {
                 ID = 2L,
                 Code = "E",
                 Name = "Earth",
                 Description = "Earth"
             },
             new SurfaceType
             {
                 ID = 3L,
                 Code = "M",
                 Name = "Mixed",
                 Description = "Mixed"
             },
             new SurfaceType
             {
                 ID = 4L,
                 Code = "P",
                 Name = "Paved",
                 Description = "Paved"
             },
             new SurfaceType
             {
                 ID = 5L,
                 Code = "U",
                 Name = "UnPaved",
                 Description = "UnPaved"
             }
            );
        }
    }
}
