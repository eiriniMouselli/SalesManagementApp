using Microsoft.EntityFrameworkCore;
using SalesManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesManagementApp.Data
{
    public class SalesManagementContext : DbContext 
    {
        public SalesManagementContext(DbContextOptions<SalesManagementContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SalesPerson>()
            .HasMany(sp => sp.Sales)
            .WithOne(s => s.SalesEmployee)
            .IsRequired()
            .HasPrincipalKey(sp => sp.FullName)
            .HasForeignKey(s => s.SalesPersonFullName);
        }

        public DbSet<Sale> Sales { get; set; }
        public DbSet<SalesPerson> SalesPeople { get; set; }
    }
}
