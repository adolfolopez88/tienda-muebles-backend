---
type: spec
status: active
date: 2026-05-15
tags: [backend, dotnet, auth, jwt, clean-architecture]
project: tienda-muebles-backend
---

# Spec: API Base + Autenticación JWT

Backend inicial .NET 10 con Clean Architecture, autenticación JWT + refresh tokens, y roles (Admin, Customer).

## Causa Raíz

El proyecto `tienda-muebles-backend` necesita una base sólida antes de implementar funcionalidades de negocio. Se requiere estructura de proyecto Clean Architecture, conexión a base de datos, y autenticación para dos tipos de usuarios (administradores internos y clientes finales).

## Criterios de Aceptación

### Fase 1: API Base

- [x] **AC-1:** Proyecto compila con `dotnet build` sin errores ni warnings
- [x] **AC-2:** Estructura de 4 capas: Api, Application, Domain, Infrastructure
- [x] **AC-3:** Swagger disponible en `/swagger` con documentación de endpoints
- [x] **AC-4:** DbContext configurado con SQL Server y cadena de conexión en `appsettings.json`
- [x] **AC-5:** Migración inicial ejecutable con `dotnet ef migrations add InitialCreate`
- [x] **AC-6:** Health check endpoint `GET /health` devuelve 200

### Fase 2: Autenticación

- [x] **AC-7:** `POST /api/auth/register` — crea usuario, devuelve JWT + refresh token
- [x] **AC-8:** `POST /api/auth/login` — valida credenciales, devuelve JWT + refresh token
- [x] **AC-9:** `POST /api/auth/refresh` — recibe refresh token, devuelve nuevo JWT + rota refresh
- [x] **AC-10:** `GET /api/auth/me` — protegido con `[Authorize]`, devuelve datos del usuario autenticado
- [x] **AC-11:** Passwords hasheados con BCrypt (nunca en texto plano)
- [x] **AC-12:** Roles Admin y Customer implementados, `[Authorize(Roles = "Admin")]` funcional
- [x] **AC-13:** Refresh tokens almacenados en DB con expiración de 7 días
- [x] **AC-14:** JWT expira en 15 minutos, configurable en `appsettings.json`

## Tareas Técnicas

### Fase 1: API Base

1. Crear solución .NET 10 con `dotnet new sln`
2. Crear 4 proyectos de clase: Api (webapi), Application, Domain, Infrastructure
3. Configurar referencias entre proyectos (Api → Application → Domain, Api → Infrastructure, Infrastructure → Application)
4. Configurar `Program.cs`: Swagger, CORS, controllers, health checks
5. Crear `AppDbContext` en Infrastructure con `Users` y `RefreshTokens` DbSets
6. Configurar connection string en `appsettings.json` y `appsettings.Development.json`
7. Crear `User` entity en Domain con: Id, Email, PasswordHash, Role, CreatedAt
8. Crear `RefreshToken` entity en Domain con: Id, Token, UserId, ExpiresAt, CreatedAt, RevokedAt
9. Crear migración inicial
10. Agregar `health check` endpoint

### Fase 2: Autenticación

11. Crear `IAuthService` en Application (login, register, refresh, getCurrentUser)
12. Implementar `AuthService` en Infrastructure con JWT generation (System.IdentityModel.Tokens.Jwt)
13. Implementar `PasswordHasher` con BCrypt
14. Crear DTOs: LoginRequest, RegisterRequest, RefreshRequest, AuthResponse, UserResponse
15. Implementar `AuthController` con 4 endpoints
16. Crear `JwtMiddleware` para extraer user del token y setear HttpContext
17. Configurar autenticación JWT en `Program.cs` (AddAuthentication, AddJwtBearer)
18. Sembrar un usuario Admin inicial en la migración o seed data

## Tabla de Progreso

| AC | Descripción | Estado | Fecha |
|----|------------|--------|-------|
| AC-1 | Build sin errores | ✅ | 2026-05-15 |
| AC-2 | 4 capas Clean Architecture | ✅ | 2026-05-15 |
| AC-3 | Swagger /swagger | ✅ | 2026-05-15 |
| AC-4 | DbContext SQL Server | ✅ | 2026-05-15 |
| AC-5 | Migración inicial | ✅ | 2026-05-15 |
| AC-6 | Health check /health | ✅ | 2026-05-15 |
| AC-7 | POST /api/auth/register | ✅ | 2026-05-15 |
| AC-8 | POST /api/auth/login | ✅ | 2026-05-15 |
| AC-9 | POST /api/auth/refresh | ✅ | 2026-05-15 |
| AC-10 | GET /api/auth/me | ✅ | 2026-05-15 |
| AC-11 | Passwords BCrypt | ✅ | 2026-05-15 |
| AC-12 | Roles Admin + Customer | ✅ | 2026-05-15 |
| AC-13 | Refresh tokens en DB | ✅ | 2026-05-15 |
| AC-14 | JWT 15min configurable | ✅ | 2026-05-15 |

## Bitácora

| Fecha | Evento |
|-------|--------|
| 2026-05-15 | Spec creada. Fase 1: API Base. Fase 2: Auth JWT. 14 ACs pendientes.
| 2026-05-15 | Fase 1 completa: scaffold, domain, application, infrastructure, Program.cs, HealthController, Swagger |
| 2026-05-15 | Fase 2 completa: AuthService, AuthController, JWT middleware, 8 tests unitarios pasando |
| 2026-05-15 | Seed admin creado (admin@tiendamuebles.com). Migraciones: InitialCreate + SeedAdminUser |
| 2026-05-15 | Build exitoso. dotnet test: 8 passed, 0 failed. API funcional en localhost:5000 |
