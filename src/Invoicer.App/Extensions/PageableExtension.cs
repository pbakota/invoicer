using Invoicer.Data.Utils;

using Radzen;

namespace Invoicer.App.Extensions;

public static class PageableExtension
{
    public static void FromLoadArgs(this Pageable pageable, LoadDataArgs args)
    {
        if (args.Top is not null)
        {
            pageable.PageSize = (int)args.Top;
        }
        if (args.Skip is not null)
        {
            pageable.Page = (int)(args.Skip! / pageable.PageSize);
        }
        if (!string.IsNullOrEmpty(args.OrderBy))
        {
            pageable.OrderBy = args.OrderBy;
        }
    }
}