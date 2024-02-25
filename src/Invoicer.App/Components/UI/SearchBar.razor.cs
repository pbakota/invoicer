using Invoicer.App.Resources;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;

namespace Invoicer.App.Components.UI;

public partial class SearchBar : ComponentBase
{
    [Inject] private IStringLocalizer<I18N> Loc { get; set; } = null!;
    [Parameter]
    public bool AddVisible { get; set; }
    [Parameter]
    public string? SearchPlaceholder { get; set; }
    [Parameter]
    public EventCallback<string?> OnSearch { get; set; }
    [Parameter]
    public EventCallback<MouseEventArgs> AddClick { get; set; }
    [Parameter]
    public string? AddText { get; set; }
}