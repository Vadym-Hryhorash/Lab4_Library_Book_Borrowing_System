using FsCheck;
using FsCheck.Fluent;
using Library_Book_Borrowing_System.Domain;

namespace Library_Book_Borrowing_System.Tests.Arbitraries
{
    // Зразки з проєкту лекції: FuelTankGen (залежні значення), VehicleGen (генератор з генератора).
    public static class LibraryArbitraries
    {
        // ISBN: рядок з 10 або 13 цифр (Gen.OneOf двох генераторів).
        public static Arbitrary<string> Isbn()
        {
            var part5 = Gen.Choose(0, 99999);

            // 10 цифр = 5 + 5
            var isbn10 = from a in part5
                         from b in part5
                         select a.ToString("D5") + b.ToString("D5");

            // 13 цифр = 5 + 5 + 3
            var isbn13 = from a in part5
                         from b in part5
                         from c in Gen.Choose(0, 999)
                         select a.ToString("D5") + b.ToString("D5") + c.ToString("D3");

            return Arb.From(Gen.OneOf(isbn10, isbn13));
        }

        // Назва книги: Gen.Elements(...) з кількох назв.
        private static Gen<string> Title()
        {
            return Gen.Elements(new[]
            {
                "Clean Code",
                "The Pragmatic Programmer",
                "Domain-Driven Design",
                "Refactoring",
                "Кобзар",
                "Тіні забутих предків"
            });
        }

        // Book: використайте Isbn().Generator і Title(); кількість копій >= 1
        // (конструктор кидає ArgumentException для 0). Включіть граничний випадок 1.
        public static Arbitrary<Book> Book()
        {
            // Граничний випадок 1 гарантовано входить до OneOf, решта — 1..20.
            var copiesGen = Gen.OneOf(Gen.Constant(1), Gen.Choose(1, 20));

            var gen = from isbn in Isbn().Generator
                      from title in Title()
                      from copies in copiesGen
                      select new Book(isbn, title, copies);

            return Arb.From(gen);
        }

        // User: Id генерується автоматично (Guid), від генератора потрібне лише ім'я.
        public static Arbitrary<User> User()
        {
            var nameGen = Gen.Elements(new[] { "Олена", "Андрій", "Марія", "Vadym", "Alice", "Bob" });

            var gen = from name in nameGen
                      select new User(name);

            return Arb.From(gen);
        }
    }
}
