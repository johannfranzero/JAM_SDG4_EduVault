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
Presentation Layer    →  Windows Forms (.vb / .Designer.vb) — 18 forms + helpers
Business Logic Layer  →  Service classes (AuthService, ResourceService, ReportService, CategoryService, UserService)
Data Access Layer     →  Repository classes using ADO.NET SqlClient (parameterized queries only)
Model Layer           →  POCO classes (User, Resource, Category, AccessLog, Bookmark, Session)
Database Layer        →  Microsoft SQL Server (12 related tables + 4 views + 6 indexes)
```

---

## 🗂️ Repository Structure

```
JAM_SDG4_EduVault/                 ← GitHub repository root
│   .gitignore
│   README.md
│
├── CODE/
│   ├── EduVault.sln               ← Visual Studio Solution file
│   └── EduVault/                  ← VB.NET WinForms project (BLL, DAL, Forms, Models)
│
├── DATABASE/
│   └── Database_Script.sql        ← Full database script (Schema + Seed Data)
│
├── DOCUMENTATION/
│   ├── SDAD_JAM.pdf               ← Software Design & Analysis Document
│   ├── ERD_Diagram.png            ← Entity-Relationship Diagram (12 tables)
│   ├── NTier_Architecture.png     ← N-Tier Architecture Diagram
│   ├── DFD_Level0_Context.png     ← Data Flow Diagram — Level 0
│   └── DFD_Level1_Process.png     ← Data Flow Diagram — Level 1
│
└── REPORTS/
    └── Sample_Report_Export.pdf   ← Sample exported monthly access report
```

---

## ⚙️ Setup Instructions

### Prerequisites
- Visual Studio 2019 or later (.NET Framework 4.8)
- Microsoft SQL Server / SQL Server Express
- SQL Server Management Studio (SSMS)

### Step 1 — Database Setup
1. Open **SQL Server Management Studio (SSMS)**.
2. Connect to **`(LocalDB)\MSSQLLocalDB`** — this is SQL Server LocalDB, which comes bundled with Visual Studio. Use **Windows Authentication**.
3. Open **`DATABASE/Database_Script.sql`** in SSMS (File → Open → File).
4. Press **F5** (or click Execute) to run the script.

> The script automatically drops and recreates `EduVaultDB` from scratch every time — no manual cleanup needed. It creates all tables, views, indexes, and inserts seed data.

### Step 2 — Open the Project in Visual Studio
1. Open **`CODE/EduVault.sln`** in Visual Studio 2019 or later (.NET Framework 4.8).
2. Build the solution once (Ctrl+Shift+B) to restore all references.

### Step 3 — Connection String
The connection string in **`CODE/EduVault/App.config`** is already configured for LocalDB — **no changes needed**:
```xml
<add name="EduVaultDB"
     connectionString="Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=EduVaultDB;Integrated Security=True"
     providerName="System.Data.SqlClient" />
```

### Step 4 — Run the Application
- Press **F5** in Visual Studio to build and launch.
- The startup form is `frmLogin`.

### Default Login Credentials
| Username | Password | Role |
|---|---|---|
| `admin` | `Admin@123` | Admin — full access |
| `student1` | `Student@123` | Student — read + bookmark |

---

## 🗄️ Database Schema

### Tables
| Table | Purpose |
|---|---|
| `tblUsers` | Student and admin accounts (SHA-256 hashed passwords, roles) |
| `tblCategories` | Resource categories (Mathematics, Programming, etc.) |
| `tblResources` | Educational resources (title, URL, type, education level, tags) |
| `tblAccessLog` | View / bookmark / download events — feeds the monthly report |
| `tblBookmarks` | Per-user saved resource bookmarks |
| `tblLog` | Application error and event log (visible in admin System Logs) |

### Views
| View | Purpose |
|---|---|
| `vwResourceSummary` | Resources joined with category and uploader name |
| `vwMonthlyAccessSummary` | Monthly access counts grouped by resource and category |

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
| Firmalan, Johann | Database + Model Layer | SQL Schema, Seed Data, User.vb, Resource.vb, Category.vb |
| Firmalan, Johann | Data Access Layer (DAL) | UserRepository, ResourceRepository, CategoryRepository |
| Anderson, Kurt | Business Logic Layer (BLL) | AuthService, ResourceService, CategoryService |
| Jongco, Mark Steven | Presentation Layer (Forms) | frmLogin, frmDashboard, frmManageResources |
| Empeno, AJ | Reporting + Documentation | ReportService, frmReport, SDAD, README |

---

*EduVault — Built with 💙 in support of UN SDG 4: Quality Education*
*BSIT 2nd Year · ITELEC1 · 2nd Semester, Cycle 2 · A.Y. 2025–2026*
