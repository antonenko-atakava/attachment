using Database.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Database.Database;

public class DatabaseContext : DbContext
{
    public DbSet<Attachment> Attachments { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.ToTable("attachment");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()").IsRequired();
            entity.Property(e => e.Extension).HasColumnName("extension").IsRequired();
            entity.Property(e => e.FileSize).HasColumnName("file_size").IsRequired();
            entity.Property(e => e.FilePath).HasColumnName("file_path").IsRequired();
            entity.Property(e => e.CreateAt).HasColumnName("create_at").IsRequired();
            entity.Property(e => e.Success).HasColumnName("success").IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}