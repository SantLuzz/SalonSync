using Microsoft.AspNetCore.Identity;

namespace SalonSync.Api.Models;

public class User : IdentityUser<long>
{
    public List<IdentityRole<long>>? Roles { get; set; }

}