using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class ARICSYearData
    {
        public static void SeedARICSYear(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ARICSYear>().HasData(
            new ARICSYear
            {
                ID = 1,
                Year = 2016,
                Description = "Year 2016"
            }
            );
        }
    }
}
