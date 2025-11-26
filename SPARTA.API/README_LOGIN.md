# Sistema de Login - SPARTA

## Configuración Inicial

### 1. Base de Datos

1. Ejecuta el script `SPARTA.BD/sparta.bd/sparta.bd/Tables/Usuario.sql` para crear la tabla Usuario.

2. Para crear un usuario de prueba:
   - Ejecuta el backend
   - Llama al endpoint `POST /api/User/hash-password` con el body:
     ```json
     {
       "password": "Admin123!"
     }
     ```
   - Copia el hash retornado
   - Ejecuta el siguiente SQL:
     ```sql
     INSERT INTO [dbo].[Usuario] 
         ([NombreUsuario], [Email], [PasswordHash], [Nombre], [Apellido], [Activo], [FechaCreacion])
     VALUES 
         ('admin', 'admin@sparta.com', 'TU_HASH_AQUI', 'Administrador', 'SPARTA', 1, GETDATE());
     ```

### 2. Configuración del Backend

El archivo `appsettings.json` ya está configurado con:
- JWT Key, Issuer y Audience
- Connection String para SQL Server

### 3. Endpoints Disponibles

- `POST /api/User/login` - Iniciar sesión
  - Body: `{ "nombreUsuario": "admin", "password": "Admin123!" }`
  - Retorna: Token JWT y datos del usuario

- `POST /api/User/hash-password` - Generar hash de contraseña (solo para desarrollo)
  - Body: `{ "password": "tu_contraseña" }`
  - Retorna: Hash para insertar en la BD

### 4. Frontend

El frontend está configurado para:
- Conectarse a `https://localhost:7109/api`
- Proteger todas las rutas excepto `/login`
- Almacenar el token en localStorage
- Redirigir a login si no hay token

## Uso

1. Inicia el backend en `https://localhost:7109`
2. Inicia el frontend en `http://localhost:5173`
3. Navega a la aplicación
4. Serás redirigido a `/login` si no tienes token
5. Ingresa las credenciales del usuario creado
6. Serás redirigido a `/history` después del login exitoso

## Notas de Seguridad

- El endpoint `hash-password` debe ser removido o protegido en producción
- Cambia la JWT Key en `appsettings.json` por una clave segura y única
- Los tokens expiran después de 24 horas

