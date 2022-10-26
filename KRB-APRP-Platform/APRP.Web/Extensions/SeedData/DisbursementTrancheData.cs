using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class DisbursementTrancheData
    {
        public static void SeedDisbursementTranche(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DisbursementTranche>().HasData(
            new DisbursementTranche
            {
                ID = 1L,
                Code = "0100",
                Name = "1st Tranche"
            },
            new DisbursementTranche
            {
                ID = 2L,
                Code = "0200",
                Name = "2nd Tranche"
            },
            new DisbursementTranche
            {
                ID = 3L,
                Code = "0300",
                Name = "3rd Tranche"
            },
            new DisbursementTranche {ID = 4L,Code = "0400",Name = "4th Tranche"},
            new DisbursementTranche { ID = 5L, Code = "0500", Name = "5th Tranche" },
            new DisbursementTranche { ID = 6L, Code = "0600", Name = "6th Tranche" },
            new DisbursementTranche { ID = 7L, Code = "0700", Name = "7th Tranche" },
            new DisbursementTranche { ID = 8L, Code = "0800", Name = "8th Tranche" },
            new DisbursementTranche { ID = 9L, Code = "0900", Name = "9th Tranche" },
            new DisbursementTranche { ID = 10L, Code = "1000", Name = "10th Tranche" },
            new DisbursementTranche { ID = 11L, Code = "1100", Name = "11th Tranche" },
            new DisbursementTranche { ID = 12L, Code = "1200", Name = "12th Tranche" }
        );
        }
    }
}
