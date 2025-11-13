using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Fetch counts
            var totalBooks = await _context.Books.CountAsync();
            var totalBorrowers = await _context.Borrowers.CountAsync();
            var totalRecords = await _context.BorrowRecords.CountAsync();

            // Active borrowed books (not returned yet)
            var activeBorrows = await _context.BorrowRecords.CountAsync(r => r.ReturnDate == null);

            // Overdue books
            var overdueBooks = await _context.BorrowRecords.CountAsync(r => r.ReturnDate == null && r.DueDate < DateTime.Now);

            // Pass to ViewBag
            ViewBag.TotalBooks = totalBooks;
            ViewBag.TotalBorrowers = totalBorrowers;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.ActiveBorrows = activeBorrows;
            ViewBag.OverdueBooks = overdueBooks;

            return View();
        }
    }
}
