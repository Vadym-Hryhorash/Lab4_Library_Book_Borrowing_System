using FluentAssertions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Library_Book_Borrowing_System.Domain;
using Library_Book_Borrowing_System.Tests.Arbitraries;

namespace Library_Book_Borrowing_System.Tests.Properties
{
    public class UserProperties
    {
        // P1 · Туди й назад
        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void P1_RoundTrip_borrow_then_return(Book book, User user)
        {
            var fx = new LibraryFixture(book, user);
            var availableBefore = book.AvailableCopies;

            fx.Service.BorrowBook(user.Id, book.Isbn);
            fx.Service.ReturnBook(user.Id, book.Isbn);

            book.AvailableCopies.Should().Be(availableBefore);
            user.BorrowedIsbns.Should().BeEmpty();
        }

        // P4 · Різні шляхи — один результат
        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public Property P4_DifferentPaths_borrow_order_does_not_matter(Book book, User first, User second)
        {
            bool applicable = book.TotalCopies >= 2;
            bool ok = true;

            if (applicable)
            {
                var a1 = LibraryFixture.CopyUser(first);
                var a2 = LibraryFixture.CopyUser(second);
                var pathA = new LibraryFixture(LibraryFixture.CopyBook(book), a1, a2);

                var b1 = LibraryFixture.CopyUser(first);
                var b2 = LibraryFixture.CopyUser(second);
                var pathB = new LibraryFixture(LibraryFixture.CopyBook(book), b1, b2);

                pathA.Service.BorrowBook(a1.Id, book.Isbn);
                pathA.Service.BorrowBook(a2.Id, book.Isbn);

                pathB.Service.BorrowBook(b2.Id, book.Isbn);
                pathB.Service.BorrowBook(b1.Id, book.Isbn);

                ok = pathA.Book.AvailableCopies == pathB.Book.AvailableCopies
                  && a1.BorrowedIsbns.SequenceEqual(b1.BorrowedIsbns)
                  && a2.BorrowedIsbns.SequenceEqual(b2.BorrowedIsbns);
            }

            return ok.When(applicable);
        }

        // P5 · Оракул
        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void P5_Oracle_service_borrow_equals_direct_borrow(Book book, User user)
        {
            var viaService = LibraryFixture.CopyBook(book);
            var direct = LibraryFixture.CopyBook(book);

            var fx = new LibraryFixture(viaService, user);

            fx.Service.BorrowBook(user.Id, book.Isbn);
            direct.Borrow();

            viaService.AvailableCopies.Should().Be(direct.AvailableCopies);
            user.BorrowedIsbns.Should().Equal(book.Isbn);
        }
    }
}
