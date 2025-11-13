// 📁 Models/BorrowRecordDto.cs
using System;

namespace LibraryManagementSystem.Models
{
    public class BorrowRecordDto
    {
        public int BorrowRecordId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public int BorrowerId { get; set; }
        public string BorrowerName { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsOverdue { get; set; }
      

    }
}
