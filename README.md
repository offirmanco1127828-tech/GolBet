# GolBet — Módulo 7 resuelto

Incluye la solución completa `GolBet.sln` con cuatro proyectos: Entities, Repositories, Services y Web. Incluye el módulo 6 más ASP.NET Core Identity: registro, inicio/cierre de sesión, saldo inicial de 100.000 FutCoins, roles `Apostador` y `Admin`, menú con nombre/saldo y autorización para las operaciones administrativas. También queda modelada la relación de apuestas y el índice único `(MatchId, UserId, Pick)` para el módulo posterior.

## Requisitos
- .NET SDK 8 instalado (`dotnet --list-sdks` debe mostrar una versión 8.x).
- Visual Studio Code y extensión C#.
- Conexión a internet la primera vez para restaurar paquetes y cargar Bootstrap/validaciones externas.

Se configuró SQLite (`golbet.db`) para que pueda ejecutarse en macOS sin SQL Server/LocalDB. La base y los equipos/partidos de demostración se crean al iniciar. No se requieren migraciones.

## Ejecutar en Visual Studio Code (Mac)
1. Descomprime este ZIP en una carpeta distinta a la del módulo 6.
2. En VS Code, selecciona **File > Open Folder...** y abre la carpeta `GolBet_Modulo_7_Resuelto` (la que contiene `GolBet.sln`).
3. Abre **Terminal > New Terminal**.
4. Ejecuta, una línea a la vez:

```bash
dotnet --list-sdks
dotnet restore GolBet.sln
dotnet build GolBet.sln
dotnet run --project GolBet.Web/GolBet.Web.csproj
```

5. Abre en Chrome la URL que aparezca como `Now listening on: ...`.

## Cómo comprobarlo
- **Bien:** `dotnet build` termina con `Build succeeded` y `0 Error(s)`.
- **Bien:** la página muestra enlaces de registro e inicio de sesión.
- **Bien:** regístrate con un correo válido y una contraseña de al menos 8 caracteres, incluyendo mayúscula, minúscula, número y símbolo. Debes entrar y ver el saldo inicial de **100.000**.
- **Bien:** cierra sesión y vuelve a iniciar sesión con las mismas credenciales.
- **Bien:** una cuenta normal con rol `Apostador` no puede abrir las pantallas de crear/editar/desactivar equipos o partidos.

### Crear una cuenta administradora de prueba
Por seguridad no hay una contraseña de administrador escrita en el código. Detén la app y, en la misma terminal, define variables antes de volver a ejecutarla:

```bash
export AdminBootstrap__Email="admin@golbet.local"
export AdminBootstrap__Password="AdminGolBet2026!"
dotnet run --project GolBet.Web/GolBet.Web.csproj
```

Luego inicia sesión con ese correo y contraseña. Debe aparecer el acceso a crear equipos/partidos. Cambia la contraseña de ejemplo si vas a conservar la base. La cuenta se crea solamente si aún no existe.

## Si algo sale mal
- `dotnet: command not found` o no aparece SDK 8: instala .NET SDK 8 para macOS y vuelve a abrir la terminal.
- `Build FAILED` o errores `CS...`: copia todo el primer bloque de errores de la terminal.
- Error de contraseña al registrar: comprueba mayúscula, minúscula, número, símbolo y longitud mínima de 8.
- Si la base quedó en un estado de prueba incorrecto, detén la aplicación y elimina `GolBet.Web/golbet.db`; la base se recrea al iniciar. Esto borra las cuentas y datos locales.

> Nota honesta: el paquete se preparó y se revisaron las referencias y archivos, pero en el entorno donde se generó no estaba instalado el SDK .NET; por eso la compilación final debe confirmarse con los comandos anteriores en tu Mac.
