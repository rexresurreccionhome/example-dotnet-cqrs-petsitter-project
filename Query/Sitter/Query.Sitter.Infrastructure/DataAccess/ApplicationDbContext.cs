using System;
using Microsoft.EntityFrameworkCore;
using Query.Sitter.Domain.Entities;

namespace Query.Sitter.Infrastructure.DataAccess;

public class ApplicationDbContext : DbContext
{
    public DbSet<SitterEntity> Sitters { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }
}
