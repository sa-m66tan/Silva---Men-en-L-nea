using Microsoft.EntityFrameworkCore;
using SilvaAPI.Models;

namespace SilvaAPI.Data;

public class SilvaContext : DbContext
{
    public SilvaContext(DbContextOptions<SilvaContext> options) : base(options)
    {
    }

    public DbSet<Roles> Roles { get; set; }
    public DbSet<Usuarios> Usuarios { get; set; }
    public DbSet<Categorias> Categorias { get; set; }
    public DbSet<Platillos> Platillos { get; set; }
    public DbSet<Ingredientes> Ingredientes { get; set; }
    public DbSet<AuditoriaCatalogos> AuditoriaCatalogos { get; set; }

    // vistas
    public DbSet<VwMenuPublico> VwMenuPublicos { get; set; }
    public DbSet<VwReporteAuditoria> VwReporteAuditorias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Platillos>(entity =>
        {
            entity.ToTable("Platillos", tb => tb.HasTrigger("trg_AuditoriaPlatillos"));
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.IdRol);
            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsUnicode(false);
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100);
            entity.Property(e => e.Apellido).HasColumnName("apellido").HasMaxLength(100);
            entity.Property(e => e.Correo).HasColumnName("correo").HasMaxLength(150).IsUnicode(false);
            entity.Property(e => e.Contraseña).HasColumnName("contraseña").HasMaxLength(255).IsUnicode(false);
            entity.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasColumnName("fechaRegistro").HasDefaultValueSql("GETDATE()");

            entity.HasIndex(e => e.Correo).IsUnique();

            entity.HasOne(d => d.Roles)
                  .WithMany(p => p.Usuarios)
                  .HasForeignKey(d => d.IdRol)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Usuarios_Roles");
        });

        modelBuilder.Entity<Categorias>(entity =>
        {
            entity.ToTable("Categorias");
            entity.HasKey(e => e.IdCategoria);
            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50);
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        modelBuilder.Entity<Platillos>(entity =>
        {
            entity.ToTable("Platillos");
            entity.HasKey(e => e.IdPlatillo);
            entity.Property(e => e.IdPlatillo).HasColumnName("idPlatillo");
            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.IdUsuarioUltimaModif).HasColumnName("idUsuarioUltimaModif");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Precio).HasColumnName("precio").HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ImagenUrl).HasColumnName("imagenUrl").HasMaxLength(255).IsUnicode(false);
            entity.Property(e => e.TiempoPreparacion).HasColumnName("tiempoPreparacion");
            entity.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(20).HasDefaultValue("Disponible");

            entity.HasOne(d => d.Categorias)
                  .WithMany(p => p.Platillos)
                  .HasForeignKey(d => d.IdCategoria)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Platillos_Categorias");

            entity.HasOne(d => d.UsuarioUltimaModif)
                  .WithMany(p => p.PlatillosModificados)
                  .HasForeignKey(d => d.IdUsuarioUltimaModif)
                  .HasConstraintName("FK_Platillos_Usuarios");

            entity.HasMany(d => d.Ingredientes)
                  .WithMany(p => p.Platillos)
                  .UsingEntity<Dictionary<string, object>>(
                      "PlatilloIngrediente",
                      j => j.HasOne<Ingredientes>().WithMany().HasForeignKey("idIngrediente").HasConstraintName("FK_PI_Ingredientes"),
                      j => j.HasOne<Platillos>().WithMany().HasForeignKey("idPlatillo").HasConstraintName("FK_PI_Platillos"),
                      j =>
                      {
                          j.ToTable("PlatilloIngredientes");
                          j.HasKey("idPlatillo", "idIngrediente");
                      });
        });

        modelBuilder.Entity<Ingredientes>(entity =>
        {
            entity.ToTable("Ingredientes");
            entity.HasKey(e => e.IdIngrediente);
            entity.Property(e => e.IdIngrediente).HasColumnName("idIngrediente");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100);
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        modelBuilder.Entity<AuditoriaCatalogos>(entity =>
        {
            entity.ToTable("AuditoriaCatalogos");
            entity.HasKey(e => e.IdAuditoria);
            entity.Property(e => e.IdAuditoria).HasColumnName("idAuditoria");
            entity.Property(e => e.Accion).HasColumnName("accion").HasMaxLength(50);
            entity.Property(e => e.DetalleCambio).HasColumnName("detalleCambio");
            entity.Property(e => e.FechaRegistro).HasColumnName("fechaRegistro").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.IdUsuarioModif).HasColumnName("idUsuarioModif");
            entity.Property(e => e.IdPlatillo).HasColumnName("idPlatillo");

            entity.HasOne(d => d.UsuarioModif)
                  .WithMany(p => p.Auditorias)
                  .HasForeignKey(d => d.IdUsuarioModif)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("FK_Auditoria_Usuarios");

            entity.HasOne(d => d.Platillos)
                  .WithMany(p => p.Auditorias)
                  .HasForeignKey(d => d.IdPlatillo)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("FK_Auditoria_Platillo");
        });

        modelBuilder.Entity<VwMenuPublico>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_MenuPublico");
            entity.Property(e => e.IdPlatillo).HasColumnName("idPlatillo");
            entity.Property(e => e.Platillo).HasColumnName("platillo");
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<VwReporteAuditoria>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_ReporteAuditoria");
            entity.Property(e => e.IdAuditoria).HasColumnName("idAuditoria");
            entity.Property(e => e.IdPlatillo).HasColumnName("idPlatillo");
        });
    }
}