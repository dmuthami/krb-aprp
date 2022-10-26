using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class ItemActivityPBCData
    {
        public static void SeedItemActivityPBCData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemActivityPBC>().HasData(
            new ItemActivityPBC
            {
                ID = 1L,
                Code = "26-50-001",
                Description = "Performance Based Routine Maintenance Off carriageway paved",
                TechnologyId=1L
            },
             new ItemActivityPBC
             {
                 ID = 2L,
                 Code = "26-50-002",
                 Description = "Performance Based Routine Maintenance On carriageway unpaved",
                 TechnologyId = 1L
             },
            new ItemActivityPBC
            {
                ID = 3L,
                Code = "26-50-003",
                Description = "Performance Based Routine Maintenance On carriageway paved",
                TechnologyId = 1L
            }
            );
        }
    }
}
