namespace Forum.Models.AccountViewModels;

public class ProfileViewModel
{
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string AvatarPath { get; set; } = default!;
    public int PostCount { get; set; }
}