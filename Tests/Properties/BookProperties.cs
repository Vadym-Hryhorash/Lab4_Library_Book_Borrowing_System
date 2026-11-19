using FluentAssertions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Library_Book_Borrowing_System.Domain;
using Library_Book_Borrowing_System.Tests.Arbitraries;

namespace Library_Book_Borrowing_System.Tests.Properties
{
    public class BookProperties
    {
        // P6 · Передбачувана зміна (метаморфна властивість)
        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public Property P6_Metamorphic_two_borrows_take_twice_as_many_copies(Book book)
        {
            bool applicable = book.TotalCopies >= 2;
            bool ok = true;

            if (applicable)
            {
                var once = LibraryFixture.CopyBook(book);
                var twice = LibraryFixture.CopyBook(book);

                once.Borrow();
                twice.Borrow();
                twice.Borrow();

                var takenOnce = book.TotalCopies - once.AvailableCopies;
                var takenTwice = book.TotalCopies - twice.AvailableCopies;

                ok = takenTwice == 2 * takenOnce;
            }

            return ok.When(applicable);
        }
    }
}
