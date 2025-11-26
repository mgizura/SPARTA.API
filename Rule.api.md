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

