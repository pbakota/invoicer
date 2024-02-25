#if ELECTRON_APP
using ElectronNET.API;
using ElectronNET.API.Entities;

using Invoicer.App.Services;
#endif

namespace Invoicer.App.Configuration;

#if ELECTRON_APP
public static class ElectronMenu
{
    public static MenuItem[] Build(WebApplication app)
    {
        var menu = new MenuItem[]
        {
            new()
            {
                Label = "File",
                Submenu =
                [
                    new MenuItem { Label = "Quit", Click = Electron.App.Quit }
                ]
            },
            new()
            {
                Label = "Edit",
                Submenu =
                [
                    new MenuItem { Label = "Undo", Accelerator = "CmdOrCtrl+Z", Role = MenuRole.undo },
                    new MenuItem { Label = "Redo", Accelerator = "Shift+CmdOrCtrl+Z", Role = MenuRole.redo },
                    new MenuItem { Type = MenuType.separator },
                    new MenuItem { Label = "Cut", Accelerator = "CmdOrCtrl+X", Role = MenuRole.cut },
                    new MenuItem { Label = "Copy", Accelerator = "CmdOrCtrl+C", Role = MenuRole.copy },
                    new MenuItem { Label = "Paste", Accelerator = "CmdOrCtrl+V", Role = MenuRole.paste },
                    new MenuItem { Label = "Select All", Accelerator = "CmdOrCtrl+A", Role = MenuRole.selectall }
                ]
            },
            new()
            {
                Label = "View",
                Submenu =
                [
                    new MenuItem
                    {
                        Label = "Reload",
                        Accelerator = "CmdOrCtrl+R",
                        Click = () =>
                        {
                            // on reload, start fresh and close any old
                            // open secondary windows
                            Electron.WindowManager.BrowserWindows.ToList().ForEach(browserWindow =>
                            {
                                if (browserWindow.Id != 1)
                                {
                                    browserWindow.Close();
                                }
                                else
                                {
                                    browserWindow.Reload();
                                }
                            });
                        }
                    },
                    new MenuItem
                    {
                        Label = "Toggle Full Screen",
                        Accelerator = "F11", //"CmdOrCtrl+F",
                        Click = async () =>
                        {
                            bool isFullScreen = await Electron.WindowManager.BrowserWindows.First().IsFullScreenAsync();
                            Electron.WindowManager.BrowserWindows.First().SetFullScreen(!isFullScreen);
                        }
                    },
                    new MenuItem
                    {
                        Label = "Open Developer Tools",
                        Accelerator = "CmdOrCtrl+I",
                        Click = () => Electron.WindowManager.BrowserWindows.First().WebContents.OpenDevTools()
                    }
                ]
            },
            new()
            {
                Label = "Window",
                Role = MenuRole.window,
                Submenu =
                [
                    new MenuItem { Label = "Minimize", Accelerator = "CmdOrCtrl+M", Role = MenuRole.minimize },
                ]
            },
            new()
            {
                Label = "Help",
                Submenu =
                [
                    new MenuItem { Label = "About ...", Click = Electron.App.ShowAboutPanel, },
                    new MenuItem { Label = "Information", Click = () => ShowInformation(app) }
                ]
            }
        };
        return menu;
    }

    private static void ShowInformation(IHost app)
    {
        var settings = app.Services.GetRequiredService<ISettingsService>();

        var connectionString = settings.GetCurrentDatabasePath();
        var info = new List<string>
        {
            $"Platform:\n{Environment.OSVersion}",
            $"Command line:\n{Environment.CommandLine}",
            $"Connection String:\n{connectionString}"
        };

        Task.Run(async () =>
        {
            await Electron.Dialog.ShowMessageBoxAsync(string.Join("\n\n", info));
        });
    }
}
#endif
