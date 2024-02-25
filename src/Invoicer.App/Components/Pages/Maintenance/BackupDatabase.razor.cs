
#if ELECTRON_APP
using ElectronNET.API;
using ElectronNET.API.Entities;

using Invoicer.App.Constants;
using Invoicer.App.Services;
using Invoicer.App.Resources;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
#endif


using Microsoft.AspNetCore.Components;

namespace Invoicer.App.Components.Pages.Maintenance;

#if !ELECTRON_APP
    public partial class BackupDatabase: ComponentBase {}    
#else


public partial class BackupDatabase : ComponentBase
{
    [Inject] private IStringLocalizer<I18N> Loc { get; set; } = null!;
    [Inject] private ISettingsService Settings { get; set; } = null!;

    private async Task BackupClick(MouseEventArgs e)
    {
        var parent = Electron.WindowManager.BrowserWindows.First();
        var backupFile =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/backup-{DateTime.Today.ToString(AppConstants.DATE_FORMAT)}.db";
        var dataFile = Settings.GetCurrentDatabasePath();
        var dest = await Electron.Dialog.ShowSaveDialogAsync(parent, new SaveDialogOptions
        {
            Title = Loc["Backup.Save.Title"],
            Filters = [
                new FileFilter { Name = "Data Files", Extensions = ["db"] },
                new FileFilter { Name = "All Files", Extensions= ["*"] }
            ],
            DefaultPath = backupFile,
        });
        if (string.IsNullOrEmpty(dest)) return;

        try
        {
            await Settings.Backup(dest);
        }
        catch (Exception ex)
        {
            await Electron.Dialog.ShowMessageBoxAsync(parent, new MessageBoxOptions(ex.Message));
        }
    }
}


#endif