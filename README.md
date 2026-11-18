# 💼 CareerLink — Job Portal

> A web-based recruitment platform connecting job seekers with recruiters through job discovery, applications, and profile-based job matching.

![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET_Core-MVC-purple)
![C#](https://img.shields.io/badge/Language-C%23-blue)
![SQL Server](https://img.shields.io/badge/Database-SQL_Server-red)
![Entity Framework Core](https://img.shields.io/badge/ORM-Entity_Framework_Core-green)
![Project Type](https://img.shields.io/badge/Project-Academic-orange)

---

## 📌 Overview

**CareerLink** is an online job portal developed using ASP.NET Core MVC that simplifies the recruitment process for both job seekers and recruiters.

Job seekers can create professional profiles, explore employment opportunities, save jobs, and apply for positions. Recruiters can manage company profiles, publish job vacancies, and review applications received from candidates.

The platform also incorporates a **rule-based job matching system** that evaluates a candidate's profile against job requirements using skills, education, experience, and location.

CareerLink aims to make job searching and recruitment more organized, accessible, and efficient through a centralized platform.

---

## ✨ Features

### 👤 Job Seeker

- User registration and secure login
- Professional profile management
- Manage skills and proficiency levels
- Add education qualifications
- Add work experience
- Manage certifications and projects
- Browse and search job listings
- View detailed job requirements
- Save jobs for future reference
- Apply for jobs
- Upload resumes
- Submit cover letters
- View application history and status
- View job matching scores
- Access interview information and notifications

### 🏢 Recruiter

- Recruiter registration and authentication
- Recruiter profile management
- Company profile management
- Create and publish job listings
- Specify job requirements and skills
- Manage posted jobs
- View candidate applications
- Review applicant information
- Update application statuses
- Manage interview-related information

### 🛡️ Administrator

- Administrative dashboard
- Platform-level management
- User management
- Company management
- Job listing management
- Administrative access control

---

## 🎯 Job Matching System

CareerLink implements a rule-based job matching mechanism to help job seekers identify relevant employment opportunities.

The matching system evaluates the candidate's profile against the job's requirements using the following criteria:

| Criteria | Weight |
|---|---|
| Skills | 50% |
| Education | 20% |
| Experience | 20% |
| Location | 10% |
| **Total** | **100%** |

The weights are configurable for individual job listings.

The matching score provides an indication of how closely a candidate's profile matches a job's requirements.

---

## 🛠️ Tech Stack

| Technology | Usage |
|---|---|
| C# | Backend programming language |
| ASP.NET Core MVC | Web application framework |
| Razor Views | Dynamic frontend rendering |
| HTML5 | Web page structure |
| CSS3 | Styling and layout |
| Bootstrap | Responsive UI components |
| JavaScript | Client-side functionality |
| Entity Framework Core | Database ORM |
| Microsoft SQL Server | Relational database |
| ASP.NET Core Identity | Authentication and authorization |
| Visual Studio Community | Development environment |

---

## 🏗️ Architecture

CareerLink follows the **Model-View-Controller (MVC)** architectural pattern.

```text
                ┌──────────────────────┐
                │       User           │
                │ JobSeeker / Recruiter│
                │       / Admin        │
                └──────────┬───────────┘
                           │
                           ▼
                ┌──────────────────────┐
                │        View          │
                │    Razor + HTML      │
                │ Bootstrap + CSS      │
                └──────────┬───────────┘
                           │
                           ▼
                ┌──────────────────────┐
                │     Controller       │
                │ Application Logic    │
                │ Validation & Actions│
                └──────────┬───────────┘
                           │
                           ▼
                ┌──────────────────────┐
                │       Model          │
                │   Entities & Logic   │
                └──────────┬───────────┘
                           │
                           ▼
                ┌──────────────────────┐
                │   Entity Framework   │
                │        Core          │
                └──────────┬───────────┘
                           │
                           ▼
                ┌──────────────────────┐
                │   SQL Server DB      │
                └──────────────────────┘
```

### Architectural Components

- **Model:** Represents application entities and database structure.
- **View:** Displays user interfaces using Razor, HTML, CSS, and Bootstrap.
- **Controller:** Handles user requests, application logic, validation, and database operations.
- **Entity Framework Core:** Connects application models with the SQL Server database.
- **ASP.NET Core Identity:** Handles authentication and role-based authorization.

---

## 📂 Project Structure

```text
CareerLink/
│
├── Controllers/
│   ├── ApplicationsController.cs
│   ├── JobsController.cs
│   ├── JobSeekerController.cs
│   ├── RecruiterController.cs
│   └── AdminController.cs
│
├── Data/
│   └── ApplicationDbContext.cs
│
├── Models/
│   ├── ApplicationUser.cs
│   ├── JobSeeker.cs
│   ├── Recruiter.cs
│   ├── Company.cs
│   ├── Job.cs
│   ├── Application.cs
│   ├── Skill.cs
│   ├── JobSkill.cs
│   ├── JobSeekerSkill.cs
│   ├── Education.cs
│   ├── Experience.cs
│   ├── Certification.cs
│   ├── Project.cs
│   ├── Interview.cs
│   ├── SavedJob.cs
│   └── Notification.cs
│
├── Views/
│   ├── Applications/
│   ├── Jobs/
│   ├── JobSeeker/
│   ├── Recruiter/
│   └── Admin/
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   ├── images/
│   └── uploads/
│
├── Data/
├── Migrations/
│
├── Program.cs
├── appsettings.json
└── CareerLink.csproj
```

---

## 🗄️ Database

CareerLink uses **Microsoft SQL Server** with **Entity Framework Core**.

### Main Entities

- `ApplicationUser`
- `JobSeeker`
- `Recruiter`
- `Company`
- `Job`
- `Application`
- `Skill`
- `JobSkill`
- `JobSeekerSkill`
- `Education`
- `Experience`
- `Certification`
- `Project`
- `Interview`
- `SavedJob`
- `Notification`

Entity relationships are managed using primary keys, foreign keys, and navigation properties.

---

## 🚀 Getting Started

Follow these steps to run CareerLink locally.

### Prerequisites

Make sure you have installed:

- [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/)
- ASP.NET and web development workload
- .NET SDK compatible with the project
- Microsoft SQL Server
- [SQL Server Management Studio](https://learn.microsoft.com/sql/ssms/download-ssms), recommended
- Git

### 1. Clone the Repository

```bash
git clone https://github.com/TirthaPatel978/CareerLink.git
```

Navigate to the project directory:

```bash
cd CareerLink
```

### 2. Open the Project

Open the `.sln` solution file in Visual Studio.

If a solution file is not available, open the `CareerLink.csproj` file directly.

### 3. Configure the Database

Ensure that SQL Server is running.

Update the connection string in `appsettings.json` to match your local SQL Server instance.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=CareerLinkDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Replace `YOUR_SERVER_NAME` with your actual SQL Server instance name.

### 4. Apply Database Migrations

Open the Package Manager Console in Visual Studio:

**Tools → NuGet Package Manager → Package Manager Console**

Run:

```powershell
Update-Database
```

This applies the existing Entity Framework Core migrations and creates or updates the database.

### 5. Run the Application

Set CareerLink as the startup project.

Run the application using:

```text
F5
```

or click the Run button in Visual Studio.

The application will launch in your default web browser.

---

## 🔐 Authentication & Authorization

CareerLink uses ASP.NET Core Identity for authentication and user management.

The application supports the following roles:

| Role | Access |
|---|---|
| JobSeeker | Search jobs, manage profile, save jobs, and apply |
| Recruiter | Manage companies, jobs, and applicants |
| Admin | Access administrative features |

Role-based authorization restricts access to functionalities according to the user's assigned role.

---

## 📸 Screenshots

_Add screenshots of the CareerLink application here._

Suggested screenshots:

- Home Page
- Login and Registration
- Job Seeker Dashboard
- Job Search Page
- Job Details Page
- Recruiter Dashboard
- Create Job Page
- Application Management
- Admin Dashboard

---

## 🎓 Project Information

**Project Name:** CareerLink – Job Portal  
**Project Type:** Academic / Web Application Development Project  
**Architecture:** Model-View-Controller (MVC)  
**Database:** Microsoft SQL Server  

The project demonstrates the practical application of web development concepts, database design, authentication, authorization, MVC architecture, and recruitment workflow management.

---

## 🔮 Future Enhancements

- AI-powered job recommendations
- Resume parsing and automated skill extraction
- Email notifications for application updates
- Advanced job search filters
- Improved recruiter analytics and reporting
- Video interview integration
- Mobile-friendly enhancements
- Automated unit and integration testing
- Deployment to a cloud hosting platform

---

## 👨‍💻 Author

Developed as an academic project.

---

## 📄 License

This project is developed for educational and academic purposes.
