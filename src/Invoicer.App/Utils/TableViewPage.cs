using System.Diagnostics.CodeAnalysis;

using Invoicer.App.Resources;
using Invoicer.App.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;

using Radzen;

using Radzen.Blazor;

namespace Invoicer.App.Utils;

public abstract class TableViewPage<TData> : ComponentBase where TData : class
{
    [Inject] protected IStringLocalizer<I18N> Loc { get; set; } = null!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = null!;
    [Inject] protected DialogService DialogService { get; set; } = null!;
    [Inject] protected IAppNotificationService NotificationService { get; set; } = null!;

    protected RadzenDataGrid<TData>? _grid { get; set; }
    protected int _count { get; set; }
    protected bool _isLoading { get; set; }
    [NotNull]
    protected IList<TData> _items = null!;

    protected override async Task OnInitializedAsync() => await LoadData(new LoadDataArgs());

    protected abstract Task LoadData(LoadDataArgs args);

    protected virtual void CreateButtonClick(MouseEventArgs e) {}

    protected virtual void EditButtonClick(MouseEventArgs e, int id) {}

    protected virtual void DeleteButtonClick(MouseEventArgs e, int id) {}

    protected async Task<bool> ConfirmDelete(string text = "Are you sure?")
    {
        return await DialogService.Confirm(Loc[text], Loc["Delete entry"], new ConfirmOptions()
        {
            OkButtonText = "Yes", CancelButtonText = "No"
        }) == true;
    }
}