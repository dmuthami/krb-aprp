using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class DisbursementCodeListData
    {
        public static void SeedDisbursementCodeList(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DisbursementCodeList>().HasData(
            new DisbursementCodeList
            {
                ID = 1L,
                Code = "4",
                Name = "Kenya Roads Board",
                DisplayName = "Kenya Roads Board",
                Display = true,
                Order = 1,
                SNo=1,
                ReleaseOrder=8
            },
            new DisbursementCodeList
            {
                ID = 2L,
                Code = "15",
                Name = "National Roads Maintenance",
                DisplayName = "National Roads Maintenance",
                Display = true,
                Order = 3,
                ReleaseOrder = 1
            },
            new DisbursementCodeList
            {
                ID = 3L,
                Code = "42",
                Name = "National Roads Development",
                DisplayName = "National Roads Development",
                Display = true,
                Order = 4

            },
            new DisbursementCodeList
            {
                ID = 4L,
                Code = "16",
                Name = "Transit Tolls",
                DisplayName = "Transit Tolls",
                Display = true,
                Order = 5,
                ReleaseOrder=2
            },
            new DisbursementCodeList
            {
                ID = 5L,
                Code = "31",
                Name = "Constituency",
                DisplayName = "Constituency",
                Display = true,
                Order = 8,
                ReleaseOrder=3
            },
            new DisbursementCodeList
            {
                ID = 6L,
                Code = "32",
                Name = "Critical Links",
                DisplayName = "Critical Links",
                Display = true,
                Order = 9,
                ReleaseOrder=4
            },
            new DisbursementCodeList
            {
                ID = 7L,
                Code = "22",
                Name = "KURA",
                DisplayName = "KURA",
                Display = true,
                Order = 11,
                SNo = 4,
                ReleaseOrder=5
            },
            new DisbursementCodeList
            { ID = 8L, Code = "21", Name = "KWS", DisplayName = "KWS", Display = true, Order = 12, SNo = 5,ReleaseOrder=7 },
            new DisbursementCodeList
            { ID = 9L, Code = "20", Name = "KRB/CS Allocation", DisplayName = "KRB/CS Allocation", 
                Display = true, Order = 13, SNo = 6,ReleaseOrder=9 },
            new DisbursementCodeList
            { ID = 10L, Code = "14", Name = "County Government", DisplayName = "County Government", 
                Display = true, Order = 14, SNo = 7,ReleaseOrder=6 },
            new DisbursementCodeList
            { ID = 11L, Code = "9", Name = "Road Annuity Fund", DisplayName = "Road Annuity Fund", Display = true, Order = 16 },
            new DisbursementCodeList
            { ID = 12L, Code = "", Name = "Total KeNHA", DisplayName = "Total KeNHA", Display = true, Order = 6,AuthorityId=1},
            new DisbursementCodeList
            { ID = 13L, Code = "", Name = "KENHA", DisplayName = "KENHA", Display = true, Order = 2, SNo = 2,ReleaseOrder=null },
            new DisbursementCodeList
            { ID = 14L, Code = "", Name = "KERRA", DisplayName = "KERRA", Display = true, Order = 7, SNo = 3 },
            new DisbursementCodeList
            { ID = 15L, Code = "", Name = "Total KERRA", DisplayName = "Total KERRA", Display = true, Order = 10,AuthorityId=2 },
            new DisbursementCodeList
            { ID = 16L, Code = "", Name = "Sub Total 1", DisplayName = "", Display = true, Order = 15 },
            new DisbursementCodeList
            { ID = 17L, Code = "", Name = "Sub Total 2", DisplayName = "", Display = true, Order = 17 }
        );
        }
    }
}
