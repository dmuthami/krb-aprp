using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class ARICSApprovalLevelData
    {
        public static void SeedARICSApprovalLevel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ARICSApprovalLevel>().HasData(
            new ARICSApprovalLevel
            {
                ID = 1,
                Status = 1,
                Role = "Preparer/Submitter",
                Designation = "Roads Engineer",
                AuthorityType = 2,
                Order = 1
            },

            new ARICSApprovalLevel
            {
                ID = 2,
                Status = 2,
                Role = "1st Reviewer",
                Designation = "Director Roads",
                AuthorityType = 2,
                Order = 2
            },
            new ARICSApprovalLevel
            {
                ID = 3,
                Status = 3,
                Role = "CO-Roads",
                Designation = "2nd Reviewer",
                AuthorityType = 2,
                Order = 3
            },
            new ARICSApprovalLevel
            {
                ID = 4,
                Status = 4,
                Role = "Internal Approver",
                Designation = "CECM-Roads",
                AuthorityType = 2,
                Order = 4
            },
            new ARICSApprovalLevel
            {
                ID = 5,
                Status = 5,
                Role = "1st Reviewer",
                Designation = "Deputy Manager PP (3)",
                AuthorityType = 0,
                Order = 5
            },
            new ARICSApprovalLevel
            {
                ID = 6,
                Status = 6,
                Role = "2nd Reviewer",
                Designation = "Manager PP (3)",
                AuthorityType = 0,
                Order = 6
            },
            new ARICSApprovalLevel
            {
                ID = 7,
                Status = 7,
                Role = "Approver",
                Designation = "GMPP & ED",
                AuthorityType = 0,
                Order = 7
            }, 
            new ARICSApprovalLevel
            {
                ID = 8,
                Status = 8,
                Role = "Preparer/Submitter",
                Designation = "Roads Engineer",
                AuthorityType = 1,
                Order = 1
            },
            new ARICSApprovalLevel
            {
                ID = 9,
                Status = 9,
                Role = "1st Reviewer",
                Designation = "Deputy Director",
                AuthorityType = 1,
                Order = 2
            }, 
            new ARICSApprovalLevel
            {
                ID = 10,
                Status = 10,
                Role = "Internal Approver",
                Designation = "Director RACM",
                AuthorityType = 1,
                Order = 3
            }
            );
        }
    }
}
