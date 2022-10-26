using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class QuarterCodeListData
    {
        public static void SeedQurterCodeList(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuarterCodeList>().HasData(
            new QuarterCodeList
            {
                ID = 1,Name = "(Q1) Jul-Aug-Sep",Description = "Jul-Aug-Sep",ReferenceID = 1
            },

            new QuarterCodeList
            {
                ID = 2,
                Name = "(Q2) Oct-Nov-Dec",
                Description = "Oct-Nov-Dec",
                ReferenceID = 2
            },
            new QuarterCodeList
            {
                ID = 3,
                Name = "(Q3) Jan-Feb-March",
                Description = "Jan-Feb-March",
                ReferenceID = 3
            },
            new QuarterCodeList
            {
                ID = 4,
                Name = "(Q4) Apr-May-Jun",
                Description = "Apr-May-Jun",
                ReferenceID = 4
            }
            );
        }
    }
}
