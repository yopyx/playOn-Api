# PlayOn — Backend

**An ASP.NET Core backend for a YouTube-inspired video discovery application.**

PlayOn's backend separates authentication responsibilities from application resource management using a **Backend-for-Frontend (BFF) architecture**. It integrates OpenID Connect authentication through Keycloak, manages server-side session and token state, and persists application data using Entity Framework Core and PostgreSQL.

🌐 **Live application:** [play0n.vercel.app](https://play0n.vercel.app/)

---

## At a Glance

| Area | Implementation |
|---|---|
| Backend framework | ASP.NET Core / C# |
| Architecture | Backend-for-Frontend (BFF) + Resources API |
| Authentication | OpenID Connect / Keycloak |
| Session management | Server-side session state, Redis, HTTP-only cookies |
| Database | PostgreSQL |
| ORM | Entity Framework Core |
| Data access | EF Core projections, `AsNoTracking`, pagination |
| API error handling | `Results.Problem`, explicit HTTP status handling |
| API exploration | Swagger / OpenAPI |
| Development environment | Docker / Docker Compose |
| Deployment | BFF hosted on MonsterASP, database hosted on Neon, Redis through Upstash |

## Key Engineering Implementations

### 1. BFF Authentication Architecture

Implemented an authentication flow using the Backend-for-Frontend pattern and Keycloak as the OpenID Connect identity provider.

- Centralized authentication responsibilities in the BFF instead of exposing OAuth tokens directly to the React application.
- Used server-side session and token management, with an HTTP-only session cookie identifying the browser session.
- Integrated authentication challenges and logout flows through ASP.NET Core.
- Used Redis for server-side session-related storage.
- Implemented token introspection and refresh handling.
- Added per-session concurrency coordination to prevent overlapping token operations from being performed unnecessarily.

**Why it matters:** This separates browser-facing authentication from token management and provides a controlled boundary between the frontend and backend services.

### 2. Database Design and EF Core

Used Entity Framework Core with PostgreSQL to manage application resources and user-related data.

- Designed relational entities and one-to-many relationships.
- Implemented many-to-many relationships using explicit join tables.
- Applied EF Core migrations to evolve the database schema.
- Used unique indexes to enforce data integrity.
- Applied `AsNoTracking` and query projections where entity tracking or retrieving complete entities was unnecessary.
- Implemented server-side pagination to avoid retrieving unnecessarily large result sets.

### 3. API Design and Error Handling

Implemented ASP.NET Core API endpoints with explicit response handling.

- Used structured error responses through `Results.Problem`.
- Distinguished authentication and authorization failures using appropriate HTTP status codes, including `401 Unauthorized` and `403 Forbidden`.
- Configured Swagger/OpenAPI endpoint discovery for API exploration and testing.
- Handled backend failures so the frontend could distinguish unsuccessful requests and present corresponding error states.

### 4. Deployment and Development Infrastructure

The backend uses separate services for its application responsibilities and supporting infrastructure.

- PostgreSQL database hosted on Neon.
- Redis storage provided by Upstash.
- Keycloak integrated as the identity provider.
- Dockerfiles and Docker Compose used to configure the local development environment.

---

## Architecture Overview

### Main Components

| Component | Responsibility |
|---|---|
| React SPA | User interface and interaction with backend endpoints |
| BFF | Browser-facing backend boundary, authentication flow, and session/token management |
| Keycloak | Identity provider and OpenID Connect authentication |
| Resources API | Application resource operations |
| Entity Framework Core | Relational data access and schema management |
| PostgreSQL | Persistent application data |
| Redis | Server-side session-related storage |

---

## Technologies

**Backend:** C#, ASP.NET Core, Entity Framework Core

**Authentication:** Keycloak, OpenID Connect, OAuth 2.0

**Database & Storage:** PostgreSQL, Redis, Neon, Upstash

**API:** HTTP APIs, Swagger / OpenAPI

**Infrastructure:** Docker, Docker Compose

**Frontend integration:** React, TypeScript, Vercel

---

## Project Scope

PlayOn is a personal full-stack project developed to explore practical backend engineering concepts, including authentication architecture, session management, relational database design, API implementation, and deployment.

The implementation has evolved beyond basic CRUD operations, with particular attention to authentication boundaries, concurrent token operations, database efficiency, and frontend/backend integration.
