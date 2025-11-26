# Reglas de Estructura para Casos de Uso

Este documento define la estructura estándar que debe seguirse cada vez que se implemente un nuevo caso de uso en el proyecto SPARTA.

## Estructura por Capas

### Ejemplo: Crear Usuario

---

## 📁 Capa de Presentación (SPARTA.API)

### Controllers
```
SPARTA.API/
└── Controllers/
    └── Feature/
        └── [FeatureName]/
            └── [FeatureName]Controller.cs
```

**Ejemplo:**
```
SPARTA.API/
└── Controllers/
    └── Feature/
        └── Users/
            └── UserController.cs
```

### Models (Request/Response)
```
SPARTA.API/
└── Controllers/
    └── Feature/
        └── Models/
            └── [Request/Response Models]
```

**Ejemplo:**
```
SPARTA.API/
└── Controllers/
    └── Feature/
        └── Models/
            └── CreateUserRequest.cs
            └── UpdateUserRequest.cs
            └── UserResponse.cs
```

---

## 📁 Capa de Servicio (SPARTA.Service)

### Interfaces
```
SPARTA.Service/
└── Feature/
    └── [FeatureName]/
        └── Interfaces/
            └── I[FeatureName]Service.cs
```

**Ejemplo:**
```
SPARTA.Service/
└── Feature/
    └── Users/
        └── Interfaces/
            └── IUserService.cs
```

### Casos de Uso (Implementaciones)
```
SPARTA.Service/
└── Feature/
    └── [FeatureName]/
        └── CaseUse/
            └── [FeatureName]Service.cs
```

**Ejemplo:**
```
SPARTA.Service/
└── Feature/
    └── Users/
        └── CaseUse/
            └── UserService.cs
```

---

## 📁 Capa de Dominio (SPARTA.Domain)

### Entidades
```
SPARTA.Domain/
└── Entities/
    └── [FeatureName]/
        └── [FeatureName].cs
```

**Ejemplo:**
```
SPARTA.Domain/
└── Entities/
    └── Users/
        └── User.cs
```

### DTOs
```
SPARTA.Domain/
└── Entities/
    └── [FeatureName]/
        └── Dtos/
            └── [FeatureName]Dto.cs
```

**Ejemplo:**
```
SPARTA.Domain/
└── Entities/
    └── Users/
        └── Dtos/
            └── UserDto.cs
```

### Interfaces de Repositorio
```
SPARTA.Domain/
└── Interfaces/
    └── [FeatureName]/
        └── I[FeatureName]Repository.cs
```

**Ejemplo:**
```
SPARTA.Domain/
└── Interfaces/
    └── Users/
        └── IUserRepository.cs
```

---

## 📁 Capa de Repositorio (SPARTA.Infrastructure)

### Implementación de Repositorio
```
SPARTA.Infrastructure/
└── Repositories/
    └── [FeatureName]Repository.cs
```

**Ejemplo:**
```
SPARTA.Infrastructure/
└── Repositories/
    └── UserRepository.cs
```

---

## 📋 Checklist para Nuevo Caso de Uso

Al crear un nuevo caso de uso (por ejemplo: "Products"), seguir esta estructura:

### ✅ Capa de Presentación
- [ ] `Controllers/Feature/[FeatureName]/[FeatureName]Controller.cs`
- [ ] `Controllers/Feature/Models/[Request/Response Models]`

### ✅ Capa de Servicio
- [ ] `Feature/[FeatureName]/Interfaces/I[FeatureName]Service.cs`
- [ ] `Feature/[FeatureName]/CaseUse/[FeatureName]Service.cs`

### ✅ Capa de Dominio
- [ ] `Entities/[FeatureName]/[FeatureName].cs`
- [ ] `Entities/[FeatureName]/Dtos/[FeatureName]Dto.cs`
- [ ] `Interfaces/[FeatureName]/I[FeatureName]Repository.cs`

### ✅ Capa de Repositorio
- [ ] `Repositories/[FeatureName]Repository.cs`

---

## 📝 Notas Importantes

1. **Nomenclatura**: Usar PascalCase para nombres de clases y archivos
2. **Organización**: Cada feature debe estar agrupada en su propia carpeta
3. **Separación de Responsabilidades**: 
   - Controllers solo manejan HTTP
   - Services contienen la lógica de negocio
   - Domain contiene entidades y contratos
   - Infrastructure contiene implementaciones de acceso a datos
4. **Dependencias**: 
   - API → Service → Domain
   - Service → Domain
   - Infrastructure → Domain
   - Domain no depende de ninguna otra capa

---

## 🌐 Regla de Nomenclatura: Campos en Inglés

**IMPORTANTE**: Todos los campos, propiedades y nombres de columnas en la base de datos DEBEN estar en inglés.

### Base de Datos
- ✅ Usar nombres de columnas en inglés: `Username`, `Email`, `FirstName`, `LastName`, `IsActive`, `CreatedAt`, `UpdatedAt`
- ❌ NO usar español: `NombreUsuario`, `Nombre`, `Apellido`, `Activo`, `FechaCreacion`

### Backend (C#)
- ✅ Propiedades en inglés: `Username`, `Email`, `FirstName`, `LastName`, `IsActive`, `CreatedAt`, `UpdatedAt`
- ✅ Parámetros de métodos en inglés: `username`, `email`, `firstName`
- ✅ Nombres de métodos en inglés: `GetByUsernameAsync`, `ExistsByEmailAsync`

### Frontend (JavaScript/React)
- ✅ Variables y propiedades en inglés: `username`, `email`, `firstName`, `lastName`, `isActive`
- ✅ Nombres de campos en formularios en inglés: `username`, `password`
- ✅ Propiedades de objetos en inglés: `user.username`, `user.email`

### Ejemplo de Mapeo Correcto

**Base de Datos:**
```sql
CREATE TABLE [dbo].[Users] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Username] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [FirstName] NVARCHAR(200) NULL,
    [LastName] NVARCHAR(200) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2 NULL
);
```

**Entidad C#:**
```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
```

**Frontend JavaScript:**
```javascript
const loginData = {
  username: 'admin',
  password: 'password123'
};

const user = {
  id: 1,
  username: 'admin',
  email: 'admin@example.com',
  firstName: 'John',
  lastName: 'Doe',
  isActive: true
};
```

---

## 🔄 Ejemplo Completo: Users

```
SPARTA.API/
├── Controllers/
│   └── Feature/
│       ├── Users/
│       │   └── UserController.cs
│       └── Models/
│           ├── CreateUserRequest.cs
│           └── UserResponse.cs

SPARTA.Service/
└── Feature/
    └── Users/
        ├── Interfaces/
        │   └── IUserService.cs
        └── CaseUse/
            └── UserService.cs

SPARTA.Domain/
├── Entities/
│   └── Users/
│       ├── User.cs
│       └── Dtos/
│           └── UserDto.cs
└── Interfaces/
    └── Users/
        └── IUserRepository.cs

SPARTA.Infrastructure/
└── Repositories/
    └── UserRepository.cs
```

---

**Última actualización**: 2024
