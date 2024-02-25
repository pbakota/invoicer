using Invoicer.App.Resources;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;

namespace Invoicer.App.Components.UI;

public partial class SearchBox : ComponentBase
{
    [Inject] private IStringLocalizer<I18N> Loc { get; set; } = null!;
    
    [Parameter]
    public EventCallback<string?> OnSearch { get; set; }
    private string? _searchText = string.Empty;

    public async Task OnInput(ChangeEventArgs args)
    {
        var value = $"{args.Value}";
        value = value.Trim();

        if (value.Length >= 2 || value.Length == 0)
        {
            await OnSearch.InvokeAsync(value);
        }
    }

    public async Task OnChange(string? value)
    {
        value = value?.Trim();
        await OnSearch.InvokeAsync(value);
    }

    private async Task ClearButtonClick(MouseEventArgs e)
    {
        _searchText = string.Empty;
        await OnSearch.InvokeAsync(_searchText);
    }
}