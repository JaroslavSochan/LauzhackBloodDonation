using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WindowsFormsAppDoctor
{
    public partial class GeartestContext : DbContext
    {
        public virtual DbSet<Bloddpicture> Bloddpictures { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }

        // Unable to generate entity type for table 'dbo.Persons'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.PassengersInflights'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.Flights'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.Payments'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Data Source=mssql6.gear.host;Initial Catalog=geartest;Persist Security Info=True;User ID=geartest;Password=Ck2143!Eq66-");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.Property(e => e.Lastname).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Pass).IsUnicode(false);

                entity.Property(e => e.Username).IsUnicode(false);
            });
        }
    }
}
