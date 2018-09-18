﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OptimaJet.DWKit.StarterApplication.Data;

namespace OptimaJet.DWKit.StarterApplication.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20180803141430_AddEmail")]
    partial class AddEmail
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("OptimaJet.DWKit.StarterApplication.Models.Email", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Bcc");

                    b.Property<string>("Cc");

                    b.Property<string>("ContentModelJson");

                    b.Property<string>("ContentTemplate");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Subject");

                    b.Property<string>("To");

                    b.HasKey("Id");

                    b.ToTable("Emails");
                });

            modelBuilder.Entity("OptimaJet.DWKit.StarterApplication.Models.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Abbreviation");

                    b.Property<string>("Name");

                    b.Property<int>("OwnerId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("OptimaJet.DWKit.StarterApplication.Models.GroupMembership", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GroupId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupMemberships");
                });

            modelBuilder.Entity("OptimaJet.DWKit.StarterApplication.Models.Organization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BuildEngineApiAccessToken");

                    b.Property<string>("BuildEngineUrl");

                    b.Property<string>("Name");

                    b.Property<int>("OwnerId");

                    b.Property<string>("WebsiteUrl");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("OptimaJet.DWKit.StarterApplication.Models.OrganizationInvite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("OwnerEmail");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.ToTable("OrganizationInvites");
                });

            modelBuilder.Entity("OptimaJet.DWKit.StarterApplication.Models.OrganizationInviteRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("OrgAdminEmail");

                    b.Property<string>("WebsiteUrl");

                    b.HasKey("Id");

                    b.ToTable("OrganizationInviteRequests");
                });

            modelBuilder.Entity("OptimaJet.DWKit.StarterApplication.Models.OrganizationMembership", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("OrganizationId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("UserId");

                    b.ToTable("OrganizationMemberships");
                });

            modelBuilder.Entity("OptimaJet.DWKit.StarterApplication.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<string>("Description");

                    b.Property<int>("GroupId");

                    b.Property<string>("Language");

                    b.Property<string>("Name");

                    b.Property<int>("OrganizationId");

                    b.Property<int>("OwnerId");

                    b.Property<bool>("Private");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("OptimaJet.DWKit.StarterApplication.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("ExternalId");

                    b.Property<string>("FamilyName");

                    b.Property<string>("GivenName");

                    b.Property<bool>("IsLocked");

                    b.Property<string>("Locale");

                    b.Property<string>("Name");

                    b.Property<string>("Phone");

                    b.Property<string>("Timezone");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("OptimaJet.DWKit.StarterApplication.Models.Group", b =>
                {
                    b.HasOne("OptimaJet.DWKit.StarterApplication.Models.Organization", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OptimaJet.DWKit.StarterApplication.Models.GroupMembership", b =>
                {
                    b.HasOne("OptimaJet.DWKit.StarterApplication.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("OptimaJet.DWKit.StarterApplication.Models.User", "User")
                        .WithMany("GroupMemberships")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OptimaJet.DWKit.StarterApplication.Models.Organization", b =>
                {
                    b.HasOne("OptimaJet.DWKit.StarterApplication.Models.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OptimaJet.DWKit.StarterApplication.Models.OrganizationMembership", b =>
                {
                    b.HasOne("OptimaJet.DWKit.StarterApplication.Models.Organization", "Organization")
                        .WithMany("OrganizationMemberships")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("OptimaJet.DWKit.StarterApplication.Models.User", "User")
                        .WithMany("OrganizationMemberships")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OptimaJet.DWKit.StarterApplication.Models.Project", b =>
                {
                    b.HasOne("OptimaJet.DWKit.StarterApplication.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("OptimaJet.DWKit.StarterApplication.Models.Organization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("OptimaJet.DWKit.StarterApplication.Models.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
