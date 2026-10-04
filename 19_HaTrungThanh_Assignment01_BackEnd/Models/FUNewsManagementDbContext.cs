using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Models
{
    public class FUNewsManagementDbContext : DbContext
    {
        public FUNewsManagementDbContext()
        {
        }

        public FUNewsManagementDbContext(DbContextOptions<FUNewsManagementDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<NewsArticle> NewsArticles { get; set; } = null!;
        public virtual DbSet<NewsTag> NewsTags { get; set; } = null!;
        public virtual DbSet<SystemAccount> SystemAccounts { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                var configuration = builder.Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? configuration.GetConnectionString("FUNewsManagementDB")
                    ?? "Server=.\\SQLEXPRESS;Database=19_HaTrungThanh_Assignment;Trusted_Connection=True;TrustServerCertificate=True;";
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NewsTag>(entity =>
            {
                entity.HasKey(e => new { e.NewsArticleID, e.TagID });

                entity.HasOne(d => d.NewsArticle)
                    .WithMany(p => p.NewsTags)
                    .HasForeignKey(d => d.NewsArticleID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_NewsTag_NewsArticle");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.NewsTags)
                    .HasForeignKey(d => d.TagID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_NewsTag_Tag");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasOne(d => d.ParentCategory)
                    .WithMany(p => p.SubCategories)
                    .HasForeignKey(d => d.ParentCategoryID)
                    .HasConstraintName("FK_Category_Category");
            });

            modelBuilder.Entity<NewsArticle>(entity =>
            {
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.NewsArticles)
                    .HasForeignKey(d => d.CategoryID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_NewsArticle_Category");

                entity.HasOne(d => d.CreatedBy)
                    .WithMany(p => p.CreatedArticles)
                    .HasForeignKey(d => d.CreatedByID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_NewsArticle_SystemAccount");
            });
        }
    }
}
