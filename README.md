# Template API – .NET IAM Core

Starter template for secure .NET APIs with centralized session-based JWT authentication.

This template provides a **real session authority layer**, enabling runtime revocation, kill-switch and multi-device control.

---

## Stack

- ASP.NET Core
- JWT (JSON Web Tokens)
- Centralized Session Authority (database)
- Entity Framework Core
- PostgreSQL / MySQL
- Rate limiting

---

## Authentication Model

Login
→ Short-lived JWT Access Token
→ users_sessions (runtime session authority)
→ Controllers

---

Every protected request validates:

- JWT signature  
- Token lifetime  
- Runtime session status (revocation, expiration, multi-device control)

---

## Main Endpoints

POST /functions/v1/auth-api/signup
POST /functions/v1/auth-api/login
GET /functions/v1/auth-api/me

---

## Getting Started

1. Clone the repository
2. Open a PowerShell terminal at the root of the repository
3. Initialize the project with:
```powershell
powershell -ExecutionPolicy Bypass -File _init.ps1 MyProjectName
```
4. Configure database connection in appsettings.Environments.json
5. Configure JWT signing key (Jwt:SigningKey)
6. Configure PasswordPepper and RefreshPepper (min 64 chars) (Auth:PasswordPepper, Auth:RefreshPepper)
7. Run database migrations

Start the API

---

## Security

- Short-lived JWT tokens  
- Centralized session control  
- Real-time session revocation  
- Ready for refresh tokens, SSO and MFA

---

Template designed as a security-first API foundation.