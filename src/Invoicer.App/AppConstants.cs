namespace Invoicer.App.Constants;

public sealed class AppConstants
{
#if ELECTRON_APP
    public const bool IsElectronApp = true;
#else
    public const bool IsElectronApp = false;
#endif
    
    public const string VALUTA = "RSD";
    public const string DATE_FORMAT = "dd/MM/yyyy";
    public const string DATETIME_FORMAT = "yyyy-MM-dd HH:mm";
}