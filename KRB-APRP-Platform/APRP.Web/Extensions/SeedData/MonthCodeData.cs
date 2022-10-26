using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class MonthCodeData
    {
        public static void SeedMonthCode(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MonthCode>().HasData(
                new MonthCode
                {
                    ID = 1L,
                    Code = "JAN",
                    Description = "JANUARY"
                },
                 new MonthCode
                 {
                     ID = 2L,
                     Code = "FEB",
                     Description = "FEBRUARY"
                 },
                 new MonthCode
                 {
                     ID = 3L,
                     Code = "MAR",
                     Description = "MARCH"
                 },
                 new MonthCode
                 {
                     ID = 4L,
                     Code = "APR",
                     Description = "APRIL"
                 },
                 new MonthCode
                 {
                     ID = 5L,
                     Code = "MAY",
                     Description = "MAY"
                 },
                 new MonthCode
                 {
                     ID = 6L,
                     Code = "JUN",
                     Description = "JUNE"
                 },
                 new MonthCode
                 {
                     ID = 7L,
                     Code = "JUL",
                     Description = "JULY"
                 },
                 new MonthCode
                 {
                     ID = 8L,
                     Code = "AUG",
                     Description = "AUGUST"
                 },
                 new MonthCode
                 {
                     ID = 9L,
                     Code = "SEP",
                     Description = "SEPTEMBER"
                 },
                 new MonthCode
                 {
                     ID = 10L,
                     Code = "OCT",
                     Description = "OCTOBER"
                 },
                 new MonthCode
                 {
                     ID = 11L,
                     Code = "NOV",
                     Description = "NOVEMBER"
                 },
                 new MonthCode
                 {
                     ID = 12L,
                     Code = "DEC",
                     Description = "DECEMBER"
                 }
            );
        }
    }
}
