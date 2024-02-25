using Invoicer.App.Resources;
using Invoicer.App.Services;
using Invoicer.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

using Radzen.Blazor;

namespace Invoicer.App.Components.Pages.Users;

public partial class AddEditUser : ComponentBase
{
    [Inject] private IUserService UserService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IAppNotificationService NotificationService { get; set; } = null!;
    [Inject] private IStringLocalizer<I18N> Loc { get; set; } = null!;

    [Parameter]
    public string? _id { get; set; }
    [CascadingParameter]
    public string TopRowPageTitle { get; set; } = null!;
    private RadzenTemplateForm<ApplicationUser> _form = null!;
    private bool _editMode;
    private string? _password;
    private ApplicationUser _model = null!;
    private IEnumerable<string>? _selectedRoles;
    private IEnumerable<IdentityRole>? _roles;
    
    protected override async Task OnInitializedAsync()
    {
        _editMode = ! string.IsNullOrEmpty(_id);

        if (_editMode)
        {
            var user = await UserService.GetSingle(_id!);
            if (user is null)
            {
                NotificationService.Error(Loc["User not found"]);
                NavigationManager.NavigateTo("/users");
                return;
            }
            _model = await UserService.NewUser();
            _model.UserName = user.UserName;
            _model.Email = user.Email;
            _model.PhoneNumber = user.PhoneNumber;
            _model.Roles = user.Roles;
            TopRowPageTitle = Loc["User.Edit"];

            _selectedRoles = _model.Roles?.Select(x => x.Id).ToArray();
        }
        else
        {
            _model = await UserService.NewUser();
            TopRowPageTitle = Loc["User.New"];

            _selectedRoles = Array.Empty<string>();
        }

        _roles = await UserService.GetRoles();
    }

    private void CancelClick()
        => NavigationManager.NavigateTo("/users");

    private void OnInvalidSubmit()
        => NotificationService.Error(Loc["Form has errors!"]);

    private async Task SaveClick()
    {
        if (!_form.EditContext.Validate())
        {
            OnInvalidSubmit();
            return;
        }

        if (_editMode)
        {
            _model.Roles = _roles!.Where(x => _selectedRoles!.Contains(x.Id)).ToArray();
            var result = await UserService.SaveUser(_id!, _model, _password);
            if(result.Succeeded) 
            {
                NotificationService.Success(Loc["Text.Saved"]);
            }
            else
            {
                NotificationService.Error(Loc["User save failed"]);
            }
        }
        else
        {
            _model.Roles = _roles!.Where(x => _selectedRoles!.Contains(x.Id)).ToArray();
            var result = await UserService.CreateUser(_model, _password!, "User");
            if(result.Succeeded) {
                NotificationService.Success(Loc["Text.Created"]);
            }
            else
            {
                NotificationService.Error(Loc["User not created"]);
            }
        }
        NavigationManager.NavigateTo("/users");
    }
}