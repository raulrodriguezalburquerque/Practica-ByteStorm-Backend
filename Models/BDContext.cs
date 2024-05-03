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
            // Asignacion de clave primaria
            entity.HasKey(o => o.ID);
        });

        modelBuilder.Entity<Mision>(entity =>
        {
            // Asignacion de clave primaria
            entity.HasKey(m => m.codigo);

            // Configuracion de la relacion entre misiones y operativos
            entity.HasOne(m => m.operativo)
            .WithMany(o => o.misiones)
            .HasForeignKey(m => m.idOperativo)
            .OnDelete(DeleteBehavior.SetNull);

            // Configuracion de la relacion entre misiones y equipos
            entity.HasMany(m => m.equipos)
            .WithOne(e => e.mision)
            .HasForeignKey(e => e.codigoMision)
            .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Equipo>(entity =>
        {
            // Asignacion de clave primaria
            entity.HasKey(e => e.ID);
        });

        base.OnModelCreating(modelBuilder);
    }

}