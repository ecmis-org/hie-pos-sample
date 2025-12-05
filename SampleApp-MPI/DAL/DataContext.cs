using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Options;
using SampleApp_MPI.Models;
using System.IO;
using System.Security.AccessControl;
using static SampleApp_MPI.Utilities.Constants;

namespace SampleApp_MPI.DAL
{
    public class DataContext : DbContext
    {
        public DbSet<Patient> Patient { get; set; }
        public DbSet<Encounter> Encounters { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<ServicePoint> ServicePoint { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Practitioner> Practitioner { get; set; }
        public DbSet<Prescription> Prescription { get; set; }
        public DbSet<Medication> Medication { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<MedicationDispense> MedicationDispense { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("SampleHIE");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Code = "OPD", Name = "Out Patient Department" },
                new Department { Id = 2, Code = "IPD", Name = "In Patient Department" },
                new Department { Id = 3, Code = "EMG", Name = "Emergency" }
            );

            modelBuilder.Entity<ServicePoint>().HasData(
                new ServicePoint { Id = 1, Code = "ART", Name = "Anti Retroviral Therapy" },
                new ServicePoint { Id = 2, Code = "HTS", Name = "HIV Testing Services" },
                new ServicePoint { Id = 3, Code = "ANC", Name = "Ante Natal Care" },
                new ServicePoint { Id = 4, Code = "PNC", Name = "Post Natal Care" },
                new ServicePoint { Id = 5, Code = "FP", Name = "Family Planning" },
                new ServicePoint { Id = 6, Code = "MAL", Name = "Malaria" }
            );

            modelBuilder.Entity<Location>().HasData(
                new Location { Id = Guid.NewGuid(), Name = "Mahwalala Red Cross Clinic", Code = "OF", Tell = "00000000" },
                new Location { Id = Guid.NewGuid(), Name = "Mbabane Public Health Unit", Code = "OF", Tell = "00000011" },
                new Location { Id = Guid.NewGuid(), Name = "King Sobhuza Health Unit", Code = "OF", Tell = "00000022" },
                new Location { Id = Guid.NewGuid(), Name = "Matsanjeni Health Center", Code = "OF", Tell = "00000033" },
                new Location { Id = Guid.NewGuid(), Name = "Siteki Public Health Unit", Code = "OF", Tell = "00000044" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Code = "102324", Name = "Norgestrel 300mcg / Ethinylestradiol 30mcg Tablets 28 TABS", Form = ProductForm.Tablet },
                new Product { Id = 2, Code = "100648", Name = "Abacavir 300mg Tablets 60 TABS", Form = ProductForm.Tablet },
                new Product { Id = 3, Code = "102273", Name = "Isoniazid Tablets 300mg 28 TABS", Form = ProductForm.Tablet },
                new Product { Id = 4, Code = "102268", Name = "Ethambutol FILM COATEDTablets 100mg 100 TABS", Form = ProductForm.Tablet },
                new Product { Id = 5, Code = "100686", Name = "Isoniazid 100mg Tablets 100 TABS", Form = ProductForm.Tablet },
                new Product { Id = 6, Code = "102276", Name = "Levofloxacin Tablets 500mg 100 TABS", Form = ProductForm.Tablet },
                new Product { Id = 7, Code = "100449", Name = "Acyclovir Eye Ointment 3 % 4.5 G", Form = ProductForm.Tablet },
                new Product { Id = 8, Code = "100304", Name = "Adrenaline Injection 1:1000 10 AMPS", Form = ProductForm.Tablet },
                new Product { Id = 9, Code = "100460", Name = "Betamethasone Cream 0.1 % 15G", Form = ProductForm.Tablet },
                new Product { Id = 10, Code = "100014", Name = "Cefaclor Tablets 375mg 10 TABS", Form = ProductForm.Tablet },
                new Product { Id = 11, Code = "102443", Name = "Cefazolin 1g; 10 Vial 10 VIAL", Form = ProductForm.Vial },
                new Product { Id = 12, Code = "100689", Name = "Saquinavir 200mg Capsules 270 CAPS", Form = ProductForm.Capsule },
                new Product { Id = 13, Code = "100009", Name = "Amoxycillin Capsules 500mg 500 CAPS", Form = ProductForm.Capsule }
            );

            modelBuilder.Entity<Practitioner>().HasData(
                new Practitioner { Id = Guid.NewGuid(), PIN = "9002026200111", FirstName = "Mary", LastName = "Jane", Sex = Sex.female, DOB = DateTime.Parse("1990-02-02") },
                new Practitioner { Id = Guid.NewGuid(), PIN = "7702026200123", FirstName = "Susan", LastName = "Smith", Sex = Sex.female, DOB = DateTime.Parse("1989-02-02") },
                new Practitioner { Id = Guid.NewGuid(), PIN = "8902026200873", FirstName = "Carol", LastName = "Dlamini", Sex = Sex.female, DOB = DateTime.Parse("1989-02-02") },
                new Practitioner { Id = Guid.NewGuid(), PIN = "6002026200147", FirstName = "Peter", LastName = "Pan", Sex = Sex.female, DOB = DateTime.Parse("1960-02-02") },
                new Practitioner { Id = Guid.NewGuid(), PIN = "8102026200159", FirstName = "Gloria", LastName = "Madu", Sex = Sex.female, DOB = DateTime.Parse("1981-02-02") }
            );
        }
    }
}
