﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UserAuthAPI.Data;

#nullable disable

namespace UserAuthAPI.Migrations
{
    [DbContext(typeof(UserAuthContext))]
    partial class UserAuthContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("UserAuthAPI.Models.OTP", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Code")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("Useremail")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("OTP", (string)null);
                });

            modelBuilder.Entity("UserAuthAPI.Models.RecoveryPhrase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<byte[]>("Phrase")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<byte[]>("PhraseIV")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<byte[]>("PhraseKey")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("Useremail")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("Useremail")
                        .IsUnique();

                    b.ToTable("RecoveryPhrase", (string)null);
                });

            modelBuilder.Entity("UserAuthAPI.Models.SecurityQandA", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<byte[]>("Answer")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<byte[]>("Answerkey")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("SecurityQandA", (string)null);
                });

            modelBuilder.Entity("UserAuthAPI.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("OtherEmail")
                        .HasColumnType("longtext");

                    b.Property<byte[]>("Password")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<byte[]>("Passwordkey")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("UserAuthAPI.Models.OTP", b =>
                {
                    b.HasOne("UserAuthAPI.Models.User", "User")
                        .WithMany("Otp")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserAuthAPI.Models.RecoveryPhrase", b =>
                {
                    b.HasOne("UserAuthAPI.Models.User", "User")
                        .WithMany("Phrases")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserAuthAPI.Models.SecurityQandA", b =>
                {
                    b.HasOne("UserAuthAPI.Models.User", "User")
                        .WithMany("SecurityQandA")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserAuthAPI.Models.User", b =>
                {
                    b.Navigation("Otp");

                    b.Navigation("Phrases");

                    b.Navigation("SecurityQandA");
                });
#pragma warning restore 612, 618
        }
    }
}
