#!/bin/sh
# Activa los hooks compartidos en ESTE repo. Correr una vez por clon, desde la raiz del repo.
[ -d .git ] || { echo "Corre esto desde la raiz del repo (no veo .git)."; exit 1; }
[ -f .githooks/commit-msg ] || { echo "Falta .githooks/ en la raiz. Copia la carpeta primero."; exit 1; }
git config core.hooksPath .githooks
echo "OK: core.hooksPath -> .githooks (hook commit-msg activo en este repo)."
echo "Prueba:  git commit --allow-empty -m 'chore(repo): prueba hook'"
