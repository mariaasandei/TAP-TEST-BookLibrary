using BookLibrary.Models;

namespace BookLibrary.Interfaces;

public interface IBookService
{
    Task<Book> AddBookAsync(Book book);
    Task<Book> GetBookAsync(int bookId);
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<bool> BorrowBookAsync(int bookId, int userId, int loanDurationDays = 14);
    Task<DateTime?> GetReturnDateForUserAsync(int userId, int bookId);
    Task<IEnumerable<Book>> GetBooksWithFiltersAsync(
        string genre = null,
        string author = null,
        int? yearFrom = null,
        int? yearTo = null,
        bool? isBorrowed = null,
        string orderBy = "Title",
        bool ascending = true);
    Task<IEnumerable<IGrouping<string, Book>>> GetBooksGroupedByGenreAsync();
    Task<IEnumerable<IGrouping<string, Book>>> GetBooksGroupedByAuthorAsync();
}