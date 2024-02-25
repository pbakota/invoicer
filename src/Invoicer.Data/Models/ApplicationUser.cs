using Microsoft.AspNetCore.Identity;

namespace Invoicer.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public virtual IEnumerable<IdentityRole>? Roles { get; set; }
}

public class ApplicationRole : IdentityRole
{ }

// public class ApplicationUserRole : IdentityUserRole<string>
// {
//     public virtual ApplicationUser User { get; set; } = null!;
//     public virtual ApplicationRole Role { get; set; } = null!;
// }