using BookLibrary.Models;

namespace BookLibrary.Interfaces;

public interface IBookLoanRepository : IRepository<BookLoan>
{
    Task<BookLoan> GetActiveBookLoanAsync(int bookId);
    Task<IEnumerable<BookLoan>> GetUserLoansAsync(int userId);
    Task<BookLoan> GetUserBookLoanAsync(int userId, int bookId);
}