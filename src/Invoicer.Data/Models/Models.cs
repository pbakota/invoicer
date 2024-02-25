namespace Invoicer.Models;

public interface IEntity
{
    public int Id { get; set; }
}

public record Tax : IEntity
{
    public int Id { get; set; }
    public string LongDescription { get; set; } = null!;
    public string ShortDescription { get; set; } = null!;
    public float Rate { get; set; }
    public bool Active { get; set; }
}

public enum PartnerType
{
    CUSTOMER
}

public record Partner : IEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? PIB { get; set; }
    public string? MaticniBroj { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public PartnerType PartnerType { get; set; }
    public string? BankAccount { get; set; }
    public string? BankName { get; set; }
}

public record Article : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int TaxId { get; set; }
    public Tax Tax { get; set; } = null!;
    public string? UOM { get; set; }
    public float Price { get; set; }
}

public enum InvoiceType
{
    NORMAL,
    PROFORMA,
    PREPAYMENT
}

public enum TypeOfPayment
{
    VIRMANSKI,
    GOTOVINA,
    KARTICA
}

public record Invoice : IEntity
{
    public int Id { get; set; }
    public InvoiceType InvoiceType { get; set; }
    public DateTime DateOfIssue { get; set; }
    public DateTime DateOfTraffic { get; set; }
    public string? PlaceOfIssue { get; set; } = null!;
    public string? PlaceOfTraffic { get; set; } = null!;
    public TypeOfPayment? TypeOfPayment { get; set; }
    public int PartnerId { get; set; }
    public Partner Partner { get; set; } = null!;
    public bool Storno { get; set; }
    public int Year { get; set; }
    public string Number { get; set; } = null!;
    public int NumberId { get; set; }
    public string? Text { get; set; }
    public float InvoiceSum { get; set; }
    public ICollection<InvoiceItem> Items { get; set; } = null!;
    public int? PrepaymentId { get; set; }
    public Invoice? Prepayment { get; set; }
    public float? PrepaymentSum { get; set; }
}

public record InvoiceItem : IEntity
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    public int ArticleId { get; set; }
    public Article Article { get; set; } = null!;
    public float Qty { get; set; }
    public float Price { get; set; }
    public string? UOM { get; set; }
    public float TaxRate { get; set; }
    public float Rabat { get; set; }
}

public record Company
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string PIB { get; set; } = null!;
    public string MaticniBroj { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? SiteURL { get; set; }
    public string BankAccount { get; set; } = null!;
    public string BankName { get; set; } = null!;
}

public record Settings : IEntity
{
    public int Id { get; set; }
    public Company Company { get; set; } = null!;
    public string PlaceOfIssue { get; set; } = null!;
    public string PlaceOfTraffic { get; set; } = null!;
    public string? InvoiceInfoText { get; set; }
    public bool PrintIPSQRCode { get; set; } = false;
    public int CodeForPayment { get; set; } = 221;
    public bool HideStornoInvoice { get; set; }
}