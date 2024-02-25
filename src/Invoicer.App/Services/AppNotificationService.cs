using Invoicer.App.Resources;

using Microsoft.Extensions.Localization;

using Radzen;

namespace Invoicer.App.Services;

public interface IAppNotificationService
{
    void Info(string message);
    void Warn(string message);
    void Success(string message);
    void Error(string message);
}

public class AppNotificationService(NotificationService notificationService,
    IStringLocalizer<I18N> localizer) : IAppNotificationService
{
    private readonly NotificationService _notificationService = notificationService;
    private readonly IStringLocalizer<I18N> _localizer = localizer;

    public void Info(string message)
        => _notificationService.Notify(NotificationSeverity.Info, _localizer["Info"], message);

    public void Warn(string message)
        => _notificationService.Notify(NotificationSeverity.Warning, _localizer["Warning"], message);

    public void Success(string message)
        => _notificationService.Notify(NotificationSeverity.Success, _localizer["Success"], message);

    public void Error(string message)
        => _notificationService.Notify(NotificationSeverity.Error, _localizer["Error"], message);
}