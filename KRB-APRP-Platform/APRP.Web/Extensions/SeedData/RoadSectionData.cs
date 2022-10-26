using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class RoadSectionData
    {
        public static void SeedRoadSection(this ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<RoadSection>().HasData(
            //    new RoadSection
            //    {
            //        ID = 1L,
            //        SectionName = "Webuye-Tongaren",
            //        SectionID = "C781_0_0",
            //        StartChainage = 0,
            //        EndChainage = 0,
            //        Length = 29988.220000000001164D,
            //        Interval = 200,
            //        SurfaceTypeId = 4,
            //        ConstituencyId = 1,
            //        RoadId = 5
            //    },
            //    new RoadSection
            //    {
            //        ID = 2L,
            //        SectionName = "Chwele-Kimilili",
            //        SectionID = "C780_0_0",
            //        StartChainage = 0,
            //        EndChainage = 0,
            //        Length = 38544.80000000000291D,
            //        Interval = 200,
            //        SurfaceTypeId = 4,
            //        ConstituencyId = 1,
            //        RoadId = 10
            //    }
            //   );
        }
    }
}
