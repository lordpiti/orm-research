using Microsoft.EntityFrameworkCore;

namespace DataAccess.EFModels
{
    public partial class researchContext : DbContext
    {
        public researchContext()
        {
        }

        public researchContext(DbContextOptions<researchContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Product> Product { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseNpgsql("User ID=postgres;Host=localhost;Port=5432;Database=research;Pooling=true;");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category", "test");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseNpgsqlIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product", "test");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("fki_category_fk");

                entity.Property(e => e.Id).UseNpgsqlIdentityAlwaysColumn();

                entity.Property(e => e.CategoryId).HasColumnName("categoryId");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("category_fk");
            });
        }
    }
}
