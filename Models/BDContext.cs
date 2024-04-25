using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ByteStorm.Models;
public class BDContext : DbContext
{
    // Colecciones de las entidades del contexto
    public DbSet<Operativo> Operativos { get; set; } = null!;
    public DbSet<Mision> Misiones { get; set; } = null!;
    public DbSet<Equipo> Equipos { get; set; } = null!;

    public BDContext(DbContextOptions<BDContext> options) 
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Operativo>(entity => 
        {
            entity.HasKey(o => o.ID);
        });

        modelBuilder.Entity<Mision>(entity =>
        {
            entity.HasKey(m => m.codigo);

            entity.HasOne(m => m.operativo)
            .WithMany(o => o.misiones)
            .HasForeignKey(m => m.idOperativo);
        });

        modelBuilder.Entity<Equipo>(entity =>
        {
            entity.HasKey(e => e.ID);
        });

        base.OnModelCreating(modelBuilder);
    }

}