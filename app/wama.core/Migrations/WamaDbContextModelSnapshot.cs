﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using WAMA.Core.Models;

namespace WAMA.Core.Migrations
{
    [DbContext(typeof(WamaDbContext))]
    partial class WamaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WAMA.Core.Models.DTOs.Certification", b =>
                {
                    b.Property<int>("__ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CertifiedOn");

                    b.Property<DateTimeOffset>("ExpiresOn");

                    b.Property<string>("MemberId");

                    b.Property<int>("Type");

                    b.Property<DateTimeOffset>("__CreatedOn");

                    b.Property<DateTimeOffset>("__LastUpdatedOn")
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("__ID");

                    b.HasIndex("MemberId");

                    b.ToTable("Certifications");
                });

            modelBuilder.Entity("WAMA.Core.Models.DTOs.CheckInActivity", b =>
                {
                    b.Property<int>("__ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CheckInDateTime");

                    b.Property<bool>("IsCheckedIn");

                    b.Property<string>("MemberId");

                    b.Property<DateTimeOffset>("__CreatedOn");

                    b.Property<DateTimeOffset>("__LastUpdatedOn")
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("__ID");

                    b.HasIndex("MemberId");

                    b.ToTable("CheckInActivities");
                });

            modelBuilder.Entity("WAMA.Core.Models.DTOs.LogInCredential", b =>
                {
                    b.Property<int>("__ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MemberId");

                    b.Property<string>("Password");

                    b.Property<bool>("RequiresPassword");

                    b.Property<DateTimeOffset>("__CreatedOn");

                    b.Property<DateTimeOffset>("__LastUpdatedOn")
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("__ID");

                    b.HasIndex("MemberId")
                        .IsUnique();

                    b.ToTable("LogInCredentials");
                });

            modelBuilder.Entity("WAMA.Core.Models.DTOs.UserAccount", b =>
                {
                    b.Property<int>("__ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountType");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("Gender");

                    b.Property<bool>("HasBeenApproved");

                    b.Property<bool>("IsSuspended");

                    b.Property<string>("LastName");

                    b.Property<string>("MemberId")
                        .IsRequired();

                    b.Property<string>("MiddleName");

                    b.Property<DateTimeOffset>("__CreatedOn");

                    b.Property<DateTimeOffset>("__LastUpdatedOn")
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("__ID");

                    b.ToTable("UserAccounts");
                });

            modelBuilder.Entity("WAMA.Core.Models.DTOs.Waiver", b =>
                {
                    b.Property<int>("__ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MemberId");

                    b.Property<DateTimeOffset>("SignedOn");

                    b.Property<DateTimeOffset>("__CreatedOn");

                    b.Property<DateTimeOffset>("__LastUpdatedOn")
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("__ID");

                    b.HasIndex("MemberId");

                    b.ToTable("Waivers");
                });

            modelBuilder.Entity("WAMA.Core.Models.DTOs.Certification", b =>
                {
                    b.HasOne("WAMA.Core.Models.DTOs.UserAccount", "Member")
                        .WithMany("Certifications")
                        .HasForeignKey("MemberId")
                        .HasPrincipalKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WAMA.Core.Models.DTOs.CheckInActivity", b =>
                {
                    b.HasOne("WAMA.Core.Models.DTOs.UserAccount", "Member")
                        .WithMany("CheckInActivities")
                        .HasForeignKey("MemberId")
                        .HasPrincipalKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WAMA.Core.Models.DTOs.LogInCredential", b =>
                {
                    b.HasOne("WAMA.Core.Models.DTOs.UserAccount", "Member")
                        .WithOne("LogInCredential")
                        .HasForeignKey("WAMA.Core.Models.DTOs.LogInCredential", "MemberId")
                        .HasPrincipalKey("WAMA.Core.Models.DTOs.UserAccount", "MemberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WAMA.Core.Models.DTOs.Waiver", b =>
                {
                    b.HasOne("WAMA.Core.Models.DTOs.UserAccount", "Member")
                        .WithMany("Waivers")
                        .HasForeignKey("MemberId")
                        .HasPrincipalKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
