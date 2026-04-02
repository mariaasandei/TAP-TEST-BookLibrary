using BookLibrary.Interfaces;
using BookLibrary.Models;
using BookLibrary.Repositories;

namespace BookLibrary.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookLoanRepository _bookLoanRepository;

    public BookService(IBookRepository bookRepository, IBookLoanRepository bookLoanRepository)
    {
        _bookRepository = bookRepository;
        _bookLoanRepository = bookLoanRepository;
    }

    public async Task<Book> AddBookAsync(Book book)
    {
        await _bookRepository.AddAsync(book);
        await _bookRepository.SaveChangesAsync();
        return book;
    }

    public async Task<Book> GetBookAsync(int bookId)
    {
        return await _bookRepository.GetByIdAsync(bookId);
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _bookRepository.GetAllAsync();
    }

    public async Task<bool> BorrowBookAsync(int bookId, int userId, int loanDurationDays = 14)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);

        if (book == null)
        {
            throw new InvalidOperationException("Book not found.");
        }

        if (book.IsBorrowed)
        {
            return false;
        }

        var activeLoan = await _bookLoanRepository.GetActiveBookLoanAsync(bookId);
        if (activeLoan != null)
        {
            return false;
        }

        var bookLoan = new BookLoan
        {
            BookId = bookId,
            UserId = userId,
            BorrowDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(loanDurationDays),
            IsReturned = false
        };

        book.IsBorrowed = true;

        await _bookLoanRepository.AddAsync(bookLoan);
        await _bookRepository.UpdateAsync(book);
        await _bookLoanRepository.SaveChangesAsync();

        return true;
    }

    public async Task<DateTime?> GetReturnDateForUserAsync(int userId, int bookId)
    {
        var loan = await _bookLoanRepository.GetUserBookLoanAsync(userId, bookId);
        return loan?.DueDate;
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
        return await _bookRepository.GetBooksWithFiltersAsync(
            genre, author, yearFrom, yearTo, isBorrowed, orderBy, ascending);
    }

    public async Task<IEnumerable<IGrouping<string, Book>>> GetBooksGroupedByGenreAsync()
    {
        return await _bookRepository.GetBooksGroupedByGenreAsync();
    }

    public async Task<IEnumerable<IGrouping<string, Book>>> GetBooksGroupedByAuthorAsync()
    {
        return await _bookRepository.GetBooksGroupedByAuthorAsync();
    }
}