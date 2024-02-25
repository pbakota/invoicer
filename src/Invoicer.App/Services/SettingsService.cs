using Invoicer.Data.Dao;
using Invoicer.Models;

namespace Invoicer.App.Services;

public interface ISettingsService
{
    Task<Settings> LoadSettings();
    Task SaveSettings(Models.Settings settings);
    string GetCurrentDatabasePath();

    Task Backup(string dest);
    Task Restore(string from);
}

public class SettingsService(ISettingsRepository settingsRepository) : ISettingsService
{
    private readonly ISettingsRepository _settingsRepository = settingsRepository;

    public async Task<Settings> LoadSettings()
        => await _settingsRepository.FindFirst();

    public async Task SaveSettings(Settings settings)
    {
        var s = await _settingsRepository.FindFirst();
        s.Company = settings.Company;
        s.PlaceOfIssue = settings.PlaceOfIssue;
        s.PlaceOfTraffic = settings.PlaceOfTraffic;
        s.InvoiceInfoText = settings.InvoiceInfoText;
        s.PrintIPSQRCode = settings.PrintIPSQRCode;
        s.CodeForPayment = settings.CodeForPayment;
        s.HideStornoInvoice = settings.HideStornoInvoice;

        await _settingsRepository.UpdateById(s.Id, s);
    }

    public string GetCurrentDatabasePath()
        => _settingsRepository.GetCurrentDatabasePath();

    public async Task Backup(string dest)
        => await _settingsRepository.Backup(dest);

    public async Task Restore(string from) 
        => await _settingsRepository.Restore(from);
}
