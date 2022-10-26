using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class WorkCategoryData
    {
        public static void SeedWorkCategory(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkCategory>().HasData(
            new WorkCategory
            {
                ID = 1L,
                Code = "RM_INSTRUCTED ",
                Name = "Routine Maintenance Instructed Works"
            },
             new WorkCategory
             {
                 ID = 2L,
                 Code = "RM_PBC",
                 Name = "Routine Maintenance PBC"
             },
            new WorkCategory
            {
                ID = 3L,
                Code = "PERIODIC_MTC",
                Name = "Periodic Maintenance"
            },
            new WorkCategory
            {
                ID = 4L,
                Code = "SPOT_IMPROVEMENT",
                Name = "Spot Improvement"
            },

            new WorkCategory
            {
                ID = 5L,
                Code = "REHABILITATION",
                Name = "Rehabilitation Work"
            },
            new WorkCategory
            {
                ID = 6L,
                Code = "CONSTRUCTION_RE",
                Name = "Reconstruction"
            },
            new WorkCategory
            {
                ID = 7L,
                Code = "CONSTRUCTION_NEW",
                Name = "New Construction"
            },
            new WorkCategory
            {
                ID = 8L,
                Code = "UPGRADING",
                Name = "Upgrading"
            }
            ,
            new WorkCategory
            {
                ID = 9L,
                Code = "STRUCTURES",
                Name = "Construction of Bridges and Culverts"
            },
            new WorkCategory
            {
                ID = 10L,
                Code = "ADMINISTRATION",
                Name = "Administration"
            },
            new WorkCategory
            {
                ID = 11L,
                Code = "Routine_Maintenance",
                Name = "Routine Maintenance"
            },
            new WorkCategory
            {
                ID = 12L,
                Code = "PBC_Management",
                Name = "PBC Maintenance"
            },
            new WorkCategory
            {
                ID = 13L,
                Code = "RAM",
                Name = "RAM"
            },
            new WorkCategory
            {
                ID = 14L,
                Code = "Network",
                Name = "Network"
            },
            new WorkCategory
            {
                ID = 15L,
                Code = "Weigh_Bridges",
                Name = "Weigh Bridges"
            },
            new WorkCategory
            {
                ID = 16L,
                Code = "Alehu_Operations_And_Purchase_Of_Vehicles",
                Name = "ALEHU OPERATIONS  AND PURCHASE OF VEHICLES"
            },
            new WorkCategory
            {
                ID = 17L,
                Code = "Axle_Load_Activities",
                Name = "AXLE LOAD ACTIVITIES"
            },
            new WorkCategory
            {
                ID = 18L,
                Code = "Emergency_Works",
                Name = "EMERGENCY WORKS"
            },
            new WorkCategory
            {
                ID = 19L,
                Code = "Road_Safety",
                Name = "Road Safety"
            },
            new WorkCategory
            {
                ID = 20L,
                Code = "Road_Condition_Survey",
                Name = "Road Condition Survey"
            },
            new WorkCategory
            {
                ID = 21L,
                Code = "Based_Contracts",
                Name = "Based Contracts"
            },
            new WorkCategory
            {
                ID = 22L,
                Code = "Matters",
                Name = "Matters"
            },
            new WorkCategory
            {
                ID = 23L,
                Code = "Support",
                Name = "Support"
            },
            new WorkCategory
            {
                ID = 24L,
                Code = "HQ_Activities",
                Name = "HQ Activities"
            },
            new WorkCategory
            {
                ID = 25L,
                Code = "New_Structure",
                Name = "New Structure"
            },
            new WorkCategory
            {
                ID = 26L,
                Code = "RSIP_Critical_Links",
                Name = "RSIP Critical Links"
            }
            );
        }
    }
}
