using Invoicer.Data.Extensions;
using Invoicer.Data.Utils;
using Invoicer.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Invoicer.App.Services;

public interface IUserService
{
    Pageable Pageable { get; }    
    Task<IdentityResult> CreateUser(ApplicationUser user, string password, string role);
    Task<ApplicationUser> NewUser();
    Task<IdentityResult> SaveUser(string id, ApplicationUser user, string? password);
    Task DeleteUser(string userId);
    Task<ApplicationUser?> GetSingle(string userId);
    Task<PagedResult<ApplicationUser>> GetPagedData();
    Task<IList<IdentityRole>> GetRoles();
}

public class UserService(ILogger<UserService> logger,
    IUserStore<ApplicationUser> userStore,
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager) : IUserService
{
    public Pageable Pageable { get; } = new()
    {
        Page = 0,
        PageSize = 50,
        OrderBy = "UserName asc",
        SearchTerm = null,
        Searchables = [
            nameof(ApplicationUser.UserName),
            nameof(ApplicationUser.Email),
        ],
    };

    public async Task<IdentityResult> CreateUser(ApplicationUser user, string password, string role)
    {
        var newUser = new ApplicationUser
        {
            EmailConfirmed = true,
        };

        await userStore.SetUserNameAsync(newUser, user.Email, CancellationToken.None);
        var emailStore = GetEmailStore();
        await emailStore.SetEmailAsync(newUser, user.Email, CancellationToken.None);
        
        var result = await userManager.CreateAsync(newUser, password);
        if(!result.Succeeded)
        {
            logger.LogError("User not created, {}", string.Join(",", result.Errors.Select(s => s.Description)));
        }

        await userManager.AddToRolesAsync(newUser, user.Roles?.Select(x => x.Name).ToArray()!);
        if(!result.Succeeded)
        {
            logger.LogError("User not assigned to role, {}", string.Join(",", result.Errors.Select(s => s.Description)));
        }
        
        return result;
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)userStore;
    }

    public async Task DeleteUser(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if(user is not null) 
        {
            await userManager.DeleteAsync(user);
        }
    }

    public async Task<PagedResult<ApplicationUser>> GetPagedData()
    {
        var query = userManager.Users.AsNoTracking().Include(f => f.Roles).ApplySearchAndOrder(Pageable);
        var total = await query.CountAsync();
        query = query.ApplyPagination(Pageable);

        return new PagedResult<ApplicationUser> {
            Content = await query.ToListAsync(),
            Total = total,
        };
    }

    public async Task<IList<IdentityRole>> GetRoles()
        => await roleManager.Roles.AsNoTracking().ToListAsync();

    public async Task<ApplicationUser?> GetSingle(string userId)
        => await userManager.Users.AsNoTracking().Include(u => u.Roles).SingleOrDefaultAsync(u => u.Id == userId);

    public Task<ApplicationUser> NewUser()
        => Task.FromResult(new ApplicationUser());

    public async Task<IdentityResult> SaveUser(string id, ApplicationUser temp, string? password)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return IdentityResult.Failed([new IdentityError { Description = "User not found"}]);
        }

        var roles = await roleManager.Roles.AsNoTracking().ToListAsync();
        foreach (var role in roles)
        {
            var inRole = await userManager.IsInRoleAsync(user, role.Name!);
            switch (inRole)
            {
                case false when temp.Roles!.Any(f => f.Id == role.Id):
                    await userManager.AddToRoleAsync(user, role.Name!);
                    break;
                case true when temp.Roles!.All(f => f.Id != role.Id):
                    await userManager.RemoveFromRoleAsync(user, role.Name!);
                    break;
            }
        }

        await userStore.SetUserNameAsync(user, temp.Email, CancellationToken.None);
        await GetEmailStore().SetEmailAsync(user, temp.Email, CancellationToken.None);
        var result = await userManager.SetPhoneNumberAsync(user, temp.PhoneNumber);
        if (!result.Succeeded || string.IsNullOrEmpty(password))
        {
            return result;
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        result = await userManager.ResetPasswordAsync(user, token, password);

        return result;
    }
}
