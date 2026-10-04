# Baitul Kitab (بيت الكتاب) - Online Book Store

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/Entity%20Framework%20Core-8.0-purple.svg)](https://docs.microsoft.com/ef/core/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-4.6-purple.svg)](https://getbootstrap.com/)

**Baitul Kitab** is a full-featured, enterprise-grade Online Book Store built with **ASP.NET Core 8 MVC**, following **N-Tier Clean Architecture** with separation of concerns between presentation, business logic, data access, and models.

---

## 📑 Table of Contents
- [Architecture Overview](#-architecture-overview)
- [Key Features](#-key-features)
  - [Customer / User Storefront](#1-customer--user-storefront)
  - [Admin Management Portal](#2-admin-management-portal)
  - [Digital Reading & Media Management](#3-digital-reading--media-management)
  - [Integrated Ad Engine](#4-integrated-ad-engine)
- [Technology Stack](#-technology-stack)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
  - [Prerequisites](#prerequisites)
  - [Configuration](#configuration)
  - [Database Migration & Seeding](#database-migration--seeding)
  - [Run the Application](#run-the-application)
- [Default Roles & Accounts](#-default-roles--accounts)
- [Contributing & License](#-author--license)

---

## 🏛 Architecture Overview

The solution strictly adheres to an N-Tier architecture pattern:

| Project Layer | Description |
| :--- | :--- |
| **`Baitul_Kitab`** (Web Layer) | ASP.NET Core 8 MVC presentation layer. Contains Controllers, Razor Views, Areas (`User`, `Admin`, `Category`, `Author`, `Book`), View Components, and static assets. |
| **`Baitul_Kitab.BAL`** (Business Access Layer) | Contains business logic, validation, service implementations, and interfaces (`IUserBooks`, `IBooks`, `ICategories`, `IAuthors`, `ILanguages`). |
| **`Baitul_Kitab.Data`** (Data Access Layer) | Entity Framework Core 8 context (`ApplicationDbContext`), database migrations, and EF entities (`Book`, `Category`, `Author`, `Language`, `ApplicationUser`). |
| **`Baitul_Kitab.Models`** (Model Layer) | Plain C# DTOs, ViewModels, Queryable extensions, and DataTable parameters. Prevents direct EF Core entity exposure to views. |

---

## 🌟 Key Features

### 1. Customer / User Storefront
- **Top Navigation Bar:** Distraction-free, responsive top navigation (`layout-top-nav`) for standard users and visitors.
- **Storefront Home:** Browse featured, popular, and recently published books.
- **Single-Row Compact Filter Bar:**
  - Search by Title
  - Filter by Category, Author, and Language
  - Min & Max Price range filter
  - Instant Filter and Reset controls
- **Uniform & Compact Book Cards:**
  - Proportional book cover displays with hover zoom animations.
  - Floating badges for **SALE** and **PDF Available**.
  - 2-line clamped titles for grid symmetry.
  - Side-by-side Effective Price (with discount strikethrough) and live stock availability (`In Stock`, `Low Stock`, `Out of Stock`).
- **Comprehensive Book Details Page:** Detailed publication info (ISBN, Publisher, Release Date, Language, Category, Description) and action buttons.
- **Store Pages:** Dedicated **About Us** and **Contact Us** pages with contact info cards and an inquiry form.

### 2. Admin Management Portal
- **Dashboard & Persistent Navigation:** Sidebar menu with automatic active-menu tracking that prevents collapse when navigating submenus.
- **Category Management:** Full CRUD operations with AJAX DataTables, sorting, pagination, and search.
- **Author Management:** Author metadata management with validation and DataTables integration.
- **Language Management:** Multilingual book support and language categorization.
- **Book Management:**
  - Book metadata (Title, ISBN, Publisher, Page count, Description).
  - Price and Discount Price configurations with automated discount calculations.
  - Real-time inventory tracking (Quantity, In-Stock status).
  - Soft delete support (`IsDeleted`) to preserve historical transactions.

### 3. Digital Reading & Media Management
- **Book Cover Uploads:** Upload and display front covers in web-optimized formats.
- **PDF E-Book Integration:** Optional PDF upload during book creation/editing with a dedicated **Read / Download PDF** reader button on the customer details view.

### 4. Integrated Ad Engine
- Non-intrusive banner placement system driven by `IAdPlacementProvider`.
- Supports placements: `TopBanner`, `Sidebar`, `BookListing` (in-grid ad intervals), and `Footer`.
- Easily toggled on or off via configuration without altering view templates.

---

## 💻 Technology Stack

- **Framework:** .NET 8.0 (C# 12)
- **Web Engine:** ASP.NET Core MVC (Razor Views & Tag Helpers)
- **ORM:** Entity Framework Core 8.0 (SQL Server)
- **Security & Auth:** ASP.NET Core Identity (Role-based: Admin & Customer)
- **UI & Styling:**
  - Bootstrap 4.6
  - AdminLTE 3.2 Theme
  - FontAwesome 5 Icons
  - DataTables.net (Server-side & Client-side)
  - SweetAlert2 & Toastr Notifications

---

## 📁 Project Structure

```text
Baitul_Kitab/
├── Baitul_Kitab/                       # Web Application (MVC)
│   ├── Ads/                            # Ad placement provider & settings
│   ├── Areas/
│   │   ├── Admin/                      # Admin controllers & views
│   │   ├── Author/                     # Author management module
│   │   ├── Category/                   # Category & Language modules
│   │   └── User/                       # Customer Storefront (Home, Books, Details, About, Contact)
│   ├── Controllers/                    # Root / Account / Home controllers
│   ├── ViewComponents/                 # Reusable components (e.g., AdPlacement)
│   ├── Views/                          # Shared layouts (_Layout, _LoginPartial)
│   └── wwwroot/                        # Static assets (CSS, JS, AdminLTE, uploads)
│       └── uploads/books/              # Stored covers and uploaded PDFs
├── Baitul_Kitab.BAL/                   # Business Access Layer
│   ├── Interfaces/                     # Service contracts (IBooks, IUserBooks, etc.)
│   └── Services/                       # Service implementations
├── Baitul_Kitab.Data/                  # Data Access Layer
│   ├── Migrations/                     # EF Core code-first migrations
│   ├── Models/                         # EF Database entity definitions
│   └── ApplicationDbContext.cs         # Database Context configuration
├── Baitul_Kitab.Models/                # Data Transfer Objects (DTOs)
│   ├── DTO/                            # Domain DTOs (BookDTO, CategoryDTO, etc.)
│   └── ViewModels/                     # Response & DataTable viewmodels
├── .gitignore                          # Visual Studio & .NET Git ignore rules
└── Baitul_Kitab.sln                    # Visual Studio Solution File
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (v17.8+) or [VS Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/sql-server/) (LocalDB, Express, or standard instance)

### Configuration
1. Clone the repository:
   ```bash
   git clone https://github.com/MudasserRaza96/BaitulKitab.git
   cd BaitulKitab
   ```

2. Configure your database connection string in `Baitul_Kitab/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BaitulKitabDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```

### Database Migration & Seeding
Open the **Package Manager Console** in Visual Studio or run via CLI:

```bash
dotnet ef database update --project Baitul_Kitab.Data --startup-project Baitul_Kitab
```

### Run the Application
```bash
dotnet run --project Baitul_Kitab
```
Navigate to `https://localhost:7123` (or the port specified in `launchSettings.json`).

---

## 🔐 Default Roles & Accounts

The application implements role-based access control:
- **Admin:** Access to configuration, master data (Categories, Authors, Languages, Books), and inventory.
- **Customer / User:** Customer storefront, book search, filtered listings, PDF viewer, and contact inquiry.
- *Upon login, users are automatically routed to their corresponding module based on assigned roles.*

---

## 👤 Author & License

- **Developer:** [Mudasser Raza](https://github.com/MudasserRaza96)
- **Repository:** [https://github.com/MudasserRaza96/BaitulKitab](https://github.com/MudasserRaza96/BaitulKitab)

*Built with passion for quality software engineering and modern web practices.*
