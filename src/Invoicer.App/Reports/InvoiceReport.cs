using Invoicer.App.Constants;
using Invoicer.App.Extensions;
using Invoicer.App.Resources;
using Invoicer.Models;

using Microsoft.Extensions.Localization;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using Svg.Skia;

using static Net.Codecrete.QrCodeGenerator.QrCode;

namespace Invoicer.App.Reports;

public interface IInvoiceReport
{
    IDocument CreateDocument(Models.Invoice invoice, Settings settings);
}

public class InvoiceReport(IStringLocalizer<I18N> loc) : IInvoiceReport
{
    internal readonly IStringLocalizer<I18N> Loc = loc;
    internal Models.Invoice Invoice = null!;
    internal Models.Settings Settings = null!;

    private const string DEFAULT_FONT = Fonts.TimesNewRoman;
    private const int FONT_SIZE_SM = 10;
    private const int FONT_SIZE_L = 14;
    private const int FONT_SIZE_XXL = 48;
    public IDocument CreateDocument(Models.Invoice invoice, Settings settings)
    {
        Invoice = invoice;
        Settings = settings;

        QuestPDF.Settings.EnableDebugging = true;
        return Document.Create(document => document.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(0.80f, Unit.Centimetre);
            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
            if (invoice.Storno)
            {
                page.Background()
                    .AlignCenter()
                    .AlignMiddle()
                    .Text(text =>
                    {
                        text.Span($"{Loc["Report.Text.Storno"]} {Loc["Report.Text.Storno"]}").FontSize(FONT_SIZE_XXL).FontColor(Colors.BlueGrey.Lighten5);
                        text.Span($"{Loc["Report.Text.Storno"]} {Loc["Report.Text.Storno"]}").FontSize(FONT_SIZE_XXL).FontColor(Colors.BlueGrey.Lighten5);
                        text.Span($"{Loc["Report.Text.Storno"]} {Loc["Report.Text.Storno"]}").FontSize(FONT_SIZE_XXL).FontColor(Colors.BlueGrey.Lighten5);
                        text.Span($"{Loc["Report.Text.Storno"]} {Loc["Report.Text.Storno"]}").FontSize(FONT_SIZE_XXL).FontColor(Colors.BlueGrey.Lighten5);
                        text.Span($"{Loc["Report.Text.Storno"]} {Loc["Report.Text.Storno"]}").FontSize(FONT_SIZE_XXL).FontColor(Colors.BlueGrey.Lighten5);
                        text.Span($"{Loc["Report.Text.Storno"]} {Loc["Report.Text.Storno"]}").FontSize(FONT_SIZE_XXL).FontColor(Colors.BlueGrey.Lighten5);
                        text.Span($"{Loc["Report.Text.Storno"]} {Loc["Report.Text.Storno"]}").FontSize(FONT_SIZE_XXL).FontColor(Colors.BlueGrey.Lighten5);
                        text.Span($"{Loc["Report.Text.Storno"]} {Loc["Report.Text.Storno"]}").FontSize(FONT_SIZE_XXL).FontColor(Colors.BlueGrey.Lighten5);
                        text.Span($"{Loc["Report.Text.Storno"]} {Loc["Report.Text.Storno"]}").FontSize(FONT_SIZE_XXL).FontColor(Colors.BlueGrey.Lighten5);
                        text.Span($"{Loc["Report.Text.Storno"]} {Loc["Report.Text.Storno"]}").FontSize(FONT_SIZE_XXL).FontColor(Colors.BlueGrey.Lighten5);
                    });
            }
        }));
    }

    private void ComposeHeader(IContainer container)
    {
        container.Height(20).BorderBottom(1).AlignRight().Text(text
            => text.Span($"{Loc["Report.Text.Date"]}: {DateTime.Now.ToString(AppConstants.DATE_FORMAT)}").FontFamily(DEFAULT_FONT));
    }

    private void ComposeFooter(IContainer container)
    {
        container.Height(20).BorderTop(1).Row(row =>
        {
            row.RelativeItem().Text(text => text.Span($"Invoicer OSE 1.0 (c){DateTime.Now.Year} - 5R Business Solutions").FontSize(FONT_SIZE_SM));
            row.RelativeItem().AlignRight().Text(text =>
            {
                text.Span($"{Loc["Report.Text.Page"]}: ").FontFamily(DEFAULT_FONT).FontSize(FONT_SIZE_SM);
                text.CurrentPageNumber().FontFamily(DEFAULT_FONT).FontSize(FONT_SIZE_SM);
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        var invoiceText = Invoice.InvoiceType switch
        {
            InvoiceType.NORMAL => Loc["Report.Text.Invoice.NORMAL"],
            InvoiceType.PROFORMA => Loc["Report.Text.Invoice.PROFORMA"],
            InvoiceType.PREPAYMENT => Loc["Report.Text.Invoice.PREPAYMENT"],
            _ => "",
        };
        container.DefaultTextStyle(style => style.FontFamily(DEFAULT_FONT).FontSize(FONT_SIZE_SM))
        .Column(column =>
        {
            column.Item().Row(row => row.RelativeItem().Component(new InvoicePartiesComponent(this)));
            column.Item().BorderBottom(1);
            column.Item().PaddingTop(10).PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Text(text => text.Span(invoiceText).FontSize(FONT_SIZE_L).Bold().Strikethrough(Invoice.Storno));
                row.RelativeItem().Text(text => text.Span(Invoice.Number).FontSize(FONT_SIZE_L).Bold().Strikethrough(Invoice.Storno));
            });
            column.Item().Row(row => row.RelativeItem().Component(new InvoiceHeaderComponent(this)));
            column.Item().PaddingBottom(2);
            column.Item().Row(row => row.RelativeItem().Component(new InvoiceBodyComponent(this)));
        });
    }

    internal class InvoiceHeaderComponent(InvoiceReport report) : IComponent
    {
        private readonly InvoiceReport _report = report;
        public void Compose(IContainer container)
        {
            container.DefaultTextStyle(style => style.FontFamily(DEFAULT_FONT).FontSize(FONT_SIZE_SM)).PaddingTop(2)
            .Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(text => text.Span($"{_report.Loc["Report.Text.Number"]}: ").Bold());
                    row.RelativeItem().Text(text => text.Span(_report.Invoice.Number));
                    row.RelativeItem().Text(text => text.Span($"{_report.Loc["Report.Text.DateOfIssue"]}: ").Bold());
                    row.RelativeItem().Text(text => text.Span(_report.Invoice.DateOfIssue.ToLocalTime().ToString(AppConstants.DATE_FORMAT)));
                    row.RelativeItem().Text(text => text.Span($"{_report.Loc["Report.Text.DateOfTraffic"]}: ").Bold());
                    row.RelativeItem().Text(text => text.Span(_report.Invoice.DateOfTraffic.ToLocalTime().ToString(AppConstants.DATE_FORMAT)));
                });
                column.Item().Row(row =>
                {
                    if(_report.Invoice.InvoiceType is not InvoiceType.PROFORMA) 
                    {
                        row.RelativeItem().Text(text => text.Span($"{_report.Loc["Report.Text.TypeOfPayment"]}: ").Bold());
                        row.RelativeItem().Text(text => text.Span(_report.Invoice.TypeOfPayment.ToString()));
                    }
                    else
                    {
                        row.RelativeItem().Text(text => text.Span(""));
                        row.RelativeItem().Text(text => text.Span(""));
                    }
                    row.RelativeItem().Text(text => text.Span($"{_report.Loc["Report.Text.PlaceOfIssue"]}: ").Bold());
                    row.RelativeItem().Text(text => text.Span(_report.Invoice.PlaceOfIssue));
                    if(_report.Invoice.InvoiceType is not InvoiceType.PROFORMA)
                    {
                        row.RelativeItem().Text(text => text.Span($"{_report.Loc["Report.Text.PlaceOfTraffic"]}: ").Bold());
                        row.RelativeItem().Text(text => text.Span(_report.Invoice.PlaceOfTraffic));
                    }
                    else
                    {
                        row.RelativeItem().Text(text => text.Span(""));
                        row.RelativeItem().Text(text => text.Span(""));
                    }
                });
            });
        }
    }

    internal class InvoicePartiesComponent(InvoiceReport report) : IComponent
    {
        private readonly InvoiceReport _report = report;
        public void Compose(IContainer container)
        {
            container.DefaultTextStyle(style => style.FontFamily(DEFAULT_FONT).FontSize(FONT_SIZE_SM))
            .PaddingVertical(5).Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Component(new InvoiceCompanyComponent(_report));
                    row.RelativeItem().Component(new InvoicePartnerComponent(_report));
                });
            });
        }
    }

    internal class InvoicePartnerComponent(InvoiceReport report) : IComponent
    {
        private readonly InvoiceReport _report = report;
        public void Compose(IContainer container)
        {
            container.DefaultTextStyle(style => style.FontFamily(DEFAULT_FONT).FontSize(FONT_SIZE_SM)).Border(1).PaddingLeft(5)
            .Column(column =>
            {
                column.Item().Row(row => row.RelativeItem().Text(text => text.Span(_report.Invoice.Partner.Name).FontSize(FONT_SIZE_L).Bold()));
                column.Item().Row(row => row.RelativeItem().Text(text => text.Span($"{_report.Invoice.Partner.Address}, {_report.Invoice.Partner.City}, {_report.Invoice.Partner.PostalCode}").Bold()));
                if (_report.Invoice.Partner.MaticniBroj is not null)
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text(text => text.Span($"{_report.Loc["Report.Text.MaticniBroj"]}: "));
                        row.RelativeItem(2).Text(text => text.Span(_report.Invoice.Partner.MaticniBroj));
                    });
                }
                if (_report.Invoice.Partner.PIB is not null)
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text(text => text.Span($"{_report.Loc["Report.Text.PIB"]}: "));
                        row.RelativeItem(2).Text(text => text.Span(_report.Invoice.Partner.PIB));
                    });
                }
            });
        }
    }

    internal class InvoiceCompanyComponent(InvoiceReport report) : IComponent
    {
        private readonly InvoiceReport _report = report;
        public void Compose(IContainer container)
        {
            var company = _report.Settings.Company;

            container.DefaultTextStyle(style => style.FontFamily(DEFAULT_FONT).FontSize(FONT_SIZE_SM))
            .PaddingVertical(0).Column(column =>
            {
                column.Item().Row(row => row.RelativeItem().Text(text => text.Span(company.Name).FontSize(FONT_SIZE_L).Bold()));
                column.Item().Row(row => row.RelativeItem(2).Text(text => text.Span($"{company.Address}, {company.City} {company.PostalCode}").Bold()));
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(text => text.Span($"{_report.Loc["Report.Text.MaticniBroj"]}: "));
                    row.RelativeItem(2).Text(text => text.Span(company.MaticniBroj));
                });
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(text => text.Span($"{_report.Loc["Report.Text.PIB"]}: "));
                    row.RelativeItem(2).Text(text => text.Span(company.PIB));
                });
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(text => text.Span($"{_report.Loc["Report.Text.Phone"]}: "));
                    row.RelativeItem(2).Text(text => text.Span(company.Phone));
                });
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(text => text.Span($"{_report.Loc["Report.Text.Email"]}: "));
                    row.RelativeItem(2).Text(text => text.Span(company.Email));
                });
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(text => text.Span($"{_report.Loc["Report.Text.SiteURL"]}: "));
                    row.RelativeItem(2).Text(text => text.Span(company.SiteURL));
                });
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(text => text.Span($"{_report.Loc["Report.Text.Bank"]}: "));
                    row.RelativeItem(2).Text(text => text.Span($"{company.BankAccount} {company.BankName}"));
                });
            });
        }
    }

    internal class InvoiceBodyComponent(InvoiceReport report) : IComponent
    {
        private readonly InvoiceReport _report = report;
        public void Compose(IContainer container)
        {
            container.DefaultTextStyle(style => style.FontFamily(DEFAULT_FONT).FontSize(FONT_SIZE_SM))
            .Column(column =>
            {
                column.Item().Row(row => row.RelativeItem().Component(new InvoiceBodyContentComponent(_report)));
                column.Item().Row(row => row.RelativeItem().Component(new InvoiceBodyFooterComponent(_report)));
            });
        }
    }

    internal class InvoiceBodyContentComponent(InvoiceReport report) : IComponent
    {
        private readonly InvoiceReport _report = report;
        public void Compose(IContainer container)
        {
            container.DefaultTextStyle(style => style.FontFamily(DEFAULT_FONT).FontSize(FONT_SIZE_SM))
            .Column(column =>
            {
                column.Item().BorderTop(1).BorderBottom(1).Padding(1).Row(row =>
                {
                    row.ConstantItem(20).Text(text => text.Span(_report.Loc["Report.Text.No"]));
                    row.ConstantItem(60 + 155).Text(text => text.Span(_report.Loc["Report.Text.Name"]));
                    row.ConstantItem(25).Text(text => text.Span(_report.Loc["Report.Text.UOM"]));
                    row.ConstantItem(32).AlignRight().Text(text => text.Span(_report.Loc["Report.Text.Qty"]));
                    row.ConstantItem(60).AlignRight().Text(text => text.Span(_report.Loc["Report.Text.Price"]));
                    row.ConstantItem(35).AlignRight().Text(text => text.Span(_report.Loc["Report.Text.PDVAmount"]));
                    row.ConstantItem(30).AlignRight().Text(text => text.Span(_report.Loc["Report.Text.Rabat"]));
                    row.ConstantItem(60).AlignRight().Text(text => text.Span(_report.Loc["Report.Text.BaseAmount"]));
                    row.RelativeItem().AlignRight().Text(text => text.Span(_report.Loc["Report.Text.Amount"]));
                });

                var no = 1; var total = 0.0d; var sum = 0.0d; var taxes = new Dictionary<float, List<InvoiceItem>>();
                foreach (var inv in _report.Invoice.Items)
                {
                    total += PriceHelper.CalculateBase(inv);
                    sum += PriceHelper.CalculateAmount(inv);
                    column.Item().Row(row =>
                    {
                        row.ConstantItem(20)/*.DebugArea()*/.Text(text => text.Span($"{no++}"));
                        row.ConstantItem(60 + 155)/*.DebugArea()*/.Text(text => text.Span(inv.Article.Name));
                        row.ConstantItem(25)/*.DebugArea()*/.Text(text => text.Span(inv.UOM));
                        row.ConstantItem(32)/*.DebugArea()*/.AlignRight().Text(text => text.Span(string.Format("{0:N2}", inv.Qty)));
                        row.ConstantItem(60)/*.DebugArea()*/.AlignRight().Text(text => text.Span(string.Format("{0:N2}", inv.Price)));
                        row.ConstantItem(35)/*.DebugArea()*/.AlignRight().Text(text => text.Span(string.Format("{0:N2}", inv.TaxRate)));
                        row.ConstantItem(30)/*.DebugArea()*/.AlignRight().Text(text => text.Span(string.Format("{0:N2}", inv.Rabat)));
                        row.ConstantItem(60)/*.DebugArea()*/.AlignRight().Text(text => text.Span(string.Format("{0:N2}", inv.Price * inv.Qty)));
                        row.RelativeItem()/*.DebugArea()*/.AlignRight().Text(text => text.Span(string.Format("{0:N2}", PriceHelper.CalculateAmount(inv))));
                    });

                    if (!taxes.ContainsKey(inv.TaxRate))
                    {
                        taxes.Add(inv.TaxRate, new());
                    }
                    taxes[inv.TaxRate].Add(inv);
                }

                column.Item().BorderBottom(1);

                column.Item().Row(row =>
                {
                    row.RelativeItem(2).AlignRight().Text(text => text.Span($"{_report.Loc["Report.Text.Total"]}:").FontSize(FONT_SIZE_L));
                    row.RelativeItem().AlignRight().Text(text => text.Span(string.Format("{0:N2}", total)).FontSize(FONT_SIZE_L));
                });

                foreach (var tax in taxes.Keys)
                {
                    if (tax == 0.0f) continue;
                    column.Item().Row(row =>
                    {
                        row.RelativeItem(2).AlignRight().Text(text => text.Span($"{_report.Loc["Report.Text.Tax"]} {tax}%: ").FontSize(FONT_SIZE_L));
                        row.RelativeItem().AlignRight().Text(text => text.Span(string.Format("{0:N2}",
                            taxes[tax].Sum(x => x.Qty * x.Price * (x.TaxRate / 100)))).FontSize(FONT_SIZE_L));
                    });
                };
                column.Item().PaddingLeft(200).BorderBottom(1);

                if(_report.Invoice.PrepaymentSum is not null)
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem(2).AlignRight().Text(text => text.Span($"{_report.Loc["Report.Text.Prepayed"]}:").FontSize(FONT_SIZE_L));
                        row.RelativeItem().AlignRight().Text(text => text.Span(string.Format("{0:N2}", _report.Invoice.PrepaymentSum)).FontSize(FONT_SIZE_L));
                    });
                }

                column.Item().BorderTop(1).Row(row =>
                {
                    row.RelativeItem(2).AlignRight().Text(text => text.Span($"{_report.Loc["Report.Text.SumToBePaid"]}:").FontSize(FONT_SIZE_L).Bold());
                    row.RelativeItem().AlignRight().Text(text => text.Span(string.Format("{0:N2}", sum - (_report.Invoice.PrepaymentSum ?? 0.0d))).FontSize(FONT_SIZE_L).Bold());
                });
            });
        }
    }

    internal class InvoiceBodyFooterComponent(InvoiceReport report) : IComponent
    {
        private readonly InvoiceReport _report = report;
        public void Compose(IContainer container)
        {
            container.DefaultTextStyle(style => style.FontFamily(DEFAULT_FONT).FontSize(FONT_SIZE_SM))
            .Column(column =>
            {
                column.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Row(row => row.RelativeItem().Text(text =>
                            {
                                text.Span($"{_report.Loc["Report.Text.WithLetters"]} :");
                                text.Span(PriceHelper.AmountToString(_report.Invoice.InvoiceSum)).Bold();
                            }));
                        column.Item().PaddingTop(10).Row(row => row.RelativeItem().Text(text => text.Span(_report.Invoice.Text)));
                    });
                    row.ConstantItem(80).Row(row => row.RelativeItem().Component(new IPSQRComponent(_report)));
                });
                column.Item().PaddingTop(50).Row(row =>
                {
                    row.RelativeItem().PaddingLeft(30).PaddingRight(30).AlignCenter().Text(text => text.Span(_report.Loc["Report.Text.Counter"]));
                    row.RelativeItem().PaddingLeft(30).PaddingRight(30).AlignCenter().Text(text => text.Span(_report.Loc["Report.Text.CUSTOMER"]));
                });
                column.Item().PaddingTop(20).Row(row =>
                {
                    row.RelativeItem().PaddingLeft(30).PaddingRight(30).PaddingTop(20).BorderBottom(1);
                    row.RelativeItem().PaddingLeft(30).PaddingRight(30).PaddingTop(20).BorderBottom(1);
                });
            });
        }
    }

    internal class IPSQRComponent(InvoiceReport report) : IComponent
    {
        public void Compose(IContainer container)
        {
            if (!report.Settings.PrintIPSQRCode || report.Invoice.Storno || report.Invoice.InvoiceType == InvoiceType.PROFORMA) return;

            var company = report.Settings.Company;
            var v = company.BankAccount.Split('-');
            var racun = $"{v[0]}{v[1].PadLeft(13, '0')}{v[2]}";

            // https://ips.nbs.rs/PDF/pdfPreporukeValidacijaLat.pdf
            var tags = new List<string>
            {
                "K:PR", // identifikacioni kôd (obavezan)
                "V:01", // verzija: 01 (obavezan)
                "C:1",  // znakovni skup: 1. UTF-8 (obavezan)
                $"R:{racun}", // broj računa primaoca plaćanja (obavezan) broj računa primaoca plaćanja 
                              //upisuje se isključivo kao niz od 18 cifara fiksno, bez crtica.
                $"N:{company.Name}\n{company.City} {company.PostalCode}",   // naziv primaoca plaćanja (obavezan)
                $"I:RSD{report.Invoice.InvoiceSum-(report.Invoice.PrepaymentSum ?? 0.0f):0.00}", // valuta i iznos novčanih sredstava (obavezan)
                $"SF:{report.Settings.CodeForPayment}", // šifra plaćanja (obavezan)
                $"RO:00{report.Invoice.Number.Replace('/', '-')}" // model i poziv na broj odobrenja primaoca plaćanja (opcioni)
            };

            var text = string.Join("|", tags);
            var qr = EncodeText(text, Ecc.Medium);
            var svgString = qr.ToSvgString(border: 4);

            container.Column(column =>
            {
                column.Item().Row(row => row.RelativeItem().PaddingTop(10).AlignCenter().Svg(SKSvg.CreateFromSvg(svgString), 1.5f));
                column.Item().Row(row => row.RelativeItem().AlignCenter().Text(text => text.Span("NBS IPS")));
            });
        }
    }

}