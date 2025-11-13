using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Service;

namespace LibraryManagementSystem.Controllers
{
    public class BorrowRecordController : Controller
    {
        private readonly BorrowRecordService _borrowRecordService;
        private readonly BookService _bookService;
        private readonly BorrowerService _borrowerService;

        public BorrowRecordController(
            BorrowRecordService borrowRecordService,
            BookService bookService,
            BorrowerService borrowerService)
        {
            _borrowRecordService = borrowRecordService;
            _bookService = bookService;
            _borrowerService = borrowerService;
        }

        // ✅ List all borrow records
        public async Task<IActionResult> Index()
        {
            var records = await _borrowRecordService.GetAllBorrowRecordsAsync();
            return View(records);
        }

        // ✅ Borrow form (GET)
        public async Task<IActionResult> Borrow()
        {
            ViewBag.Books = new SelectList(await _bookService.GetAllBooksAsync(), "BookId", "Title");
            ViewBag.Borrowers = new SelectList(await _borrowerService.GetAllBorrowersAsync(), "BorrowerId", "Name");
            return View();
        }

        // ✅ Borrow form (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrow(int borrowerId, int bookId, DateTime borrowDate, DateTime dueDate)
        {
            if (borrowerId == 0 || bookId == 0)
            {
                ModelState.AddModelError("", "Borrower and Book are required.");
                ViewBag.Books = new SelectList(await _bookService.GetAllBooksAsync(), "BookId", "Title");
                ViewBag.Borrowers = new SelectList(await _borrowerService.GetAllBorrowersAsync(), "BorrowerId", "Name");
                return View();
            }

            var message = await _borrowRecordService.BorrowBookAsync(borrowerId, bookId, borrowDate, dueDate);
            TempData["Message"] = message;

            return RedirectToAction(nameof(Index));
        }

        // ✅ Return book (GET)
        public async Task<IActionResult> Return(int id)
        {
            var record = await _borrowRecordService.GetByIdAsync(id);
            if (record == null)
                return NotFound();

            return View(record);
        }

        // ✅ Return book (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnConfirmed(int borrowRecordId)
        {
            var message = await _borrowRecordService.ReturnBookAsync(borrowRecordId);
            TempData["Message"] = message;
            return RedirectToAction(nameof(Index));
        }

        // GET: BorrowRecord/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var record = await _borrowRecordService.GetByIdAsync(id);
            if (record == null)
                return NotFound();

            return View(record);
        }

        // POST: BorrowRecord/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _borrowRecordService.DeleteBorrowRecordAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Delete), new { id });
            }
        }


        // ✅ Show all active (unreturned) borrow records
        public async Task<IActionResult> Active()
        {
            var activeRecords = await _borrowRecordService.GetActiveBorrowsAsync();
            return View(activeRecords);
        }

        // ✅ Show all overdue records
        public async Task<IActionResult> Overdue()
        {
            var overdueRecords = await _borrowRecordService.GetOverdueRecordsAsync();
            return View(overdueRecords);
        }

        // ✅ Show all active (unreturned) records
     

    }
}
