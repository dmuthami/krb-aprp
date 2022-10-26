using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class RevenueCollectionCodeUnitTypeData
    {
        public static void SeedRevenueCollectionCodeUnitType(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RevenueCollectionCodeUnitType>().HasData(
                new RevenueCollectionCodeUnitType
                {
                    ID = 1L,
                    Type = "KRB"
                }, new RevenueCollectionCodeUnitType
                {
                    ID = 2L,
                    Type = "Others"
                }
                );
        }

    }
}
