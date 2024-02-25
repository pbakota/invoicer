using Bogus;

using Invoicer.Models;

using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invoicer.Data.Seeding;

public class Seeder(IDbContextFactory<
    InvoicerContext> contextFactory, 
    UserManager<ApplicationUser> userManager, 
    RoleManager<IdentityRole> roleManager,
    ILogger<Seeder> logger)
{
    private readonly IDbContextFactory<InvoicerContext> _contextFactory = contextFactory;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    private readonly ILogger<Seeder> _logger = logger;

    public async Task SeedDevelopment()
    {
        _logger.LogInformation("Seeding the database");

        await SeedUsers();
        await SeedSettings();
        await SeedTaxes();
        await SeedPartners();
        await SeedArticles();
        await SeedInvoices();

        _logger.LogInformation("Finished the database seeding");
    }

    public async Task SeedProduction()
    {
        _logger.LogInformation("Seeding the database");

        await using var db = await _contextFactory.CreateDbContextAsync();
        await db.Database.EnsureCreatedAsync();

        await SeedUsers();
        await SeedSettings();

        _logger.LogInformation("Finished the database seeding");
    }

    private async Task SeedUsers() 
    {
        if(!await _roleManager.Roles.AnyAsync())
        {
            var roles = new[] { "admin", "user" };
            foreach (var role in roles)
            {
                IdentityRole roleRole = new IdentityRole(role);
                var result = await _roleManager.CreateAsync(roleRole);
                if(!result.Succeeded) {
                    throw new Exception($"Unable to create role {string.Join(",", result.Errors.Select(x => x.Description))}");
                }
            }
        }

        if (!await _userManager.Users.AnyAsync())
        {
            var admin = new ApplicationUser {
                UserName = "admin@local",
                Email = "admin@local",
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(admin, "P@ssw0rd");
            await _userManager.AddToRoleAsync(admin, "admin");

            var user = new ApplicationUser {
                UserName = "user@local",
                Email = "user@local",
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(user, "P@ssw0rd");
            await _userManager.AddToRoleAsync(user, "user");
        }
    }

    private async Task SeedSettings()
    {
        using var db = _contextFactory.CreateDbContext();
        if (!await db.Settings.AnyAsync(x => x.Company.Name == "Acme corp"))
        {
            db.Add(new Models.Settings
            {
                Company = new Models.Company
                {
                    Name = "Acme corp",
                    Address = "4925 White River Way",
                    City = "Springville",
                    PostalCode = "84663",
                    PIB = "54491890",
                    MaticniBroj = "38082580",
                    Phone = "+381 24 555 7654",
                    Email = "office@acme-corp.com",
                    SiteURL = "https://acme-corp.com",
                    // NOTE: The bank account is validated by model97,
                    // therefore this example account number is not valid.
                    BankAccount = "99-00000000000999-99",
                    BankName = "Foobar bank",
                },
                PlaceOfIssue = "SUBOTICA",
                PlaceOfTraffic = "SUBOTICA",
                InvoiceInfoText = "Napomena o preskom oslobodjenju: /",
                PrintIPSQRCode = true,
                CodeForPayment = 221,
            });
            await db.SaveChangesAsync();
        }
    }

    private async Task SeedTaxes()
    {
        using var db = _contextFactory.CreateDbContext();
        if (!await db.Taxes.AnyAsync(x => x.Rate == 20.0f))
        {
            db.Taxes.Add(new Models.Tax
            {
                LongDescription = "Porez na promet 20%",
                ShortDescription = "20%",
                Active = true,
                Rate = 20.0f
            });
        }
        if (!db.Taxes.Any(x => x.Rate == 0.0f))
        {
            db.Taxes.Add(new Models.Tax
            {
                LongDescription = "Bez poreza",
                ShortDescription = "0%",
                Active = true,
                Rate = 0.0f
            });
        }
        await db.SaveChangesAsync();
    }

    private async Task SeedPartners()
    {
        using var db = _contextFactory.CreateDbContext();
        if (!await db.Partners.AnyAsync())
        {
            var partnerId = 1;
            var partner = new Faker<Partner>()
                .RuleFor(f => f.Name, u => u.Company.CompanyName())
                .RuleFor(f => f.Code, u => $"PAR{partnerId++:0000}")
                .RuleFor(f => f.Address, u => u.Address.StreetAddress())
                .RuleFor(f => f.City, u => u.Address.City())
                .RuleFor(f => f.PostalCode, u => u.Address.ZipCode())
                .RuleFor(f => f.PIB, u => string.Join("", u.Random.Digits(9)))
                .RuleFor(f => f.PartnerType, u => Models.PartnerType.CUSTOMER);

            var partners = partner.Generate(100);
            await db.Partners.AddRangeAsync(partners);
            await db.SaveChangesAsync();
        }
    }

    private async Task SeedArticles()
    {
        using var db = _contextFactory.CreateDbContext();
        if (!await db.Articles.AnyAsync())
        {
            var uoms = new string[] { "KG", "M", "PICE", "L" };
            var taxes = await db.Taxes.Select(f => f.Id).ToArrayAsync();
            var articleId = 1;
            var article = new Faker<Article>()
                .RuleFor(f => f.Name, u => u.Commerce.ProductName())
                .RuleFor(f => f.Code, u => $"ART{articleId++:0000}")
                .RuleFor(f => f.Price, u => (float)Convert.ToDouble(u.Commerce.Price()))
                .RuleFor(f => f.TaxId, u => u.PickRandom(taxes))
                .RuleFor(f => f.UOM, u => u.PickRandom(uoms));
            var articles = article.Generate(1600);
            await db.Articles.AddRangeAsync(articles);
            await db.SaveChangesAsync();
        }
    }

    private async Task SeedInvoices()
    {
        using var db = _contextFactory.CreateDbContext();
        if (!await db.Invoices.AnyAsync())
        {
            var articles = await db.Articles.Select(f => f.Id).ToArrayAsync();
            var taxes = await db.Taxes.Select(f => f.Rate).ToArrayAsync();
            var partners = await db.Partners.Select(f => f.Id).ToArrayAsync();

            var uoms = new string[] { "KG", "M", "PICE", "L" };
            var rabats = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 5.0f, 10.0f, 20.0f };

            var item = new Faker<InvoiceItem>()
                .RuleFor(f => f.ArticleId, u => u.PickRandom(articles))
                .RuleFor(f => f.TaxRate, u => u.PickRandom(taxes))
                .RuleFor(f => f.Qty, u => u.Random.Int(1, 5))
                .RuleFor(f => f.Price, u => (float)u.Random.Double(1000, 5000))
                .RuleFor(f => f.Rabat, u => u.PickRandom(rabats))
                .RuleFor(f => f.UOM, u => u.PickRandom(uoms));

            var typedInvoice = new Dictionary<Models.InvoiceType, int> { 
                    { InvoiceType.NORMAL, 1 },
                    { InvoiceType.PROFORMA, 1},
                    { InvoiceType.PREPAYMENT, 1}
            };

            for (var year = DateTime.Now.AddYears(-1).Year; year <= DateTime.Now.Year; ++year)
            {
                for (var i = 1; i <= 100; ++i)
                {
                    var items = item.Generate(Random.Shared.Next(1, 30));
                    var invoiceTypes = new Models.InvoiceType[] { Models.InvoiceType.NORMAL, Models.InvoiceType.PROFORMA, Models.InvoiceType.PREPAYMENT };
                    var invoiceType = invoiceTypes[Random.Shared.Next(0, invoiceTypes.Length)];
                    var issue = new DateTime(year, 1, 1).AddDays(Random.Shared.Next(0, year == DateTime.Now.Year ? DateTime.Now.DayOfYear : 365));

                    var invoiceId = typedInvoice[invoiceType]++;

                    var code = invoiceType switch
                    {
                        InvoiceType.NORMAL => "RO",
                        InvoiceType.PROFORMA => "PR",
                        InvoiceType.PREPAYMENT => "AV",
                        _ => ""
                    };

                    var number = $"{code}-{invoiceId:00000}/{year}";
                    var invoiceSum = items.Sum(c => Math.Round(c.Price / (1 + c.Rabat / 100) * (1 + c.TaxRate / 100) * c.Qty, 2));

                    var invoice = new Faker<Invoice>()
                        .RuleFor(f => f.Year, u => year)
                        .RuleFor(f => f.Number, u => number)
                        .RuleFor(f => f.NumberId, u => invoiceId++)
                        .RuleFor(f => f.PartnerId, u => u.PickRandom(partners))
                        .RuleFor(f => f.DateOfIssue, u => issue.ToUniversalTime())
                        .RuleFor(f => f.DateOfTraffic, u => issue.ToUniversalTime())
                        .RuleFor(f => f.PlaceOfIssue, u => u.Address.City())
                        .RuleFor(f => f.PlaceOfTraffic, (u, f) => f.PlaceOfIssue)
                        .RuleFor(f => f.InvoiceType, u => invoiceType)
                        .RuleFor(f => f.TypeOfPayment, u => u.PickRandom(new Models.TypeOfPayment[] { Models.TypeOfPayment.GOTOVINA, Models.TypeOfPayment.VIRMANSKI, Models.TypeOfPayment.KARTICA }))
                        .RuleFor(f => f.Text, u => u.Lorem.Text())
                        .RuleFor(f => f.InvoiceSum, u => (float)invoiceSum)
                        .RuleFor(f => f.Items, u => items);
                    var invoices = invoice.Generate(1);
                    await db.Invoices.AddRangeAsync(invoices);
                }
            }
            await db.SaveChangesAsync();
        }
    }
}