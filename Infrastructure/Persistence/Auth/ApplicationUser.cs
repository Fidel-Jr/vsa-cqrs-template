using Microsoft.AspNetCore.Identity;

namespace VsaCqrsTemplate.Infrastructure.Persistence.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}