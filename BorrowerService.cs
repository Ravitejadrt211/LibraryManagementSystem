using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagementSystem.Service
{
    public class BorrowerService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BorrowerService> _logger;

        public BorrowerService(ApplicationDbContext context, ILogger<BorrowerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ Add a new borrower
        public async Task<bool> AddBorrowerAsync(BorrowerDto dto)
        {
            // Check duplicate membership ID
            if (await _context.Borrowers.AnyAsync(b => b.MembershipId == dto.MembershipId))
            {
                _logger.LogWarning("Membership ID already exists: {MembershipId}", dto.MembershipId);
                return false;
            }

            var borrower = new Borrower
            {
                Name = dto.Name.Trim(),
                MembershipId = dto.MembershipId.Trim(),
                Email = dto.Email?.Trim(),
                Phone = dto.Phone?.Trim()
            };

            _context.Borrowers.Add(borrower);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Borrower added: {Name}", dto.Name);

            return true;
        }

        // ✅ Update existing borrower
        public async Task<bool> UpdateBorrowerAsync(BorrowerDto dto)
        {
            var borrower = await _context.Borrowers.FindAsync(dto.BorrowerId);
            if (borrower == null)
            {
                _logger.LogWarning("Borrower not found for update: ID {BorrowerId}", dto.BorrowerId);
                return false;
            }

            // Ensure unique Membership ID
            if (await _context.Borrowers.AnyAsync(b => b.MembershipId == dto.MembershipId && b.BorrowerId != dto.BorrowerId))
            {
                _logger.LogWarning("Duplicate Membership ID detected on update: {MembershipId}", dto.MembershipId);
                return false;
            }

            borrower.Name = dto.Name.Trim();
            borrower.MembershipId = dto.MembershipId.Trim();
            borrower.Email = dto.Email?.Trim();
            borrower.Phone = dto.Phone?.Trim();

            _context.Update(borrower);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Borrower updated: {BorrowerId}", borrower.BorrowerId);
            return true;
        }

        // ✅ Delete borrower (only if no active borrow records)
        public async Task<bool> DeleteBorrowerAsync(int borrowerId)
        {
            var borrower = await _context.Borrowers
                .Include(b => b.BorrowRecords)
                .FirstOrDefaultAsync(b => b.BorrowerId == borrowerId);

            if (borrower == null)
            {
                _logger.LogWarning("Borrower not found for deletion: ID {BorrowerId}", borrowerId);
                return false;
            }

            bool hasActiveBorrows = borrower.BorrowRecords.Any(r => r.ReturnDate == null);
            if (hasActiveBorrows)
            {
                _logger.LogWarning("Cannot delete borrower {BorrowerId} — active borrow records exist", borrowerId);
                return false;
            }

            _context.Borrowers.Remove(borrower);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Borrower deleted: ID {BorrowerId}", borrowerId);
            return true;
        }

        // ✅ Get all borrowers
        public async Task<List<BorrowerDto>> GetAllBorrowersAsync()
        {
            return await _context.Borrowers
                .Select(b => new BorrowerDto
                {
                    BorrowerId = b.BorrowerId,
                    Name = b.Name,
                    MembershipId = b.MembershipId,
                    Email = b.Email,
                    Phone = b.Phone
                }).ToListAsync();
        }

        // ✅ Get borrower by ID
        public async Task<BorrowerDto?> GetBorrowerByIdAsync(int id)
        {
            var borrower = await _context.Borrowers.FirstOrDefaultAsync(b => b.BorrowerId == id);
            if (borrower == null) return null;

            return new BorrowerDto
            {
                BorrowerId = borrower.BorrowerId,
                Name = borrower.Name,
                MembershipId = borrower.MembershipId,
                Email = borrower.Email,
                Phone = borrower.Phone
            };
        }

        // ✅ Search borrowers by name or membership ID
        public async Task<List<BorrowerDto>> SearchBorrowersAsync(string keyword)
        {
            keyword = keyword?.Trim() ?? string.Empty;
            return await _context.Borrowers
                .Where(b => b.Name.Contains(keyword) || b.MembershipId.Contains(keyword))
                .Select(b => new BorrowerDto
                {
                    BorrowerId = b.BorrowerId,
                    Name = b.Name,
                    MembershipId = b.MembershipId,
                    Email = b.Email,
                    Phone = b.Phone
                }).ToListAsync();
        }
    }
}
