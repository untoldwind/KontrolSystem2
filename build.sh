#!/bin/sh

set -e

rm -rf GameData/KontrolSystem/Plugins

(cd Parsing; msbuild -t:build -restore -Property:Configuration=Release)
(cd TO2; msbuild -t:build -restore -Property:Configuration=Release)


(cd Parsing-Test; msbuild -t:build -restore -Property:Configuration=Release)
(cd Parsing-Test; msbuild -t:test -Property:Configuration=Release)
(cd TO2-Test; msbuild -t:build -restore -Property:Configuration=Release)
(cd TO2-Test; msbuild -t:test -Property:Configuration=Release)
