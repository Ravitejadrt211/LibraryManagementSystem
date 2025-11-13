using Microsoft.EntityFrameworkCore;


namespace LibraryManagementSystem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public DbSet<Book> Books { get; set; }
        public DbSet<Borrower> Borrowers { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Book>()
            .HasIndex(b => b.ISBN)
            .IsUnique();


            modelBuilder.Entity<Borrower>()
            .HasIndex(b => b.MembershipId)
            .IsUnique();


            modelBuilder.Entity<BorrowRecord>()
            .HasOne(r => r.Book)
            .WithMany(b => b.BorrowRecords)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Borrower>()
       .HasIndex(b => b.MembershipId)
       .IsUnique();

            modelBuilder.Entity<Borrower>()
                .HasMany(b => b.BorrowRecords)
                .WithOne(r => r.Borrower)
                .HasForeignKey(r => r.BorrowerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}