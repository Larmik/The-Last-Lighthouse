#!/bin/bash
project_dir="${CLAUDE_PROJECT_DIR:-$(git rev-parse --show-toplevel)}"
if pgrep -fl "Unity.app/Contents/MacOS/Unity" | grep -v AssetImportWorker | grep -qF "$project_dir"; then
  echo "open"
  exit 0
fi
echo "closed"
exit 1
