﻿// <auto-generated />
using System;
using Inventorizer_DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Inventorizer_DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20201118155237_AddItemDetailsTable")]
    partial class AddItemDetailsTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Inventorizer_Models.Models.Category", b =>
                {
                    b.Property<int>("Category_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Category_Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Inventorizer_Models.Models.Item", b =>
                {
                    b.Property<int>("Item_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("Category_Id")
                        .HasColumnType("integer");

                    b.Property<int?>("ItemDetail_Id")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Item_Id");

                    b.HasIndex("Category_Id");

                    b.HasIndex("ItemDetail_Id");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("Inventorizer_Models.Models.ItemDetail", b =>
                {
                    b.Property<int>("ItemDetail_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("Item_Id")
                        .HasColumnType("integer");

                    b.Property<string>("Type")
                        .HasColumnType("text");

                    b.HasKey("ItemDetail_Id");

                    b.ToTable("ItemDetails");
                });

            modelBuilder.Entity("Inventorizer_Models.Models.Item", b =>
                {
                    b.HasOne("Inventorizer_Models.Models.Category", "Category")
                        .WithMany("Items")
                        .HasForeignKey("Category_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Inventorizer_Models.Models.ItemDetail", "ItemDetail")
                        .WithMany()
                        .HasForeignKey("ItemDetail_Id");

                    b.Navigation("Category");

                    b.Navigation("ItemDetail");
                });

            modelBuilder.Entity("Inventorizer_Models.Models.Category", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
