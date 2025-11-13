using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace LibraryManagementSystem.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }


        [Required]
        [StringLength(150)]
        public string Title { get; set; }


        [Required]
        [StringLength(100)]
        public string Author { get; set; }


        [Required]
        [StringLength(20)]
        public string ISBN { get; set; }


        [StringLength(50)]
        public string Genre { get; set; }


        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }


        // Concurrency token to prevent race conditions when borrowing/updating
        [Timestamp]
        public byte[] RowVersion { get; set; }


        // Navigation
        public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
    }
}