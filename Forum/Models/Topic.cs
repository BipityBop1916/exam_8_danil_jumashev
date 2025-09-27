using System.ComponentModel.DataAnnotations;

namespace Forum.Models;

public class Topic
{
    public int Id { get; set; }

    [Required, MaxLength(160)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string AuthorName { get; set; }
}