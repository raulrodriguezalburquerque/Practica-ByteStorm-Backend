﻿// <auto-generated />
using System;
using ByteStorm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ByteStorm.Migrations
{
    [DbContext(typeof(BDContext))]
    [Migration("20240430110434_Inicial")]
    partial class Inicial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("ByteStorm.Models.Equipo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("codigoMision")
                        .HasColumnType("INTEGER");

                    b.Property<string>("descripcion")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("estado")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("tipo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("codigoMision");

                    b.ToTable("Equipos");
                });

            modelBuilder.Entity("ByteStorm.Models.Mision", b =>
                {
                    b.Property<int>("codigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("descripcion")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("estado")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("idOperativo")
                        .HasColumnType("INTEGER");

                    b.HasKey("codigo");

                    b.HasIndex("idOperativo");

                    b.ToTable("Misiones");
                });

            modelBuilder.Entity("ByteStorm.Models.Operativo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("nombre")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("rol")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Operativos");
                });

            modelBuilder.Entity("ByteStorm.Models.Equipo", b =>
                {
                    b.HasOne("ByteStorm.Models.Mision", "mision")
                        .WithMany("equipos")
                        .HasForeignKey("codigoMision")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("mision");
                });

            modelBuilder.Entity("ByteStorm.Models.Mision", b =>
                {
                    b.HasOne("ByteStorm.Models.Operativo", "operativo")
                        .WithMany("misiones")
                        .HasForeignKey("idOperativo")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("operativo");
                });

            modelBuilder.Entity("ByteStorm.Models.Mision", b =>
                {
                    b.Navigation("equipos");
                });

            modelBuilder.Entity("ByteStorm.Models.Operativo", b =>
                {
                    b.Navigation("misiones");
                });
#pragma warning restore 612, 618
        }
    }
}
