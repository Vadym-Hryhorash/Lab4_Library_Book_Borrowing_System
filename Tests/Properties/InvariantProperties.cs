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
                try
                {
                    if (borrow) book.Borrow();
                    else book.Return();
                }
                catch (InvalidOperationException)
                {
                    
                }

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
                try
                {
                    if (borrow) fx.Service.BorrowBook(user.Id, book.Isbn);
                    else fx.Service.ReturnBook(user.Id, book.Isbn);
                }
                catch (InvalidOperationException)
                {

                }
            }

            (book.TotalCopies - book.AvailableCopies).Should().Be(user.BorrowedIsbns.Count);
        }
    }
}
