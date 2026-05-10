# URL Shortener API (Coding Challenge Solution)

A lightweight URL Shortener built with ASP.NET Core, Entity Framework Core, and SQLite.



# Setup Guide – URL Shortener API

This guide explains how to set up and run the project locally.

---


# 1. Clone the Repository

```bash
git clone https://github.com/M-Sila/UrlShortener.git
cd url-shortener
```

---

# 2. Restore Dependencies

```bash
dotnet restore
```

---

# 3. Configure Database (SQLite)

This project uses SQLite by default (no external DB required).

Default connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=url-shortener.db"
  }
}
```

👉 The database file will be created automatically.

---

# 4. Install EF Core Tool (if needed)

```bash
dotnet tool install --global dotnet-ef
```

Check installation:

```bash
dotnet ef --version
```

---

# 🗄️ 5. Create Database (Migrations)

Apply existing migrations:

```bash
dotnet ef database update
```

If migrations are not created yet:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

# ▶️ 6. Run the Application

```bash
dotnet run
```


---

# 📡 7. Swagger (API Documentation)

If enabled in Development mode:

```
https://localhost:7244/swagger
```

---

# 🧪 8. Run Tests (not implemented yet)

```bash
dotnet test
```

Includes:
- Unit tests (Moq)
- Integration tests (EF Core InMemory)

---

# 🧹 Optional: Clean & Rebuild

```bash
dotnet clean
dotnet build
```

---



This project was implemented as a coding challenge with a focus on:
- clean architecture
- fast redirects
- asynchronous click tracking
- simplicity over overengineering
- testability

---

# Important Note About Requirements

Not all optional features from the challenge were implemented within the time frame:

- ❌ Statistics endpoint (`/api/links/{slug}/stats`) was NOT implemented
- ❌ Advanced analytics aggregation (top referrers, user agents) was not completed
- ✔ Core requirements (shortening, redirect, tracking, disable, expiry) are implemented

The system is designed in a way that statistics can be added without architectural changes.

---

# Features Implemented

## Core functionality

- Create short links with optional custom slug
- Automatic slug generation
- Redirect to original URL
- Link expiration support
- Link disabling
- Click tracking (timestamp, referrer, user-agent)

## Performance-focused design

- Redirects are ultra-fast (no database writes in request path)
- Click tracking is asynchronous
- Background worker processes click queue

---

# Architecture Overview

## Layered architecture

- API Layer (Controllers)
- Application Layer (Business logic / services)
- Domain Layer (Entities + rules)
- Infrastructure Layer (EF Core + repositories)

---

## Key design decision: async click tracking

Click tracking is the most performance-critical part of the system.

Instead of writing to the database during redirect:

- Clicks are pushed into an in-memory queue (`Channel<T>`)
- A `BackgroundService` processes and persists them asynchronously

### Why?

Because redirect endpoints must be:
- extremely fast
- non-blocking
- highly scalable

---

## Repository Pattern

All database access is abstracted via repositories.

Benefits:
- separation of concerns
- easier unit testing
- replaceable persistence layer (SQLite → PostgreSQL later)

---

# Why SQLite?

SQLite was chosen intentionally for this implementation:

### Advantages

- zero setup required (runs instantly after clone)
- lightweight and portable
- ideal for coding challenges and demos
- fast enough for read-heavy workloads (like redirects)
- fully supported by EF Core migrations

### Scalability note

The architecture is not bound to SQLite.
It can be migrated to:

- PostgreSQL (recommended for production)
- SQL Server
- MySQL

without changing business logic.

---

# Scalability Considerations (IMPORTANT)

## Problem: millions of clicks per day

If this system were scaled to production level (e.g. Bitly-scale traffic), the current design would evolve as follows:

### 1. Click ingestion

Current:
- In-memory queue + background worker

At scale:
- Replace with message broker:
  - Kafka
  - RabbitMQ
  - Azure Service Bus

Reason:
- durability
- horizontal scaling
- replay capability

---

### 2. Database writes

Current:
- single-row insert per click

At scale:
- batch inserts (e.g. every 1–5 seconds)
- or stream processing

---

### 3. Analytics storage

Current:
- raw click table

At scale:
- separate analytics database
- pre-aggregated tables:
  - clicks per minute/hour/day
  - top referrers cached
- possibly OLAP system (ClickHouse / BigQuery)

---

### 4. Caching layer

At scale:
- Redis cache for slug → URL resolution
- avoids hitting database for every redirect

---

### 5. Horizontal scaling

- stateless API instances
- load balancer in front
- shared database + message broker


---

# Testing Strategy (not implemented in this challenge)

## Approach

- Unit tests: Moq (business logic isolation)
- Integration tests: EF Core InMemory database


---