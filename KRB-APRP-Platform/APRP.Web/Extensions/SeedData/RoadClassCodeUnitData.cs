using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class RoadClassCodeUnitData
    {
        public static void SeedRoadClassCodeUnit(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoadClassCodeUnit>().HasData(
                new RoadClassCodeUnit
                {
                    ID = 1,
                    RoadClass = "A"
                },
                new RoadClassCodeUnit
                {
                    ID = 2,
                    RoadClass = "B"
                },
                new RoadClassCodeUnit
                {
                    ID = 3,
                    RoadClass = "C"
                },
                new RoadClassCodeUnit
                {
                    ID = 4,
                    RoadClass = "S"
                },
                new RoadClassCodeUnit
                {
                    ID = 5,
                    RoadClass = "D"
                },
                new RoadClassCodeUnit
                {
                    ID = 6,
                    RoadClass = "E"
                }, new RoadClassCodeUnit
                {
                    ID = 7,
                    RoadClass = "F"
                }, new RoadClassCodeUnit
                {
                    ID = 8,
                    RoadClass = "G"
                }
                );
        }

    }
}
