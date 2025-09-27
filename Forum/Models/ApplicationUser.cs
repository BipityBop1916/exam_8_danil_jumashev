namespace Forum.Models;

using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string? AvatarPath { get; set; }
    public int PostCount { get; set; }
}