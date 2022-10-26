using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class RoadSheetIntervalData
    {
        public static void SeedRoadSheetInterval(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoadSheetInterval>().HasData(
                new RoadSheetInterval
                {
                    ID = 1,
                    IntervalInMeters = 200
                }, new RoadSheetInterval
                {
                    ID = 2,
                    IntervalInMeters = 100
                }, new RoadSheetInterval
                {
                    ID = 3,
                    IntervalInMeters = 10
                }, new RoadSheetInterval
                {
                    ID = 4,
                    IntervalInMeters = 20
                }, new RoadSheetInterval
                {
                    ID = 5,
                    IntervalInMeters = 50
                }
                );
        }

    }
}
