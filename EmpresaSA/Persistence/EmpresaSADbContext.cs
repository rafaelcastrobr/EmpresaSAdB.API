using EmpresaSA.Entities;
using EmpresaSA.Enums;
using EmpresaSA.Models;
using Microsoft.EntityFrameworkCore;

namespace EmpresaSA.Persistence
{
    public class EmpresaSADbContext : DbContext
    {
        public EmpresaSADbContext(DbContextOptions<EmpresaSADbContext> options) : base(options)
        {

        }

        public DbSet<ColaboradorEntitie> Colaborador { get; set; }
        public DbSet<DepartamentoEntitie> Departamento { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ColaboradorModel>(e =>
            {
                e.ToTable("Colaborador");

                e.HasKey(cx => cx.Id);

                e.Property(cx => cx.Nome)
                    .HasColumnType("varChar(255)");

                e.Property(cx => cx.Documento)
                    .HasColumnType("varchar(255)");

                e.Property(cx => cx.Status)
                    .HasConversion(
                        cv => cv.ToString(),
                        cv => (StatusEnum)Enum.Parse(typeof(StatusEnum), cv))
                    .IsRequired();
                    

                
            });


            builder.Entity<DepartamentoModel>(e => 
            {
                e.ToTable("Departamento");

                e.HasKey(cx => cx.Id);

                e.Property(cx => cx.Nome)
                    .HasColumnType("varChar(255)");

                e.Property(cx => cx.Status)
                    .HasColumnType("varChar(100)");

                e.Property(cx => cx.Status)
                    .HasConversion(
                        cv => cv.ToString(),
                        cv => (StatusEnum)Enum.Parse(typeof(StatusEnum), cv))
                    .IsRequired();

                e.HasMany(cx => cx.Colaboradores)
                    .WithOne()
                    .HasForeignKey(cx => cx.Id_Departamento);

                
            });
        }
    }
}
