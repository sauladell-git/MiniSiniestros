---
name: configuracion-ci
description: Genera y configura el archivo de workflow de GitHub Actions para automatizar las pruebas unitarias y validaciones de compilación en los Pull Requests (PR). Se activa cuando el usuario solicita "configurar CI", "chequeos de PR", "github actions", "pipeline", "workflow de test", o "automatizar pruebas en git".
---

# Skill: Configuración de CI para Pull Requests

Esta skill genera y configura el flujo de trabajo (workflow) en GitHub Actions para asegurar que todo Pull Request (PR) compile correctamente y pase las pruebas unitarias antes de poder fusionarse.

## Paso 1 – Relevar la versión de .NET y Solución
Identificar la versión de .NET utilizada en el proyecto y el nombre de la solución.
- Leer el archivo `.csproj` principal para verificar el `<TargetFramework>` (por ejemplo, `net8.0`).
- Confirmar el nombre del archivo `.sln` para ejecutar la compilación y pruebas globales.

## Paso 2 – Crear el Archivo de Workflow de GitHub Actions
Crear el archivo `.github/workflows/dotnet-ci.yml` si no existe. 

El contenido del archivo debe seguir la estructura recomendada para proyectos .NET:

```yaml
name: .NET Core CI

on:
  push:
    branches: [ "main", "master", "develop" ]
  pull_request:
    branches: [ "main", "master", "develop" ]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x' # Ajustar según el TargetFramework detectado

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore --configuration Release

    - name: Test
      run: dotnet test --no-build --configuration Release --verbosity normal
```

## Paso 3 – Manejo de Referencias Locales Externas (Si aplica)
Si el archivo `.csproj` referencia archivos DLL locales que se encuentran fuera del repositorio (por ejemplo, en rutas como `..\..\OtroProyecto\bin\Debug\net8.0\Otro.dll` o `..\..\..\DLLs\Otra.dll`):
1. Copiar los archivos DLL correspondientes al repositorio local en una carpeta designada (por ejemplo, `libs/`).
2. Actualizar las referencias `<HintPath>` en el archivo `.csproj` para que apunten a la ruta local interna (por ejemplo, `..\libs\Otro.dll`).
3. Asegurarse de que esta carpeta `libs/` no esté excluida en `.gitignore` para que las dependencias suban a Git y estén disponibles en el pipeline de CI.

## Paso 4 – Manejo de Proyectos de Test Inexistentes (Si aplica)
Si la solución no contiene proyectos de test:
1. Advertir al usuario que el workflow ejecutará `dotnet test`, pero que actualmente la solución no tiene ningún proyecto de pruebas unitarias configurado.
2. Ofrecer crear un proyecto de pruebas estándar (como xUnit) y agregarlo a la solución, o adaptar temporalmente el comando `dotnet test` para que no falle si no hay pruebas.

## Paso 5 – Confirmar y Guardar
1. Presentar el archivo `.github/workflows/dotnet-ci.yml` propuesto.
2. Escribir el archivo tras la aprobación del usuario.
3. Sugerir los comandos git para subir el archivo de configuración a la rama remota.
