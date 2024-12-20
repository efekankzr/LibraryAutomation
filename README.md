# Kütüphane Yönetim Sistemi - Library Management System

## Proje Hakkında

Bu proje, C# Console uygulaması olarak geliştirilmiş bir **Kütüphane Yönetim Sistemi**dir.  
Projenin amacı, bir kütüphane içerisinde kitapların kullanıcılar arasında yönetimini kolaylaştırmak ve kayıtları düzenli bir şekilde saklamaktır.  

## Özellikler

- **Kitap Teslim Etme ve Alma:** Kitaplar kullanıcılarla ilişkilendirilebilir veya ilişki kaldırılabilir.  
- **Kitap Türleri ve Konumları:** Her kitap bir türe ve konuma bağlıdır.  
- **Kullanıcı Yorumları ve Derecelendirme:** Kullanıcılar kitaplara yorum yapabilir ve puan verebilir.  
- **Veritabanı Yönetimi:** MySQL ile güvenli ve organize bir veri yönetimi sağlar.  

## Kullanılan Teknolojiler

- **C#**
- **.NET Framework 8.0.0**
- **MySQL**

## Kurulum

1. Bu repository'yi klonlayın:
   ```bash
   git clone [repository_link]
   ```
2.Aşağıda belirtlen satırdaki ConnectionString için gerekli değerleri tamamlayınız ve sonrasında migration oluşturup database'i güncelleyiniz.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseMySql("server=__;user=__;password=__;database=__", 
            new MySqlServerVersion(new Version(8, 0, 34)));
    }
     
3. Projeyi bir C# IDE'sinde (.NET Core 8.0 destekli) açın ve çalıştırın.

---

# Library Management System - Kütüphane Yönetim Sistemi

## About the Project

This project is a **Library Management System** developed as a C# Console application.  
The purpose of the project is to facilitate the management of books among users in a library and keep records organized.  

## Features

- **Lending and Returning Books:** Books can be linked or unlinked with users.  
- **Book Categories and Locations:** Each book is associated with a category and location.  
- **User Reviews and Ratings:** Users can leave reviews and rate books.  
- **Database Management:** Provides secure and organized data management with MySQL.  

## Technologies Used

- **C#**
- **.NET Framework 8.0.0**
- **MySQL**

## Setup

1. Clone this repository:
   ```bash
   git clone [repository_link]
   ```
2. Complete the required values in the ConnectionString specified below, and then create a migration and update the database.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseMySql("server=__;user=__;password=__;database=__", 
            new MySqlServerVersion(new Version(8, 0, 34)));
    } 
3. Open the project in a C# IDE (with .NET Core 8.0 support) and run it.
