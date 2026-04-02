using BookLibrary.Data;
using BookLibrary.Interfaces;
using BookLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(BookLibraryContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Book>> GetBooksWithFiltersAsync(
        string genre = null,
        string author = null,
        int? yearFrom = null,
        int? yearTo = null,
        bool? isBorrowed = null,
        string orderBy = "Title",
        bool ascending = true)
    {
        IQueryable<Book> query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(genre))
        {
            query = query.Where(b => b.Genre == genre);
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(b => b.Author.Contains(author));
        }

        if (yearFrom.HasValue)
        {
            query = query.Where(b => b.PublicationYear >= yearFrom.Value);
        }

        if (yearTo.HasValue)
        {
            query = query.Where(b => b.PublicationYear <= yearTo.Value);
        }

        if (isBorrowed.HasValue)
        {
            query = query.Where(b => b.IsBorrowed == isBorrowed.Value);
        }

        query = orderBy switch
        {
            "Author" => ascending ? query.OrderBy(b => b.Author) : query.OrderByDescending(b => b.Author),
            "PublicationYear" => ascending ? query.OrderBy(b => b.PublicationYear) : query.OrderByDescending(b => b.PublicationYear),
            "Genre" => ascending ? query.OrderBy(b => b.Genre) : query.OrderByDescending(b => b.Genre),
            _ => ascending ? query.OrderBy(b => b.Title) : query.OrderByDescending(b => b.Title)
        };

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<IGrouping<string, Book>>> GetBooksGroupedByGenreAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .GroupBy(b => b.Genre)
            .ToListAsync();
    }

    public async Task<IEnumerable<IGrouping<string, Book>>> GetBooksGroupedByAuthorAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .GroupBy(b => b.Author)
            .ToListAsync();
    }

    public async Task<Book> GetBookWithLoansAsync(int bookId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(b => b.Loans.Where(l => !l.IsReturned))
            .FirstOrDefaultAsync(b => b.Id == bookId);
    }
}