using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Service;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class BorrowerController : Controller
    {
        private readonly BorrowerService _borrowerService;

        public BorrowerController(BorrowerService borrowerService)
        {
            _borrowerService = borrowerService;
        }

        // 📘 GET: /Borrower/
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var borrowers = string.IsNullOrWhiteSpace(searchTerm)
                ? await _borrowerService.GetAllBorrowersAsync()
                : await _borrowerService.SearchBorrowersAsync(searchTerm);

            ViewBag.SearchTerm = searchTerm;
            return View(borrowers);
        }

        // 📘 GET: /Borrower/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var borrower = await _borrowerService.GetBorrowerByIdAsync(id);
            if (borrower == null)
                return NotFound();

            return View(borrower);
        }

        // 📘 GET: /Borrower/Create
        public IActionResult Create() => View();

        // 📘 POST: /Borrower/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BorrowerDto borrower)
        {
            if (!ModelState.IsValid)
                return View(borrower);

            var success = await _borrowerService.AddBorrowerAsync(borrower);
            if (!success)
            {
                ModelState.AddModelError("", "Membership ID already exists. Please use a different one.");
                return View(borrower);
            }

            TempData["SuccessMessage"] = "Borrower added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // 📘 GET: /Borrower/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var borrower = await _borrowerService.GetBorrowerByIdAsync(id);
            if (borrower == null)
                return NotFound();

            return View(borrower);
        }

        // 📘 POST: /Borrower/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BorrowerDto borrower)
        {
            if (!ModelState.IsValid)
                return View(borrower);

            var success = await _borrowerService.UpdateBorrowerAsync(borrower);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to update borrower. Membership ID may already exist.");
                return View(borrower);
            }

            TempData["SuccessMessage"] = "Borrower details updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // 📘 GET: /Borrower/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var borrower = await _borrowerService.GetBorrowerByIdAsync(id);
            if (borrower == null)
                return NotFound();

            return View(borrower);
        }

        // 📘 POST: /Borrower/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _borrowerService.DeleteBorrowerAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = "Cannot delete borrower. They may have active borrowed books.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Borrower deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
