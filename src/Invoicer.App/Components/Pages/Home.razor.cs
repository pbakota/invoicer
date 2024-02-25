
using Invoicer.App.Constants;
using Invoicer.App.Resources;
using Invoicer.App.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Invoicer.App.Components.Pages;

public partial class Home : ComponentBase
{
    class DataItem
    {
        public string Quarter { get; set; } = null!;
        public double Revenue { get; set; }
    }

    [Inject] private IStringLocalizer<I18N> Loc { get; set; } = null!;
    [Inject] private IDashboardService DashboardService { get; set; } = null!;
    private int _numberOfPartners;
    private int _numberOfArticles;
    private int _numberOfInvoices;
    private IList<QuarterlyData> _revenue = null!;
    private bool _loading = true;

    protected async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var result = await DashboardService.GetPlainStats();

        _numberOfArticles = result.NumberOfArticles;
        _numberOfPartners = result.NumberOfPartners;
        _numberOfInvoices = result.NumberOfInvoices;

        _revenue = await DashboardService.GetRevenue();        
        _loading = false;
    }

    private List<QuarterlyData> GetRevenue(int year)
        => _revenue is null ? [] : _revenue.Where(x => x.Year == year).ToList();

    private string FormatAsRSD(object value)
        => $"{(double)value:N2} {AppConstants.VALUTA}";
}