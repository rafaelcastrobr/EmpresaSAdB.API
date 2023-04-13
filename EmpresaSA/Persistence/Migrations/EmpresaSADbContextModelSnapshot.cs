﻿// <auto-generated />
using System;
using EmpresaSA.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EmpresaSA.Persistence.Migrations
{
    [DbContext(typeof(EmpresaSADbContext))]
    partial class EmpresaSADbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EmpresaSA.Models.ColaboradorModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Documento")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<Guid>("Id_Departamento")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("varChar(255)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Id_Departamento");

                    b.ToTable("Colaborador", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("ColaboradorModel");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("EmpresaSA.Models.DepartamentoModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("varChar(255)");

                    b.Property<string>("Sigla")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varChar(100)");

                    b.HasKey("Id");

                    b.ToTable("Departamento", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("DepartamentoModel");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("EmpresaSA.Entities.ColaboradorEntitie", b =>
                {
                    b.HasBaseType("EmpresaSA.Models.ColaboradorModel");

                    b.HasDiscriminator().HasValue("ColaboradorEntitie");
                });

            modelBuilder.Entity("EmpresaSA.Entities.DepartamentoEntitie", b =>
                {
                    b.HasBaseType("EmpresaSA.Models.DepartamentoModel");

                    b.HasDiscriminator().HasValue("DepartamentoEntitie");
                });

            modelBuilder.Entity("EmpresaSA.Models.ColaboradorModel", b =>
                {
                    b.HasOne("EmpresaSA.Models.DepartamentoModel", null)
                        .WithMany("Colaboradores")
                        .HasForeignKey("Id_Departamento")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EmpresaSA.Models.DepartamentoModel", b =>
                {
                    b.Navigation("Colaboradores");
                });
#pragma warning restore 612, 618
        }
    }
}
