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
