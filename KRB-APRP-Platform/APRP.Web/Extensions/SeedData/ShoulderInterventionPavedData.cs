using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class ShoulderInterventionPavedData
    {
        public static void SeedShoulderInterventionPaved(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShoulderInterventionPaved>().HasData(
            new ShoulderInterventionPaved
            {
                ID = 1L,
                Code = "NA",
                Name = "NA",
                Description = "Not Applicable"
            },
             new ShoulderInterventionPaved
             {
                 ID = 2L,
                 Code = "G",
                 Name = "Grading",
                 Description = "Shoulder Grading Required"
             },
             new ShoulderInterventionPaved
             {
                 ID = 3L,
                 Code = "R",
                 Name = "Re-Instatement",
                 Description = "Shoulder Re-Instatement Required"
             }
            );
        }
    }
}
