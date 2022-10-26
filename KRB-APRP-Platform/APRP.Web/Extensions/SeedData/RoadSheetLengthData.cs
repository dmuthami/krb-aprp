using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class RoadSheetLengthData
    {
        public static void SeedRoadSheetLength(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoadSheetLength>().HasData(
                new RoadSheetLength
                {
                    ID = 1,
                    LengthInKm = 5
                }, new RoadSheetLength
                {
                    ID = 2,
                    LengthInKm = 1
                }
                );
        }

    }
}
