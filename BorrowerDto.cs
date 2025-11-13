// 📁 Models/BorrowerDto.cs
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class BorrowerDto
    {
        public int BorrowerId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(50)]
        public string MembershipId { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Phone, StringLength(15)]
        public string Phone { get; set; }
    }
}
