using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pedigree.Core.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Text;

namespace Pedigree.Infrastructure.Database
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
            this.Database.SetCommandTimeout(1000);
        }

        public virtual DbSet<Horse> Horses { get; set; }
        public virtual DbSet<Relationship> Relationships { get; set; }
        public virtual DbSet<Race> Races { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<Inbreed> Inbreeds { get; set; }
        public virtual DbSet<Coefficient> Coefficients { get; set; }
        public virtual DbSet<HaploGroup> HaploGroups { get; set; }
        public virtual DbSet<HaploType> HaploTypes { get; set; }
        public virtual DbSet<StallionRating> StallionRatings { get; set; }
        public virtual DbSet<Pedig> Pedigs { get; set; }
        public virtual DbSet<Ancestry> Ancestries { get; set; }
        public virtual DbSet<MLModel> MLModels { get; set; }
        public virtual DbSet<Auction> Auction { get; set; }
        public virtual DbSet<AuctionDetail> AuctionDetail { get; set; }
        public virtual DbSet<MtDNAFlag> MtDNAFlags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region Table Names
            builder.Entity<Horse>().ToTable("Horse");
            builder.Entity<Relationship>().ToTable("Relationship");
            builder.Entity<Race>().ToTable("Race");
            builder.Entity<Position>().ToTable("Position");
            builder.Entity<Weight>().ToTable("Weight");
            builder.Entity<Inbreed>().ToTable("Inbreed");
            builder.Entity<Coefficient>().ToTable("Coefficient");
            builder.Entity<HaploGroup>().ToTable("HaploGroup");
            builder.Entity<HaploType>().ToTable("HaploType");
            builder.Entity<StallionRating>().ToTable("StallionRating");
            builder.Entity<Pedig>().ToTable("Pedig");
            builder.Entity<Ancestry>().ToTable("Ancestry");
            #endregion

            builder.Entity<Horse>()
            .HasOne(h => h.HaploType)
            .WithMany(t => t.Horses)
            .HasForeignKey(h => h.MtDNA);

            builder.AddJsonFields();
        }
    }
}
