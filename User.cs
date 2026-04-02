using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookLibrary.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public ICollection<BookLoan> Loans { get; set; } = new List<BookLoan>();
}