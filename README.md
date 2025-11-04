** Gestion des Articles avec ASP.NET Core et EF Core **

## Description
This project is a web application . 
The application allows managing products and categories, connecting to a SQL Server database using **Entity Framework Core** with the **Code First** approach. 
It also includes user authentication and role-based access control using **ASP.NET Identity**.

## Features

### Database & Models
- **Entities:** `Product` and `Category`
- **EF Core Code First Approach** to generate SQL Server database
- **Image Upload:** Each product can have an associated image
- **Data Validation:** Enforced using data annotations

### Repository Pattern
- `IProductRepository` and `ICategoryRepository` interfaces
- `ProductRepository` and `CategoryRepository` implementations
- CRUD operations and search functionalities
- Fetch products by category

### ASP.NET Core MVC
- Controllers: `ProductController` and `CategoryController`
- Views: `Index`, `Create`, `Edit`, `Delete`, `Details`
- Bootstrap-based UI with cards for product display

### Authentication & Authorization
- **ASP.NET Identity** integration
- User registration and login (`AccountController`)
- Role-based access control (`AdminController`)
- Only authenticated users can access Product and Category management
- Login, Logout, and Register links dynamically displayed in navbar

### Additional Features
- Product image upload and preview
- Search functionality for products and categories
- Role creation and management for administrators
- Password configuration and validation rules

---

## Prerequisites
- **Visual Studio 2022**
- **.NET 6.0 / 7.0 SDK**
- **SQL Server** (LocalDB or full version)
- NuGet packages:
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.EntityFrameworkCore.SqlServer`
  - `Microsoft.EntityFrameworkCore.Tools`
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore`

---

## Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd TP_N2_ArticleManagement
````

2. **Open in Visual Studio**

   * Open the `.sln` file in Visual Studio 2022.
   * Right-click the project → `Manage NuGet Packages`.
   * Install the required EF Core and Identity packages.

3. **Configure the Database**

   * Update `appsettings.json` with your SQL Server connection string:

     ```json
     "ConnectionStrings": {
       "ProductDBConnection": "server=(localdb)\\MSSQLLocalDB;database=MyBaseDB;Trusted_Connection=true"
     }
     ```

4. **Add Migrations and Update Database**

   * Open Package Manager Console:

     ```powershell
     Add-Migration InitialCreate
     Update-Database
     ```
   * For subsequent model changes (e.g., adding image path to Product):

     ```powershell
     Add-Migration AddPhotoPathToProducts
     Update-Database
     ```

5. **Run the Application**

   * Press `F5` or click `Run`.
   * Navigate to the home page.
   * Access product and category management after registering and logging in.

---

## Project Structure

```
TP_N2_ArticleManagement/
│
├─ Controllers/
│  ├─ ProductController.cs
│  ├─ CategoryController.cs
│  └─ AccountController.cs
│
├─ Models/
│  ├─ Product.cs
│  ├─ Category.cs
│  └─ AppDbContext.cs
│
├─ Repositories/
│  ├─ IProductRepository.cs
│  ├─ ICategorieRepository.cs
│  ├─ ProductRepository.cs
│  └─ CategoryRepository.cs
│
├─ ViewModels/
│  ├─ CreateViewModel.cs
│  ├─ EditViewModel.cs
│  ├─ RegisterViewModel.cs
│  └─ LoginViewModel.cs
│
├─ Views/
│  ├─ Product/
│  ├─ Category/
│  └─ Account/
│
├─ wwwroot/
│  └─ images/
│
└─ appsettings.json
```

---

## Usage

* **Product Management:** Create, edit, delete, and view products.
* **Category Management:** Create, edit, delete, and view categories.
* **Image Upload:** Add or update images when creating/editing products.
* **Search:** Search products by name or category.
* **User Management:** Register new users and assign roles (Admin can manage roles).

---

## Notes

* Only authenticated users can manage products and categories.
* Images are stored in `wwwroot/images`.
* The UI is responsive using Bootstrap cards.
* Password rules can be configured in `Program.cs`.

---

## License

This project is for educational purposes. Please contact the author before using it in production.


