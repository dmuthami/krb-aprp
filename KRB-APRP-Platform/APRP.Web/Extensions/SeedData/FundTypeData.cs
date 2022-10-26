using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class FundTypeData
    {
        public static void SeedFundType(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FundType>().HasData(
               new FundType
               {
                   ID = 1L,
                   Code = "Carried Over",
                   Name = "Carried Over"
               },
               new FundType
               {
                   ID = 2L,
                   Code = "Regular",
                   Name = "Regular"
               },
               new FundType
               {
                   ID = 3L,
                   Code = "Revoted",
                   Name = "Revoted"
               },
               new FundType
               {
                   ID = 4L,
                   Code = "Surplus",
                   Name = "Surplus"
               }
           );
        }
    }
}
