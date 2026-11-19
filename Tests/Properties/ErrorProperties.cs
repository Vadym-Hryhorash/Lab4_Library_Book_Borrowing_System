using FluentAssertions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Library_Book_Borrowing_System.Domain;
using Library_Book_Borrowing_System.Tests.Arbitraries;

namespace Library_Book_Borrowing_System.Tests.Properties
{
    public class ErrorProperties
    {
        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void Book_constructor_with_less_than_one_copy_throws(string isbn, string title, NonNegativeInt n)
        {
            Action act = () => new Book(isbn, title, -n.Get);
            act.Should().Throw<ArgumentException>();
        }

        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void Borrow_when_no_copies_left_throws(Book book)
        {
            for (int i = 0; i < book.TotalCopies; i++)
                book.Borrow();

            Action act = () => book.Borrow();
            act.Should().Throw<InvalidOperationException>();
        }

        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void Return_when_all_copies_present_throws(Book book)
        {
            Action act = () => book.Return();
            act.Should().Throw<InvalidOperationException>();
        }

        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void ReturnBook_not_borrowed_by_user_throws(Book book, User user)
        {
            var fx = new LibraryFixture(book, user);

            Action act = () => fx.Service.ReturnBook(user.Id, book.Isbn);

            act.Should().Throw<InvalidOperationException>();
            book.AvailableCopies.Should().Be(book.TotalCopies);
        }

        // P8 · Без винятків
        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void P8_NoException_borrow_of_available_book_never_throws(Book book, User user)
        {
            var fx = new LibraryFixture(book, user);

            Action act = () => fx.Service.BorrowBook(user.Id, book.Isbn);

            act.Should().NotThrow();
        }
    }
}
