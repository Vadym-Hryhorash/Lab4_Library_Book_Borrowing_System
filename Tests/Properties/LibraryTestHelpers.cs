using Library_Book_Borrowing_System.Domain;

namespace Library_Book_Borrowing_System.Tests.Properties
{
    /// <summary>
    /// Допоміжний клас: репозиторій + сервіс з однією книгою та кількома користувачами.
    /// Copy* — незалежні копії (як Copy/Clone у лекції) для тестів з двома шляхами виконання.
    /// </summary>
    internal sealed class LibraryFixture
    {
        public InMemoryLibraryRepository Repo { get; } = new();
        public LibraryService Service { get; }
        public Book Book { get; }

        public LibraryFixture(Book book, params User[] users)
        {
            Book = book;
            Repo.AddBook(book);
            foreach (var user in users)
                Repo.AddUser(user);

            Service = new LibraryService(Repo);
        }

        public static Book CopyBook(Book b) => new Book(b.Isbn, b.Title, b.TotalCopies);

        public static User CopyUser(User u) => new User(u.Name);
    }
}
