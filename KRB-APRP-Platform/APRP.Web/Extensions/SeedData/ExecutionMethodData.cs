using APRP.Web.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Extensions.SeedData
{
    public static class ExecutionMethodData
    {
        public static void SeedExecutionMethod(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExecutionMethod>().HasData(
            new ExecutionMethod
            {
                ID = 1L,
                Code = "Contracting",
                Name = "Contracting"
            },
            new ExecutionMethod
            {
                ID = 2L,
                Code = "Force Acc",
                Name = "Force Acc"
            }
            );
        }
    }
}
