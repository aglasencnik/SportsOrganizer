﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SportsOrganizer.MySqlMigrations;

#nullable disable

namespace SportsOrganizer.MySqlMigrations.Migrations
{
    [DbContext(typeof(MySqlDbContextFactory))]
    [Migration("20230411154217_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SportsOrganizer.Data.Models.ActivityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ActivityNumber")
                        .HasColumnType("int");

                    b.Property<int>("ActivityType")
                        .HasColumnType("int");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("NumberOfPlayers")
                        .HasColumnType("int");

                    b.Property<int>("OrderType")
                        .HasColumnType("int");

                    b.Property<string>("Props")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Rules")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("SportsOrganizer.Data.Models.ActivityResultModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ActivityId")
                        .HasColumnType("int");

                    b.Property<double>("Result")
                        .HasColumnType("double");

                    b.Property<int>("TeamId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("TeamId");

                    b.ToTable("ActivityResults");
                });

            modelBuilder.Entity("SportsOrganizer.Data.Models.PlayerResultModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ActivityResultId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("ActivityResultId1")
                        .HasColumnType("int");

                    b.Property<double>("Result")
                        .HasColumnType("double");

                    b.HasKey("Id");

                    b.HasIndex("ActivityResultId1");

                    b.ToTable("PlayerResults");
                });

            modelBuilder.Entity("SportsOrganizer.Data.Models.TeamModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("SportsOrganizer.Data.Models.UserActivityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ActivityId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("UserId");

                    b.ToTable("UserActivities");
                });

            modelBuilder.Entity("SportsOrganizer.Data.Models.UserModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SportsOrganizer.Data.Models.ActivityResultModel", b =>
                {
                    b.HasOne("SportsOrganizer.Data.Models.ActivityModel", "Activity")
                        .WithMany()
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SportsOrganizer.Data.Models.TeamModel", "Team")
                        .WithMany("ActivityResults")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Activity");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("SportsOrganizer.Data.Models.PlayerResultModel", b =>
                {
                    b.HasOne("SportsOrganizer.Data.Models.ActivityResultModel", "ActivityResult")
                        .WithMany("PlayerResults")
                        .HasForeignKey("ActivityResultId1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ActivityResult");
                });

            modelBuilder.Entity("SportsOrganizer.Data.Models.UserActivityModel", b =>
                {
                    b.HasOne("SportsOrganizer.Data.Models.ActivityModel", "Activity")
                        .WithMany("UserActivities")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SportsOrganizer.Data.Models.UserModel", "User")
                        .WithMany("UserActivities")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Activity");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SportsOrganizer.Data.Models.ActivityModel", b =>
                {
                    b.Navigation("UserActivities");
                });

            modelBuilder.Entity("SportsOrganizer.Data.Models.ActivityResultModel", b =>
                {
                    b.Navigation("PlayerResults");
                });

            modelBuilder.Entity("SportsOrganizer.Data.Models.TeamModel", b =>
                {
                    b.Navigation("ActivityResults");
                });

            modelBuilder.Entity("SportsOrganizer.Data.Models.UserModel", b =>
                {
                    b.Navigation("UserActivities");
                });
#pragma warning restore 612, 618
        }
    }
}
