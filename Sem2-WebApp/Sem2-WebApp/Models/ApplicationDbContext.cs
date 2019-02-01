using Microsoft.EntityFrameworkCore;

namespace Sem2_WebApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Cleaner> Cleaners { get; set; }
        public DbSet<Toilet> Toilets { get; set; }
        public DbSet<ToiletSettings> ToiletsSettings { get; set; }
        public DbSet<SensorValues> SensorsValues { get; set; }
        public DbSet<ToiletLog> ToiletLogs { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cleaner>()
                .Property(c => c.Rfid)
                .HasDefaultValue("No card registered");

            modelBuilder.Entity<Cleaner>()
                .Property(c => c.Status)
                .HasDefaultValue("Available");

            modelBuilder.Entity<Cleaner>()
                .Property(c => c.CleanedCount)
                .HasDefaultValue(0);

            modelBuilder.Entity<Toilet>()
                .Property(t => t.ToiletGender)
                .HasDefaultValue("Female");

            modelBuilder.Entity<Toilet>()
                .HasIndex(t => t.SensorId)
                .IsUnique();

            modelBuilder.Entity<ToiletSettings>()
                .Property(s => s.UpdateFrequency)
                .HasDefaultValue(1000);

            modelBuilder.Entity<ToiletSettings>()
                .Property(s => s.FanMode)
                .HasDefaultValue("Auto");

            modelBuilder.Entity<ToiletSettings>()
                .Property(s => s.FanThreshold)
                .HasDefaultValue(40);

            modelBuilder.Entity<ToiletSettings>()
                .Property(s => s.UserThreshold)
                .HasDefaultValue(20);

            modelBuilder.Entity<ToiletSettings>()
                .Property(s => s.GasValueThreshold)
                .HasDefaultValue(60);

            modelBuilder.Entity<ToiletSettings>()
                .Property(s => s.RequestThreshold)
                .HasDefaultValue(10);
        }
    }
}
