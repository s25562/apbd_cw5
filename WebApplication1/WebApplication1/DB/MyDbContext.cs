using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1;

public class MyDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>(d =>
        {
            d.ToTable("Doctor");
            d.HasKey(k => k.IdDoctor);
            d.Property(p => p.FirstName).HasMaxLength(100);
            d.Property(p => p.LastName).HasMaxLength(100);
            d.Property(p => p.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<Medicament>(m =>
        {
            m.ToTable("Medicament");
            m.HasKey(k => k.MedicamentId);
            m.Property(p => p.Name).HasMaxLength(100);
            m.Property(p => p.Description).HasMaxLength(100);
            m.Property(p => p.Type).HasMaxLength(100);
        });

        modelBuilder.Entity<Patient>(p =>
        {
            p.ToTable("Patient");
            p.HasKey(k => k.IdPatient);
            p.Property(p => p.FirstName).HasMaxLength(100);
            p.Property(p => p.LastName).HasMaxLength(100);
            p.Property(p => p.DateOfBirth).HasColumnType("date");
        });

        modelBuilder.Entity<PrescriptionMedicament>(pm =>
        {
            pm.ToTable("PrescriptionMedicament");
            pm.HasKey(pm => new { pm.MedicamentId, pm.PrescriptionId });
            pm.HasOne(m => m.Medicament).
                WithMany(pm => pm.PrescriptionMedicaments).
                HasForeignKey(m => m.Medicament);
            pm.HasOne(p => p.Prescription).
                WithMany(pm => pm.PrescriptionMedicaments).
                HasForeignKey(p => p.PrescriptionId);
            pm.Property(pm => pm.Dose).IsRequired(false);
            pm.Property(pm => pm.Details).HasMaxLength(100);
        });

        modelBuilder.Entity<Prescription>(p =>
        {
            p.ToTable("Prescription");
            p.HasKey(k => k.IdPrescription);
            p.Property(p => p.Date).HasColumnType("date");
            p.Property(p => p.DueDate).HasColumnType("date");
            p.HasOne(p => p.Patient).
                WithMany(pr => pr.Prescriptions).
                HasForeignKey(p => p.IdPatient);
            
            p.HasOne(d => d.Doctor).
                WithMany(pr => pr.Prescriptions).
                HasForeignKey(d => d.IdDoctor);
        });

        modelBuilder.Entity<Doctor>().HasData(new List<Doctor>()
        {
            new Doctor() { IdDoctor = 1, FirstName = "James", LastName = "Bond", Email = "james@gmail.com" },
            new Doctor() { IdDoctor = 2, FirstName = "Mary", LastName = "Smith", Email = "mary@gmail.com" },
            new Doctor() { IdDoctor = 3, FirstName = "Joanna", LastName = "Doe", Email = "joanna@gmail.com" }
        });

        modelBuilder.Entity<Patient>().HasData(new List<Patient>()
        {
            new Patient()
                { IdPatient = 1, FirstName = "Garry", LastName = "Go", DateOfBirth = Convert.ToDateTime("1995-01-01") },
            new Patient()
            {
                IdPatient = 2, FirstName = "Amanda", LastName = "Jones", DateOfBirth = Convert.ToDateTime("1995-01-01")
            }

        });
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SchoolDb;Trusted_Connection=True;");
    }
}