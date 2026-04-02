using BookLibrary.Data;
using BookLibrary.Interfaces;
using BookLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Repositories;

public class BookLoanRepository : Repository<BookLoan>, IBookLoanRepository
{
    public BookLoanRepository(BookLibraryContext context) : base(context)
    {
    }

    public async Task<BookLoan> GetActiveBookLoanAsync(int bookId)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(bl => bl.BookId == bookId && !bl.IsReturned);
    }

    public async Task<IEnumerable<BookLoan>> GetUserLoansAsync(int userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(bl => bl.Book)
            .Where(bl => bl.UserId == userId)
            .ToListAsync();
    }

    public async Task<BookLoan> GetUserBookLoanAsync(int userId, int bookId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(bl => bl.Book)
            .FirstOrDefaultAsync(bl => bl.UserId == userId && bl.BookId == bookId && !bl.IsReturned);
    }
}