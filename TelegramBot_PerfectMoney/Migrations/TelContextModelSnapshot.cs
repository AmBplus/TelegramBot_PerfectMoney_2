﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TelegramBot_PerfectMoney.DataBase;

#nullable disable

namespace TelegramBot_PerfectMoney.Migrations
{
    [DbContext(typeof(TelContext))]
    partial class TelContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("TelegramBot_PerfectMoney.Model.BankCart", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("CartNumber")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("id");

                    b.HasIndex("UserId");

                    b.ToTable("BankCart");
                });

            modelBuilder.Entity("TelegramBot_PerfectMoney.Model.BotSetting", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("RuleText")
                        .HasColumnType("longtext");

                    b.Property<bool>("StopSelling")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("id");

                    b.ToTable("botSettings");

                    b.HasData(
                        new
                        {
                            id = 1L,
                            RuleText = "متنی وجود ندارد",
                            StopSelling = false
                        });
                });

            modelBuilder.Entity("TelegramBot_PerfectMoney.Model.RoleModel", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Role")
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("RoleModel");

                    b.HasData(
                        new
                        {
                            id = 1L,
                            CreationDate = new DateTime(2023, 6, 25, 22, 56, 55, 460, DateTimeKind.Local).AddTicks(6283),
                            Role = "Admin"
                        },
                        new
                        {
                            id = 2L,
                            CreationDate = new DateTime(2023, 6, 25, 22, 56, 55, 460, DateTimeKind.Local).AddTicks(6305),
                            Role = "Customer"
                        });
                });

            modelBuilder.Entity("TelegramBot_PerfectMoney.Model.UserModel", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ChatId")
                        .HasColumnType("longtext");

                    b.Property<string>("CodeId")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("LastName")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserNameTelegram")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.HasKey("id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            id = 1L,
                            Active = true,
                            CreationDate = new DateTime(2023, 6, 25, 22, 56, 55, 460, DateTimeKind.Local).AddTicks(9193),
                            PhoneNumber = "+989394059810",
                            RoleId = 1L
                        });
                });

            modelBuilder.Entity("TelegramBot_PerfectMoney.Model.BankCart", b =>
                {
                    b.HasOne("TelegramBot_PerfectMoney.Model.UserModel", "User")
                        .WithMany("BankAccountNumbers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("TelegramBot_PerfectMoney.Model.UserModel", b =>
                {
                    b.HasOne("TelegramBot_PerfectMoney.Model.RoleModel", "Roles")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Roles");
                });

            modelBuilder.Entity("TelegramBot_PerfectMoney.Model.RoleModel", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("TelegramBot_PerfectMoney.Model.UserModel", b =>
                {
                    b.Navigation("BankAccountNumbers");
                });
#pragma warning restore 612, 618
        }
    }
}
