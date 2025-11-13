// 📁 Models/BorrowRecord.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class BorrowRecord
    {
        [Key]
        public int BorrowRecordId { get; set; }

        [Required, ForeignKey("Book")]
        public int BookId { get; set; }

        [Required, ForeignKey("Borrower")]
        public int BorrowerId { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime BorrowDate { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }

        // ✅ Navigation properties
        public Book Book { get; set; }
        public Borrower Borrower { get; set; }

        // ✅ Computed property
        [NotMapped]
        public bool IsOverdue => ReturnDate == null && DueDate < DateTime.Now;
    }
}
