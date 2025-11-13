using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace LibraryManagementSystem.Service
{
    public class BookService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookService> _logger;


        public BookService(ApplicationDbContext context, ILogger<BookService> logger)
        {
            _context = context;
            _logger = logger;
        }


        // Add new book (async)
        public async Task<BookDto> AddBookAsync(BookDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));


            // Validate uniqueness of ISBN
            if (await _context.Books.AnyAsync(b => b.ISBN == dto.ISBN))
                throw new InvalidOperationException("A book with the same ISBN already exists.");


            var book = new Book
            {
                Title = dto.Title?.Trim(),
                Author = dto.Author?.Trim(),
                ISBN = dto.ISBN?.Trim(),
                Genre = dto.Genre?.Trim(),
                Quantity = Math.Max(0, dto.Quantity)
            };


            _context.Books.Add(book);
            await _context.SaveChangesAsync();


            dto.BookId = book.BookId;
            return dto;
        }


        // Update book (async) with concurrency handling
        public async Task<BookDto> UpdateBookAsync(BookDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));


            var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == dto.BookId);
            if (book == null) throw new KeyNotFoundException("Book not found.");


            // If ISBN changed, ensure uniqueness
            if (!string.Equals(book.ISBN, dto.ISBN, StringComparison.OrdinalIgnoreCase))
            {
                if (await _context.Books.AnyAsync(b => b.ISBN == dto.ISBN && b.BookId != dto.BookId))
                    throw new InvalidOperationException("Another book with the same ISBN exists.");
            }


            book.Title = dto.Title?.Trim();
            book.Author = dto.Author?.Trim();
            book.ISBN = dto.ISBN?.Trim();
            book.Genre = dto.Genre?.Trim();
            book.Quantity = Math.Max(0, dto.Quantity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict when updating Book {BookId}", dto.BookId);
                throw new InvalidOperationException("The book was modified by another user. Please reload and retry.");
            }


            return dto;
        }


        // Delete book
        public async Task DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) throw new KeyNotFoundException("Book not found.");


            // Prevent deletion if any active borrow records exist
            var hasActiveBorrows = await _context.BorrowRecords.AnyAsync(r => r.BookId == id && r.ReturnDate == null);
            if (hasActiveBorrows) throw new InvalidOperationException("Cannot delete a book that has active borrow records.");


            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }


        // Get all books
        public async Task<List<BookDto>> GetAllBooksAsync()
        {
            return await _context.Books
            .OrderBy(b => b.Title)
            .Select(b => new BookDto
            {
                BookId = b.BookId,
                Title = b.Title,
                Author = b.Author,
                ISBN = b.ISBN,
                Genre = b.Genre,
                Quantity = b.Quantity
            }).ToListAsync();
        }
        // Search books (title, author, genre) - case insensitive
        public async Task<List<BookDto>> SearchBooksAsync(string keyword)
        {
            keyword = (keyword ?? string.Empty).Trim().ToLower();
            return await _context.Books
            .Where(b => b.Title.ToLower().Contains(keyword) || b.Author.ToLower().Contains(keyword) || b.Genre.ToLower().Contains(keyword))
            .OrderBy(b => b.Title)
            .Select(b => new BookDto
            {
                BookId = b.BookId,
                Title = b.Title,
                Author = b.Author,
                ISBN = b.ISBN,
                Genre = b.Genre,
                Quantity = b.Quantity
            }).ToListAsync();
        }


        // Get book by id
        public async Task<BookDto> GetBookByIdAsync(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null) return null;


            return new BookDto
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Genre = book.Genre,
                Quantity = book.Quantity
            };
        }


        // Get list of available books (Quantity > number of active borrows)
        public async Task<List<BookDto>> GetAvailableBooksAsync()
        {
            var books = await _context.Books
            .Select(b => new
            {
                Book = b,
                ActiveBorrows = b.BorrowRecords.Count(r => r.ReturnDate == null)
            }).ToListAsync();


            return books
            .Where(x => x.Book.Quantity - x.ActiveBorrows > 0)
            .Select(x => new BookDto
            {
                BookId = x.Book.BookId,
                Title = x.Book.Title,
                Author = x.Book.Author,
                ISBN = x.Book.ISBN,
                Genre = x.Book.Genre,
                Quantity = x.Book.Quantity - x.ActiveBorrows // returning available count here
            }).ToList();
        }
    }
}
    
