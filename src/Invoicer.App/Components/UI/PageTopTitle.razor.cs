using Microsoft.AspNetCore.Components;

namespace Invoicer.App.Components.UI;

public partial class PageTopTitle: ComponentBase
{
    [Parameter]
    public string? Text { get; set; }
}