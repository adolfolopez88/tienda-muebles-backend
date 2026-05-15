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

- [ ] **AC-1:** Proyecto compila con `dotnet build` sin errores ni warnings
- [ ] **AC-2:** Estructura de 4 capas: Api, Application, Domain, Infrastructure
- [ ] **AC-3:** Swagger disponible en `/swagger` con documentación de endpoints
- [ ] **AC-4:** DbContext configurado con SQL Server y cadena de conexión en `appsettings.json`
- [ ] **AC-5:** Migración inicial ejecutable con `dotnet ef migrations add InitialCreate`
- [ ] **AC-6:** Health check endpoint `GET /health` devuelve 200

### Fase 2: Autenticación

- [ ] **AC-7:** `POST /api/auth/register` — crea usuario, devuelve JWT + refresh token
- [ ] **AC-8:** `POST /api/auth/login` — valida credenciales, devuelve JWT + refresh token
- [ ] **AC-9:** `POST /api/auth/refresh` — recibe refresh token, devuelve nuevo JWT + rota refresh
- [ ] **AC-10:** `GET /api/auth/me` — protegido con `[Authorize]`, devuelve datos del usuario autenticado
- [ ] **AC-11:** Passwords hasheados con BCrypt (nunca en texto plano)
- [ ] **AC-12:** Roles Admin y Customer implementados, `[Authorize(Roles = "Admin")]` funcional
- [ ] **AC-13:** Refresh tokens almacenados en DB con expiración de 7 días
- [ ] **AC-14:** JWT expira en 15 minutos, configurable en `appsettings.json`

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
| AC-1 | Build sin errores | ⏳ | — |
| AC-2 | 4 capas Clean Architecture | ⏳ | — |
| AC-3 | Swagger /swagger | ⏳ | — |
| AC-4 | DbContext SQL Server | ⏳ | — |
| AC-5 | Migración inicial | ⏳ | — |
| AC-6 | Health check /health | ⏳ | — |
| AC-7 | POST /api/auth/register | ⏳ | — |
| AC-8 | POST /api/auth/login | ⏳ | — |
| AC-9 | POST /api/auth/refresh | ⏳ | — |
| AC-10 | GET /api/auth/me | ⏳ | — |
| AC-11 | Passwords BCrypt | ⏳ | — |
| AC-12 | Roles Admin + Customer | ⏳ | — |
| AC-13 | Refresh tokens en DB | ⏳ | — |
| AC-14 | JWT 15min configurable | ⏳ | — |

## Bitácora

| Fecha | Evento |
|-------|--------|
| 2026-05-15 | Spec creada. Fase 1: API Base. Fase 2: Auth JWT. 14 ACs pendientes.
