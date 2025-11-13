using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagementSystem.Service
{
    public class BorrowRecordService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BorrowRecordService> _logger;

        public BorrowRecordService(ApplicationDbContext context, ILogger<BorrowRecordService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ Borrow a book
        public async Task<string> BorrowBookAsync(int borrowerId, int bookId, DateTime borrowDate, DateTime dueDate)
        {
            var book = await _context.Books.FindAsync(bookId);
            var borrower = await _context.Borrowers.FindAsync(borrowerId);

            if (book == null || borrower == null)
                return "Invalid borrower or book ID.";

            if (book.Quantity <= 0)
                return $"No copies of '{book.Title}' are currently available.";

            // Create borrow record
            var record = new BorrowRecord
            {
                BookId = bookId,
                BorrowerId = borrowerId,
                BorrowDate = borrowDate,
                DueDate = dueDate
            };

            _context.BorrowRecords.Add(record);
            book.Quantity--; // Decrease book quantity
            await _context.SaveChangesAsync();

            _logger.LogInformation("Book borrowed: {Book} by {Borrower}", book.Title, borrower.Name);
            return "Book borrowed successfully!";
        }

        // ✅ Return a borrowed book
        public async Task<string> ReturnBookAsync(int borrowRecordId)
        {
            var record = await _context.BorrowRecords
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.BorrowRecordId == borrowRecordId);

            if (record == null)
                return "Borrow record not found.";

            if (record.ReturnDate != null)
                return "Book already returned.";

            record.ReturnDate = DateTime.Now;
            record.Book.Quantity++; // Increase book quantity

            await _context.SaveChangesAsync();
            _logger.LogInformation("Book returned: {Book}", record.Book.Title);
            return "Book returned successfully!";
        }

        // ✅ Get all borrow records
        public async Task<List<BorrowRecordDto>> GetAllBorrowRecordsAsync()
        {
            return await _context.BorrowRecords
                .Include(r => r.Book)
                .Include(r => r.Borrower)
                .Select(r => new BorrowRecordDto
                {
                    BorrowRecordId = r.BorrowRecordId,
                    BookId = r.BookId,
                    BookTitle = r.Book.Title,
                    BorrowerId = r.BorrowerId,
                    BorrowerName = r.Borrower.Name,
                    BorrowDate = r.BorrowDate,
                    DueDate = r.DueDate,
                    ReturnDate = r.ReturnDate,
                    IsOverdue = r.DueDate < DateTime.Now && r.ReturnDate == null
                }).ToListAsync();
        }

        // ✅ Get active (not returned) borrow records
        public async Task<List<BorrowRecordDto>> GetActiveBorrowsAsync()
        {
            return await _context.BorrowRecords
                .Include(r => r.Book)
                .Include(r => r.Borrower)
                .Where(r => r.ReturnDate == null)
                .Select(r => new BorrowRecordDto
                {
                    BorrowRecordId = r.BorrowRecordId,
                    BookTitle = r.Book.Title,
                    BorrowerName = r.Borrower.Name,
                    BorrowDate = r.BorrowDate,
                    DueDate = r.DueDate,
                    IsOverdue = r.DueDate < DateTime.Now
                }).ToListAsync();
        }

        // ✅ Get overdue borrow records
        public async Task<List<BorrowRecordDto>> GetOverdueRecordsAsync()
        {
            return await _context.BorrowRecords
                .Include(r => r.Book)
                .Include(r => r.Borrower)
                .Where(r => r.ReturnDate == null && r.DueDate < DateTime.Now)
                .Select(r => new BorrowRecordDto
                {
                    BorrowRecordId = r.BorrowRecordId,
                    BookTitle = r.Book.Title,
                    BorrowerName = r.Borrower.Name,
                    BorrowDate = r.BorrowDate,
                    DueDate = r.DueDate,
                    IsOverdue = true
                }).ToListAsync();
        }

        // ✅ Get record by ID
        public async Task<BorrowRecordDto?> GetByIdAsync(int id)
        {
            var record = await _context.BorrowRecords
                .Include(r => r.Book)
                .Include(r => r.Borrower)
                .FirstOrDefaultAsync(r => r.BorrowRecordId == id);

            if (record == null) return null;

            return new BorrowRecordDto
            {
                BorrowRecordId = record.BorrowRecordId,
                BookId = record.BookId,
                BookTitle = record.Book?.Title ?? "Unknown Book",
                BorrowerId = record.BorrowerId,
                BorrowerName = record.Borrower?.Name ?? "Unknown Borrower",
                BorrowDate = record.BorrowDate,
                DueDate = record.DueDate,
                ReturnDate = record.ReturnDate,
                IsOverdue = record.IsOverdue
            };
        }
        // ✅ Delete a borrow record (with safety checks)
        public async Task<bool> DeleteBorrowRecordAsync(int id)
        {
            var record = await _context.BorrowRecords
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.BorrowRecordId == id);

            if (record == null)
                return false;

            // If not returned yet, restore the book quantity before deleting
            if (record.ReturnDate == null && record.Book != null)
            {
                record.Book.Quantity++;
            }

            _context.BorrowRecords.Remove(record);

            try
            {
                await _context.SaveChangesAsync();
                _logger?.LogInformation("Borrow record deleted successfully (ID: {Id})", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error deleting borrow record with ID: {Id}", id);
                return false;
            }
        }

    }
}
