using BookLibrary.Models;

namespace BookLibrary.Interfaces;

public interface IBookRepository : IRepository<Book>
{
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
    Task<Book> GetBookWithLoansAsync(int bookId);
}
