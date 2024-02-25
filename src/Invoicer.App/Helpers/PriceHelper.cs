
using Invoicer.App.Constants;
using Invoicer.Models;

namespace Invoicer.App;

public sealed class PriceHelper
{
    public static double CalculateAmount(InvoiceItem c)
        => Math.Round(c.Price / (1 + c.Rabat / 100) * (1 + c.TaxRate / 100) * c.Qty, 2);

    public static double CalculateBase(InvoiceItem c)
        => Math.Round(c.Price / (1 + c.Rabat / 100) * c.Qty, 2);

    public static string ISOCurrencySymbol()
    {
        // var region = new RegionInfo(CultureInfo.CurrentCulture.TwoLetterISOLanguageName );
        // return region.ISOCurrencySymbol;
        return AppConstants.VALUTA;
    }

    private static readonly string[] AN1 = [
    "","jedan", "dva", "tri", "četri", "pet",
    "šest", "sedam", "osam", "devet", "deset",
    "jedanaest", "dvanest", "trinaest", "četrnaest", "petnaest",
    "šesnaest", "sedamnaest", "osamnaest", "devetnaest"];

    private static readonly string[] AN10 = [
    "","deset", "dvadeset", "trideset", "četrdeset", "pedeset",
    "šesdeset", "sedamdeset", "osamdeset", "devedeset"];

    private static readonly string[] AN100 = [
    "","sto", "dvesto", "tristo", "četristo", "petsto",
    "šeststo", "sedamsto", "osamsto", "devetsto" ];

    private static string P(int e)
    {
        var result = string.Empty;
        var d = e / 100;
        if (e >= 100)
        {
            result += AN100[d];
            e -= d * 100;
        }

        d = e / 10;
        if (d >= 2)
        {
            result += AN10[d];
            e -= d * 10;
        }

        if (e > 0)
        {
            result += AN1[e];
        }
        return result;
    }

    public static string AmountToString(float n)
    {
        var result = string.Empty;

        if (n <= 0) return result;

        var r = n-Math.Truncate(n);
        var f = (int)Math.Floor(n - r);

        int d = f / 1_000_000;
        if (d > 0)
        {
            result += d == 1 ? "milion" : P(d) + "miliona";
            f -= d * 1_000_000;
        }

        d = f / 1_000;
        if (d > 0)
        {
            var s = d switch
            {
                1 => "hiljadu",
                2 => "dvehiljade",
                _ => P(d) + "hiljade"
            };
            result += s;
            f -= d * 1_000;
        }

        result += P(f);

        if (r * 100 > 0)
        {
            result = result + " " + ((int)Math.Round((double)(r * 100))).ToString("D") + "/100.";
        }

        return result;
    }
}
