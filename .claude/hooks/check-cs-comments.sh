#!/bin/bash
file=$(jq -r '.tool_input.file_path // empty')
case "$file" in
  *.cs) ;;
  *) exit 0 ;;
esac
[ -f "$file" ] || exit 0
hits=$(grep -nE '^[[:space:]]*(//|/\*)|[;{})][[:space:]]*(//|/\*)' "$file" | grep -v '://' | head -n 10)
[ -z "$hits" ] && exit 0
{
  echo "Convention du projet : aucun commentaire dans le code C# (CLAUDE.md, csharp/style.md)."
  echo "Commentaires détectés dans $file :"
  echo "$hits"
  echo "Les supprimer et rendre le code explicite par les noms."
} >&2
exit 2
