#!/bin/sh

(cd Tools/GenDocs; msbuild -t:build -restore -Property:Configuration=Release)

mono ./bin/Release/KontrolSystemGenDocs.exe
