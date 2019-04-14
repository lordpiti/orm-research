using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //            if (!optionsBuilder.IsConfigured)
            //            {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            //                optionsBuilder.UseNpgsql("User ID=postgres;Host=localhost;Port=5432;Database=research;Pooling=true;");

            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLoggerFactory(LoggerFactory);
        }

        public static readonly LoggerFactory LoggerFactory =
            new LoggerFactory(new[] {
                //new ConsoleLoggerProvider((_, __) => true, true);
                // https://elanderson.net/2018/10/entity-framework-core-logging/
                // https://docs.microsoft.com/en-us/ef/core/miscellaneous/logging#filtering-what-is-logged
                new ConsoleLoggerProvider((category, level) => category == DbLoggerCategory.Database.Command.Name
                       && level == LogLevel.Information, true)
            });

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

                entity.Property(e => e.UnitPrice)
                    .HasColumnName("Unit_Price");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("category_fk");
            });
        }
    }
}
