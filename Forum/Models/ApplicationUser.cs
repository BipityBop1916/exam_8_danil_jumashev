namespace Forum.Models;

using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string? AvatarPath { get; set; }
}