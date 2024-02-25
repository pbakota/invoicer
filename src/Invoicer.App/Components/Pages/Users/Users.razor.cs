using Invoicer.App.Extensions;
using Invoicer.App.Services;
using Invoicer.App.Utils;
using Invoicer.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using Radzen;

namespace Invoicer.App.Components.Pages.Users;

public partial class Users : TableViewPage<ApplicationUser>
{
    [Inject] private IUserService UserService { get; set; } = null!;

    protected async override Task LoadData(LoadDataArgs args)
    {
        UserService.Pageable.FromLoadArgs(args);
        
        var result = await UserService.GetPagedData();

        _items = result.Content.ToList();
        _count = result.Total;
    }

    private async Task OnSearch(string? text)
    {
        UserService.Pageable.SearchTerm = text;
        
        var result = await UserService.GetPagedData();
        _items = result.Content.ToList();
        _count = result.Total;
    }

    protected override void CreateButtonClick(MouseEventArgs e)
        => NavigationManager.NavigateTo("/users/new");

    protected void EditButtonClick(MouseEventArgs e, string id)
        => NavigationManager.NavigateTo($"/users/edit/{id}");

    protected async void DeleteButtonClick(MouseEventArgs e, string id)
    {
        if (await ConfirmDelete())
        {
            await UserService.DeleteUser(id);
            NotificationService.Success(Loc["Deleted"]);
            NavigationManager.NavigateTo("/users", forceLoad: true);
        }
    }
}