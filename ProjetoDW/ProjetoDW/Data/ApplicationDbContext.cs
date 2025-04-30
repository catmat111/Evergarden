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

public DbSet<ProjetoDW.Models.UtilizadoresR> UtilizadoresR { get; set; } = default!;

public DbSet<ProjetoDW.Models.UtilizadoresD> UtilizadoresD { get; set; } = default!;

public DbSet<ProjetoDW.Models.Cartas> Cartas { get; set; } = default!;
}