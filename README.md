# EduVault — Open-Access Resource Tracker for Students

[![SDG 4](https://img.shields.io/badge/UN%20SDG-4%20Quality%20Education-blue?style=flat-square)](https://sdgs.un.org/goals/goal4)
[![VB.NET](https://img.shields.io/badge/Language-VB.NET%20.NET%204.8-purple?style=flat-square)](https://docs.microsoft.com/en-us/dotnet/visual-basic/)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-red?style=flat-square)](https://www.microsoft.com/en-us/sql-server)
[![ADO.NET](https://img.shields.io/badge/Data%20Access-ADO.NET-orange?style=flat-square)](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/)

> **EduVault** is a Windows desktop application that helps students discover, access, and bookmark open educational resources (OERs), and gives administrators a complete library management system with reporting capabilities.

---

## 🌍 SDG Alignment

EduVault directly supports **UN Sustainable Development Goal 4 — Quality Education**:

| Target | How EduVault Helps |
|---|---|
| **4.4** — Technical & vocational skills | Provides access to curated programming, science, and skills resources |
| **4.b** — Access to educational resources | Students can browse, search, and bookmark free learning materials |

---

## ✨ Features

### 👨‍🎓 For Students
- 🔍 **Smart Search** — Search by keyword, category, type, and education level
- ⭐ **Bookmarks** — Save favourite resources for quick access
- 🕐 **Activity History** — Track recently viewed resources
- 🌐 **One-click Access** — Double-click any resource to open it in the browser

### 🛠️ For Administrators
- ➕ **Resource CRUD** — Add, edit, and soft-delete educational resources
- 👥 **User Management** — Create accounts, assign roles, deactivate users
- 📊 **Monthly Reports** — View and export access summary reports
- 🔥 **Engagement Tags** — Automatic Hot / Popular / New badges based on view counts
- 📈 **Dashboard Stats** — Total resources, accesses, e-books, and videos at a glance

---

## 🏗️ Architecture — N-Tier Design

```
Presentation Layer    →  Windows Forms (.vb / .Designer.vb)
Business Logic Layer  →  Service classes (AuthService, ResourceService, ReportService, CategoryService)
Data Access Layer     →  Repository classes using ADO.NET SqlClient (parameterized queries only)
Model Layer           →  POCO classes (User, Resource, Category, AccessLog, Bookmark)
Database Layer        →  Microsoft SQL Server (5 related tables + 2 views)
```

---

## 🗂️ Repository Structure

```
EduVault/                          ← GitHub repository root
│   .gitignore
│   README.md
│
├── CODE/
│   └── EduVault/                  ← Active VB.NET WinForms project (open this in Visual Studio)
│
├── SQL/                           ← **Canonical** database setup (use this for new installs)
│   ├── 00_Setup.sql               ← Runs schema + migrations + seed (SQLCMD Mode)
│   ├── 01_Schema.sql              ← Core v2 tables/columns (matches DAL)
│   ├── 02_SeedData.sql            ← Users, categories, sample resources
│   ├── 03_Migrations.sql          ← Idempotent upgrades (legacy DBs + optional v2 tables)
│   └── README.md                  ← Script order and troubleshooting
│
├── DATABASE/
│   └── Database_Script.sql        ← Legacy v1 all-in-one script (prefer `SQL/` instead)
│
├── DOCUMENTATION/
│   ├── SDAD_EduVault.pdf          ← Full SDAD (converted from .md)
│   └── ERD_Diagram.png            ← Entity-Relationship Diagram
│
└── REPORTS/
    └── Sample_Report_Export.pdf   ← Sample exported report (generated at runtime)
```

---

## ⚙️ Setup Instructions

### Prerequisites
- Visual Studio 2019 or later (.NET Framework 4.8)
- Microsoft SQL Server / SQL Server Express
- SQL Server Management Studio (SSMS)

### Step 1 — Database Setup
1. Open **SSMS** and connect to your SQL Server instance.
2. Open **`SQL/00_Setup.sql`**, enable **Query → SQLCMD Mode**, and execute (F5).

   This runs, in order: `01_Schema.sql` → `03_Migrations.sql` → `02_SeedData.sql`.

   Without SQLCMD Mode, run those three files manually in the same order (see **`SQL/README.md`**).

3. **Existing database from an older script?** Run only **`SQL/03_Migrations.sql`** (safe to re-run).

The schema includes columns the app expects (`FailedLoginCount`, `DownloadCount`, `PasswordResetToken`, `tblLog`, etc.). Seed data ships with SHA-256 hashes for the default passwords below.

> **Legacy:** `DATABASE/Database_Script.sql` is an older v1 bundle and does not include all v2 columns. Use the `SQL/` folder instead.

### Step 2 — Visual Studio Project Setup
1. Open **`CODE/EduVault/EduVault.vbproj`** in Visual Studio 2019+ (.NET Framework 4.8).
2. Confirm **`App.config`** connection string points at your SQL Server instance (see Step 4).

### Step 3 — Add Required Reference
- Right-click **References** → **Add Reference** → search for **`System.Configuration`**

### Step 4 — Update Connection String
Edit `App.config`:
```xml
<add name="EduVaultDB"
     connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=EduVaultDB;Integrated Security=True;"
     providerName="System.Data.SqlClient" />
```
Change `.\SQLEXPRESS` to match your SQL Server instance name.

### Step 5 — Set Startup Form
- Go to **My Project → Application tab**
- Set **Startup form** to `frmLogin`

### Default Credentials
| Username | Password | Role |
|---|---|---|
| `admin` | `Admin@123` | Admin — full access |
| `student1` | `Student@123` | Student — read + bookmark |

---

## 🗄️ Database Schema

| Table / view | Purpose |
|---|---|
| `tblUsers` | Users (roles, lockout, password reset, dark mode preference) |
| `tblCategories` | Resource categories |
| `tblResources` | Resources (views, downloads, thumbnails, versioning) |
| `tblAccessLog` | View / bookmark / download events for reports |
| `tblBookmarks` | Student bookmarks |
| `tblLog` | Application error log (admin System Logs screen) |
| `vwResourceSummary` | Resource list JOIN view |
| `vwMonthlyAccessSummary` | Monthly report aggregation |

Optional v2 tables (created by `03_Migrations.sql`, not all wired in UI yet): `tblRatings`, `tblNotifications`, `tblResourceRequests`, `tblFavourites`, `tblResourceVersions`, `tblBackupSchedule`.

---

## 📊 Report Feature

The **Monthly Access Summary Report** (`frmReport`) shows:
- Resource access counts grouped by year and month
- Unique student counts per resource
- Category and type breakdown
- **CSV export** via `SaveFileDialog`
- Compatible with **Microsoft Report Viewer (RDLC)** — see integration notes in `frmReport.vb`

---

## 🔒 Security

| Feature | Implementation |
|---|---|
| Password hashing | SHA-256 via `System.Security.Cryptography.SHA256` |
| SQL Injection prevention | All queries use `cmd.Parameters.Add()` — zero string concatenation |
| Role-based access | `Session.IsAdmin` checked at every form load and privileged action |
| Audit trail | Soft-delete (`IsActive = 0`) preserves all access log history |

---

## 🤖 AI Assistance Disclosure

> **Academic Integrity Notice — Required Citation**
>
> Portions of the source code, SQL schema, and documentation in this project were developed with the assistance of **Antigravity AI (powered by Google DeepMind)** as an AI pair-programming tool. This assistance was used to:
> - Generate boilerplate N-Tier architecture scaffolding (DAL, BLL, Model layers)
> - Produce parameterized ADO.NET query templates
> - Draft initial SDAD documentation structure
> - Generate Windows Forms Designer files
>
> **All AI-generated code was reviewed, understood, tested, and customized by the group members.** Each member is fully accountable for the modules assigned to them and can explain, defend, and modify any part of the codebase during the individual practical exam.
>
> This citation is provided in compliance with the ITELEC1 Academic Integrity Policy (ITELEC1 Final Project Guidelines, 2026).

---

## 👥 Group Members & Contributions

| Member | Assigned Layer | Specific Modules |
|---|---|---|
| [Member 1 — Full Name] | Database + Model Layer | SQL Schema, Seed Data, User.vb, Resource.vb, Category.vb |
| [Member 2 — Full Name] | Data Access Layer (DAL) | UserRepository, ResourceRepository, CategoryRepository |
| [Member 3 — Full Name] | Business Logic Layer (BLL) | AuthService, ResourceService, CategoryService |
| [Member 4 — Full Name] | Presentation Layer (Forms) | frmLogin, frmDashboard, frmManageResources |
| [Member 5 — Full Name] | Reporting + Documentation | ReportService, frmReport, SDAD, README |

> ✏️ Replace placeholder names with your actual group members before submission.

---

*EduVault — Built with 💙 in support of UN SDG 4: Quality Education*
*BSIT 2nd Year · ITELEC1 · 2nd Semester, Cycle 2 · A.Y. 2025–2026*
