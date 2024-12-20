using Microsoft.EntityFrameworkCore;

public class LibraryContext : DbContext{
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookType> BookType { get; set; }
        public DbSet<BookLocation> BooksLocation { get; set; }
        public DbSet<Review> Reviews { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder){
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CustomerId)
                .IsRequired();

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookId)
                .IsRequired();

            modelBuilder.Entity<Book>()
                .HasOne(b => b.BookType)
                .WithMany(bt => bt.Books)
                .HasForeignKey(b => b.BookTypeId)
                .IsRequired();
        }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseMySql("server=__;user=__;password=__;database=__", 
            new MySqlServerVersion(new Version(8, 0, 34)));
    }
}

    public class User{
        public int Id { get; set;}
        public string Username { get; set;}
        public string Password { get; set;}

        public Customer Customer { get; set;}
        public Book Book { get; set;}       
    }

    public class Customer{
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }

    public class Book{
        public int Id { get; set; }
        public string Name { get; set; }

        public int BookLocationId { get; set; }
        public BookLocation BookLocation { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public  int BookTypeId { get; set; }
        public BookType BookType { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }

    public class BookType{
        public int Id { get; set;}
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
    
    public class BookLocation{
        public int Id { get; set;}
        public string Location { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }

    public class Review{
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }

        public int BookId { get; set; }
        public  Book Book { get; set; }

        public int CustomerId { get; set; }
        public  Customer Customer { get; set; }
    }

    public class SignInResult {
        public string Role { get; set; }
        public int? UserId { get; set; }
    }

internal class Program{
    private static void Main(string[] args){
        Console.WriteLine("Kütüphane Programına Hoşgeldiniz.");
        SignInSignUp();
    }

            static int TakeNumber(int range1, int range2){
            while (true){
                Console.Write($"Bir rakam girin ({range1}-{range2} arası): ");
                string input = Console.ReadLine() ;

                if (int.TryParse(input, out int number) && number >= range1 && number <= range2){
                    return number;
                } else{
                    Console.WriteLine("Geçersiz giriş! Lütfen belirtilen aralıkta bir rakam girin.");
                }
            }
        }
        
        static void SignInSignUp() {
            while (true) {
                Console.WriteLine("\nMenü");
                Console.WriteLine("1 - Giriş Yap");
                Console.WriteLine("2 - Kayıt Ol");
                Console.WriteLine("0 - Çıkış Yap");
                int choice = TakeNumber(0, 2);

                switch (choice) {
                    case 0:
                        Console.WriteLine("Uygulamadan çıkılıyor...");
                        return;
                    case 1:
                        var signInResult = SignIn();
                        if (signInResult != null && signInResult.Role == "admin") {
                            StaffMenu();
                        } else if (signInResult != null && signInResult.Role == "customer") {
                            CustomerMenu(signInResult.UserId);
                        }
                        break;
                    case 2:
                        SignUp();
                        break;
                    default:
                        Console.WriteLine("Lütfen sadece 0-2 arasında bir değer girin.");
                        break;
                }
            }
        }

        static SignInResult SignIn() {
            Console.WriteLine("\n- Giriş Yap -");
            Console.Write("Kullanıcı Adı: ");
            string username = Console.ReadLine();
            Console.Write("Şifre: ");
            string password = Console.ReadLine();

            if (username == "admin" && password == "admin") {
                Console.WriteLine("Admin girişi başarılı!");
                return new SignInResult { Role = "admin", UserId = null };
            } else {
                using (var db = new LibraryContext()) {
                    var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

                    if (user != null) {
                        Console.WriteLine("Kullanıcı girişi başarılı!");
                        return new SignInResult { Role = "customer", UserId = user.Id };
                    } else {
                        Console.WriteLine("Hatalı kullanıcı adı veya şifre.");
                        return null;
                    }
                }
            }
        }

        static void SignUp() {
            Console.WriteLine("\n- Kayıt Ol -");
            string username;
            
            while (true) {
                Console.Write("Kullanıcı Adı: ");
                username = Console.ReadLine();

                using (var db = new LibraryContext()) {
                    var existingUser = db.Users.FirstOrDefault(u => u.Username == username);
                    if (existingUser != null) {
                        Console.WriteLine("Bu kullanıcı adı zaten alınmış. Lütfen başka bir kullanıcı adı girin.");
                    } else {
                        break;
                    }
                }
            }

            Console.Write("Şifre: ");
            string password = Console.ReadLine();

            Console.Write("Ad: ");
            string name = Console.ReadLine();

            Console.Write("Soyad: ");
            string surname = Console.ReadLine();

            using (var db = new LibraryContext()) {
                var user = new User { Username = username, Password = password };
                db.Users.Add(user);
                db.SaveChanges();

                var customer = new Customer { Name = name, Surname = surname, User = user };
                db.Customers.Add(customer);
                db.SaveChanges();
            }

            Console.WriteLine("Kayıt işleminiz tamamlanmıştır.");
        }
        
        static void StaffMenu(){
            while (true){
                Console.WriteLine("\nPersonel Menüsü");
                Console.WriteLine("1 - Kitap Ekle");
                Console.WriteLine("2 - Kitap Türü Ekle");
                Console.WriteLine("3 - Kitap Ödünç Alma");
                Console.WriteLine("4 - Kitap Teslim Alma");
                Console.WriteLine("0 - Çıkış Yap");

                int choice = TakeNumber(0, 4);

                switch (choice){
                    case 0:
                        Console.WriteLine("Giriş/Kayıt Sayfasına Yönlendiriliyor...");
                        return;
                    case 1:
                        Console.WriteLine("Menü 1: Kitap Ekle");
                        AddBook();
                        break;
                    case 2:
                        Console.WriteLine("Menü 2: Kitap Türü Ekle");
                        AddBookType();
                        break;
                    case 3:
                        Console.WriteLine("Menü 3: Kitap Ödünç Alma");
                        LendBook();
                        break;
                    case 4:
                        Console.WriteLine("Menü 4: Kitap Teslim Alma");
                        ReturnBook();
                        break;
                    default:
                        Console.WriteLine("Lütfen sadece 0-4 arasında bir değer girin.");
                        break;
                }
            }
        }
        
        static void CustomerMenu(int? userId){
            while (true){
                Console.WriteLine("\nMüşteri Menüsü");
                Console.WriteLine("1 - Tüm Kitaplar");
                Console.WriteLine("2 - Kitap Ara");
                Console.WriteLine("3 - Kitap Yorumu Ekleme");
                Console.WriteLine("4 - Kitap Yorumlarını İnceleme");
                Console.WriteLine("0 - Çıkış Yap");

                int choice = TakeNumber(0, 5);

                switch (choice){
                    case 0:
                        Console.WriteLine("Giriş/Kayıt Sayfasına Yönlendiriliyor...");
                        return;
                    case 1:
                        Console.WriteLine("Menü 1: Tüm Kitaplar Getiriliyor...");
                        ListBooks();
                        break;
                    case 2:
                        Console.WriteLine("Menü 2: Kitap Arama Sayfası Getiriliyor...");
                        ListBook();
                        break;
                    case 3:
                        Console.WriteLine("Menü 4: Kitap Yorumu Ekleme Sayfası Getiriliyor...");
                        AddBookReview(userId.Value);
                        break;
                    case 4:
                        Console.WriteLine("Menü 5: Kitap Yorumlarını İnceleme Sayfası Getiriliyor...");
                        ListBookReview();
                        break;
                    default:
                        Console.WriteLine("Lütfen sadece 0-5 arasında bir değer girin.");
                        break;
                }
            }
        }
        
        static void AddBook(){
            using (var db = new LibraryContext()){
    Console.WriteLine("\n- Kitap Ekle -");

    string bookName = null;
    bool isBookNameUnique = false;

    while (!isBookNameUnique){
        Console.Write("Kitap Adı: ");
        bookName = Console.ReadLine();

        var existingBook = db.Books
            .AsEnumerable() // Switch to in-memory comparison
            .FirstOrDefault(b => b.Name.Equals(bookName, StringComparison.OrdinalIgnoreCase));
        if (existingBook == null){
            isBookNameUnique = true;
        } else {
            Console.WriteLine("Bu kitap adı zaten mevcut. Lütfen başka bir isim girin.");
        }
    }

    Console.WriteLine("\nMevcut Kitap Türleri:");
    var bookTypes = db.BookType.ToList();
    foreach (var bookType in bookTypes){
        Console.WriteLine($"ID: {bookType.Id}, Tür: {bookType.Name}");
    }

    BookType selectedBookType = null;
    while (selectedBookType == null){
        Console.Write("\nKitap Türü (ID): ");
        if (int.TryParse(Console.ReadLine(), out int bookTypeId)){
            selectedBookType = db.BookType.FirstOrDefault(bt => bt.Id == bookTypeId);
            if (selectedBookType == null){
                Console.WriteLine("Geçersiz tür ID'si. Lütfen listedeki bir ID girin.");
            }
        }else{
            Console.WriteLine("Geçersiz giriş. Lütfen bir sayı girin.");
        }
    }

    BookLocation selectedLocation = null;
    while (selectedLocation == null){
        Console.Write("Kitap Konumu: ");
        string bookLocationName = Console.ReadLine();

        selectedLocation = db.BooksLocation.FirstOrDefault(bl => bl.Location == bookLocationName);

        if (selectedLocation == null){
            selectedLocation = new BookLocation { Location = bookLocationName };
            db.BooksLocation.Add(selectedLocation);
            db.SaveChanges();
        }
    }

    var book = new Book{
        Name = bookName,
        BookType = selectedBookType,
        BookLocationId = selectedLocation.Id
    };

    db.Books.Add(book);
    db.SaveChanges();

    Console.WriteLine("Kitap başarıyla eklendi.");
}
        }

        static void AddBookType(){
            using(var db = new LibraryContext()){
                Console.WriteLine("\n- Kitap Türü Ekle -");
                Console.Write("Tür: ");
                string bookType = Console.ReadLine();

                var type = new BookType{Name = bookType};
                db.BookType.Add(type);
                db.SaveChanges();
                Console.WriteLine("Kitap Türü Eklendi.");
            }  
        }        
        
        static void ListBooks() {
            using (var db = new LibraryContext()) {
                Console.WriteLine("\n- Tüm Kitaplar -\n");

                var books = db.Books
                            .Include(b => b.BookType)
                            .ToList();

                foreach (var book in books) {
                    string availability = book.UserId == null ? "Mevcut" : "Mevcut Değil";
                    string bookTypeName = book.BookType != null ? book.BookType.Name : "Tür belirtilmemiş";

                    Console.WriteLine($"Kitap Adı: {book.Name}, Tür: {bookTypeName}, Durum: {availability}");
                }
            }
        }
        
        static void ListBook() {
            using (var db = new LibraryContext()) {
                Console.WriteLine("\n- Kitap Arama -");
                Console.WriteLine("1 - Kitap ismine göre ara");
                Console.WriteLine("2 - Kitap türüne göre ara");
                Console.Write("Seçiminiz: ");
                int choice = int.Parse(Console.ReadLine());

                if (choice == 1) {
                    Console.Write("Aramak istediğiniz kitap adını girin: ");
                    string searchName = Console.ReadLine();

                    var booksByName = db.Books
                        .Include(b => b.BookType)
                        .Where(b => b.Name.Contains(searchName))
                        .ToList();

                    if (booksByName.Any()) {
                        Console.WriteLine($"\n- '{searchName}' ile eşleşen kitaplar -\n");
                        foreach (var book in booksByName) {
                            string availability = book.UserId == null ? "Mevcut" : "Mevcut Değil";
                            string bookTypeName = book.BookType != null ? book.BookType.Name : "Tür belirtilmemiş";
                            Console.WriteLine($"ID: {book.Id}, Kitap Adı: {book.Name}, Tür: {bookTypeName}, Durum: {availability}");
                        }
                    } else {
                        Console.WriteLine("Eşleşen kitap bulunamadı.");
                    }
                }
                else if (choice == 2) {
                    Console.Write("Aramak istediğiniz kitap türünü girin: ");
                    string searchType = Console.ReadLine();

                    var booksByType = db.Books
                        .Include(b => b.BookType)
                        .Where(b => b.BookType != null && b.BookType.Name.Contains(searchType))
                        .ToList();

                    if (booksByType.Any()) {
                        Console.WriteLine($"\n- '{searchType}' türündeki kitaplar -\n");
                        foreach (var book in booksByType) {
                            string availability = book.UserId == null ? "Mevcut" : "Mevcut Değil";
                            Console.WriteLine($"ID: {book.Id}, Kitap Adı: {book.Name}, Tür: {book.BookType.Name}, Durum: {availability}");
                        }
                    } else {
                        Console.WriteLine("Bu türde kitap bulunamadı.");
                    }
                }
                else {
                    Console.WriteLine("Geçersiz seçim! Lütfen 1 veya 2 girin.");
                }
            }
        }
        
        static void AddBookReview(int customerId) {
            using (var db = new LibraryContext()) {
                Console.WriteLine("\n- Kitap Arama -");
                Console.Write("Aramak istediğiniz kitap adını girin: ");
                string searchName = Console.ReadLine();

                var books = db.Books
                    .Include(b => b.BookType)
                    .Where(b => b.Name.Contains(searchName))
                    .ToList();

                if (books.Any()) {
                    Console.WriteLine("\n- Eşleşen Kitaplar -");
                    foreach (var book in books) {
                        Console.WriteLine($"ID: {book.Id}, Kitap Adı: {book.Name}, Tür: {book.BookType?.Name ?? "Tür belirtilmemiş"}");
                    }

                    Console.Write("Bir kitap seçin (ID girin): ");
                    int bookId = int.Parse(Console.ReadLine());

                    var selectedBook = books.FirstOrDefault(b => b.Id == bookId);
                    if (selectedBook != null) {
                        var existingReview = db.Reviews
                            .FirstOrDefault(r => r.BookId == bookId && r.CustomerId == customerId);

                        if (existingReview != null) {
                            Console.WriteLine("Bu kitaba daha önce yorum yapmışsınız. Aynı kitaba birden fazla yorum yapamazsınız.");
                            return;
                        }

                        Console.Write("Yorumunuzu yazın: ");
                        string comment = Console.ReadLine();

                        Console.Write("Puan (1-5 arası): ");
                        int rating = int.Parse(Console.ReadLine());

                        var review = new Review {
                            BookId = selectedBook.Id,
                            CustomerId = customerId,
                            Comment = comment,
                            Rating = rating
                        };

                        db.Reviews.Add(review);
                        db.SaveChanges();
                        Console.WriteLine("Yorumunuz başarıyla eklendi.");
                    } else {
                        Console.WriteLine("Geçersiz kitap ID'si.");
                    }
                } else {
                    Console.WriteLine("Eşleşen kitap bulunamadı.");
                }
            }
        }

        static void ListBookReview() {
            using (var db = new LibraryContext()) {
                Console.WriteLine("\n- Kitap Yorumları -");
                Console.Write("Yorumlarını görmek istediğiniz kitabın adını girin: ");
                string searchName = Console.ReadLine();

                var books = db.Books
                    .Include(b => b.BookType)
                    .Where(b => b.Name.Contains(searchName))
                    .ToList();

                if (books.Any()) {
                    Console.WriteLine("\n- Eşleşen Kitaplar -");
                    foreach (var book in books) {
                        Console.WriteLine($"ID: {book.Id}, Kitap Adı: {book.Name}, Tür: {book.BookType?.Name ?? "Tür belirtilmemiş"}");
                    }

                    Console.Write("Bir kitap seçin (ID girin): ");
                    int bookId = int.Parse(Console.ReadLine());

                    var selectedBook = books.FirstOrDefault(b => b.Id == bookId);
                    if (selectedBook != null) {
                        var reviews = db.Reviews
                            .Where(r => r.BookId == selectedBook.Id)
                            .ToList();

                        if (reviews.Any()) {
                            Console.WriteLine("\n- Kitap Yorumları -");
                            foreach (var review in reviews) {
                                var user = db.Users.FirstOrDefault(u => u.Id == review.CustomerId);
                                string username = user != null ? user.Username : "Bilinmeyen Kullanıcı";
                                Console.WriteLine($"Yorum: {review.Comment}, Puan: {review.Rating}, Kullanıcı: {username}");
                            }
                        } else {
                            Console.WriteLine("Bu kitap için henüz yorum yapılmamış.");
                        }
                    } else {
                        Console.WriteLine("Geçersiz kitap ID'si.");
                    }
                } else {
                    Console.WriteLine("Eşleşen kitap bulunamadı.");
                }
            }
        }   

        static void LendBook(){
            using (var db = new LibraryContext()){
                Console.WriteLine("\n- Kitap Teslim Et -");

                Console.Write("Kitap Adı: ");
                string bookName = Console.ReadLine();

                var books = db.Books.Where(b => b.Name.Contains(bookName) && b.UserId == null).ToList();
                if (books.Count == 0){
                    Console.WriteLine("Bu isimde boşta olan kitap bulunamadı.");
                    return;
                }

                Console.WriteLine("\nMevcut Boşta Olan Kitaplar:");
                foreach (var book in books){
                    Console.WriteLine($"ID: {book.Id}, Kitap Adı: {book.Name}");
                }

                Book selectedBook = null;
                while (selectedBook == null){
                    Console.Write("\nKitap ID'si girin: ");
                    if (int.TryParse(Console.ReadLine(), out int bookId)){
                        selectedBook = db.Books.FirstOrDefault(b => b.Id == bookId);
                        if (selectedBook == null || selectedBook.UserId != null){
                            Console.WriteLine("Geçersiz kitap ID'si veya kitap zaten bir kullanıcıya teslim edilmiş.");
                        }
                    } else {
                        Console.WriteLine("Geçersiz giriş. Lütfen bir sayı girin.");
                    }
                }

                Console.Write("Kullanıcı Adı: ");
                string username = Console.ReadLine();

                var users = db.Users.Where(u => u.Username.Contains(username)).ToList();
                if (users.Count == 0){
                    Console.WriteLine("Bu kullanıcı bulunamadı.");
                    return;
                }

                Console.WriteLine("\nMevcut Kullanıcılar:");
                foreach (var user in users){
                    Console.WriteLine($"ID: {user.Id}, Kullanıcı Adı: {user.Username}");
                }

                User selectedUser = null;
                while (selectedUser == null){
                    Console.Write("\nKullanıcı ID'si girin: ");
                    if (int.TryParse(Console.ReadLine(), out int userId)){
                        selectedUser = db.Users.FirstOrDefault(u => u.Id == userId);
                        if (selectedUser == null){
                            Console.WriteLine("Geçersiz kullanıcı ID'si. Lütfen geçerli bir ID girin.");
                        }
                    } else {
                        Console.WriteLine("Geçersiz giriş. Lütfen bir sayı girin.");
                    }
                }

                selectedBook.UserId = selectedUser.Id;
                db.SaveChanges();

                Console.WriteLine($"Kitap '{selectedBook.Name}' başarıyla teslim edildi.");
            }
        }

         static void ReturnBook(){
            using (var db = new LibraryContext()){
    Console.WriteLine("\n- Kitap Teslim Alma -");

    Console.Write("Kullanıcı Adı: ");
    string username = Console.ReadLine();

    var user = db.Users
        .AsEnumerable() // Switch to in-memory comparison
        .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

    if (user == null){
        Console.WriteLine("Bu kullanıcı bulunamadı.");
        return;
    }

    var userBooks = db.Books.Where(b => b.UserId == user.Id).ToList();
    if (userBooks.Count == 0){
        Console.WriteLine("Bu kullanıcıya ait teslim alınacak kitap yok.");
        return;
    }

    Console.WriteLine("\nTeslim Alınabilecek Kitaplar:");
    foreach (var book in userBooks){
        Console.WriteLine($"ID: {book.Id}, Kitap Adı: {book.Name}");
    }

    Book selectedBook = null;
    while (selectedBook == null){
        Console.Write("\nTeslim alınacak Kitap ID'si girin: ");
        if (int.TryParse(Console.ReadLine(), out int bookId)){
            selectedBook = db.Books.FirstOrDefault(b => b.Id == bookId && b.UserId == user.Id);
            if (selectedBook == null){
                Console.WriteLine("Geçersiz kitap ID'si. Bu kitap teslim alınmak üzere atanmış değil.");
            }
        } else {
            Console.WriteLine("Geçersiz giriş. Lütfen bir sayı girin.");
        }
    }

    selectedBook.UserId = null;
    db.SaveChanges();

    Console.WriteLine($"Kitap '{selectedBook.Name}' başarıyla teslim alındı.");
}

        } 
}
