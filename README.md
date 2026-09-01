# PokeApiEntrevista

Proyecto realizado con ASP.NET Core MVC y .NET 8 para consumir información de [PokeAPI](https://pokeapi.co/).

La aplicación muestra un listado de Pokémon y permite buscar, filtrar, cambiar de página, exportar información a Excel y enviar un Pokémon por correo.

## Funcionalidades

* Listado de Pokémon con nombre e imagen
* Filtro por nombre
* Filtro por especie
* Uso combinado de ambos filtros
* Paginación manual
* Tamaños de página de 12, 24, 48 y 96
* Exportación de la página visible a Excel
* Envío individual de Pokémon por correo
* Manejo de errores y tiempos de espera

## Requisitos

Para ejecutar el proyecto se necesita:

* .NET 8 SDK
* Conexión a internet
* Visual Studio Code o Visual Studio
* Credenciales SMTP si se quiere probar el correo

Puedes comprobar la versión de .NET con:

```powershell
dotnet --version
```

## Cómo ejecutar el proyecto

Clona el repositorio y entra a la carpeta:

```powershell
git clone https://github.com/Phantom-DLG00/PokeApiEntrevista.git
cd PokeApiEntrevista
```

Restaura las dependencias:

```powershell
dotnet restore
```

Compila el proyecto:

```powershell
dotnet build
```

Ejecuta la aplicación:

```powershell
dotnet run
```

La terminal mostrará la dirección local de la aplicación. Después se puede entrar a:

```text
/Pokemon
```

## Configuración de PokeAPI

La dirección base se encuentra en `appsettings.json`:

```json
"PokeApi": {
  "BaseUrl": "https://pokeapi.co/api/v2/"
}
```

Es necesario conservar la diagonal `/` al final de la dirección.

## Configuración del correo

Las credenciales del correo no se guardan directamente en `appsettings.json`.

Para desarrollo local se utilizan User Secrets:

```powershell
dotnet user-secrets init
```

Después se configuran los datos SMTP:

```powershell
dotnet user-secrets set "Email:Host" "smtp.gmail.com"
dotnet user-secrets set "Email:Port" "587"
dotnet user-secrets set "Email:EnableSsl" "true"
dotnet user-secrets set "Email:UserName" "TU_CORREO"
dotnet user-secrets set "Email:Password" "TU_CONTRASENA_SMTP"
dotnet user-secrets set "Email:FromAddress" "TU_CORREO"
dotnet user-secrets set "Email:FromName" "PokeApiEntrevista"
```

Si se utiliza Gmail, se necesita una contraseña de aplicación en lugar de la contraseña normal de la cuenta.

Para probar el envío:

1. Configura los datos SMTP
2. Ejecuta el proyecto
3. Abre el listado de Pokémon
4. Escribe un correo en una tarjeta
5. Presiona el botón `Enviar por correo`
6. Revisa la bandeja de entrada o correo no deseado

## Filtros y paginación

El filtro por nombre acepta coincidencias parciales.

Por ejemplo, buscar:

```text
char
```

puede mostrar:

```text
charmander
charmeleon
charizard
```

El filtro por especie se carga desde PokeAPI. Los filtros por nombre y especie se pueden utilizar al mismo tiempo.

Cuando cambia un filtro o el tamaño de página, la aplicación regresa a la página 1.

La paginación se realiza manualmente calculando el desplazamiento:

```csharp
var offset = (page - 1) * pageSize;
```

Para los resultados filtrados se utilizan `Skip` y `Take`.

## Exportación a Excel

La aplicación utiliza ClosedXML para generar archivos `.xlsx`.

El botón de exportación descarga solamente los Pokémon visibles en la página actual y conserva los filtros seleccionados.

El archivo contiene:

* Nombre del Pokémon
* URL de la imagen
* URL del recurso en PokeAPI
* Página actual
* Tamaño de página
* Filtros utilizados

## Manejo de errores

La aplicación controla diferentes problemas:

* PokeAPI tarda demasiado en responder
* No existe conexión con PokeAPI
* La solicitud es cancelada
* PokeAPI devuelve una respuesta vacía
* Ocurre un error inesperado

El tiempo máximo de espera para PokeAPI es de 10 segundos.

Los errores técnicos se escriben en la consola y el usuario recibe un mensaje más sencillo.

## Decisiones técnicas

### HttpClient

Se utilizó `AddHttpClient` para configurar la dirección de PokeAPI y el tiempo máximo de espera en un solo lugar.

Otra opción era crear manualmente un `HttpClient` en cada consulta, pero eso repetiría configuración en varias partes del proyecto.

### ClosedXML

Se utilizó ClosedXML porque permite generar archivos de Excel desde C# sin necesitar Microsoft Excel instalado.

También se consideró generar un archivo CSV, pero se eligió `.xlsx` porque permite manejar mejor las columnas y el formato.

### User Secrets

Las credenciales SMTP se guardan con User Secrets para evitar subir contraseñas a GitHub.

Guardar la contraseña directamente en `appsettings.json` sería más sencillo, pero no sería seguro.

## Notas

* La aplicación necesita internet para consultar PokeAPI
* No utiliza una base de datos
* Las credenciales SMTP deben configurarse localmente
* Los archivos de `bin` y `obj` no se incluyen en el repositorio
