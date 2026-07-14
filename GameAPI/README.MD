# 🎮 Tournament Management System (.NET 8 + React)

A full-stack Tournament Management System built with **ASP.NET Core Web API**, **Entity Framework Core**, **SQL Server**, **React**, **TypeScript**, and **SignalR**.

The project is designed as a real-world application to learn enterprise-level backend development while building production-ready features.

---

# 🚀 Tech Stack

## Backend

- ASP.NET Core 8 Web API
- Entity Framework Core
- SQL Server
- SignalR
- JWT Authentication
- Dependency Injection
- Repository/Service Architecture
- AES-256 Message Encryption

## Frontend

- React
- TypeScript
- React Query
- Zustand
- Material UI
- SignalR Client

---

# For run the project

dotnet run --launch-profile https

# 📂 Project Architecture

```
Controllers
    ↓
Services
    ↓
Entity Framework Core
    ↓
SQL Server
```

The project follows a clean layered architecture where business logic is completely separated from controllers.

---

# 🔐 Authentication

Implemented JWT Authentication.

Features

- User Login
- Secure JWT Token Generation
- Token Validation
- Protected APIs
- Role Based Authorization

---

## Database Setup

The project includes a ready-to-use SQL script for creating the database schema and inserting sample data.

### Steps

1. Navigate to the following folder:

```text
Data/SQLScript
```

2. Execute the provided SQL script in SQL Server.

The script will automatically:

- Create all required database tables.
- Configure relationships and constraints.
- Insert sample users.
- Insert sample application data (Games, Tournaments, etc.).

After executing the script, update the `DefaultConnection` string in `appsettings.json` to point to your SQL Server instance, then run the application.

> **Note:** The sample data is provided for development and testing purposes, allowing you to start using the application immediately without manually creating records.

# 👤 User Management

Implemented

- Login
- Profile
- User Listing
- Role Support
- Active/Inactive Users

---

# 🎮 Game Management

Implemented CRUD APIs for Games.

Features

- Create Game
- Update Game
- Delete Game
- Pagination
- Searching
- Soft Delete

---

# 🏆 Tournament Management

Implemented

- Create Tournament
- Update Tournament
- Delete Tournament
- Tournament Status
- Participants Per Match
- Pagination
- Search

---

# ⚔ Tournament Matches

Implemented

- Generate Tournament Matches
- Match Creation
- Tournament Brackets
- Match Winner Update
- Automatic Next Round Progression

---

# 📢 Announcement Module

Implemented complete announcement management.

Features

- Create Announcement
- Edit Announcement
- Delete Announcement
- Read/Unread Status
- Notification Count
- Priority

Priority Levels

- Low
- Normal
- High
- Critical

---

# 🔔 Real Time Notifications (SignalR)

Implemented using SignalR.

Features

- Instant Announcements
- Live Notification Count
- No Page Refresh Required
- Automatic Reconnection

---

# 🟢 Online User Tracking

Implemented using SignalR.

Features

- Real Time Online Users
- Live User Count
- Automatic Disconnect Detection
- Multi Browser Support
- Automatic Refresh on Login/Logout

---

# 💬 Private Chat System

Implemented one-to-one private chat.

Database Design

- Conversations
- ConversationParticipants
- ConversationMessages

Features

- Create Conversation
- Send Message
- Load Messages
- Mark Conversation As Read
- Real Time Messaging using SignalR
- Conversation Groups

---

# 🔐 AES-256 Message Encryption

Messages are encrypted before storing into SQL Server.

Workflow

```
Message

↓

AES Encryption

↓

Database

↓

AES Decryption

↓

Frontend
```

Features

- AES-256 Encryption
- Random IV Per Message
- Base64 Encoding
- Secure Storage
- Automatic Decryption While Reading

The database never stores plain text messages.

---

# 📊 Admin Dashboard

Implemented dashboard APIs.

Features

- Total Users
- Total Games
- Total Tournaments
- Notifications
- Online Users

---

# 📝 Logging

Implemented application logging.

Features

- Activity Logs
- Error Logging
- User Actions

---

# ⚡ SignalR Hubs

## AnnouncementHub

Responsible for

- Announcements
- Notification Count
- Online Users

## ChatHub

Responsible for

- Join Conversation
- Leave Conversation
- Receive Messages

Keeping hubs separated improves scalability and maintainability.

---

# 📧 Email & 🏆 Winner Certificate Module

## Overview

This project supports sending emails for:

- User Invitations
- Tournament Winner Congratulations (with PDF Certificate Attachment)

The implementation is designed to be reusable and extensible for future email templates and document generation.

---

# Email Provider

The project uses **Mailtrap SMTP** for email delivery during development.

## NuGet Packages

```bash
MailKit
MimeKit
```

These libraries are used to:

- Connect to SMTP
- Send HTML emails
- Send file attachments (PDF)
- Support future email templates

---

# Email Templates

All email templates are stored inside:

```text
Templates/
```

Example:

```text
Templates/
    InviteUser.html
    WinnerEmail.html
```

Templates use placeholders such as:

```html
{{WinnerName}} {{TournamentName}} {{GameName}} {{AwardDate}} {{IssuedBy}}
```

These placeholders are replaced dynamically before sending the email.

---

# PDF Certificate Generation

Winner certificates are generated as PDF documents from an HTML template.

The project uses:

```text
HTML Template
        ↓
Placeholder Replacement
        ↓
HTML
        ↓
PDF Generator
        ↓
byte[]
```

The generated PDF is reused for:

- Downloading from the application
- Sending as an email attachment

No duplicate PDF generation logic exists.

---

# Certificate Service

The `CertificateService` is responsible for:

- Generate Winner Certificate
- Download Winner Certificate
- Send Winner Certificate Email

Available methods:

```csharp
GenerateWinnerCertificateAsync()

SendWinnerCertificateAsync()
```

This keeps all certificate-related functionality in one place.

---

# Email Service

`EmailService` is generic and reusable.

Supports:

- HTML emails
- Plain text emails
- PDF attachments
- Future document attachments

Method:

```csharp
SendEmailAsync(
    string toEmail,
    string subject,
    string body,
    byte[]? attachment = null,
    string? attachmentName = null,
    bool isHtml = true);
```

This method can be reused for:

- Invitations
- Winner Certificates
- Agreements
- Invoices
- Reports
- Future documents

---

# Winner Email Flow

```text
Match Completed
        ↓
Tournament Winner Updated
        ↓
SaveChanges()
        ↓
Create Notification
        ↓
CertificateService.SendWinnerCertificateAsync()
        ↓
Load Tournament
        ↓
Generate PDF Certificate
        ↓
Read Winner Email Template
        ↓
Replace HTML Placeholders
        ↓
EmailService.SendEmailAsync()
        ↓
Winner Receives Email + PDF Certificate
```

---

# Download Certificate Flow

```text
User Clicks Download
        ↓
CertificateService
        ↓
GenerateWinnerCertificateAsync()
        ↓
Return PDF (byte[])
        ↓
File Download
```

---

# Design Principles

- Single Responsibility Principle (SRP)
- Reusable Email Service
- Reusable Certificate Generator
- No duplicated PDF generation logic
- HTML templates separated from business logic
- Database access remains inside business services
- Certificate generation remains independent of Entity Framework models

---

# 📦 API Modules

- Authentication
- Users
- Games
- Tournaments
- Tournament Matches
- Announcements
- Notifications
- Conversations
- Online Users
- Dashboard
- Logs

---

# 🛠 Design Patterns Used

- Dependency Injection
- Service Layer
- DTO Pattern
- Soft Delete
- Pagination
- Async Programming
- JWT Authentication
- SignalR
- Clean Architecture Principles

---

# 🔒 Security

- JWT Authentication
- Role Authorization
- AES-256 Encryption
- Secure Password Storage
- Protected APIs

---

# 🚀 Features Completed

- JWT Authentication
- User Management
- Game CRUD
- Tournament CRUD
- Tournament Match Generation
- Admin Dashboard
- Announcement Module
- Notification Count
- Real Time Announcements
- Online User Tracking
- Private Chat
- SignalR Chat
- AES Encrypted Messages
- Conversation Management
- Read Messages
- Pagination
- Soft Delete

---

# 🎯 Learning Objectives

This project was built to gain hands-on experience with enterprise backend development including:

- ASP.NET Core
- Entity Framework Core
- SQL Server
- SignalR
- Authentication
- Real Time Communication
- Encryption
- Clean Architecture
- Production Ready API Design

---

### Hangfire Background Jobs

- Added **Hangfire** for background job scheduling.
- Using **SQL Server Storage** to persist job information.
- Hangfire Dashboard available at:

```text
https://localhost:{port}/hangfire
```

- Current implementation:
  - `Logger` background job.
  - Runs **every minute** using `Cron.Minutely()`.
  - Currently logs the current date and time (used for learning and Hangfire verification).

#### NuGet Packages

- Hangfire
- Hangfire.AspNetCore
- Hangfire.SqlServer
