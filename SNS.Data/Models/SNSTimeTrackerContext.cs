using Microsoft.EntityFrameworkCore;

namespace SNS.Data.Models
{
    public partial class SNSTimeTrackerContext : DbContext
    {
        public SNSTimeTrackerContext()
        {
        }

        public SNSTimeTrackerContext(DbContextOptions<SNSTimeTrackerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Tsclients> Tsclients { get; set; }
        public virtual DbSet<Tsengagements> Tsengagements { get; set; }
        public virtual DbSet<TstimeEntries> TstimeEntries { get; set; }
        public virtual DbSet<Tsusers> Tsusers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tsclients>(entity =>
            {
                entity.HasKey(e => e.TsclientId);

                entity.ToTable("TSClients");

                entity.Property(e => e.TsclientId)
                    .HasColumnName("TSClientID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ClientCode).HasMaxLength(75);

                entity.Property(e => e.ClientName)
                    .IsRequired()
                    .HasMaxLength(75);

                entity.Property(e => e.Type).HasMaxLength(30);
            });

            modelBuilder.Entity<Tsengagements>(entity =>
            {
                entity.HasKey(e => e.TsengagementId);

                entity.ToTable("TSEngagements");

                entity.Property(e => e.TsengagementId)
                    .HasColumnName("TSEngagementID")
                    .ValueGeneratedNever();

                entity.Property(e => e.EngagementName)
                    .IsRequired()
                    .HasMaxLength(75);

                entity.Property(e => e.TsclientId).HasColumnName("TSClientID");

                entity.Property(e => e.Type).HasMaxLength(30);
            });

            modelBuilder.Entity<TstimeEntries>(entity =>
            {
                entity.HasKey(e => e.TstimeEntryId);

                entity.ToTable("TSTimeEntries");

                entity.Property(e => e.TstimeEntryId)
                    .HasColumnName("TSTimeEntryID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Location).HasMaxLength(100);

                entity.Property(e => e.QboserviceItem)
                    .HasColumnName("QBOServiceItem")
                    .HasMaxLength(100);

                entity.Property(e => e.TsengagementId).HasColumnName("TSEngagementID");

                entity.Property(e => e.TsuserId).HasColumnName("TSUserID");

                entity.Property(e => e.Type).HasMaxLength(50);
            });

            modelBuilder.Entity<Tsusers>(entity =>
            {
                entity.HasKey(e => e.TsuserId);

                entity.ToTable("TSUsers");

                entity.Property(e => e.TsuserId)
                    .HasColumnName("TSUserID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Company).HasMaxLength(100);

                entity.Property(e => e.EmailAddress).HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
