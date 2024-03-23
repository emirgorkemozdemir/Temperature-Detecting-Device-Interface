using System;
using System.Collections.Generic;
using deviceInterfacev2.Models;
using Microsoft.EntityFrameworkCore;

namespace deviceInterfacev2.Models;

public partial class TempDatabaseContext : DbContext
{
    public TempDatabaseContext()
    {
    }

    public TempDatabaseContext(DbContextOptions<TempDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TableCity> TableCities { get; set; }

    public virtual DbSet<TableContact> TableContacts { get; set; }

    public virtual DbSet<TableCounty> TableCounties { get; set; }

    public virtual DbSet<TableDevice> TableDevices { get; set; }

    public virtual DbSet<TablePerson> TablePersons { get; set; }

    public virtual DbSet<TableUser> TableUsers { get; set; }
    public virtual DbSet<TableTm> TableTMS { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Server=DESKTOP-A1P13JI;Database=tempDatabase;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TableCity>(entity =>
        {
            entity.HasKey(e => e.CityId);

            entity.ToTable("TableCity");

            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.CityName).HasMaxLength(14);
            entity.Property(e => e.CityPlate).HasMaxLength(2);
        });

        modelBuilder.Entity<TableContact>(entity =>
        {
            entity.HasKey(e => e.ContactId);

            entity.ToTable("TableContact");

            entity.Property(e => e.ContactId).HasColumnName("ContactID");
            entity.Property(e => e.ContactFirmAdress).HasMaxLength(100);
            entity.Property(e => e.ContactFirmName).HasMaxLength(40);
            entity.Property(e => e.ContactTaxLoc).HasMaxLength(40);
            entity.Property(e => e.ContactTaxNo).HasMaxLength(10);
            entity.Property(e => e.ContactTc)
                .HasMaxLength(10)
                .HasColumnName("ContactTC");
            entity.Property(e => e.ContactUserId).HasColumnName("ContactUserID");

            entity.HasOne(d => d.ContactUser).WithMany(p => p.TableContacts)
                .HasForeignKey(d => d.ContactUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TableContact_TableUser");
        });

        modelBuilder.Entity<TableCounty>(entity =>
        {
            entity.HasKey(e => e.CountyId);

            entity.ToTable("TableCounty");

            entity.Property(e => e.CountyId).HasColumnName("CountyID");
            entity.Property(e => e.CountyCityId).HasColumnName("CountyCityID");
            entity.Property(e => e.CountyName).HasMaxLength(25);

            entity.HasOne(d => d.CountyCity).WithMany(p => p.TableCounties)
                .HasForeignKey(d => d.CountyCityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TableCounty_TableCity");
        });

        modelBuilder.Entity<TableDevice>(entity =>
        {
            entity.HasKey(e => e.DeviceId);

            entity.ToTable("TableDevice");

            entity.Property(e => e.DeviceId).HasColumnName("DeviceID");
            entity.Property(e => e.DeviceName).HasMaxLength(50);

            entity.Property(e => e.DeviceUniqeKey).HasMaxLength(30);

            entity.HasOne(d => d.DeviceConnectedUserNavigation).WithMany(p => p.TableDevices)
                .HasForeignKey(d => d.DeviceConnectedUser)
                .HasConstraintName("FK_TableDevice_TableUser");

            entity.Property(e => e.DeviceRegisterDate)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        });

        modelBuilder.Entity<TableTm>(entity =>
        {
            entity.HasKey(e => e.Tmid);

            entity.ToTable("TableTM");

            entity.Property(e => e.Tmid).HasColumnName("TMID");
            entity.Property(e => e.TmdeviceId).HasColumnName("TMDeviceID");

            entity.HasOne(d => d.Tmdevice).WithMany(p => p.TableTms)
                .HasForeignKey(d => d.TmdeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TableTM_TableDevice");

            entity.Property(e => e.TMDate)
               .HasDefaultValueSql("(getdate())")
               .HasColumnType("datetime");
        });

        modelBuilder.Entity<TablePerson>(entity =>
        {
            entity.HasKey(e => e.PersonId);

            entity.ToTable("TablePerson");

            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.PersonContactId).HasColumnName("PersonContactID");
            entity.Property(e => e.PersonMail).HasMaxLength(40);
            entity.Property(e => e.PersonName)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.PersonPhone).HasMaxLength(10);
            entity.Property(e => e.PersonSurname)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.PersonUserId).HasColumnName("PersonUserID");

            entity.HasOne(d => d.PersonContact).WithMany(p => p.TablePeople)
                .HasForeignKey(d => d.PersonContactId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TablePerson_TableContact");

            entity.HasOne(d => d.PersonUser).WithMany(p => p.TablePeople)
                .HasForeignKey(d => d.PersonUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TablePerson_TableUser");
        });

        modelBuilder.Entity<TableUser>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("TableUser");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserMail).HasMaxLength(40);
            entity.Property(e => e.UserPassword).HasMaxLength(64);
            entity.Property(e => e.UserRegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("date");
            entity.Property(e => e.UserConfigurateDate)
               .HasDefaultValueSql("(getdate())")
               .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
