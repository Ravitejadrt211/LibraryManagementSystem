// 📁 Models/Borrower.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Borrower
    {
        [Key]
        public int BorrowerId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(50)]
        public string MembershipId { get; set; } // Unique member code

        [Required, EmailAddress]
        public string Email { get; set; }

        [Phone, StringLength(15)]
        public string Phone { get; set; }

        public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
    }
}
