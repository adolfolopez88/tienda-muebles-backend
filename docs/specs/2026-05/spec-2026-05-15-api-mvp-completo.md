---
type: spec
status: completed
date: 2026-05-15
proyecto: tienda-muebles-backend
area: backend
prioridad: alta
tags:
  - spec
  - sdd
  - api
  - mvp
  - ecommerce
  - net10
---

# SPEC -- API MVP Completo Tienda Muebles (MAISON NOIR)

> [!INFO] Especificacion SDD
> **Proyecto**: tienda-muebles-backend
> **Stack**: .NET 10 + EF Core + SQL Server
> **Estado**: Completado
> **Extiende**: spec-2026-05-15-api-base-auth (auth base ya construida)

---

## 1. Proposito

Extender el backend .NET existente (solo auth) para convertirlo en el API completa que abastece al frontend MAISON NOIR, reemplazando al backend Node.js de HomeVision.

### Drivers
- Arquitectura: Clean Architecture 4 capas (Domain, Application, Infrastructure, Api)
- Stack: .NET 10, EF Core 10, SQL Server, JWT + refresh tokens
- Frontend destino: Next.js 14 en HomeVision

---

## 2. Alcance

| Feature | Descripcion |
|---------|-------------|
| Productos | CRUD completo + paginacion + filtro categoria + toggle |
| Categorias | CRUD (crear y listar) |
| Pedidos | Crear (guest/cliente), consultar por numero, listar admin |
| Imagenes | Upload a Cloudinary, asociar a producto |
| Pagos | Stripe PaymentIntent + webhook |
| Emails | Confirmacion de pedido via Resend |
| Seed | Endpoint protegido para poblar BD |
| Validacion | FluentValidation en todos los DTOs |
| Paginacion | Formato estandarizado |

### Fuera de alcance
- Dashboard admin -- se implementa en el frontend
- Panel de agenda / RBAC avanzado -- spec futura
- Diseno con IA (Claude Design) -- spec futura
---

## 3. Modelo de Datos

### Entidades nuevas en Domain

- Producto.cs: Guid Id, string Nombre, string Slug (unique), string Descripcion, decimal Precio, string CategoriaSlug (FK), int Stock, string? Badge, string? Dimensiones, string? Material, string[]? Colores, bool Activo, Categoria Categoria, ICollection<ImagenProducto> Imagenes
- Categoria.cs: Guid Id, string Nombre, string Slug (unique), int Orden
- ImagenProducto.cs: Guid Id, Guid ProductoId, string Url (Cloudinary), string? Alt, int Orden
- Pedido.cs: Guid Id, string Numero (MN-XXXXX unique), Guid? ClienteId (nullable), string Email, EstadoPedido Estado, string MetodoPago, decimal Subtotal, ICollection<ItemPedido> Items (owned), DatosEnvio DatosEnvio (owned)
- EstadoPedido enum: Completado=0, Procesando=1, Enviado=2, Entregado=3, Cancelado=4

### Decisiones de diseno
- Producto.CategoriaSlug es FK a Categoria.Slug -- URLs limpias con integridad referencial
- Pedido.Items son snapshots -- preservan valor historico aunque el producto cambie
- Pedido.ClienteId nullable -- guest checkout sin registro
- ImagenProducto entidad separada -- producto tiene N imagenes ordenadas
---

## 4. Endpoints API REST

### Auth /api/auth (existe)

| POST | /register | -- | Registro cliente |
| POST | /login | -- | Login JWT + refresh |
| POST | /refresh | -- | Rotar refresh token |
| GET | /me | JWT | Perfil usuario |

### Productos /api/productos

| GET | / | -- | Listar (?categoria=, ?page=1, ?limit=12) |
| GET | /destacados | -- | Productos con badge + stock > 0 |
| GET | /{slug} | -- | Detalle con imagenes |
| POST | / | Admin | Crear producto |
| PUT | /{slug} | Admin | Editar producto |
| PATCH | /{slug}/toggle | Admin | Activar/desactivar |
| DELETE | /{slug} | Admin | Soft-delete |

### Categorias /api/categorias

| GET | / | -- | Listar ordenadas por Orden |
| POST | / | Admin | Crear categoria |

### Pedidos /api/pedidos

| POST | / | -- | Crear pedido |
| GET | /{numero} | -- | Consultar por numero |
| GET | / | Admin | Listar (?estado=, ?page=) |
| PATCH | /{numero}/estado | Admin | Cambiar estado |

### Imagenes /api/imagenes

| POST | /upload | Admin | Subir imagen (multipart) |
| DELETE | /{id} | Admin | Eliminar imagen |

### Pagos /api/pagos

| POST | /create-intent | -- | Crear Stripe PaymentIntent |
| POST | /webhook | -- | Webhook Stripe |

### Seed /api/seed

| POST | / | Admin | Poblar BD (8 productos, 4 categorias, admin) |
---

## 5. Integraciones Externas

### 5.1 Stripe (Pagos)
- Paquete: Stripe.net
- Flujo: Frontend > POST /create-intent > PaymentIntent(amount,currency=mxn) > clientSecret > stripe.confirmPayment > webhook > actualizar pedido

### 5.2 Resend (Emails)
- Paquete: Resend
- Disparadores: Pedido creado -> confirmacion. Pedido enviado -> notificacion.
- Interfaz: IEmailService en Application, implementacion en Infrastructure

### 5.3 Cloudinary (Imagenes)
- Paquete: CloudinaryDotNet
- Flujo: multipart upload -> Cloudinary -> guardar URL en BD
- Limite: 5MB por imagen

### Interfaces en Application
- IPaymentService: CreatePaymentIntentAsync, ConfirmPaymentAsync
- IEmailService: SendOrderConfirmationAsync, SendShippingNotificationAsync
- IImageStorageService: UploadAsync, DeleteAsync

---

## 6. Middleware y Error Handling

### Pipeline
Rate Limiting -> CORS -> JWT Auth -> FluentValidation -> Controller -> Service -> EF Core

### Validacion (FluentValidation)
- Paquete: FluentValidation.AspNetCore
- Cada DTO tiene su validador, fallo -> 422 con errors: { campo: [mensaje] }

### Error Handling (RFC 7807)
| NotFoundException | 404 |
| ValidationException | 422 |
| UnauthorizedAccessException | 403 |
| DuplicateException | 409 |
| Exception | 500 |

### Paginacion
{ data: [...], pagination: { page, limit, total, totalPages } }
---

## 7. Testing

### Cobertura objetivo

| Feature | Unitarios | Integracion |
|---------|-----------|-------------|
| Auth | 8 (existentes) | -- |
| Productos | ~12 | ~4 |
| Categorias | ~4 | ~2 |
| Pedidos | ~10 | ~3 |
| Pagos | ~6 | -- |
| Imagenes | ~4 | ~2 |
| Total | ~44 | ~11 |
---

## 8. Criterios de Aceptacion

### AC-01 -- Catalogo publico
- [ ] [x] AC-01.1: GET /api/productos devuelve productos activos paginados (12 por pagina)
- [ ] [x] AC-01.2: GET /api/productos?categoria=sala filtra por categoria
- [ ] [x] AC-01.3: GET /api/productos/destacados productos con badge y stock > 0
- [ ] [x] AC-01.4: GET /api/productos/{slug} detalle con imagenes

### AC-02 -- Categorias
- [ ] [x] AC-02.1: GET /api/categorias lista ordenada por Orden

### AC-03 -- Gestion admin productos
- [ ] [x] AC-03.1: POST /api/productos crea producto (admin only)
- [ ] [x] AC-03.2: PUT /api/productos/{slug} actualiza producto
- [ ] [x] AC-03.3: PATCH /api/productos/{slug}/toggle activa/desactiva
- [ ] [x] AC-03.4: DELETE /api/productos/{slug} soft-delete
- [ ] [x] AC-03.5: 401 sin JWT, 403 con JWT cliente

### AC-04 -- Upload imagenes
- [ ] [x] AC-04.1: POST /api/imagenes/upload multipart -> Cloudinary -> URL
- [ ] [x] AC-04.2: DELETE /api/imagenes/{id} elimina imagen
- [ ] [x] AC-04.3: Archivos > 5MB rechazados con 422

### AC-05 -- Pedidos
- [ ] [x] AC-05.1: POST /api/pedidos crea pedido con items, envio
- [ ] [x] AC-05.2: Numero orden unico MN-XXXXX
- [ ] [x] AC-05.3: GET /api/pedidos/{numero} consulta publica
- [ ] [x] AC-05.4: GET /api/pedidos admin paginado con filtro
- [ ] [x] AC-05.5: PATCH /api/pedidos/{numero}/estado admin

### AC-06 -- Pagos
- [ ] [x] AC-06.1: POST /api/pagos/create-intent -> clientSecret
- [ ] [x] AC-06.2: Webhook recibe evento y actualiza pedido
- [ ] [x] AC-06.3: Webhook verifica firma Stripe-Signature

### AC-07 -- Emails
- [ ] [x] AC-07.1: Pedido creado -> email confirmacion
- [ ] [x] AC-07.2: Email contiene numero orden, resumen, total

### AC-08 -- Seed
- [ ] [x] AC-08.1: POST /api/seed admin only
- [ ] [x] AC-08.2: Crea 4 categorias, 8 productos, 1 admin

### AC-09 -- Validacion
- [ ] [x] AC-09.1: DTOs invalidos -> 422 con detalles
- [ ] [x] AC-09.2: Errores 500 -> Problem Details (RFC 7807)

### AC-10 -- Seguridad
- [ ] [x] AC-10.1: Admin endpoints requieren rol Admin
- [ ] [x] AC-10.2: JWT Secret configurable sin fallback hardcodeado
- [ ] [x] AC-10.3: Rate limiting 100 req/15min
- [ ] [x] AC-10.4: CORS restringido a FRONTEND_URL

### AC-11 -- Sincronizacion frontend
- [ ] [x] AC-11.1: Frontend MAISON NOIR consume este API
- [ ] [x] AC-11.2: Endpoints frontend compatibles con .NET
---

## 9. Tareas de implementacion

### Fase 1 -- Productos y Categorias
- [x] T-01: Entidades Producto, Categoria, ImagenProducto en Domain
- [x] T-02: DTOs en Application
- [x] T-03: Interfaces de servicio (IProductoService, ICategoriaService)
- [x] T-04: Implementar servicios en Infrastructure
- [x] T-05: Controladores ProductosController, CategoriasController
- [x] T-06: Migracion EF Core para nuevas tablas
- [x] T-07: Endpoint seed con datos iniciales
- [x] T-08: Tests unitarios

### Fase 2 -- Pedidos
- [x] T-09: Entidades Pedido, ItemPedido, DatosEnvio, EstadoPedido
- [x] T-10: DTOs de pedido
- [x] T-11: Servicio pedidos + generador MN-XXXXX
- [x] T-12: PedidosController
- [x] T-13: Migracion Pedidos
- [x] T-14: Tests unitarios

### Fase 3 -- Imagenes
- [x] T-15: IImageStorageService + Cloudinary impl
- [x] T-16: ImagenesController
- [x] T-17: Tests unitarios

### Fase 4 -- Pagos y Emails
- [x] T-18: IPaymentService + Stripe impl
- [x] T-19: PagosController
- [x] T-20: IEmailService + Resend impl
- [x] T-21: Disparo emails en pedido
- [x] T-22: Tests unitarios

### Fase 5 -- Infraestructura
- [x] T-23: FluentValidation en todos los DTOs
- [x] T-24: Problem Details middleware global
- [x] T-25: Paginacion helper
- [x] T-26: Rate limiting + CORS revision
- [x] T-27: Tests integracion (WebApplicationFactory)
- [x] T-28: Actualizar docker-compose
---

## 10. Tabla de Progreso

| Fase | Tareas | Estado | Fecha |
|------|--------|--------|-------|
| 1. Productos y Categorias | T-01 a T-08 | Completado | -- |
| 2. Pedidos | T-09 a T-14 | Completado | -- |
| 3. Imagenes | T-15 a T-17 | Completado | -- |
| 4. Pagos y Emails | T-18 a T-22 | Completado | -- |
| 5. Infraestructura | T-23 a T-28 | Completado | -- |
| **Total** | **28 tareas** | **100%** | |

---

## 11. Bitacora

| Fecha | Evento |
|-------|--------|
| 2026-05-15 | Spec creada.
| 2026-05-17 | Fases 1-5 completadas. PostgreSQL migrado. 22 endpoints. 27 tests. Build 0/0.. Analisis de codigo base (.NET + Node.js HomeVision). Diseno aprobado (Clean Architecture extendida). |
