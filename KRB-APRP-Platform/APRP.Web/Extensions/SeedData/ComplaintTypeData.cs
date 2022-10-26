using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class ComplaintTypeData
    {
        public static void SeedComplaintType(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ComplaintType>().HasData(
             new ComplaintType
             {
                 ID = 1L,
                 Code = "GENERAL",
                 Description = "Genaral System Issue"
             },
             new ComplaintType
             {
                 ID = 2L,
                 Code = "ARICS",
                 Description = "ARICS Issues"
             },
             new ComplaintType
             {
                 ID = 3L,
                 Code = "WORKPLANNING",
                 Description = "Work planning"
             },
              new ComplaintType
              {
                  ID = 4L,
                  Code = "CONTRACTING",
                  Description = "Work packaging and contracting "
              },
              new ComplaintType
              {
                  ID = 5L,
                  Code = "USERMANAGEMENT",
                  Description = "User management and administration"
              }
            );
        }
    }
}
