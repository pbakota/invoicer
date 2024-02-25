#if ELECTRON_APP
using ElectronNET.API;
using ElectronNET.API.Entities;

using Invoicer.App.Constants;
using Invoicer.App.Resources;
using Invoicer.App.Services;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
#endif

using Microsoft.AspNetCore.Components;

namespace Invoicer.App.Components.Pages.Maintenance;

#if !ELECTRON_APP
    public partial class RestoreDatabase: ComponentBase { }
#else

public partial class RestoreDatabase: ComponentBase
{
    [Inject] private IStringLocalizer<I18N> Loc { get; set; } = null!;
    
    [Inject] private ISettingsService Settings { get; set; } = null!;

    private async Task RestoreClick(MouseEventArgs e)
    {
        var parent = Electron.WindowManager.BrowserWindows.First();
        var backupFile =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/backup-{DateTime.Today.ToString(AppConstants.DATE_FORMAT)}.db";
        var dataFile = Settings.GetCurrentDatabasePath();
        var files = await Electron.Dialog.ShowOpenDialogAsync(parent, new OpenDialogOptions
        {
            Title = Loc["Restore.Open.Title"],
            Filters = [
                new FileFilter { Name = "Data Files", Extensions = ["db"] },
                new FileFilter { Name = "All Files", Extensions= ["*"] }
            ],
            DefaultPath = backupFile,
            Properties = [ OpenDialogProperty.openFile ]
        });
        if (files.Length == 0) return;

        var src = files[0];
        try
        {   
            await Settings.Restore(src);
        }
        catch (Exception ex)
        {
            await Electron.Dialog.ShowMessageBoxAsync(parent, new MessageBoxOptions(ex.Message));
        }
    }
}


#endif