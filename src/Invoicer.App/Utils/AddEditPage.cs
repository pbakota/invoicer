using Invoicer.App.Resources;
using Invoicer.App.Services;
using Invoicer.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Invoicer.App.Utils;

public abstract class AddEditPage<TData> : ComponentBase where TData : IEntity
{
    [Inject] protected IStringLocalizer<I18N> Loc { get; set; } = null!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = null!;
    [Inject] protected IAppNotificationService NotificationService { get; set; } = null!;
    [CascadingParameter] protected string? TopRowPageTitle { get; set; }

    [Parameter]
    public int? _id { get; set; }
    protected bool _editMode { get; set; } = false;
    protected TData _model { get; set; } = default!;

    protected override Task OnInitializedAsync()
    {
        _editMode = _id is not null;
        return Task.CompletedTask;
    }

    protected virtual void OnInvalidSubmit()
        => NotificationService.Error(Loc["Form has errors!"]);

    protected abstract Task SaveClick();

    protected abstract void CancelClick();
}