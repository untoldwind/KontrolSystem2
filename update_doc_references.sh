#!/bin/sh

dotnet build  -restore -Property:Configuration=Release

./bin/Release/KontrolSystemGenDocs

dotnet build  -restore -Property:Configuration=Release

./bin/Release/KontrolSystemGenRefs
