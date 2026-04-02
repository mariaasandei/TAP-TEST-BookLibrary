using BookLibrary.Data;
using BookLibrary.Interfaces;
using BookLibrary.Models;
using BookLibrary.Repositories;
using BookLibrary.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = new ServiceCollection()
    .AddDbContext<BookLibraryContext>(options =>
        options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=BookLibraryDb;Trusted_Connection=True;"))
    .AddScoped<IBookRepository, BookRepository>()
    .AddScoped<IBookLoanRepository, BookLoanRepository>()
    .AddScoped<IBookService, BookService>()
    .BuildServiceProvider();

using (var scope = serviceProvider.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BookLibraryContext>();
    await context.Database.EnsureCreatedAsync();
}

var bookService = serviceProvider.GetRequiredService<IBookService>();

Console.WriteLine("=== BookLibrary Application ===\n");

Console.WriteLine("1. Adding books...");
var book1 = await bookService.AddBookAsync(new Book
{
    Title = "Clean Code",
    Author = "Robert C. Martin",
    ISBN = "978-0132350884",
    PublicationYear = 2008,
    Genre = "Programming"
});

var book2 = await bookService.AddBookAsync(new Book
{
    Title = "Design Patterns",
    Author = "Gang of Four",
    ISBN = "978-0201633610",
    PublicationYear = 1994,
    Genre = "Programming"
});

var book3 = await bookService.AddBookAsync(new Book
{
    Title = "The Pragmatic Programmer",
    Author = "Andrew Hunt",
    ISBN = "978-0135957059",
    PublicationYear = 2019,
    Genre = "Programming"
});

var book4 = await bookService.AddBookAsync(new Book
{
    Title = "1984",
    Author = "George Orwell",
    ISBN = "978-0451524935",
    PublicationYear = 1949,
    Genre = "Fiction"
});

Console.WriteLine($"Added {book1.Title}, {book2.Title}, {book3.Title}, {book4.Title}\n");

using (var scope = serviceProvider.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BookLibraryContext>();

    var user1 = new User { Name = "John Doe", Email = "john@example.com" };
    var user2 = new User { Name = "Jane Smith", Email = "jane@example.com" };

    context.Users.Add(user1);
    context.Users.Add(user2);
    await context.SaveChangesAsync();

    Console.WriteLine($"2. Created users: {user1.Name}, {user2.Name}\n");

    Console.WriteLine("3. Borrowing books...");
    var borrowResult1 = await bookService.BorrowBookAsync(book1.Id, user1.Id, 14);
    Console.WriteLine($"User {user1.Name} borrowed '{book1.Title}': {(borrowResult1 ? "Success" : "Failed")}");

    var borrowResult2 = await bookService.BorrowBookAsync(book2.Id, user2.Id, 21);
    Console.WriteLine($"User {user2.Name} borrowed '{book2.Title}': {(borrowResult2 ? "Success" : "Failed")}");

    var borrowResult3 = await bookService.BorrowBookAsync(book1.Id, user2.Id, 14);
    Console.WriteLine($"User {user2.Name} tried to borrow '{book1.Title}' (already borrowed): {(borrowResult3 ? "Success" : "Failed")}\n");

    Console.WriteLine("4. Getting return dates...");
    var returnDate1 = await bookService.GetReturnDateForUserAsync(user1.Id, book1.Id);
    var returnDate2 = await bookService.GetReturnDateForUserAsync(user2.Id, book2.Id);

    Console.WriteLine($"Return date for {user1.Name} - '{book1.Title}': {returnDate1?.ToString("yyyy-MM-dd")}");
    Console.WriteLine($"Return date for {user2.Name} - '{book2.Title}': {returnDate2?.ToString("yyyy-MM-dd")}\n");
}

Console.WriteLine("5. Getting books with filters...");
var programmingBooks = await bookService.GetBooksWithFiltersAsync(genre: "Programming", orderBy: "PublicationYear", ascending: false);
Console.WriteLine($"Programming books (ordered by year desc):");
foreach (var book in programmingBooks)
{
    Console.WriteLine($"  - {book.Title} ({book.PublicationYear}) by {book.Author}");
}

Console.WriteLine();

var availableBooks = await bookService.GetBooksWithFiltersAsync(isBorrowed: false);
Console.WriteLine($"Available books:");
foreach (var book in availableBooks)
{
    Console.WriteLine($"  - {book.Title}");
}

Console.WriteLine();

Console.WriteLine("6. Getting books grouped by genre...");
var booksByGenre = await bookService.GetBooksGroupedByGenreAsync();
foreach (var group in booksByGenre)
{
    Console.WriteLine($"Genre: {group.Key}");
    foreach (var book in group)
    {
        Console.WriteLine($"  - {book.Title}");
    }
}

Console.WriteLine();

Console.WriteLine("7. Getting books grouped by author...");
var booksByAuthor = await bookService.GetBooksGroupedByAuthorAsync();
foreach (var group in booksByAuthor)
{
    Console.WriteLine($"Author: {group.Key}");
    foreach (var book in group)
    {
        Console.WriteLine($"  - {book.Title}");
    }
}

Console.WriteLine("\n=== Application Completed ===");