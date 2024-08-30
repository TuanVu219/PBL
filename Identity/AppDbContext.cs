using PBL3Hos.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PBL3Hos.Models.DbModel;
using System.Reflection.Emit;
namespace PBL3Hos.Identity
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<UserDoctor> Doctors { get; set; }
        public DbSet<UserPatient> Patients { get; set; }
        public DbSet<AppointmentDB> Appointments { get; set; }
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }
        public DbSet<DoctorSchedule>DoctorSchedules { get; set; }
        public DbSet<DoctorDayOff> DoctorDayOffs { get; set; }
        public DbSet<AppointmentHistory> AppointmentHistory { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<Rating>Ratings {  get; set; }
        public DbSet<AppointmentCancel> AppointmentCancels { get; set; }
        
        public DbSet<Specialist> Specialists {  get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);


            builder.Ignore<IdentityUserLogin<string>>();
            builder.Entity<AppointmentDB>()
                .HasOne(a => a.Doctor)
                .WithMany(a => a.Appointments)
                .HasForeignKey(a => a.DoctorId);

            builder.Entity<AppointmentDB>()
                .HasOne(a => a.Patient)
                .WithOne(a => a.Appointment)
                .HasForeignKey<AppointmentDB>(a => a.PatientId);

            builder.Entity<AppointmentHistory>()
                 .HasOne(a => a.Doctor)
                 .WithMany()
                 .HasForeignKey(a => a.DoctorId)
                 .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction, depending on your requirements

            builder.Entity<AppointmentHistory>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction, depending on your requirements


            builder.Entity<Rating>()
                 .HasOne(a => a.Doctor)
                 .WithMany(a => a.Ratings)
                 .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Rating>()
                .HasOne(a => a.Patient)
                .WithMany(a => a.Ratings)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppointmentCancel>()
               .HasOne(a => a.Doctor)
               .WithMany(a => a.AppointmentCancels)
               .HasForeignKey(a => a.DoctorId)
          .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppointmentCancel>()
                .HasOne(a => a.Patient)
                .WithMany(a => a.AppointmentCancels)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        }






    }


}
