using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class FinancialYearData
    {
        public static void SeedFinancialYear(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FinancialYear>().HasData(
                new FinancialYear
                {
                    ID = 1L,
                    Code = "2019/2020",
                    Summary = "Road Work Planning for 2019/2020",
                    IsCurrent = IsCurrent.Current,
                    ARICSYearId = 1
                }
            );
        }
    }
}
