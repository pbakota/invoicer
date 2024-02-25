using Invoicer.App.Resources;
using Invoicer.App.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

using Radzen.Blazor;

namespace Invoicer.App.Components.Pages.Settings;

public partial class SettingsEdit : ComponentBase
{
    [Inject] private IStringLocalizer<I18N> Loc { get; set; } = null!;
    [Inject] private ISettingsService SettingsService { get;set;} = null!;
    [Inject] private IAppNotificationService NotificationService { get; set; } = null!;
 
    private RadzenTemplateForm<Models.Settings> _form = null!;
    private Models.Settings _model = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _model = await SettingsService.LoadSettings();
    }

    private void SaveClick()
    {
        SettingsService.SaveSettings(_model);
        NotificationService.Success(Loc["Saved"]);
    }
}