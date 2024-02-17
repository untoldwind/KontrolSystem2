#!/bin/sh

set -e

rm -rf dist

dotnet build  -restore -Property:Configuration=Release

dotnet test --logger:"console;verbosity=detailed" -Property:Configuration=Release

if [[ ! -z "$KSP2_BASE_DIR" ]]
then
  cp -r dist/* "$KSP2_BASE_DIR"
fi