using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class TerrainTypeData
    {
        public static void SeedTerrainType(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TerrainType>().HasData(
            new TerrainType
            {
                ID = 1L,
                Code = "F",
                Name = "Flat",
                Description = "F for Flat"

            },
             new TerrainType
             {
                 ID = 2L,
                 Code = "R",
                 Name = "Rolling",
                 Description = "R for Rolling"

             },
              new TerrainType
              {
                  ID = 3L,
                  Code = "H",
                  Name = "Hilly",
                  Description = "H for Hilly"

              },
            new TerrainType
            {
                ID = 4L,
                Code = "NA",
                Name = "NA",
                Description = "Not Applicable"
            }
            );
        }
    }
}
