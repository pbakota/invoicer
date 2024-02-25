﻿// <auto-generated />
using System;
using Invoicer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Invoicer.Postgres.Migrations
{
    [DbContext(typeof(InvoicerContext))]
    [Migration("20240222153123_MIG_000")]
    partial class MIG_000
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Invoicer.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Invoicer.Models.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<int>("TaxId")
                        .HasColumnType("integer");

                    b.Property<string>("UOM")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("TaxId");

                    b.ToTable("Articles", (string)null);
                });

            modelBuilder.Entity("Invoicer.Models.Invoice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateOfIssue")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateOfTraffic")
                        .HasColumnType("timestamp with time zone");

                    b.Property<float>("InvoiceSum")
                        .HasColumnType("real");

                    b.Property<string>("InvoiceType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("NumberId")
                        .HasColumnType("integer");

                    b.Property<int>("PartnerId")
                        .HasColumnType("integer");

                    b.Property<string>("PlaceOfIssue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PlaceOfTraffic")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("PrepaymentId")
                        .HasColumnType("integer");

                    b.Property<float?>("PrepaymentSum")
                        .HasColumnType("real");

                    b.Property<bool>("Storno")
                        .HasColumnType("boolean");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.Property<string>("TypeOfPayment")
                        .HasColumnType("text");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Number")
                        .IsUnique();

                    b.HasIndex("PartnerId");

                    b.HasIndex("PrepaymentId");

                    b.HasIndex("NumberId", "InvoiceType")
                        .IsUnique();

                    b.ToTable("Invoices", (string)null);
                });

            modelBuilder.Entity("Invoicer.Models.InvoiceItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ArticleId")
                        .HasColumnType("integer");

                    b.Property<int>("InvoiceId")
                        .HasColumnType("integer");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<float>("Qty")
                        .HasColumnType("real");

                    b.Property<float>("Rabat")
                        .HasColumnType("real");

                    b.Property<float>("TaxRate")
                        .HasColumnType("real");

                    b.Property<string>("UOM")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.HasIndex("InvoiceId");

                    b.ToTable("Invoice_Items", (string)null);
                });

            modelBuilder.Entity("Invoicer.Models.Partner", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("BankAccount")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("BankName")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("City")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Email")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("MaticniBroj")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("PIB")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("PartnerType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("PostalCode")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("Partners", (string)null);
                });

            modelBuilder.Entity("Invoicer.Models.Settings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CodeForPayment")
                        .HasColumnType("integer");

                    b.Property<bool>("HideStornoInvoice")
                        .HasColumnType("boolean");

                    b.Property<string>("InvoiceInfoText")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("PlaceOfIssue")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("PlaceOfTraffic")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<bool>("PrintIPSQRCode")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Settings", (string)null);
                });

            modelBuilder.Entity("Invoicer.Models.Tax", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<string>("LongDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Rate")
                        .HasColumnType("real");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Taxes", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "ProviderKey");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("Invoicer.Models.Article", b =>
                {
                    b.HasOne("Invoicer.Models.Tax", "Tax")
                        .WithMany()
                        .HasForeignKey("TaxId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Tax");
                });

            modelBuilder.Entity("Invoicer.Models.Invoice", b =>
                {
                    b.HasOne("Invoicer.Models.Partner", "Partner")
                        .WithMany()
                        .HasForeignKey("PartnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Invoicer.Models.Invoice", "Prepayment")
                        .WithMany()
                        .HasForeignKey("PrepaymentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Partner");

                    b.Navigation("Prepayment");
                });

            modelBuilder.Entity("Invoicer.Models.InvoiceItem", b =>
                {
                    b.HasOne("Invoicer.Models.Article", "Article")
                        .WithMany()
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Invoicer.Models.Invoice", "Invoice")
                        .WithMany("Items")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Article");

                    b.Navigation("Invoice");
                });

            modelBuilder.Entity("Invoicer.Models.Settings", b =>
                {
                    b.OwnsOne("Invoicer.Models.Company", "Company", b1 =>
                        {
                            b1.Property<int>("SettingsId")
                                .HasColumnType("integer");

                            b1.Property<string>("Address")
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)");

                            b1.Property<string>("BankAccount")
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)");

                            b1.Property<string>("BankName")
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)");

                            b1.Property<string>("Email")
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)");

                            b1.Property<string>("MaticniBroj")
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)");

                            b1.Property<string>("PIB")
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)");

                            b1.Property<string>("Phone")
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)");

                            b1.Property<string>("PostalCode")
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)");

                            b1.Property<string>("SiteURL")
                                .HasMaxLength(200)
                                .HasColumnType("character varying(200)");

                            b1.HasKey("SettingsId");

                            b1.ToTable("Settings");

                            b1.WithOwner()
                                .HasForeignKey("SettingsId");
                        });

                    b.Navigation("Company")
                        .IsRequired();
                });

            modelBuilder.Entity("Invoicer.Models.Invoice", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}