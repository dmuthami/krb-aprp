using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class RoadPrioritizationData
    {
        public static void SeedRoadPrioritization(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoadPrioritization>().HasData(
                new RoadPrioritization
                {
                    ID = 1,
                    Rate = 0,
                    Code="Not Prioritized"
                },
                new RoadPrioritization
                {
                    ID = 2,
                    Rate = 1,
                    Code = "Very Lowly Prioritized"
                },
                new RoadPrioritization
                {
                    ID = 3,
                    Rate = 2,
                    Code = "Lowly Prioritized"
                },
                new RoadPrioritization
                {
                    ID = 4,
                    Rate = 3,
                    Code = "Neither Lowly or Highly Prioritzed"
                },
                new RoadPrioritization
                {
                    ID = 5,
                    Rate = 4,
                    Code = "Highly Prioritized"
                },
                new RoadPrioritization
                {
                    ID = 6,
                    Rate = 5,
                    Code = "Very Highly Prioritized"
                }
                );
        }

    }
}
