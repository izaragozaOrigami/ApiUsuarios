# Convenciones de commits y SQL — repos Flotas

> Para **Alen, Luis y Daniel**. Objetivo doble: historia limpia en Flotas **y** que el pase a Danone salga casi solo (ver el plan de sync modular). Es formalizar lo que ya hacen a medias, cerrando dos huecos: **scope consistente** y **SQL siempre en el repo**.

## Índice
1. [Formato de commit](#1-formato-de-commit)
2. [Scopes = módulos (lista cerrada)](#2-scopes--módulos-lista-cerrada)
3. [Reglas de oro](#3-reglas-de-oro)
4. [SQL: siempre en el repo](#4-sql-siempre-en-el-repo)
5. [Instalar el hook](#5-instalar-el-hook)
6. [Agregar un módulo nuevo](#6-agregar-un-módulo-nuevo)
7. [Ejemplos](#7-ejemplos)

---

## 1. Formato de commit

**Conventional Commits:**

```
tipo(scope): descripción en minúscula, imperativo, sin punto final
```

**Tipos permitidos:** `feat` · `fix` · `docs` · `refactor` · `perf` · `test` · `chore` · `build` · `ci` · `style` · `revert`

- `feat`, `fix`, `perf`, `refactor` → **scope OBLIGATORIO**: un módulo de la lista, o un sub-scope `modulo-<algo>`.
- `docs`, `chore`, `ci`, `build`, `test`, `style` → scope opcional (si lo pones: módulo, sub-scope, o infra: `repo`, `ci`, `build`, `deploy`, `dev`, `env`, `qa`, `sql`, `webapi`, `tenant`, `config`, `demo`).

## 2. Scopes = módulos (+ sub-scopes)

El scope **es** el módulo. Lista canónica (vive en `.githooks/scopes.txt`):

```
checklist · catalogos · combustibles · solicitudes · conciliacion · cargas-masivas
rendimientos · refacciones · mantenimiento · inventario · usuarios · notificaciones
neumaticos · tema · proveedores · ui
```

Puedes usar el módulo tal cual (`mantenimiento`) **o un sub-scope** con prefijo `modulo-<algo>` (`mantenimiento-externo`, `catalogos-admin`, `inventario-almacen`). El tool mapea `mantenimiento-*` → módulo `mantenimiento` para el sync. Si el scope no es un módulo ni `modulo-<sub>`, el hook lo rechaza.

> Esta lista salió del historial real (ver `REVISION-scopes.md`). Algunos casos siguen abiertos a acordar con el equipo (p.ej. `refacciones-solicitudes`, `failure-checklist`, `roles`/`login`→`usuarios`).

## 3. Reglas de oro

1. **Un scope por commit.** Un commit toca **un** módulo. Si tu cambio abarca dos módulos, haz **dos commits**. (Un commit mezclado obliga a un archivo a pertenecer a dos módulos y arruina el sync por módulo.)
2. **El scope del código y el de su SQL coinciden.** Si `feat(checklist)` necesita un SP, el `.sql` de ese SP va con scope `checklist` también.
3. **Descripción útil**, no `wip`/`cambios`/`.`.

## 4. SQL: siempre en el repo

Todo cambio de base de datos que una feature necesite **se sube como `.sql`** (el `.sql` es la fuente de verdad, no la BD viva). Esto es lo que permite reconstruir los cambios en Danone sin adivinar.

- **Carpeta:** `SQL/` en la raíz del repo.
- **Mismo scope** en el commit que el código que lo usa.
- **Idempotente / re-ejecutable:**
  - Procs: `CREATE OR ALTER PROCEDURE ...`
  - Tablas/columnas: `IF NOT EXISTS (...) BEGIN ... END`
  - Inserts de catálogo: guardados con `IF NOT EXISTS` / `MERGE`.
- **Nombre que diga qué es** (prefijo + descripción + fecha):
  - `Create_...`, `Alter_...`, `Fix_...` → esquema/procs (portables a Danone tal cual casi siempre).
  - `Carga_...` → **datos** (data loads). ⚠️ Suelen traer IDs/catálogos propios de Flotas → se revisan antes de aplicar en Danone.
  - `Borra_...` → limpiezas/pruebas (normalmente **no** se portan).
  - Sufijo de fecha `_YYYYMMDD` para orden.
- **Consultas ad-hoc de investigación NO se commitean** — solo lo que la feature necesita para funcionar.

> Excepción: nada de credenciales, cadenas de conexión ni datos sensibles dentro de los `.sql`.

## 5. Instalar el hook

Copia la carpeta `.githooks/` a la **raíz** del repo y, **una vez por clon**, corre desde la raíz:

```bash
git config core.hooksPath .githooks
```

O usa el instalador: `.githooks/install.ps1` (PowerShell) o `sh .githooks/install.sh` (Git Bash).

El hook `commit-msg` valida el formato y el scope. Si un commit no cumple, lo rechaza con un mensaje que explica por qué.

> Los hooks viven por clon: **cada dev** lo instala una vez. Por eso `.githooks/` va **versionado** en el repo (para que todos tengan el mismo) pero el `core.hooksPath` lo activa cada quien.

## 6. Agregar un módulo nuevo

1. Agrega el nombre (una línea) a `.githooks/scopes.txt`.
2. Commit aparte: `chore(repo): registra módulo <nombre> en scopes.txt`.
3. Avisa al equipo para que hagan `pull`.

## 7. Ejemplos

✅ **Bien**
```
feat(mantenimiento-externo): valida folio del proveedor
feat(checklist): asignación de unidad (Cbu/Region/Site) en VehicleInformation
fix(solicitudes): OC duplicada por subquery en el monitor
perf(combustibles): índice para el cruce Webfleet
docs(catalogos): runbook de despliegue
chore(repo): registra módulo tarifas en scopes.txt
```

❌ **Rechazado**
```
cambios varios                         (sin formato)
feat: agrega pantalla                  (feat sin scope)
feat(golpes): ...                      (scope fuera de la lista → usa checklist)
update checklist y solicitudes         (mezcla módulos → sepáralo en 2 commits)
```
