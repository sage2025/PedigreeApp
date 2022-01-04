﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pedigree.Infrastructure.Database;

namespace Pedigree.App.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210820143624_AddExtraValuesToStallionRating")]
    partial class AddExtraValuesToStallionRating
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Pedigree.Core.Data.Entity.Ancestry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AncestorOId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("AvgMC")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Ancestry");
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.Coefficient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double?>("AHC")
                        .HasColumnType("float");

                    b.Property<DateTime?>("BPRUpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<double?>("Bal")
                        .HasColumnType("float");

                    b.Property<double>("COI")
                        .HasColumnType("float");

                    b.Property<double>("COI1")
                        .HasColumnType("float");

                    b.Property<double>("COI2")
                        .HasColumnType("float");

                    b.Property<double>("COI3")
                        .HasColumnType("float");

                    b.Property<double>("COI4")
                        .HasColumnType("float");

                    b.Property<double>("COI5")
                        .HasColumnType("float");

                    b.Property<double>("COI6")
                        .HasColumnType("float");

                    b.Property<double>("COI7")
                        .HasColumnType("float");

                    b.Property<double>("COI8")
                        .HasColumnType("float");

                    b.Property<double>("COID1")
                        .HasColumnType("float");

                    b.Property<double>("COID2")
                        .HasColumnType("float");

                    b.Property<double>("COID3")
                        .HasColumnType("float");

                    b.Property<double>("COID4")
                        .HasColumnType("float");

                    b.Property<double>("COID5")
                        .HasColumnType("float");

                    b.Property<double>("COID6")
                        .HasColumnType("float");

                    b.Property<DateTime?>("COIUpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<double?>("CurrentBPR")
                        .HasColumnType("float");

                    b.Property<double?>("CurrentRD")
                        .HasColumnType("float");

                    b.Property<double>("GDGD")
                        .HasColumnType("float");

                    b.Property<double>("GDGS")
                        .HasColumnType("float");

                    b.Property<double>("GI")
                        .HasColumnType("float");

                    b.Property<DateTime?>("GRainProcessStartedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("GRainUpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<double>("GSDD")
                        .HasColumnType("float");

                    b.Property<double>("GSSD")
                        .HasColumnType("float");

                    b.Property<double?>("HistoricalBPR")
                        .HasColumnType("float");

                    b.Property<double?>("HistoricalRD")
                        .HasColumnType("float");

                    b.Property<string>("HorseOId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Kal")
                        .HasColumnType("float");

                    b.Property<double>("Pedigcomp")
                        .HasColumnType("float");

                    b.Property<int?>("UniqueAncestorsCount")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UniqueAncestorsCountUpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<double?>("ZCurrentBPR")
                        .HasColumnType("float");

                    b.Property<double?>("ZHistoricalBPR")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Coefficient");
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.HaploGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Color")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("HaploGroup");
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.HaploType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("HaploType");
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.Horse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Family")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsFounder")
                        .HasColumnType("bit");

                    b.Property<int?>("MtDNA")
                        .HasColumnType("int");

                    b.Property<bool>("MtDNAFlag")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sex")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("MtDNA");

                    b.ToTable("Horse");

                    b.HasDiscriminator();
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.Inbreed", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Depth")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InbreedOId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SD")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Inbreed");
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.MLModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Deployed")
                        .HasColumnType("bit");

                    b.Property<string>("Features")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HorsesCount")
                        .HasColumnType("int");

                    b.Property<string>("ModelName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ModelPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ModelVersion")
                        .HasColumnType("int");

                    b.Property<double>("RMSError")
                        .HasColumnType("float");

                    b.Property<double>("RSquared")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("MLModels");
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.MtDNAFlag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("EndHorseOId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StartHorseOId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("MtDNAFlags");
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.Pedig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("HorseOId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Pedigree")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("PedigreeUpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ProbOrigs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ProbOrigsUpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Pedig");
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.Position", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("HorseOId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Place")
                        .HasColumnType("int");

                    b.Property<int>("RaceId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("RaceId");

                    b.ToTable("Position");
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.Race", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Distance")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surface")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Race");
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.Relationship", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("HorseOId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentOId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Relationship");
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.StallionRating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double?>("AE")
                        .HasColumnType("float");

                    b.Property<int?>("BMSCurrentRCount")
                        .HasColumnType("int");

                    b.Property<double?>("BMSCurrentStallionRating")
                        .HasColumnType("float");

                    b.Property<int?>("BMSCurrentZCount")
                        .HasColumnType("int");

                    b.Property<int?>("BMSHistoricalRCount")
                        .HasColumnType("int");

                    b.Property<double?>("BMSHistoricalStallionRating")
                        .HasColumnType("float");

                    b.Property<int?>("BMSHistoricalZCount")
                        .HasColumnType("int");

                    b.Property<int?>("BMSOSCurrentRCount")
                        .HasColumnType("int");

                    b.Property<int?>("BMSOSCurrentSCount")
                        .HasColumnType("int");

                    b.Property<double?>("BMSOSCurrentStallionRating")
                        .HasColumnType("float");

                    b.Property<int?>("BMSOSCurrentZCount")
                        .HasColumnType("int");

                    b.Property<int?>("BMSOSHistoricalRCount")
                        .HasColumnType("int");

                    b.Property<int?>("BMSOSHistoricalSCount")
                        .HasColumnType("int");

                    b.Property<double?>("BMSOSHistoricalStallionRating")
                        .HasColumnType("float");

                    b.Property<int?>("BMSOSHistoricalZCount")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CropAge")
                        .HasColumnType("int");

                    b.Property<int?>("CurrentRCount")
                        .HasColumnType("int");

                    b.Property<double?>("CurrentStallionRating")
                        .HasColumnType("float");

                    b.Property<int?>("CurrentZCount")
                        .HasColumnType("int");

                    b.Property<int?>("HistoricalRCount")
                        .HasColumnType("int");

                    b.Property<double?>("HistoricalStallionRating")
                        .HasColumnType("float");

                    b.Property<int?>("HistoricalZCount")
                        .HasColumnType("int");

                    b.Property<string>("HorseOId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("IV")
                        .HasColumnType("float");

                    b.Property<double?>("PRB2")
                        .HasColumnType("float");

                    b.Property<int?>("SOSCurrentRCount")
                        .HasColumnType("int");

                    b.Property<int?>("SOSCurrentSCount")
                        .HasColumnType("int");

                    b.Property<double?>("SOSCurrentStallionRating")
                        .HasColumnType("float");

                    b.Property<int?>("SOSCurrentZCount")
                        .HasColumnType("int");

                    b.Property<int?>("SOSHistoricalRCount")
                        .HasColumnType("int");

                    b.Property<int?>("SOSHistoricalSCount")
                        .HasColumnType("int");

                    b.Property<double?>("SOSHistoricalStallionRating")
                        .HasColumnType("float");

                    b.Property<int?>("SOSHistoricalZCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("StallionRating");
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.Weight", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Distance")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surface")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<double>("Value")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Weight");
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.HaploType", b =>
                {
                    b.HasOne("Pedigree.Core.Data.Entity.HaploGroup", "Group")
                        .WithMany("Types")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.Horse", b =>
                {
                    b.HasOne("Pedigree.Core.Data.Entity.HaploType", "HaploType")
                        .WithMany("Horses")
                        .HasForeignKey("MtDNA");
                });

            modelBuilder.Entity("Pedigree.Core.Data.Entity.Position", b =>
                {
                    b.HasOne("Pedigree.Core.Data.Entity.Race", "Race")
                        .WithMany()
                        .HasForeignKey("RaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
