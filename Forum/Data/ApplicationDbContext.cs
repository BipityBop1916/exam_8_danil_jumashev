using Forum.Models;
using Microsoft.EntityFrameworkCore;

namespace Forum.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Topic> Topics { get; set; }
}