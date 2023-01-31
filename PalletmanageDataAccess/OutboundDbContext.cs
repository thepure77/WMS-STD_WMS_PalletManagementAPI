using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

using DataAccess.Models.Outbound.Table;

namespace DataAccess
{
    public class OutboundDbContext : DbContext
    {
        public virtual DbSet<im_PlanGoodsIssue> im_PlanGoodsIssue { get; set; }

        public virtual DbSet<im_PlanGoodsIssueItem> im_PlanGoodsIssueItem { get; set; }

        public virtual DbSet<im_Pallet> im_Pallet { get; set; }

        public virtual DbSet<im_LentPallet> im_LentPallet { get; set; }

        public virtual DbSet<im_Pallet_loc> im_Pallet_loc { get; set; }

        public virtual DbSet<im_LentPallet_loc> im_LentPallet_loc { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: false);

                var configuration = builder.Build();

                var connectionString = configuration.GetConnectionString("Outbound_ConnectionString").ToString();

                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
