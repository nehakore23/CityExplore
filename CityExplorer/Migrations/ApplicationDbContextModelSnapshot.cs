using System;
using CityExplorer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CityExplorer.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("CityExplorer.Models.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CityId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("NumberOfPeople")
                        .HasColumnType("int");

                    b.Property<string>("SpecialRequests")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("TotalAmount")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.HasIndex("UserId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("CityExplorer.Models.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("varchar(1000)");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("Price")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Cities");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Country = "France",
                            CreatedAt = new DateTime(2025, 7, 2, 20, 3, 25, 557, DateTimeKind.Local).AddTicks(4741),
                            Description = "The City of Light, famous for the Eiffel Tower, Louvre Museum, and romantic atmosphere.",
                            Duration = 5,
                            ImageUrl = "/images/paris.jpg",
                            IsActive = true,
                            Name = "Paris",
                            Price = 299.99m
                        },
                        new
                        {
                            Id = 2,
                            Country = "Japan",
                            CreatedAt = new DateTime(2025, 7, 2, 20, 3, 25, 557, DateTimeKind.Local).AddTicks(4744),
                            Description = "A vibrant metropolis blending traditional culture with modern technology.",
                            Duration = 7,
                            ImageUrl = "/images/tokyo.jpg",
                            IsActive = true,
                            Name = "Tokyo",
                            Price = 399.99m
                        },
                        new
                        {
                            Id = 3,
                            Country = "USA",
                            CreatedAt = new DateTime(2025, 7, 2, 20, 3, 25, 557, DateTimeKind.Local).AddTicks(4746),
                            Description = "The Big Apple, home to Times Square, Central Park, and the Statue of Liberty.",
                            Duration = 6,
                            ImageUrl = "/images/newyork.jpg",
                            IsActive = true,
                            Name = "New York",
                            Price = 349.99m
                        });
                });

            modelBuilder.Entity("CityExplorer.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Phone")
                        .HasColumnType("longtext");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2025, 7, 2, 20, 3, 25, 557, DateTimeKind.Local).AddTicks(4619),
                            Email = "admin@cityexplorer.com",
                            FullName = "System Administrator",
                            Password = "admin123",
                            Role = "Admin",
                            Username = "admin"
                        });
                });

            modelBuilder.Entity("CityExplorer.Models.Booking", b =>
                {
                    b.HasOne("CityExplorer.Models.City", "City")
                        .WithMany("Bookings")
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CityExplorer.Models.User", "User")
                        .WithMany("Bookings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("City");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CityExplorer.Models.City", b =>
                {
                    b.Navigation("Bookings");
                });

            modelBuilder.Entity("CityExplorer.Models.User", b =>
                {
                    b.Navigation("Bookings");
                });
#pragma warning restore 612, 618
        }
    }
}
