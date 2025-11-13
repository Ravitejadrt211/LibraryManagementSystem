using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalBooks = await _context.Books.CountAsync(),
                TotalBorrowers = await _context.Borrowers.CountAsync(),
                ActiveBorrows = await _context.BorrowRecords.CountAsync(r => r.ReturnDate == null),
                OverdueBooks = await _context.BorrowRecords.CountAsync(r => r.ReturnDate == null && r.DueDate < DateTime.Now)
            };

            return View(viewModel);
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
