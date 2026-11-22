using FluentAssertions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Library_Book_Borrowing_System.Domain;
using Library_Book_Borrowing_System.Tests.Arbitraries;

namespace Library_Book_Borrowing_System.Tests.Properties
{
    public class InvariantProperties
    {
        // P2 · Інваріант
        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void P2_Invariant_available_copies_stay_within_bounds(Book book, bool[] operations)
        {
            foreach (var borrow in operations)
            {
                if (borrow && book.AvailableCopies > 0)
                    book.Borrow();
                else if (!borrow && book.AvailableCopies < book.TotalCopies)
                    book.Return();

                book.AvailableCopies.Should().BeInRange(0, book.TotalCopies);
            }
        }

        // P2 · Інваріант (рівень сервісу)
        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void P2_Invariant_missing_copies_equal_user_loans(Book book, User user, bool[] operations)
        {
            var fx = new LibraryFixture(book, user);

            foreach (var borrow in operations)
            {
                if (borrow && book.AvailableCopies > 0)
                    fx.Service.BorrowBook(user.Id, book.Isbn);
                else if (!borrow && user.BorrowedIsbns.Contains(book.Isbn))
                    fx.Service.ReturnBook(user.Id, book.Isbn);
            }

            (book.TotalCopies - book.AvailableCopies).Should().Be(user.BorrowedIsbns.Count);
        }
    }
}
