using System.ComponentModel.DataAnnotations;

namespace Forum.Models;

public class Reply
{
    public int Id { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int TopicId { get; set; }
    public Topic Topic { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}