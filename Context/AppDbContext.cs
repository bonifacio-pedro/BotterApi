using BotterApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BotterApi.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt): base(opt)
    { }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
}
