// Copyright (C) 2024 Peter Bakota
// 
// Invoicer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Invoicer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with Invoicer. If not, see <http://www.gnu.org/licenses/>.

using Invoicer.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Invoicer.Data;

public class InvoicerContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Settings> Settings { get; set; }
    public DbSet<Tax> Taxes { get; set; }
    public DbSet<Partner> Partners { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }

    public InvoicerContext()
    {
    }

    public InvoicerContext(DbContextOptions<InvoicerContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserRole<string>>().HasKey(f => new { f.UserId, f.RoleId });
        modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(f => new { f.UserId, f.LoginProvider, f.ProviderKey });
        modelBuilder.Entity<IdentityUserToken<string>>().HasKey(f => new { f.UserId, f.LoginProvider });

        modelBuilder.Entity<IdentityRole>(entity =>
        {
            entity.HasMany<ApplicationUser>().WithMany(p => p.Roles)
                .UsingEntity<IdentityUserRole<string>>(
                    r => r.HasOne<ApplicationUser>().WithMany().HasForeignKey("UserId"),
                    l => l.HasOne<IdentityRole>().WithMany().HasForeignKey("RoleId"),
                    j =>
                    {
                        // NOTE: The correct order of the keys are essential!!
                        // First "right" key id, then the "left" key id
                        j.HasKey("UserId", "RoleId");
                    });
        });

        modelBuilder.Entity<Tax>().ToTable("Taxes").Property(f => f.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Tax>().Property(f => f.LongDescription).IsRequired();
        modelBuilder.Entity<Tax>().Property(f => f.ShortDescription).IsRequired();
        modelBuilder.Entity<Tax>().Property(f => f.Rate).IsRequired();

        modelBuilder.Entity<Partner>().ToTable("Partners").Property(f => f.Id).ValueGeneratedOnAdd().IsRequired();
        modelBuilder.Entity<Partner>().Property(f => f.Code).HasMaxLength(200).IsRequired();
        modelBuilder.Entity<Partner>().HasIndex(f => f.Code).IsUnique();
        modelBuilder.Entity<Partner>().Property(f => f.Name).HasMaxLength(200).IsRequired();
        modelBuilder.Entity<Partner>().Property(f => f.Address).HasMaxLength(200);
        modelBuilder.Entity<Partner>().Property(f => f.City).HasMaxLength(200);
        modelBuilder.Entity<Partner>().Property(f => f.PostalCode).HasMaxLength(200);
        modelBuilder.Entity<Partner>().Property(f => f.PIB).HasMaxLength(200);
        modelBuilder.Entity<Partner>().Property(f => f.MaticniBroj).HasMaxLength(200);
        modelBuilder.Entity<Partner>().Property(f => f.Phone).HasMaxLength(200);
        modelBuilder.Entity<Partner>().Property(f => f.Email).HasMaxLength(200);
        modelBuilder.Entity<Partner>().Property(f => f.PartnerType).HasConversion<string>();
        modelBuilder.Entity<Partner>().Property(f => f.BankAccount).HasMaxLength(200);
        modelBuilder.Entity<Partner>().Property(f => f.BankName).HasMaxLength(200);

        modelBuilder.Entity<Article>().ToTable("Articles").Property(f => f.Id).ValueGeneratedOnAdd().IsRequired();
        modelBuilder.Entity<Article>().Property(f => f.Name).HasMaxLength(200).IsRequired();
        modelBuilder.Entity<Article>().Property(f => f.Code).HasMaxLength(200).IsRequired();
        modelBuilder.Entity<Article>().HasIndex(f => f.Code).IsUnique();
        modelBuilder.Entity<Article>().HasOne(f => f.Tax).WithMany().HasForeignKey(x => x.TaxId)
            .OnDelete(DeleteBehavior.Restrict).IsRequired();
        modelBuilder.Entity<Article>().Property(f => f.UOM).HasMaxLength(50);
        modelBuilder.Entity<Article>().Property(f => f.Price).IsRequired();

        modelBuilder.Entity<Invoice>().ToTable("Invoices").Property(f => f.Id).ValueGeneratedOnAdd().IsRequired();
        modelBuilder.Entity<Invoice>().Property(f => f.InvoiceType).HasConversion<string>().IsRequired();
        modelBuilder.Entity<Invoice>().Property(f => f.DateOfIssue).IsRequired();
        modelBuilder.Entity<Invoice>().Property(f => f.DateOfTraffic).IsRequired();
        modelBuilder.Entity<Invoice>().Property(f => f.PlaceOfIssue).IsRequired();
        modelBuilder.Entity<Invoice>().Property(f => f.PlaceOfTraffic).IsRequired();
        modelBuilder.Entity<Invoice>().Property(f => f.TypeOfPayment).HasConversion<string>();
        modelBuilder.Entity<Invoice>().HasOne(f => f.Partner).WithMany().HasForeignKey(x => x.PartnerId)
            .OnDelete(DeleteBehavior.Restrict).IsRequired();
        modelBuilder.Entity<Invoice>().Property(f => f.Storno).IsRequired();
        modelBuilder.Entity<Invoice>().Property(f => f.Year).IsRequired();
        modelBuilder.Entity<Invoice>().Property(f => f.Number).IsRequired();
        modelBuilder.Entity<Invoice>().HasIndex(f => f.Number).IsUnique();
        modelBuilder.Entity<Invoice>().Property(f => f.Number).HasMaxLength(200);
        modelBuilder.Entity<Invoice>().HasIndex(f => new { f.NumberId, f.InvoiceType }).IsUnique();
        modelBuilder.Entity<Invoice>().Property(f => f.NumberId).IsRequired();
        modelBuilder.Entity<Invoice>().HasMany(f => f.Items).WithOne(f => f.Invoice).OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        modelBuilder.Entity<Invoice>().HasOne(f => f.Prepayment).WithMany().HasForeignKey(x => x.PrepaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InvoiceItem>().ToTable("Invoice_Items").Property(f => f.Id).ValueGeneratedOnAdd()
            .IsRequired();
        modelBuilder.Entity<InvoiceItem>().Property(f => f.Qty).IsRequired();
        modelBuilder.Entity<InvoiceItem>().Property(f => f.Price).IsRequired();
        modelBuilder.Entity<InvoiceItem>().Property(f => f.UOM).HasMaxLength(50);
        modelBuilder.Entity<InvoiceItem>().Property(f => f.TaxRate).IsRequired();
        modelBuilder.Entity<InvoiceItem>().Property(f => f.Rabat).IsRequired();
        modelBuilder.Entity<InvoiceItem>().HasOne(f => f.Article).WithMany().HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Restrict).IsRequired();
        modelBuilder.Entity<InvoiceItem>().HasOne(f => f.Invoice).WithMany(f => f.Items).HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict).IsRequired();

        modelBuilder.Entity<Settings>().ToTable("Settings").Property(f => f.Id).ValueGeneratedOnAdd().IsRequired();
        modelBuilder.Entity<Settings>().OwnsOne(f => f.Company, c =>
        {
            c.Property(f => f.Name).HasMaxLength(200).IsRequired();
            c.Property(f => f.Address).HasMaxLength(200).IsRequired();
            c.Property(f => f.City).HasMaxLength(200).IsRequired();
            c.Property(f => f.PostalCode).HasMaxLength(200).IsRequired();
            c.Property(f => f.PIB).HasMaxLength(200).IsRequired();
            c.Property(f => f.MaticniBroj).HasMaxLength(200).IsRequired();
            c.Property(f => f.Phone).HasMaxLength(200).IsRequired();
            c.Property(f => f.Email).HasMaxLength(200).IsRequired();
            c.Property(f => f.SiteURL).HasMaxLength(200);
            c.Property(f => f.BankAccount).HasMaxLength(200).IsRequired();
            c.Property(f => f.BankName).HasMaxLength(200).IsRequired();
        });
        modelBuilder.Entity<Settings>().Property(f => f.PlaceOfIssue).HasMaxLength(200).IsRequired();
        modelBuilder.Entity<Settings>().Property(f => f.PlaceOfTraffic).HasMaxLength(200).IsRequired();
        modelBuilder.Entity<Settings>().Property(f => f.InvoiceInfoText).HasMaxLength(500);
    }
}