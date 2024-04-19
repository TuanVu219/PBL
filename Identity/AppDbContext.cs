using PBL3Hos.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PBL3Hos.Models.DbModel;
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
        }






    }


}
