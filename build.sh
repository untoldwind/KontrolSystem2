#!/bin/sh

set -e

rm -rf dist

(cd Parsing; msbuild -t:build -restore -Property:Configuration=Release)
(cd TO2; msbuild -t:build -restore -Property:Configuration=Release)
(cd SpaceWarpMod; msbuild -t:build -restore -Property:Configuration=Release)
(cd KSP2Runtime; msbuild -t:build -restore -Property:Configuration=Release)


(cd Parsing-Test; msbuild -t:build -restore -Property:Configuration=Release)
(cd Parsing-Test; msbuild -t:test -Property:Configuration=Release)
(cd TO2-Test; msbuild -t:build -restore -Property:Configuration=Release)
(cd TO2-Test; msbuild -t:test -Property:Configuration=Release)
(cd KSP2Runtime-Test; msbuild -t:build -restore -Property:Configuration=Release)
(cd KSP2Runtime-Test; msbuild -t:test -Property:Configuration=Release)

if [[ ! -z "$KSP2_BASE_DIR" ]]
then
  cp -r dist/* "$KSP2_BASE_DIR"
fi