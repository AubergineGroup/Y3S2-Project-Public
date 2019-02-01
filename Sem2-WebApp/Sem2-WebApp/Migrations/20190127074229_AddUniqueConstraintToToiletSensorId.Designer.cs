﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sem2_WebApp.Models;

namespace Sem2WebApp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190127074229_AddUniqueConstraintToToiletSensorId")]
    partial class AddUniqueConstraintToToiletSensorId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Sem2_WebApp.Models.Cleaner", b =>
                {
                    b.Property<int>("CleanerId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CleanedCount")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<string>("CleanerOwner")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(254);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<string>("Rfid")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(18)
                        .HasDefaultValue("No card registered");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20)
                        .HasDefaultValue("Available");

                    b.HasKey("CleanerId");

                    b.ToTable("Cleaners");
                });

            modelBuilder.Entity("Sem2_WebApp.Models.Review", b =>
                {
                    b.Property<int>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Month")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("Rating")
                        .HasMaxLength(50);

                    b.Property<string>("Year")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("ReviewId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("Sem2_WebApp.Models.SensorValues", b =>
                {
                    b.Property<int>("SensorValuesId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CurrentUsers");

                    b.Property<int>("GasValue");

                    b.Property<int>("Requests");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("smalldatetime");

                    b.Property<int>("ToiletId");

                    b.Property<int>("TotalUsers");

                    b.HasKey("SensorValuesId");

                    b.HasIndex("ToiletId");

                    b.ToTable("SensorsValues");
                });

            modelBuilder.Entity("Sem2_WebApp.Models.Subscription", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PushAuth")
                        .IsRequired();

                    b.Property<string>("PushEndpoint")
                        .IsRequired();

                    b.Property<string>("PushP256Dh")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("Sem2_WebApp.Models.Toilet", b =>
                {
                    b.Property<int>("ToiletId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comments")
                        .HasMaxLength(100);

                    b.Property<DateTime?>("LastCleaned")
                        .HasColumnType("smalldatetime");

                    b.Property<string>("SensorId")
                        .IsRequired();

                    b.Property<string>("ToiletGender")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(6)
                        .HasDefaultValue("Female");

                    b.Property<string>("ToiletName")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("ToiletOwner")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.HasKey("ToiletId");

                    b.HasIndex("SensorId")
                        .IsUnique();

                    b.ToTable("Toilets");
                });

            modelBuilder.Entity("Sem2_WebApp.Models.ToiletLog", b =>
                {
                    b.Property<int>("ToiletLogId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CleanerId");

                    b.Property<int>("Duration");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("smalldatetime");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("smalldatetime");

                    b.Property<int>("ToiletId");

                    b.HasKey("ToiletLogId");

                    b.HasIndex("CleanerId");

                    b.HasIndex("ToiletId");

                    b.ToTable("ToiletLogs");
                });

            modelBuilder.Entity("Sem2_WebApp.Models.ToiletSettings", b =>
                {
                    b.Property<int>("ToiletSettingsId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FanMode")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(4)
                        .HasDefaultValue("Auto");

                    b.Property<int>("FanThreshold")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(40);

                    b.Property<int>("GasValueThreshold")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(60);

                    b.Property<int>("RequestThreshold")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(10);

                    b.Property<int>("ToiletId");

                    b.Property<int>("UpdateFrequency")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1000);

                    b.Property<int>("UserThreshold")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(20);

                    b.HasKey("ToiletSettingsId");

                    b.HasIndex("ToiletId");

                    b.ToTable("ToiletsSettings");
                });

            modelBuilder.Entity("Sem2_WebApp.Models.SensorValues", b =>
                {
                    b.HasOne("Sem2_WebApp.Models.Toilet", "Toilet")
                        .WithMany("SensorsValues")
                        .HasForeignKey("ToiletId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sem2_WebApp.Models.ToiletLog", b =>
                {
                    b.HasOne("Sem2_WebApp.Models.Cleaner", "Cleaner")
                        .WithMany("ToiletLogs")
                        .HasForeignKey("CleanerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sem2_WebApp.Models.Toilet", "Toilet")
                        .WithMany("ToiletLogs")
                        .HasForeignKey("ToiletId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sem2_WebApp.Models.ToiletSettings", b =>
                {
                    b.HasOne("Sem2_WebApp.Models.Toilet", "Toilet")
                        .WithMany("ToiletsSettings")
                        .HasForeignKey("ToiletId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
