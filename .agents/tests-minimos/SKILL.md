---
name: tests-minimos
description: Valida o genera la suite mínima de tests de un proyecto de software. Usar cuando un desarrollador quiera saber si sus tests cubren el mínimo requerido, generar tests faltantes, revisar qué tipos de tests le faltan al proyecto, o preparar los tests antes de una entrega. Activar también cuando el usuario mencione "tests", "pruebas", "cobertura", "unitarios", "integración", "CI", o pregunte si su proyecto está listo para entregar en lo que respecta a testing.
---

# Skill: Tests Mínimos

Valida o genera la suite mínima de tests requerida para la entrega de un desarrollo.

## Paso 1 — Identificar el Tipo de Desarrollo

Antes de validar o generar, confirmar el tipo:
- **Frontend** (React, Angular, Vue, etc.)
- **Backend o API** (REST, GraphQL, etc.)
- **Proceso Batch, Job o Integración**

Si no está claro, preguntar al usuario.

## Paso 2 — Relevar el Estado Actual

Pedir al usuario que comparta o describa los tests existentes:
- ¿Qué framework de testing usa?
- ¿Qué tests tiene hasta ahora?
- ¿Los tests corren y pasan localmente?

Si comparte archivos de tests, leerlos antes de continuar.

## Paso 3 — Validar Contra el Estándar

Para cada categoría, determinar el estado:
- ✅ **Cumple** — existe y cubre lo mínimo
- ⚠️ **Parcial** — existe pero tiene gaps
- ❌ **Falta** — no existe

### Categorías a validar (aplican a todos los tipos):

**Tests Unitarios**
- [ ] Cubren funciones/métodos/componentes con reglas de negocio
- [ ] Validan entradas válidas e inválidas
- [ ] Cubren casos borde relevantes
- [ ] Validan manejo de errores esperados

**Tests de Integración**
- [ ] Existen tests con mock de cada dependencia crítica
- [ ] Los mocks respetan el contrato real del servicio
- [ ] Existe al menos una prueba de conexión real contra cada servicio involucrado
- [ ] Se valida generación de logs y archivos en ubicaciones esperadas

**Tests de Arranque**
- [ ] Se verifica que la aplicación/proceso arranca sin fallar
- [ ] Se valida carga de configuración requerida
- [ ] Se valida generación de log en carpeta destino
- [ ] Se verifica healthcheck o endpoint de estado, si aplica

**Tests de Seguridad**
- [ ] Se valida autenticación, si aplica
- [ ] Se valida autorización sobre operaciones sensibles
- [ ] Se verifica que los errores no exponen datos internos
- [ ] Se validan y rechazan entradas con formato inválido

**Test de Camino Feliz**
- [ ] Existe al menos un test que recorre el flujo principal completo
- [ ] Valida el resultado final esperado
- [ ] Verifica los efectos colaterales relevantes

### Criterios adicionales por tipo:

**Solo para Batch/Job/Integración:**
- [ ] Existe prueba de reejecución sobre la misma información, si el proceso puede correr más de una vez

## Paso 4 — Generar Tests Faltantes

Por cada ítem ⚠️ o ❌, ofrecer generar el test correspondiente.

Para generar, necesitar conocer:
- Lenguaje y framework de testing usado
- Estructura del código relevante (clases, métodos, endpoints)
- Dependencias externas involucradas (BD, APIs, colas)

Confirmar con el usuario antes de generar cada bloque de tests.

---

## Reglas de Aceptación

- Todos los tests definidos como mínimos deben existir
- Todos los tests deben ejecutarse en forma automatizada
- Todos los tests mínimos deben pasar en el pipeline o entorno de validación
- No deben quedar pruebas críticas deshabilitadas o salteadas sin justificación explícita
- Todo bug corregido debe tener su test de regresión asociado
