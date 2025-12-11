using System;
using Microsoft.EntityFrameworkCore;
using Query.Pet.Domain.Entities;

namespace Query.Pet.Infrastructure.DataAccess;

public class ApplicationDbContext : DbContext
{
    public DbSet<PetEntity> Pets { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }
}
