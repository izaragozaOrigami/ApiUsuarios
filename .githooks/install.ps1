# Activa los hooks compartidos en ESTE repo. Correr una vez por clon, desde la raiz del repo.
$ErrorActionPreference = "Stop"
if (-not (Test-Path ".git")) { Write-Error "Corre esto desde la raiz del repo (no veo .git)."; exit 1 }
if (-not (Test-Path ".githooks/commit-msg")) { Write-Error "Falta .githooks/ en la raiz. Copia la carpeta primero."; exit 1 }
git config core.hooksPath .githooks
Write-Host "OK: core.hooksPath -> .githooks (hook commit-msg activo en este repo)."
Write-Host "Prueba:  git commit --allow-empty -m 'chore(repo): prueba hook'"
