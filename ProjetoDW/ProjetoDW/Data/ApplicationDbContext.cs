using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjetoDW.Models;

namespace ProjetoDW.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

public DbSet<ProjetoDW.Models.Utilizadores> Utilizadores { get; set; } = default!;

public DbSet<ProjetoDW.Models.Cartas> Cartas { get; set; } = default!;

public DbSet<ProjetoDW.Models.Categorias> Categorias { get; set; } = default!;

public DbSet<ProjetoDW.Models.Tarefa> Tarefa { get; set; }
}