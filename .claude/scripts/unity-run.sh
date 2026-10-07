#!/bin/bash
set -u
mode="${1:-EditMode}"
project_dir="${CLAUDE_PROJECT_DIR:-$(git rev-parse --show-toplevel)}"
unity_version=$(sed -n 's/^m_EditorVersion: //p' "$project_dir/ProjectSettings/ProjectVersion.txt")
unity="/Applications/Unity/Hub/Editor/$unity_version/Unity.app/Contents/MacOS/Unity"

if "$(dirname "$0")/unity-editor-open.sh" >/dev/null; then
  echo "EDITOR_OPEN: fermer l'éditeur Unity pour lancer en batchmode, ou vérifier dans l'éditeur (Console, Test Runner)."
  exit 3
fi

mkdir -p "$project_dir/Logs"
log="$project_dir/Logs/claude-$mode.log"

case "$mode" in
  Compile)
    "$unity" -batchmode -nographics -projectPath "$project_dir" -quit -logFile "$log"
    status=$?
    grep -E "error CS[0-9]+" "$log" | sort -u | head -n 30
    echo "COMPILE_EXIT=$status (log: $log)"
    exit $status
    ;;
  EditMode|PlayMode)
    results="$project_dir/Logs/claude-$mode-results.xml"
    rm -f "$results"
    "$unity" -batchmode -nographics -projectPath "$project_dir" -runTests -testPlatform "$mode" -testResults "$results" -logFile "$log"
    status=$?
    grep -E "error CS[0-9]+" "$log" | sort -u | head -n 30
    if [ -f "$results" ]; then
      sed -n 's/.*<test-run [^>]*total="\([0-9]*\)"[^>]*passed="\([0-9]*\)"[^>]*failed="\([0-9]*\)".*/TOTAL=\1 PASSED=\2 FAILED=\3/p' "$results" | head -n 1
      grep -o '<test-case [^>]*result="Failed"[^>]*' "$results" | sed -n 's/.*fullname="\([^"]*\)".*/FAILED: \1/p' | head -n 30
    else
      echo "Aucun fichier de résultats (compilation en échec ?)."
    fi
    echo "TESTS_EXIT=$status (log: $log)"
    exit $status
    ;;
  *)
    echo "Usage: unity-run.sh [Compile|EditMode|PlayMode]"
    exit 2
    ;;
esac
