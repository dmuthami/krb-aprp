using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class RoadConditionCodeUnitData
    {
        public static void SeedRoadConditionCodeUnit(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoadConditionCodeUnit>().HasData(
                new RoadConditionCodeUnit
                {
                    ID = 1,
                    SurfaceTypeId=1,
                    Rate=1,
                    RoadCondition = "Excellent / V. Good Maintainable road with camber and drainage intact",
                    ActivitiesRequired= "Nominal light maintenance only required " +
                        "e.g. Grass cutting, Light Bush clearing, " +
                        "Culvert cleaning, Ditch clearing, Mitre drains cleaning, Repair of Road Signs"

                },
                new RoadConditionCodeUnit
                {
                    ID = 2,
                    SurfaceTypeId = 1,
                    Rate = 2,
                    RoadCondition = "Good Maintainable road.Camber and drainage require light maintenance.Or flat sandy road.",
                    ActivitiesRequired = "Rating 1 + Light Grading or Light Manual Reshaping + light pothole Filling."

                },
                new RoadConditionCodeUnit
                {
                    ID = 3,
                    SurfaceTypeId = 1,
                    Rate = 3,
                    RoadCondition = "Fair Maintainable road.Camber and drainage require some reshaping",
                    ActivitiesRequired = "Rating 1 + Grading or Manual Reshaping + pothole and ruts filling. "

                },
                new RoadConditionCodeUnit
                {
                    ID = 4,
                    SurfaceTypeId = 1,
                    Rate = 4,
                    RoadCondition = "Poor - Passable but Un-Maintenable.No camber.Requires reinstatement",
                    ActivitiesRequired = "Rating 1 + Heavy Grading or Heavy Manual Reshaping + compaction Basically, Partial Rehabilitation."

                },
                new RoadConditionCodeUnit
                {
                    ID = 5,
                    SurfaceTypeId = 1,
                    Rate = 5,
                    RoadCondition = "Bad - Impassable",
                    ActivitiesRequired = "Reconstruction. "

                },
                new RoadConditionCodeUnit
                {
                    ID = 6,
                    SurfaceTypeId = 4,
                    Rate = 1,
                    RoadCondition = "Excellent / V. Good Maintainable road with no potholes and no cracks.",
                    ActivitiesRequired = "Nominal light off carriageway maintenance only required e.g. Bush clearing, Culvert cleaning, Ditch clearing, Mitre drains cleaning, Repair of Road Signs"

                },
                new RoadConditionCodeUnit
                {
                    ID = 7,
                    SurfaceTypeId = 4,
                    Rate = 2,
                    RoadCondition = "Good Maintainable road with some cracks and under  5 % potholes.",
                    ActivitiesRequired = "Rating 1 + light pothole patching +sealing cracks"

                },
                new RoadConditionCodeUnit
                {
                    ID = 8,
                    SurfaceTypeId = 4,
                    Rate = 3,
                    RoadCondition = "Fair Maintainable road with many cracks and  potholes(more than 5 %)",
                    ActivitiesRequired = "Rating 1 + pothole patching + base repair  + resealing"

                },
                new RoadConditionCodeUnit
                {
                    ID = 9,
                    SurfaceTypeId = 4,
                    Rate = 4,
                    RoadCondition = "Poor Un - maintainable;",
                    ActivitiesRequired = "Rehabilitation  (Holding Maintenance) "

                },
                new RoadConditionCodeUnit
                {
                    ID = 10,
                    SurfaceTypeId = 4,
                    Rate = 5,
                    RoadCondition = "Very Bad Un - maintainable:",
                    ActivitiesRequired = "Reconstruction (Holding Maintenance)"
                }
                );
        }

    }
}
